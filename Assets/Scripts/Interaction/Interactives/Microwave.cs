using System;
using System.Collections.Generic;
using Snap.Matchables;
using UnityEngine;

namespace Interaction.Interactives
{
    public class Microwave : MonoBehaviour, IInteractable, IStatable
    {
        [SerializeField] private Transform doorTransform;
        [SerializeField] private float openAngle = 135;
        [SerializeField] private float angularSpeed = 200;
        [SerializeField] private bool opened;
        public string InteractionText => _targetOpened ? "close" : "open";

        private bool _moving;
        private bool _targetOpened;
        private float _angle;
        private float _targetAngle;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            ForceUpdate(opened);
        }

        public void Interact()
        {
            _targetOpened = !_targetOpened;
            _targetAngle = _targetOpened ? openAngle : 0;
            _moving = true;
        }

        private void SetAngle(float angle)
        {
            doorTransform.localEulerAngles = new Vector3(0, angle, 0);
        }

        private void ForceUpdate(bool openedState)
        {
            _targetOpened = openedState;
            _targetAngle = openedState ? openAngle : 0;
            _angle = _targetAngle;
            SetAngle(_targetAngle);
        }

        private bool FullyOpened => !_moving && _targetOpened;
        private bool FullyClosed => !_moving && !_targetOpened;

        private void ForceOpen()
        {
            ForceUpdate(true);
        }
        
        private void ForceClose()
        {
            ForceUpdate(false);
        }

        private void Update()
        {
            if (!_moving)
                return;
            
            _angle = Mathf.MoveTowards(_angle, _targetAngle, angularSpeed * Time.deltaTime);
            if (Mathf.Approximately(_angle, _targetAngle))
            {
                _angle = _targetAngle;
                _moving = false;
            }
            SetAngle(_angle);
        }

        public bool CompareState(string stateName)
        {
            return stateName switch
            {
                "FullyOpened" => FullyOpened,
                "FullyClosed" => FullyClosed,
                _ => false
            };
        }

        public void SetState(string stateName)
        {
            Debug.Log("SetState: " + stateName);
            switch (stateName)
            {
                case "FullyOpened":
                    ForceOpen();
                    break;
                case "FullyClosed":
                    ForceClose();
                    break;
            }
        }

        public void RollBack()
        {
            Init();
        }
    }
}