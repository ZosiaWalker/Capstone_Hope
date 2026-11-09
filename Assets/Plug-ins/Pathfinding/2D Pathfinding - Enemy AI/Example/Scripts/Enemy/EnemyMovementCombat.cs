using System.Collections;
using UnityEngine;

namespace Pathfinding2D
{
    public class EnemyMovementCombat : MonoBehaviour
    {
        public AIMovement aiMovement;

        public Transform target;
        public LayerMask obstacleMask;
        public float chasingRadius = 5f;
        public float attackRadius = 1f;

        public float speed = 2f;

        private EnemyState state = EnemyState.Idle;
        private Vector2 movementInput;

        private Coroutine attackCoroutine;
        private bool isAttacking = false;
        private bool isBlinking = false;

        private CircleCollider2D circleCollider;
        private Rigidbody2D enemyRigidbody;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        void Start()
        {
            circleCollider = GetComponent<CircleCollider2D>();
            enemyRigidbody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            SetEnemyState();
            Animate();
        }

        private void FixedUpdate()
        {
            if (state == EnemyState.Chasing)
            {
                Vector2 direction = aiMovement.GetDirectionToMove(enemyRigidbody.position);
                MoveTowardsDirection(direction);
            }
        }

        private void SetEnemyState()
        {
            if (state == EnemyState.Attacking || state == EnemyState.Knockback) return;
            if (IsTargetInRadius(attackRadius))
            {
                movementInput = ((Vector2)target.position - enemyRigidbody.position).normalized;
                if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
                {
                    movementInput = new Vector2(Mathf.Sign(movementInput.x), 0);
                }
                else
                {
                    movementInput = new Vector2(0, Mathf.Sign(movementInput.y));
                }
                attackCoroutine = StartCoroutine(Attacking());
            }
            else if (IsTargetInRadius(chasingRadius))
            {
                if (aiMovement.IsPathValid)
                {
                    state = EnemyState.Chasing;
                }
                else
                {
                    state = EnemyState.Calculating;
                }
                aiMovement.CalculateShortestPath(enemyRigidbody.position, target.position);
            }
            else
            {
                state = EnemyState.Idle;
                movementInput = Vector2.zero;
            }
        }

        private void Animate()
        {
            if (movementInput != Vector2.zero)
            {
                animator.SetFloat("Horizontal", movementInput.x);
                animator.SetFloat("Vertical", movementInput.y);
                if (state == EnemyState.Chasing)
                {
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        private IEnumerator Attacking()
        {
            state = EnemyState.Attacking;
            yield return StartCoroutine(CheckForKnocbackRoutine(0.25f));

            isAttacking = true;
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            isAttacking = false;

            yield return StartCoroutine(CheckForKnocbackRoutine(0.5f));
            state = EnemyState.Idle;
        }

        private IEnumerator CheckForKnocbackRoutine(float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (state == EnemyState.Knockback)
                {
                    if (attackCoroutine != null)
                    {
                        StopCoroutine(attackCoroutine);
                    }
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator KnockbackRoutine(Vector2 direction, float strength, float duration)
        {
            state = EnemyState.Knockback;
            StartCoroutine(BlinkRoutine());

            Vector2 startPosition = enemyRigidbody.position;

            var filter = new ContactFilter2D { useTriggers = false };
            filter.SetLayerMask(obstacleMask);

            RaycastHit2D[] hits = new RaycastHit2D[4];
            float allowedDistance = strength;

            int hitCount = circleCollider.Cast(direction, filter, hits, strength);
            if (hitCount > 0)
            {
                float minDist = strength;
                for (int i = 0; i < hitCount; i++)
                {
                    if (hits[i].collider != null)
                    {
                        if (hits[i].distance < minDist) minDist = hits[i].distance;
                    }
                }
                allowedDistance = Mathf.Max(0f, minDist - 0.01f);
            }

            Vector2 finalTarget = startPosition + direction * allowedDistance;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.fixedDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float s = Mathf.SmoothStep(0f, 1f, t);
                Vector2 next = Vector2.Lerp(startPosition, finalTarget, s);

                enemyRigidbody.MovePosition(next);
                yield return new WaitForFixedUpdate();
            }

            enemyRigidbody.MovePosition(finalTarget);
            yield return new WaitForSeconds(0.25f);

            state = EnemyState.Idle;
        }

        private IEnumerator BlinkRoutine()
        {
            isBlinking = true;

            Color originalColor = spriteRenderer.color;
            float elapsed = 0f;
            float blinkInterval = 0.1f;

            while (state == EnemyState.Knockback)
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(blinkInterval);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval * 2f;
            }
            spriteRenderer.color = originalColor;

            isBlinking = false;
        }

        public void ApplyKnockback(Vector2 direction, float strength, float duration)
        {
            if (state == EnemyState.Knockback || isAttacking || isBlinking) return;

            StartCoroutine(KnockbackRoutine(direction, strength, duration));
        }

        bool IsTargetInRadius(float radius)
        {
            return Vector3.Distance(transform.position, target.position) <= radius;
        }

        void MoveTowardsDirection(Vector2 direction)
        {
            Vector2 nextPosition = enemyRigidbody.position + direction * speed * Time.deltaTime;
            movementInput = nextPosition - enemyRigidbody.position;
            enemyRigidbody.MovePosition(nextPosition);
        }
    }
}
