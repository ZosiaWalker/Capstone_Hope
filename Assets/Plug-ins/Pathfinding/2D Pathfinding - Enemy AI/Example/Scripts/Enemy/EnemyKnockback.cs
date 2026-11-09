using UnityEngine;

namespace Pathfinding2D
{
    public class EnemyKnockback : MonoBehaviour
    {
        public string playerTag = "Player";
        public float knockbackStrength = 4f;
        public float knockbackDuration = 0.3f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
            {
                PlayerMovementCombat player = other.GetComponentInParent<PlayerMovementCombat>();
                if (player != null)
                {
                    Vector2 direction = (other.transform.position - transform.position).normalized;
                    player.ApplyKnockback(direction, knockbackStrength, knockbackDuration);
                }
            }
        }
    }
}
