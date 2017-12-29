using UnityEngine;
//TODO: a feared enemy that spawns an enemy will give its target to the child, meaning the child will act feared until it reaches the first waypoint
//i think making many more waypoints to smooth out the travel path might work for now, as well as allowing a teleport ability to be easily implemented
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
        waypointIndex = i - 1;
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
        if (!fear || waypointIndex >= 0)  //if you stuck at the spawn don't try to update movement target
        {
            Vector3 targetXZ = new Vector3(target.position.x, transform.position.y, target.position.z);   //move towards the waypoint on the x and z axis only

            Vector3 dir = targetXZ - transform.position;

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
            Destroy(gameObject);
            WaveSpawner.Instance.enemyGhostList.Remove(gameObject);
            return;
        }
        PlayerStats.Instance.lives--;
        WaveSpawner.Instance.enemiesAlive--;
        WaveSpawner.Instance.enemyList.Remove(gameObject);
        Destroy(gameObject);
    }

    public void SetFear(bool active)
    {
        fear = active;
        GetNextWaypoint();
    }
}
