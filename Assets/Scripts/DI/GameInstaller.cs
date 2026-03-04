using Interaction;
using Inventory;
using Players;
using SnapshotUI;
using UnityEngine;
using Zenject;

namespace DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Player player;
        [SerializeField] private SnapshotHand snapshotHand;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private InteractionManager interactionManager;
        [SerializeField] private InteractionText interactionText;

        public override void InstallBindings()
        {
            Container.Bind<Player>().FromInstance(player).AsSingle();
            Container.Bind<SnapshotHand>().FromInstance(snapshotHand).AsSingle();
            Container.Bind<InventoryManager>().FromInstance(inventoryManager).AsSingle();
            Container.Bind<InteractionManager>().FromInstance(interactionManager).AsSingle();
            Container.Bind<InteractionText>().FromInstance(interactionText).AsSingle();
        }
    }
}