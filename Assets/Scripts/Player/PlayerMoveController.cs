using UnityEngine;
using View;

namespace Player
{
    public class PlayerMoveController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Transform headTransform;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float airControl = 0.3f;
        [SerializeField] private float jumpForce = 5f;
        
        [Header("Look Settings")]
        [SerializeField] private float lookSensitivity = 100f;
        [SerializeField] private float maxLookAngle = 80f;
        
        [Header("Input Settings")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
        
        // Private variables
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private float _yaw;
        private float _pitch;
        private Vector3 _currentVelocity;
        private Vector3 _targetVelocity;
        private bool _isGrounded;
        private bool _isRunning;
        private bool _isControlled = true;
        
        public ViewTransform ViewTransform
        {
            get
            {
                var position = bodyTransform.position;
                var yaw = bodyTransform.eulerAngles.y;
                var pitch = headTransform.localEulerAngles.x;
                return new ViewTransform(position, yaw, pitch);
            }
            set
            {
                bodyTransform.position = value.Position;
                bodyTransform.eulerAngles = Vector3.up * value.Yaw;
                headTransform.localEulerAngles = Vector3.right * value.Pitch;
            }
        }
        
        private void Start()
        {
            if (rb == null) Debug.LogError("Rigidbody not assigned or found on " + gameObject.name);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void Update()
        {
            HandleInput();
            ApplyRotation();
        }
        
        private void HandleInput()
        {
            if (!_isControlled)
                return;
            
            _moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
            
            _lookInput = new Vector2(
                Input.GetAxisRaw("Mouse X") * lookSensitivity * Time.deltaTime,
                -Input.GetAxisRaw("Mouse Y") * lookSensitivity * Time.deltaTime
            );
            
            _isRunning = Input.GetKey(runKey);
            
            _yaw += _lookInput.x;
            _pitch += _lookInput.y;
            _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);
        }

        private void ApplyRotation()
        {
            if (!_isControlled)
                return;

            var viewTransform = ViewTransform;
            viewTransform.Yaw = _yaw;
            viewTransform.Pitch = _pitch;
            ViewTransform = viewTransform;
        }
        
        private void FixedUpdate()
        {
            ApplyMovement();
        }
        
        private void ApplyMovement()
        {
            if (!_isControlled)
                return;
            
            var forward = bodyTransform.forward;
            var right = bodyTransform.right;
            var moveDirection = (forward * _moveInput.y + right * _moveInput.x).normalized;
            var currentSpeed = _isRunning ? runSpeed : walkSpeed;
            _targetVelocity = moveDirection * currentSpeed;
            
            _targetVelocity.y = rb.linearVelocity.y;
            
            var accelerationRate = _isGrounded ? acceleration : acceleration * airControl;
            _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity, accelerationRate * Time.fixedDeltaTime);
            
            rb.linearVelocity = _currentVelocity;
        }

        public void DisableMotion()
        {
            rb.isKinematic = true;
            _isControlled = false;
        }

        public void EnableMotion()
        {
            rb.isKinematic = false;
            _isControlled = true;
            _moveInput = Vector2.zero;
            _lookInput = Vector2.zero;
            _yaw = ViewTransform.Yaw;
            _pitch = ViewTransform.Pitch;
        }
    }
}