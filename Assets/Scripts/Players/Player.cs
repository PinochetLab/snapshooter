using Players.MoveStrategies;
using UnityEngine;
using View;

namespace Players
{
    public class Player : Body
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider playerCollider;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float runSpeed = 4f;
        [SerializeField] private float acceleration = 10f;
        
        [Header("Ladder Settings")]
        [SerializeField] private float ladderSpeed = 5f;
        
        [Header("Look Settings")]
        [SerializeField] private float lookSensitivity = 100f;
        [SerializeField] private float maxLookAngle = 80f;
        
        [SerializeField] private float maxZoomCoef = 3f;

        private AbstractMoveStrategy _moveStrategy;
        
        private bool _isControlled = true;
        
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float Acceleration => acceleration;
        public float LookSensitivity => lookSensitivity;
        public float MaxLookAngle => maxLookAngle;
        public float LadderSpeed => ladderSpeed;

        public bool IsZoom { get; private set; }
        public float ZoomCoef => IsZoom ? maxZoomCoef : 1f;
        
        public Rigidbody Rigidbody => rb;

        private void Awake()
        {
            _moveStrategy = new WalkMoveStrategy();
            _moveStrategy.Init(this);
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void Update()
        {
            if (!_isControlled)
                return;
            _moveStrategy.ProcessInput();
            _moveStrategy.Update();
        }
        
        private void FixedUpdate()
        {
            if (!_isControlled)
                return;
            _moveStrategy.FixedUpdate();
        }

        public void StartZoom()
        {
            IsZoom = true;
        }
        
        public void EndZoom()
        {
            IsZoom = false;
        }

        public void SetStrategy(AbstractMoveStrategy moveStrategy)
        {
            _moveStrategy = moveStrategy;
            _moveStrategy.Init(this);
        }

        public void EnablePhysics()
        {
            SetPhysics(true);
        }
        
        public void DisablePhysics()
        {
            SetPhysics(false);
        }
        
        

        private void SetPhysics(bool value)
        {
            if (!value)
                rb.linearVelocity = Vector3.zero;
            rb.isKinematic = !value;
            rb.interpolation = value ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            playerCollider.enabled = value;
        }

        public void DisableMotion()
        {
            DisablePhysics();
            _isControlled = false;
        }

        public void EnableMotion()
        {
            _isControlled = true;
            _moveStrategy.Init(this);
        }
    }
}