using Interaction;
using Players;
using Players.MoveStrategies;
using UnityEngine;
using View;
using Zenject;

namespace Furniture.Ladders
{
    public class TopLadderBlock : MonoBehaviour, IInteractable
    {
        [SerializeField] private Body enterBody;
        [SerializeField] private Body exitBody;

        [Inject] private Player _player;
        
        public Ladder Ladder { private get; set; }
        
        private float ExitY => enterBody.transform.position.y;
        
        public string InteractionText => "use a ladder";
        
        public void Interact()
        {
            Enter();
        }

        private void Enter()
        {
            _player.DisablePhysics();
            _player.ViewTransform = enterBody.ViewTransform;
            _player.SetStrategy(new LadderMoveStrategy());
            Ladder.StartMove();
        }
        
        private void Exit()
        {
            _player.DisablePhysics();
            _player.ViewTransform = exitBody.ViewTransform;
            _player.SetStrategy(new WalkMoveStrategy());
        }

        public bool TryExit()
        {
            var y = _player.ViewTransform.Position.y;
            if (y <= ExitY) return false;
            Exit();
            return true;

        }
    }
}