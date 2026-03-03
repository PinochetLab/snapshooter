using System;
using UnityEngine;

namespace Interaction
{
    public class InteractionTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject interactable;

        private IInteractable _interactable;

        public IInteractable Interactable
        {
            get
            {
                _interactable ??= interactable.GetComponent<IInteractable>();
                return _interactable;
            }
        }
    }
}