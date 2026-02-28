using System;
using System.Collections.Generic;
using Interaction;
using Player;
using UnityEngine;
using Zenject;

namespace Doors
{
    public class SwingDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Transform leftPivot;
        [SerializeField] private Transform rightPivot;
        [SerializeField] private SwingDoorCollider swingDoorCollider;
        [SerializeField] private List<Outline> outlines;

        [SerializeField] private float wallWidth = 0.25f;
        [SerializeField] private float doorWidth = 0.04f;

        [SerializeField] private float maxLeftAngle = 115;
        [SerializeField] private float maxRightAngle = 115;

        [SerializeField] private float angularSpeed = 400;

        [SerializeField] private SwingDoorState startState;
        
        [Inject] private PlayerMoveController _player;

        private SwingDoorState _currentState;
        private SwingDoorState _lastState;
        private float _angle;
        private float _targetAngle;
        private bool _isMoving;
        private bool _canInteract;

        private void Awake()
        {
            _currentState = startState;
            _angle = StateToAngle(_currentState);
        }

        private float StateToAngle(SwingDoorState state)
        {
            return state switch
            {
                SwingDoorState.Closed => 0,
                SwingDoorState.LeftOpened => maxLeftAngle,
                SwingDoorState.RightOpened => -maxRightAngle,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }

        private bool IsPlayerFromLeft()
        {
            var right = pivot.right;
            var position = pivot.position;
            var playerPosition = _player.ViewTransform.Position;
            var to = playerPosition - position;
            return Vector3.Cross(right, to).y > 0;
        }

        public void OnPlayerEnter()
        {
            if (_currentState == SwingDoorState.Closed)
            {
                _currentState = _lastState;
                _lastState = SwingDoorState.Closed;
            }
            _targetAngle = StateToAngle(_currentState);
            StartMove();
        }

        private void StartMove()
        {
            _isMoving = true;
            swingDoorCollider.StartMove();
        }
        
        private void EndMove()
        {
            _isMoving = false;
            swingDoorCollider.EndMove();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (!_isMoving)
                return;
            _angle = Mathf.MoveTowards(_angle, _targetAngle, angularSpeed * Time.deltaTime);
            SetAngle(_angle);
            if (Mathf.Approximately(_angle, _targetAngle))
                EndMove();
        }

        [ContextMenu("ForceDoorUpdate")]
        private void ForceDoorUpdate()
        {
            SetAngle(StateToAngle(startState));
        }

        private float GetCoordinate(float angle)
        {
            return Mathf.Clamp(-angle / 90, -1, 1) * (wallWidth - doorWidth) / 2;
        }

        private void SetAngle(float angle)
        {
            pivot.localPosition = Vector3.forward * GetCoordinate(angle);
            if (angle < 0)
            {
                leftPivot.localEulerAngles = Vector3.zero;
                rightPivot.localEulerAngles = Vector3.up * angle;
            }
            else
            {
                rightPivot.localEulerAngles = Vector3.zero;
                leftPivot.localEulerAngles = Vector3.up * angle;
            }
        }

        public bool CanInteract => !_isMoving;
        public string InteractionText => _currentState == SwingDoorState.Closed ? "open" : "close";
        
        public void Interact()
        {
            _lastState = _currentState;
            if (_currentState == SwingDoorState.Closed)
            {
                _currentState = IsPlayerFromLeft() ? SwingDoorState.RightOpened : SwingDoorState.LeftOpened;
            }
            else
                _currentState = SwingDoorState.Closed;

            _targetAngle = StateToAngle(_currentState);
            StartMove();
        }
        
        public List<Outline> Outlines => outlines;
    }
}