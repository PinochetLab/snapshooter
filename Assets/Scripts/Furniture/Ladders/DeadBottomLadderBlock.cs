using UnityEngine;
using View;

namespace Furniture.Ladders
{
    public class DeadBottomLadderBlock : MonoBehaviour
    {
        [SerializeField] private Body minBody;
        
        private float MinY => minBody.transform.position.y;

        public Vector3 Limit(Vector3 position)
        {
            if (position.y < MinY)
                position.y = MinY;
            return position;
        }
    }
}