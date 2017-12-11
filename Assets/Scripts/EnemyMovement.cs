using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

    private Transform target;

    private int waypointIndex = 1;  //skip the first waypoint at the spawn point

    private Enemy enemy;

    public int GetWaypoint { get { return waypointIndex; } }
    public void SetWaypoint(int i)
    {
        waypointIndex = i - 1;
        GetNextWaypoint();
    }

    [HideInInspector]
    public bool fear = false;

    private void Start()
    {
        enemy = GetComponent<Enemy>();

        target = Waypoints.points[waypointIndex];
    }

    private void Update()
    {
        if (!fear || waypointIndex >= 0)  //if you stuck at the spawn don't try to update movement target
        {
            Vector3 dir = target.position - transform.position;
            transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, target.position) <= 0.4f)
            {
                GetNextWaypoint();
            }
        }

        BuffHelper.ResetDebuffs(enemy);
        BuffHelper.CheckDebuffs(enemy);
    }

    //In Unity -> Edit -> Script execution order and make movement after the rest of the scripts if movement needs to be adjusted last (not really necessary right now)

    void GetNextWaypoint()
    {
        if (waypointIndex < 0) waypointIndex = 0;
        if (waypointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
            return;
        }

        if(fear)
        {
            --waypointIndex;
            if (waypointIndex < 0) target = Waypoints.points[0];
            else target = Waypoints.points[waypointIndex];
            return;
        }

        ++waypointIndex;
        target = Waypoints.points[waypointIndex];
    }

    void EndPath()
    {
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
