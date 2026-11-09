using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding2D
{
    public class EnemyAvoidanceBehaviour : SteeringBehaviour
    {
        [SerializeField]
        private Collider2D enemy;

        [SerializeField]
        private LayerMask enemyLayer;

        [SerializeField]
        private float radius = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float sensitivity = 0.5f;

        [SerializeField]
        private AreaOfSight areaOfSight;

        [SerializeField]
        private bool showGizmos = true;
        private float[] dangerTemp = null;

        private Collider2D[] enemies;
        private Vector2[] directions;

        public override void Detect()
        {
            List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer).ToList();
            colliders.RemoveAll(collider => collider.transform.gameObject == enemy.gameObject);
            enemies = colliders.ToArray();
        }

        public override (float[] danger, float[] interest) GetSteering(SteeringBehaviourManager behaviourManager, Vector2 desiredDirection)
        {
            float[] danger = new float[behaviourManager.numberOfRays];
            float[] interest = new float[behaviourManager.numberOfRays];
            directions = behaviourManager.Directions;

            foreach (var enemyCollider in enemies)
            {
                Vector2 directionToEnemy = enemyCollider.transform.position - transform.position;
                float distanceToEnemy = directionToEnemy.magnitude;

                float weight = 1 - (distanceToEnemy / radius);

                for (int i = 0; i < directions.Length; i++)
                {
                    float alignment = Vector2.Dot(directions[i], directionToEnemy.normalized);

                    if (alignment >= areaOfSight.Alignment)
                    {
                        float value = alignment * weight;
                        if (value > danger[i])
                        {
                            danger[i] = (weight >= 1f - sensitivity) ? 1 : value;
                        }
                    }
                }
            }

            dangerTemp = danger;
            return (danger, interest);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showGizmos == false)
                return;

            if (dangerTemp != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangerTemp.Length; i++)
                {
                    Gizmos.DrawRay(
                        transform.position,
                        directions[i] * dangerTemp[i] * 2
                    );
                }
            }
            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}