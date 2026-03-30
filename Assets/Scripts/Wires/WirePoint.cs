using UnityEngine;

namespace Wires
{
    public class WirePoint : MonoBehaviour
    {
        [SerializeField] private Vector3Int offset;

        public Vector3 GetPosition(float wireRadius)
        {
            return transform.localPosition + (Vector3)offset * wireRadius;
        }
    }
}