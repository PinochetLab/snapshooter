using System.Collections.Generic;
using Interaction;
using SnapshotUI;
using UnityEngine;
using Zenject;

namespace Snapshots
{
    public class SnapshotPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private MeshRenderer meshRenderer;

        [Inject] private SnapshotHand _snapshotHand;

        private Snapshot Snapshot { get; set; }

        public void SetSnapshot(Snapshot snapshot)
        {
            Snapshot = snapshot;

            var newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                mainTexture = snapshot.Sprite.texture
            };

            meshRenderer.material = newMaterial;
        }

        public bool CanInteract => true;
        
        public string InteractionText => "take a snapshot";

        public void Interact()
        {
            _snapshotHand.AddSnapshot(Snapshot);
            Destroy(gameObject);
        }
    }
}