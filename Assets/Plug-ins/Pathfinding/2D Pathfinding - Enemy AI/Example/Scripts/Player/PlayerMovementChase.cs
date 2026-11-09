using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Pathfinding2D
{
    public class PlayerMovementChase : MonoBehaviour
    {
        public float speed = 3f;

        private PlayerState playerState = PlayerState.Idle;
        private Vector2 movementInput;

        private Rigidbody2D playerRigidbody;
        private Animator animator;

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

            if (movementInput != Vector2.zero)
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
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
}
