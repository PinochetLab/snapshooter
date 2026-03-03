using System;
using Players;
using UnityEngine;
using Zenject;

namespace Furniture.Ladders
{
    public class Ladder : MonoBehaviour
    {
        [SerializeField] private TopLadderBlock topLadderBlock;
        
        [Inject] private Player _player;

        private bool _moving;

        private void Awake()
        {
            topLadderBlock.Ladder = this;
        }

        public void StartMove()
        {
            _moving = true;
        }

        private void EndMove()
        {
            _moving = false;
        }

        private void Update()
        {
            if (!_moving)
                return;
            
            if (topLadderBlock.TryExit())
            {
                EndMove();
            }
        }
    }
}