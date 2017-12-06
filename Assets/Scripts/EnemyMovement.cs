using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

    private Transform target;
    private int waypointIndex = 0;

    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();

        target = Waypoints.points[0];
    }

    private void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWaypoint();
        }

        BuffHelper.ResetDebuffs(enemy);
        BuffHelper.CheckDebuffs(enemy);
    }

    //In Unity -> Edit -> Script execution order and make movement after the rest of the scripts if movement needs to be adjusted last (not really necessary right now)

    void GetNextWaypoint()
    {
        if (waypointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
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
}
