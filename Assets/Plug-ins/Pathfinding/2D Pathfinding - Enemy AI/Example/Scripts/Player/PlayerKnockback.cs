using UnityEngine;

namespace Pathfinding2D
{
    public class PlayerKnockback : MonoBehaviour
    {
        public string enemyTag = "Respawn";
        public float knockbackStrength = 4f;
        public float knockbackDuration = 0.3f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(enemyTag))
            {
                EnemyMovementCombat player = other.GetComponentInParent<EnemyMovementCombat>();
                if (player != null)
                {
                    Vector2 direction = (other.transform.position - transform.position).normalized;
                    player.ApplyKnockback(direction, knockbackStrength, knockbackDuration);
                }
            }
        }
    }
}
