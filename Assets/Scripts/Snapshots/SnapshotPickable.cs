using System.Collections.Generic;
using Interaction;
using SnapshotUI;
using UnityEngine;
using Zenject;

namespace Snapshots
{
    public class SnapshotPickable : AbstractPickable
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

        protected override string PickableName => "snapshot";
        
        protected override void Pick()
        {
            _snapshotHand.AddSnapshot(Snapshot);
        }
    }
}