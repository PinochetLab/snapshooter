using UnityEngine;

namespace Players.MoveStrategies
{
    public class LadderMoveStrategy : AbstractMoveStrategy
    {
        private float _ver;
        private Vector2 _lookInput;
        
        private float _yaw;
        private float _pitch;
        private Vector3 _currentVelocity;
        private Vector3 _targetVelocity;

        protected override void Init()
        {
            _yaw = Player.ViewTransform.Yaw;
            _pitch = Player.ViewTransform.Pitch;
            Player.DisablePhysics();
        }
        
        public override void ProcessInput()
        {
            _ver = Input.GetAxisRaw("Vertical");
            
            _lookInput = new Vector2(
                Input.GetAxisRaw("Mouse X"),
                -Input.GetAxisRaw("Mouse Y")
            );
        }

        public override void Update()
        {
            ApplyRotation();
            ApplyMovement();
        }
        
        private void ApplyRotation()
        {
            _yaw += _lookInput.x * Player.LookSensitivity * Time.deltaTime;
            _pitch += _lookInput.y * Player.LookSensitivity * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, -Player.MaxLookAngle, Player.MaxLookAngle);
            var viewTransform = Player.ViewTransform;
            viewTransform.Yaw = _yaw;
            viewTransform.Pitch = _pitch;
            Player.ViewTransform = viewTransform;
        }

        private void ApplyMovement()
        {
            var viewTransform = Player.ViewTransform;
            viewTransform.Position += Vector3.up * (_ver * Player.LadderSpeed * Time.fixedDeltaTime);
            Player.ViewTransform = viewTransform;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}