using UnityEngine;

namespace Pathfinding2D
{
    public class EnemyMovementChase : MonoBehaviour
    {
        public AIMovement aiMovement;

        public Transform target;
        public float minDistanceToTarget = 1f;

        public float speed = 2f;

        private EnemyState state = EnemyState.Idle;
        private Vector2 movementInput;

        private Rigidbody2D enemyRigidbody;
        private Animator animator;

        void Start()
        {
            enemyRigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            InvokeRepeating(nameof(RecalculatePath), 0f, 0.1f);
        }

        void Update()
        {
            SetState();
            SetAnimation();
        }

        void FixedUpdate()
        {
            if (state == EnemyState.Chasing)
            {
                Vector2 direction = aiMovement.GetDirectionToMove(enemyRigidbody.position);
                MoveTowardsDirection(direction);
            }
            else
            {
                movementInput = Vector2.zero;
            }
        }

        void RecalculatePath()
        {
            aiMovement.CalculateShortestPath(enemyRigidbody.position, target.position);
        }

        void SetState()
        {
            float distanceToTarget = Vector2.Distance(target.position, enemyRigidbody.position);
            if (aiMovement.IsPathValid && distanceToTarget > minDistanceToTarget)
            {
                state = EnemyState.Chasing;
            }
            else
            {
                state = EnemyState.Idle;
            }
        }

        void SetAnimation()
        {
            if (movementInput != Vector2.zero)
            {
                animator.SetFloat("Horizontal", movementInput.x);
                animator.SetFloat("Vertical", movementInput.y);
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        void MoveTowardsDirection(Vector2 direction)
        {
            Vector2 nextPosition = enemyRigidbody.position + direction * speed * Time.deltaTime;
            movementInput = nextPosition - enemyRigidbody.position;
            enemyRigidbody.MovePosition(nextPosition);
        }
    }
}
