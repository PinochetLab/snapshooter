using Furniture.Ladders;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Interaction
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private Transform lookTransform;
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private LayerMask interactionLayerMask = -1;
        [SerializeField] private float maxDistance = 1f;
        [SerializeField] private LayerMask ladderLayerMask;
        
        [Inject] private InteractionText _interactionText;
    
        private IInteractable _currentInteractable;
        private InteractionTrigger _currentTrigger;
        public bool CanInteract { get; set; } = true;
    
        private void Update()
        {
            PerformInteractionCheck();
            HandleInteractionInput();
        }

        private void PerformInteractionCheck()
        {
            var trigger = GetInteractionTrigger();
            var interactable = trigger ? trigger.Interactable : null;

            if (CanInteract)
            {
                _interactionText.SetActive(interactable != null);
                
                if (interactable != null)
                {
                    if (interactable.CanInteract)
                        _interactionText.SetActionText(interactable.InteractionText);
                    else
                    {
                        var text = interactable.CantInteractText;
                        if (!text.IsEmpty())
                            _interactionText.SetText(text);
                    }
                }
            }
            
            _currentInteractable = interactable;
            _currentTrigger = trigger;
        }

        private InteractionTrigger GetInteractionTrigger()
        {
            var ray = new Ray(lookTransform.position, lookTransform.forward);

            if (!Physics.Raycast(ray, out var hit, interactionDistance, interactionLayerMask))
                return null;

            if (hit.distance > maxDistance)
                return null;
            
            var trigger = hit.collider.GetComponent<InteractionTrigger>();

            return trigger;
        }

        private void HandleInteractionInput()
        {
            if (!CanInteract)
                return;

            if (Input.GetMouseButtonDown(0) & _currentInteractable != null && _currentInteractable.CanInteract)
                _currentInteractable.Interact();
        }

        public bool LookAtLadder(Ladder ladder)
        {
            var ray = new Ray(lookTransform.position, lookTransform.forward);

            if (!Physics.Raycast(ray, out var hit, interactionDistance, ladderLayerMask))
                return false;
            
            var ladderCollider = hit.collider.GetComponent<LadderCollider>();

            if (hit.distance > maxDistance)
                return false;

            if (!ladderCollider)
                return false;

            if (ladderCollider.Ladder != ladder)
                return false;

            return true;
        }
    }

}