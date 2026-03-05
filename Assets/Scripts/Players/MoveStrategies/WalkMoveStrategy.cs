using UnityEngine;

namespace Players.MoveStrategies
{
    public class WalkMoveStrategy : AbstractMoveStrategy
    {
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _run;
        
        private float _yaw;
        private float _pitch;
        private Vector3 _currentVelocity;
        private Vector3 _targetVelocity;

        protected override void Init()
        {
            _yaw = Player.ViewTransform.Yaw;
            _pitch = Player.ViewTransform.Pitch;
            Player.EnablePhysics();
        }
        
        public override void ProcessInput()
        {
            _moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
            
            _lookInput = new Vector2(
                Input.GetAxisRaw("Mouse X"),
                -Input.GetAxisRaw("Mouse Y")
            );
            
            _run = Input.GetKey(KeyCode.LeftShift);
        }

        public override void Update()
        {
            ApplyRotation();
        }
        
        private void ApplyRotation()
        {
            _yaw += _lookInput.x * Player.LookSensitivity * Time.deltaTime / Player.ZoomCoef;
            _pitch += _lookInput.y * Player.LookSensitivity * Time.deltaTime / Player.ZoomCoef;
            _pitch = Mathf.DeltaAngle(0f, _pitch);
            _pitch = Mathf.Clamp(_pitch, -Player.MaxLookAngle, Player.MaxLookAngle);
            var viewTransform = Player.ViewTransform;
            viewTransform.Yaw = _yaw;
            viewTransform.Pitch = _pitch;
            Player.ViewTransform = viewTransform;
        }

        public override void FixedUpdate()
        {
            ApplyMovement();
        }
        
        private void ApplyMovement()
        {
            var moveDirection = (Player.Forward * _moveInput.y + Player.Right * _moveInput.x).normalized;
            _targetVelocity = moveDirection * ((_run && !Player.IsZoom) ? Player.RunSpeed : Player.WalkSpeed) / Player.ZoomCoef;
            
            _targetVelocity.y = Player.Rigidbody.linearVelocity.y;
            
            _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity, Player.Acceleration * Time.fixedDeltaTime);
            
            Player.Rigidbody.linearVelocity = _currentVelocity;
        }
        
    }
}