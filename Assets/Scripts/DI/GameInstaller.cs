using Player;
using SnapshotUI;
using UnityEngine;
using Zenject;

namespace DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private PlayerMoveController playerMotionController;
        [SerializeField] private SnapshotHand snapshotHand;

        public override void InstallBindings()
        {
            Container.Bind<PlayerMoveController>().FromInstance(playerMotionController).AsSingle();
            Container.Bind<SnapshotHand>().FromInstance(snapshotHand).AsSingle();
        }
    }
}