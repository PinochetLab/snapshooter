using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using Players;
using Players.MoveStrategies;
using UnityEngine;
using View;
using Zenject;

namespace Furniture.Ladders
{
    public class Ladder : MonoBehaviour
    {
        public const float ExitGap = 0.2f;
        
        [Inject] private Player _player;
        [Inject] private InteractionManager _interactionManager;
        [Inject] private InteractionText _interactionText;

        private TopLadderBlock _topLadderBlock;
        private DeadTopLadderBlock _deadTopLadderBlock;
        private BottomLadderBlock _bottomLadderBlock;
        private DeadBottomLadderBlock _deadBottomLadderBlock;
        private List<MiddleLadderBlock> _middleLadderBlocks;
        
        private bool _moving;

        private bool _isTranslating;
        private ViewTransform _startVt;
        private ViewTransform _targetVt;
        private float _duration;
        private float _time;
        private bool _enter;

        private void Awake()
        {
            foreach (var ladderCollider in GetComponentsInChildren<LadderCollider>())
            {
                ladderCollider.Ladder = this;
            }

            _topLadderBlock = GetComponentInChildren<TopLadderBlock>();
            if (_topLadderBlock)
                _topLadderBlock.Ladder = this;
            
            _deadTopLadderBlock = GetComponentInChildren<DeadTopLadderBlock>();
            
            _bottomLadderBlock = GetComponentInChildren<BottomLadderBlock>();
            if (_bottomLadderBlock)
                _bottomLadderBlock.Ladder = this;
            
            _deadBottomLadderBlock = GetComponentInChildren<DeadBottomLadderBlock>();

            _middleLadderBlocks = GetComponentsInChildren<MiddleLadderBlock>().ToList();
            _middleLadderBlocks.ForEach(m => m.Ladder = this);
        }

        private void BeforeTranslatePlayer(Body targetBody)
        {
            _isTranslating = true;
            _startVt = _player.ViewTransform;
            _targetVt = targetBody.ViewTransform;
            _duration = ViewTransform.TimeDifference(_startVt, _targetVt, ViewTransformSpeed.Move);
            _time = 0;
        }

        private void ProcessTranslatePlayer()
        {
            _time += Time.deltaTime;
            if (_time > _duration)
            {
                AfterTranslatePlayer();
                return;
            }
            var t = _time / _duration;
            _player.ViewTransform = ViewTransform.Lerp(_startVt, _targetVt, t);
        }
        
        private void AfterTranslatePlayer()
        {
            _player.ViewTransform = _targetVt;
            _isTranslating = false;

            if (_enter)
            {
                _player.SetStrategy(new LadderMoveStrategy(this));
                _player.EnableMotion();
                _moving = true;
            }
            else
            {
                _interactionManager.CanInteract = true;
                _player.SetStrategy(new WalkMoveStrategy());
                _player.EnableMotion();
            }
        }

        public void Enter(Body enterBody)
        {
            _interactionText.SetActive(false);
            _interactionText.SetActionText("leave a ladder");
            _interactionManager.CanInteract = false;
            _player.DisableMotion();
            _enter = true;
            BeforeTranslatePlayer(enterBody);
        }

        private void Exit(Body exitBody)
        {
            _interactionText.SetActive(false);
            _moving = false;
            _player.DisableMotion();
            _enter = false;
            BeforeTranslatePlayer(exitBody);
        }

        private void Update()
        {
            if (_isTranslating)
            {
                ProcessTranslatePlayer();
                return;
            }
                
            if (!_moving)
                return;

            if (TopBlockLogic() || BottomBlockLogic() || _middleLadderBlocks.Any(MiddleBlockLogic))
                _interactionText.SetActive(true);
            else
                _interactionText.SetActive(false);
        }

        private bool TopBlockLogic()
        {
            if (!_topLadderBlock)
                return false;
            var position = _player.ViewTransform.Position;
            if (!_topLadderBlock.CanExit(position) || !_interactionManager.LookAtLadder(this))
                return false;
            if (Input.GetMouseButtonDown(0))
                Exit(_topLadderBlock.ExitBody);

            return true;
        }
        
        private bool BottomBlockLogic()
        {
            if (!_bottomLadderBlock)
                return false;
            var position = _player.ViewTransform.Position;
            if (!_bottomLadderBlock.CanExit(position) || !_interactionManager.LookAtLadder(this))
                return false;
            if (Input.GetMouseButtonDown(0))
                Exit(_bottomLadderBlock.ExitBody);

            return true;
        }

        private bool MiddleBlockLogic(MiddleLadderBlock block)
        {
            var position = _player.ViewTransform.Position;
            if (!block.CanExit(position) || !_interactionManager.LookAtLadder(this))
                return false;
            if (Input.GetMouseButtonDown(0))
                Exit(block.ExitBody);

            return true;
        }

        public Vector3 Limit(Vector3 position)
        {
            if (_topLadderBlock)
                position = _topLadderBlock.Limit(position);
            
            if (_deadTopLadderBlock)
                position = _deadTopLadderBlock.Limit(position);
            
            if (_bottomLadderBlock)
                position = _bottomLadderBlock.Limit(position);
            
            if (_deadBottomLadderBlock)
                position = _deadBottomLadderBlock.Limit(position);

            return position;
        }
    }
}