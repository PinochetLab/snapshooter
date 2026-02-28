using UnityEngine;

namespace View
{
    [System.Serializable]
    public class Tolerance
    {
        [SerializeField] private float distance = 0.1f;
        [SerializeField] private float angle = 10f;

        public float Distance => distance;
        public float Angle => angle;
    }
}