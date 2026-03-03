using System.Collections.Generic;

namespace Interaction
{
    public interface IInteractable
    {
        bool CanInteract => true;
        string CantInteractMessage => "";
        string InteractionText { get; }
        void Interact();
    }
}