using TMPro;
using UnityEngine;

namespace Interaction
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private Transform lookTransform;
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private LayerMask interactionLayerMask = -1;
        [SerializeField] private TMP_Text interactionText;
        [SerializeField] private float maxDistance = 1f;
    
        private IInteractable _currentInteractable;
        private InteractionTrigger _currentTrigger;
    
        private void Update()
        {
            PerformInteractionCheck();
            HandleInteractionInput();
        }

        private void PerformInteractionCheck()
        {
            var trigger = GetInteractionTrigger();
            var interactable = trigger ? trigger.Interactable : null;
            interactionText.gameObject.SetActive(interactable != null);
            
            if (interactable != null && interactable != _currentInteractable)
                interactionText.text = $"Press  <sprite name=ml>  to {interactable.InteractionText}";
            
            if (_currentTrigger)
                _currentTrigger.Outline.enabled = false;
            
            if (trigger)
                trigger.Outline.enabled = true;
            
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

        private IInteractable GetInteractable()
        {
            var ray = new Ray(lookTransform.position, lookTransform.forward);

            if (!Physics.Raycast(ray, out var hit, interactionDistance, interactionLayerMask))
                return null;

            if (hit.distance > maxDistance)
                return null;
            
            var interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable == null)
            {
                var interactivePart =  hit.collider.GetComponent<InteractionTrigger>();
                if (interactivePart)
                    interactable = interactivePart.Interactable;
            }

            if (interactable == null)
                return null;
            
            if (!interactable.CanInteract)
                return null;

            return interactable;
        }

        private void HandleInteractionInput()
        {
            if (Input.GetMouseButtonDown(0) && _currentInteractable != null)
            {
                _currentInteractable.Interact();
            }
        }
    }

}