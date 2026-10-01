using System.Collections;
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
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeInDuration = 0.2f;
        
        [Inject] private InteractionText _interactionText;
    
        private IInteractable _currentInteractable;
        private InteractionTrigger _currentTrigger;
        public bool CanInteract { get; set; } = true;
    
        private void Update()
        {
            PerformInteractionCheck();
            HandleInteractionInput();
        }

        public void FadeIn()
        {
            StartCoroutine(FadeInCor());
        }

        private IEnumerator FadeInCor()
        {
            canvasGroup.alpha = 0;

            var dt = 0.02f;
            var n = (int)(fadeInDuration / dt) + 1;
            dt = fadeInDuration / n;

            for (var i = 0; i < n; i++)
            {
                canvasGroup.alpha = (float) i / n;
                yield return new WaitForSeconds(dt);
            }

            canvasGroup.alpha = 1f;
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
            else
                _interactionText.SetActive(false);
            
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