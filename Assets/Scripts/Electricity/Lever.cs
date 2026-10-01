using System;
using Electricity.Wires;
using Interaction;
using UnityEngine;
using UnityEngine.Events;
using Wires;

namespace Electricity
{
    public class Lever : MonoBehaviour, IInteractable, ISource
    {
        [SerializeField] private Transform axisTransform;
        [SerializeField] private float spinSpeed = 50f;
        [SerializeField] private float maxAngle = 30f;
        
        private bool _activated;
        private bool _move;
        private float _targetAngle;
        private float _currentAngle;

        public bool CanInteract => !_move;
        
        public string InteractionText => "use lever";
        
        public void Interact()
        {
            Switch();
        }

        private void Switch()
        {
            _activated = !_activated;
            
            if (_activated)
            {
                Wire.TurnOn();
            }
            else
            {
                Wire.TurnOff();
            }
            
            SetTargetAngle();
            Apply();
        }

        private void Apply()
        {
            _move = true;
        }

        private void ApplyImmediately()
        {
            _currentAngle = _targetAngle;
            ApplyAngle();
        }

        private void SetTargetAngle()
        {
            _targetAngle = maxAngle * (_activated ? 1f : -1f);
        }

        private void ApplyAngle()
        {
            Debug.Log($"Angle: {_currentAngle}");
            axisTransform.localRotation = Quaternion.Euler(0f, 0f, -_currentAngle);
        }

        private void Start()
        {
            SetTargetAngle();
            ApplyImmediately();
        }

        private void Update()
        {
            if (!_move)
            {
                return;
            }
            _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, _targetAngle, spinSpeed * Time.deltaTime);
            ApplyAngle();
            if (Mathf.Approximately(_currentAngle, _targetAngle))
            {
                _move = false;
            }
        }

        public Wire Wire { get; set; }
    }
}