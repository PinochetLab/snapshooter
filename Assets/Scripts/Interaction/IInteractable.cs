using System.Collections.Generic;

namespace Interaction
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        string InteractionText { get; }
        void Interact();
    }
}