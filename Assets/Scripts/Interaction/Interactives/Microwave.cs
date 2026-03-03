using System.Collections.Generic;
using UnityEngine;

namespace Interaction.Interactives
{
    public class Microwave : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator;
        public string InteractionText => _opened ? "close" : "open";
        
        private static readonly int Opened = Animator.StringToHash("Opened");
        
        private bool _opened;
        
        public void Interact()
        {
            _opened = !_opened;
            animator.SetBool(Opened, _opened);
        }
    }
}