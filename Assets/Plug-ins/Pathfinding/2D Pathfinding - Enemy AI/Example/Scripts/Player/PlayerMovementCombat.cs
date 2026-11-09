using System.Collections;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Pathfinding2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementCombat : MonoBehaviour
    {
        public float speed = 3f;

        private PlayerState playerState = PlayerState.Idle;
        private Vector2 movementInput;
        private bool isBlinking = false;

        private Rigidbody2D playerRigidbody;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

#if ENABLE_INPUT_SYSTEM
        private InputAction moveAction;
#endif

        void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            moveAction = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            moveAction.Enable();
#endif
        }

        void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            moveAction.Disable();
#endif
        }

        void Start()
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            SetState();
            SetAnimation();
        }

        void FixedUpdate()
        {
            if (playerState == PlayerState.Attacking || playerState == PlayerState.Knockback) return;

#if UNITY_6000_0_OR_NEWER
            playerRigidbody.linearVelocity = movementInput * speed;
#else
            playerRigidbody.velocity = movementInput * speed;
#endif
        }

        void SetState()
        {
            if (playerState == PlayerState.Attacking || playerState == PlayerState.Knockback) return;

#if ENABLE_INPUT_SYSTEM
            // New Input System
            movementInput = moveAction.ReadValue<Vector2>();
#else
            // Old Input System
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
#endif
            movementInput = movementInput.normalized;

#if ENABLE_INPUT_SYSTEM
            // New Input System
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                StartCoroutine(AttackRoutine());
            }
#else
            // Old Input System
            if (Input.GetKeyDown(KeyCode.J))
            {
                StartCoroutine(AttackRoutine());
            }
#endif
            else if (movementInput != Vector2.zero)
            {
                playerState = PlayerState.Walking;
            }
            else
            {
                playerState = PlayerState.Idle;
            }
        }

        void SetAnimation()
        {
            if (movementInput != Vector2.zero)
            {
                animator.SetFloat("Horizontal", movementInput.x);
                animator.SetFloat("Vertical", movementInput.y);
                if (playerState == PlayerState.Walking)
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

        private IEnumerator AttackRoutine()
        {
            playerState = PlayerState.Attacking;
#if UNITY_6000_0_OR_NEWER
            playerRigidbody.linearVelocity = Vector2.zero;
#else
            playerRigidbody.velocity = Vector2.zero;
#endif

            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            playerState = PlayerState.Idle;
        }

        public void ApplyKnockback(Vector2 direction, float strength, float duration)
        {
            if (playerState == PlayerState.Knockback || isBlinking) return;
            StartCoroutine(KnockbackRoutine(direction, strength, duration));
        }

        private IEnumerator KnockbackRoutine(Vector2 direction, float strength, float duration)
        {
            playerState = PlayerState.Knockback;

#if UNITY_6000_0_OR_NEWER
            playerRigidbody.linearVelocity = Vector2.zero;
#else
            playerRigidbody.velocity = Vector2.zero;
#endif
            playerRigidbody.AddForce(direction.normalized * strength, ForceMode2D.Impulse);

            StartCoroutine(BlinkRoutine());

            yield return new WaitForSeconds(duration);

#if UNITY_6000_0_OR_NEWER
            playerRigidbody.linearVelocity = Vector2.zero;
#else
            playerRigidbody.velocity = Vector2.zero;
#endif
            playerState = PlayerState.Idle;
        }

        private IEnumerator BlinkRoutine()
        {
            isBlinking = true;

            Color originalColor = spriteRenderer.color;
            float elapsed = 0f;
            float blinkInterval = 0.1f;

            while (playerState == PlayerState.Knockback)
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
    }
}
