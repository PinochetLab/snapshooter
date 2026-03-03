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

        public override void InstallBindings()
        {
            Container.Bind<Player>().FromInstance(player).AsSingle();
            Container.Bind<SnapshotHand>().FromInstance(snapshotHand).AsSingle();
            Container.Bind<InventoryManager>().FromInstance(inventoryManager).AsSingle();
        }
    }
}