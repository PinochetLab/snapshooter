using System.Collections;
using System.Collections.Generic;
using Snapshots;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnapshotUI
{
    public class SnapshotHand : MonoBehaviour
    {
        [SerializeField] private GameObject snapshotCardPrefab;
        [SerializeField] private RectTransform snapshotCardRoot;
        [SerializeField] private RectTransform activeSnapshotCardRtExample;

        [SerializeField] private Sprite sprite;

        [SerializeField] private float maxCardAngle;

        [SerializeField] private float moveSpeed = 500;
        [SerializeField] private float scrollSpeed = 2000;
        [SerializeField] private float scaleSpeed = 6000;

        [SerializeField] private float topPositionY = 500;

        private bool _isMoving;
        private bool _isBig;
        private SnapshotTransform _lastTransform;

        private readonly List<SnapshotCard> _snapshotCards = new();

        private List<SnapshotTransform> CalculateSnapshotTransforms(int snapshotCount)
        {
            var snapshotTransforms = new List<SnapshotTransform>();
            for (var i = 0; i < snapshotCount; i++)
            {
                var angle = maxCardAngle / 2 - maxCardAngle * (i + 1) / (snapshotCount + 1);
                var offset = new Vector2(
                    Random.Range(-5, 5),
                    Random.Range(-5, 5));
                snapshotTransforms.Add(new SnapshotTransform(angle, offset, 300));
            }

            return snapshotTransforms;
        }

        public void AddSnapshot(Snapshot snapshot)
        {
            StartCoroutine(AddSnapshotCor(snapshot));
        }

        private IEnumerator AddSnapshotCor(Snapshot snapshot)
        {
            _isMoving = true;

            var snapshotCard = Instantiate(snapshotCardPrefab, snapshotCardRoot).GetComponent<SnapshotCard>();
            snapshotCard.SetSnapshot(snapshot);
            _snapshotCards.Add(snapshotCard);
            var snapshotTransforms = CalculateSnapshotTransforms(_snapshotCards.Count);
            _snapshotCards[^1].SnapshotTransform = snapshotTransforms[^1];

            var moves = new List<Coroutine>();

            for (var i = 0; i < _snapshotCards.Count - 1; i++)
            {
                moves.Add(StartCoroutine(_snapshotCards[i].MoveTo(snapshotTransforms[i], moveSpeed)));
            }

            foreach (var move in moves)
            {
                yield return move;
            }

            _isMoving = false;
        }

        public bool TryGetActiveSnapshot(out Snapshot activeSnapshot)
        {
            if (!_isMoving && _isBig && _snapshotCards.Count >= 1)
            {
                activeSnapshot = _snapshotCards[^1].Snapshot;
                return true;
            }
            activeSnapshot = null;
            return false;
        }

        public void StartAlign()
        {
            _isMoving = true;
        }

        public void EndAlign()
        {
            StartCoroutine(RemoveSnapshotCor());
        }

        private IEnumerator RemoveSnapshotCor()
        {
            var card = _snapshotCards[^1];
            _snapshotCards.Remove(card);
            card.Destroy();

            _isBig = false;

            var snapshotTransforms = CalculateSnapshotTransforms(_snapshotCards.Count);
            
            var moves = new List<Coroutine>();

            for (var i = 0; i < _snapshotCards.Count - 1; i++)
            {
                moves.Add(StartCoroutine(_snapshotCards[i].MoveTo(snapshotTransforms[i], moveSpeed)));
            }

            foreach (var move in moves)
            {
                yield return move;
            }

            _isMoving = false;
        }

        private SnapshotTransform TopTransform => new(0, new Vector2(0, topPositionY), 300);

        private IEnumerator ScrollBack()
        {
            _isMoving = true;

            var lastTransform = _snapshotCards[^1].SnapshotTransform;

            var moves = new List<Coroutine>();

            for (var i = 1; i < _snapshotCards.Count; i++)
            {
                var target = _snapshotCards[i - 1].SnapshotTransform;
                moves.Add(StartCoroutine(_snapshotCards[i].MoveTo(target, moveSpeed)));
            }

            moves.Add(StartCoroutine(_snapshotCards[0].MoveToWithFlip(TopTransform, lastTransform, scrollSpeed, false)));

            foreach (var move in moves)
            {
                yield return move;
            }

            var first = _snapshotCards[0];
            _snapshotCards.RemoveAt(0);
            _snapshotCards.Add(first);

            _isMoving = false;
        }

        private IEnumerator ScrollForward()
        {
            _isMoving = true;

            var firstTransform = _snapshotCards[0].SnapshotTransform;

            var moves = new List<Coroutine>();

            for (var i = 0; i < _snapshotCards.Count - 1; i++)
            {
                var target = _snapshotCards[i + 1].SnapshotTransform;
                moves.Add(StartCoroutine(_snapshotCards[i].MoveTo(target, moveSpeed)));
            }

            moves.Add(StartCoroutine(_snapshotCards[_snapshotCards.Count - 1].MoveToWithFlip(TopTransform, firstTransform, scrollSpeed, true)));

            foreach (var move in moves)
            {
                yield return move;
            }

            var last = _snapshotCards[^1];
            _snapshotCards.RemoveAt(_snapshotCards.Count - 1);
            _snapshotCards.Insert(0, last);

            _isMoving = false;
        }

        private IEnumerator ShowHideSnapshot()
        {
            _isMoving = true;
            if (!_isBig)
            {
                _lastTransform = _snapshotCards[^1].SnapshotTransform;
                var targetAngle = activeSnapshotCardRtExample.localEulerAngles.z;
                var targetSize = activeSnapshotCardRtExample.sizeDelta.x;
                var targetOffset = new Vector2(
                    (float)Screen.width / 2 - snapshotCardRoot.anchoredPosition.x,
                    (Screen.height - targetSize) / 2 - snapshotCardRoot.anchoredPosition.y);
                var targetTransform = new SnapshotTransform(targetAngle, targetOffset, targetSize);
                yield return _snapshotCards[^1].MoveTo(targetTransform, scaleSpeed);
                _isBig = true;
            }
            else
            {
                _isBig = false;
                yield return _snapshotCards[^1].MoveTo(_lastTransform, scaleSpeed);
            }

            _isMoving = false;
        }

        private bool IsScrollForard()
        {
            return Input.GetKeyDown(KeyCode.E) || Input.mouseScrollDelta.y < 0;
        }
        
        private bool IsScrollBack()
        {
            return Input.GetKeyDown(KeyCode.Q) || Input.mouseScrollDelta.y > 0;
        }

        private bool IsShow()
        {
            return Input.GetMouseButtonDown(1);
        }
        
        private bool IsHide()
        {
            return Input.GetMouseButtonUp(1);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log(_isMoving + " " + _isBig);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddSnapshot(new Snapshot(sprite));
            }

            if (_isMoving)
                return;

            if (!_isBig && _snapshotCards.Count >= 2)
            {
                if (IsScrollForard())
                    StartCoroutine(ScrollForward());

                if (IsScrollBack())
                    StartCoroutine(ScrollBack());
            }

            if ((IsShow() || IsHide()) && _snapshotCards.Count >= 1)
            {
                StartCoroutine(ShowHideSnapshot());
            }
        }
    }
}