using UnityEngine;
using View;

namespace Furniture.Ladders
{
    public class DeadTopLadderBlock : MonoBehaviour
    {
        [SerializeField] private Body maxBody;
        
        private float MaxY => maxBody.transform.position.y;

        public Vector3 Limit(Vector3 position)
        {
            if (position.y > MaxY)
                position.y = MaxY;
            return position;
        }
    }
}