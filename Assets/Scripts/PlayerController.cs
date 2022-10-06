using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 5.335f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    //[SerializeField] private float targetRotation = 5f;
    //[SerializeField] private float rotationSmoothTime = 0.12f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private Transform cameraTransform;

    [SerializeField] Animator _animator;
    private float _animationBlend;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    [SerializeField] string _walkBool;
    [SerializeField] string _jumpBool;

    private bool _hasAnimator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        AssignAnimationIDs();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }


    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            _animator.SetBool("Jump", false);
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        //controller.Move(move * Time.deltaTime * playerSpeed);

        float targetSpeed = 0f;
        if(sprintAction.IsPressed())
        {
            targetSpeed = sprintSpeed;
            _animator.SetFloat("Speed", 2);
        }
        else
        {
            targetSpeed = walkSpeed;
            _animator.SetFloat("Speed", 1);
        }

        if(move.magnitude > 0.01f)
        {
            float targetAngle = cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            transform.rotation = targetRotation;
        }
        else
        {
            _animator.SetFloat("Speed", 0);

        }


        if (jumpAction.triggered && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            _animator.SetBool("Jump", true);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;

        //move
        controller.Move( (move * targetSpeed * Time.deltaTime) + (playerVelocity  * Time.deltaTime) );

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
        

    }
}