using System;
using UnityEngine;

namespace View
{
    public class Body : MonoBehaviour
    {
        [SerializeField] private Transform pivotTransform;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Transform headTransform;
        [SerializeField] protected Camera viewCamera;

        private void Awake()
        {
            viewCamera.enabled = false;
        }

        public ViewTransform ViewTransform
        {
            get
            {
                var position = pivotTransform.position;
                var yaw = bodyTransform.eulerAngles.y;
                var pitch = headTransform.localEulerAngles.x;
                return new ViewTransform(position, yaw, pitch);
            }
            set
            {
                pivotTransform.position = value.Position;
                bodyTransform.eulerAngles = Vector3.up * value.Yaw;
                headTransform.localEulerAngles = Vector3.right * value.Pitch;
            }
        }
        
        public Vector3 Forward => bodyTransform.forward;
        public Vector3 Right => bodyTransform.right;
    }
}