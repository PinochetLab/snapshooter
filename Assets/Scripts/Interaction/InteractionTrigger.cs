using System;
using UnityEngine;

namespace Interaction
{
    public class InteractionTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject interactable;
        [SerializeField] private Outline outline;

        private IInteractable _interactable;

        private void Awake()
        {
            outline.enabled = false;
        }

        public IInteractable Interactable
        {
            get
            {
                _interactable ??= interactable.GetComponent<IInteractable>();
                return _interactable;
            }
        }
        
        public Outline Outline => outline;
    }
}