using Interaction;
using UnityEngine;
using View;

namespace Furniture.Ladders
{
    public class BottomLadderBlock : MonoBehaviour, IInteractable
    {
        [SerializeField] private Body enterBody;
        [SerializeField] private Body exitBody;
        
        public Ladder Ladder { private get; set; }

        public Body ExitBody => exitBody;
        
        private float ExitY => enterBody.transform.position.y;
        
        public string InteractionText => "use a ladder";
        
        public void Interact()
        {
            Ladder.Enter(enterBody);
        }

        public Vector3 Limit(Vector3 position)
        {
            if (position.y < ExitY)
                position.y = ExitY;
            return position;
        }

        public bool CanExit(Vector3 position)
        {
            return position.y < ExitY + Ladder.ExitGap;
        }
    }
}