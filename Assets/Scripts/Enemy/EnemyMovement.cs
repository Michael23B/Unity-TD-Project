using UnityEngine;
[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

    private Transform target;

    public int waypointIndex = 1;  //skip the first waypoint at the spawn point

    private Enemy enemy;
    bool ghost = false;

    Transform[] points;

    public int GetWaypoint { get { return waypointIndex; } }
    public void SetWaypoint(int i)
    {
        waypointIndex = i;
    }
    public void SetAndUpdateWaypoint(int i)
    {
        if (points == null) //redundant checking but errors kept appearing
        {
            waypointIndex = i;
            return;
        }
        if (points.Length == 0)
        {
            waypointIndex = i;
            return;
        }
        if (!fear) waypointIndex = i - 1;
        else waypointIndex = i + 1;
        GetNextWaypoint();
    }
    [HideInInspector]
    public bool fear = false;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        ghost = enemy.ghost;

        points = new Transform[Waypoints.points.Length];

        if (ghost) WaypointsAlternate.pointsAlternate.CopyTo(points, 0);
        else Waypoints.points.CopyTo(points, 0);

        target = points[waypointIndex];
    }

    private void Move()
    {
        if (!fear || waypointIndex >= 0)  //if you're stuck at the spawn don't try to update movement target
        {
            Vector3 targetXZ = new Vector3(target.position.x, transform.position.y, target.position.z);   //move towards the waypoint on the x and z axis only

            Vector3 dir = targetXZ - transform.position;

            enemy.graphics.transform.rotation = Quaternion.Lerp(enemy.graphics.transform.rotation, Quaternion.LookRotation(dir), enemy.speed * Time.deltaTime);

            transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, targetXZ) <= 0.4f)
            {
                GetNextWaypoint();
            }
        }
    }

    private void Update()
    {
        BuffHelper.ResetDebuffs(enemy);
        BuffHelper.CheckDebuffs(enemy);
        Move();
    }

    //In Unity -> Edit -> Script execution order and make movement after the rest of the scripts if movement needs to be adjusted last (not really necessary right now)

    void GetNextWaypoint()
    {
        if (waypointIndex < 0) waypointIndex = 0;
        if (waypointIndex >= points.Length - 1)
        {
            EndPath();
            return;
        }

        if(fear)
        {
            --waypointIndex;
            if (waypointIndex < 0) target = points[0];
            else target = points[waypointIndex];
            return;
        }

        ++waypointIndex;
        target = points[waypointIndex];
    }

    void EndPath()
    {
        if (ghost)
        {
            enemy.Kill();   //ghost kills can be handled by the enemy normally
            return;
        }

        WaveSpawner.Instance.commands.CmdReduceLives(enemy.coreDamage);
        WaveSpawner.Instance.enemiesAlive--;
        WaveSpawner.Instance.enemyList.Remove(gameObject);

        EnemyAttack attackComponent = GetComponent<EnemyAttack>();
        if (attackComponent != null) attackComponent.isQuitting = true;  //enemies that reach the end shouldn't spawn on death objects

        Destroy(gameObject);
    }

    public void SetFear(bool active)
    {
        fear = active;
        GetNextWaypoint();
    }
}
