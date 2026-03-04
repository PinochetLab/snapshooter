using Interaction;
using UnityEngine;
using View;

namespace Furniture.Ladders
{
    public class MiddleLadderBlock : MonoBehaviour, IInteractable
    {
        [SerializeField] private Body enterBody;
        [SerializeField] private Body bottomBody;
        [SerializeField] private Body exitBody;
        
        public Ladder Ladder { private get; set; }

        public Body ExitBody => exitBody;
        
        private float ExitY => enterBody.transform.position.y;
        
        public string InteractionText => "use a ladder";
        
        public void Interact()
        {
            Ladder.Enter(enterBody);
        }

        public bool CanExit(Vector3 position)
        {
            return position.y > bottomBody.ViewTransform.Position.y && position.y < ExitY + Ladder.ExitGap;
        }
    }
}