using Scripts.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class SpotPatrol : Patrol
    {
        [Tooltip("Точки маршрута (дочерние объекты или любые трансформы на сцене)")]
        public Transform[] waypoints;

        [Tooltip("Пауза между достижением точки и началом движения к следующей")]
        public float pauseBetweenPoints = 1f;

        [Tooltip("Допустимая погрешность достижения точки (по оси X)")]
        public float reachDistance = 0.2f;

        public override IEnumerator DoPatrol(Creature creature)
        {
            if (waypoints == null || waypoints.Length == 0)
                yield break;

            int index = 0;
            while (true)
            {
                Transform targetPoint = waypoints[index];
                while (Mathf.Abs(creature.transform.position.x - targetPoint.position.x) > reachDistance)
                {
                    var direction = targetPoint.position - creature.transform.position;
                    direction.y = 0;
                    creature.SetMovementDirection(direction.normalized);
                    yield return null;
                }
                creature.SetMovementDirection(Vector2.zero);
                yield return new WaitForSeconds(pauseBetweenPoints);
                index = (index + 1) % waypoints.Length;
            }
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            Gizmos.color = Color.green;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                Gizmos.DrawWireSphere(waypoints[i].position, 0.2f);
                int next = (i + 1) % waypoints.Length;
                if (waypoints[next] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }
    }
}
