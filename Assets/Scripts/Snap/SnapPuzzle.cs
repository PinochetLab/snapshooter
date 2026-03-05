using Snap.Matchables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Snapshots;
using SnapshotUI;
using UnityEngine;
using UnityEngine.Serialization;
using View;
using Zenject;

namespace Snap
{
    public class SnapPuzzle : MonoBehaviour
    {
        [SerializeField] private SnapViewer snapViewer;
        
        [SerializeField] private List<GameObject> appearingObjects;
        
        [SerializeField] private List<GameObject> disappearingObjects;
        
        [SerializeField] private List<GameObject> objectsToHide;

        [SerializeField] private List<GameObject> matchables;

        [FormerlySerializedAs("snapshotPickup")] [SerializeField] private SnapshotPickable snapshotPickable;

        [SerializeField] private bool isRoot;

        [SerializeField] private List<SnapPuzzle> childPuzzles;
        
        [Inject] private SnapshotHand _snapshotHand;

        private bool _solved;
        private bool _aligned;
        private float _duration;
        private float _time;
        private Snapshot _snapshot;
        private bool _parentSolved;
        private List<IMatchable> _matchables;
        private List<IAlignedMatchable> _alignedMatchables;
        private List<bool> _lastActive;

        private void SolveParent()
        {
            _parentSolved = true;
        }

        private void Init()
        {
            _matchables = matchables.Select(go => go.GetComponent<IMatchable>()).ToList();
            _alignedMatchables = matchables
                .Select(go => go.GetComponent<IAlignedMatchable>())
                .Where(component => component != null)
                .ToList();
            _lastActive = objectsToHide.Select(o => o.activeSelf).ToList();
        }

        private void Update()
        {
            if (!isRoot && !_parentSolved)
                return;
            if (!_solved)
                TrySolve();
            else if (!_aligned)
                ProgressAlign();
        }

        private void TrySolve()
        {
            if (_snapshotHand.TryGetActiveSnapshot(out var activeSnapshot)
                && activeSnapshot == _snapshot
                && snapViewer.IsMatched()
                && _matchables.All(mt => mt.IsMatched()))
            {
                _solved = true;
                _snapshotHand.StartAlign();
                StartAlign();
            }
        }

        private void Start()
        {
            if (isRoot)
                StartCoroutine(Capture(true));
        }

        private void PreCapture()
        {
            appearingObjects.ForEach(o => o.SetActive(true));
            disappearingObjects.ForEach(o => o.SetActive(false));
            objectsToHide.ForEach(o => o.SetActive(false));
            
            _matchables.ForEach(m => m.Prepare());
        }

        private void PostCapture()
        {
            appearingObjects.ForEach(o => o.SetActive(false));
            disappearingObjects.ForEach(o => o.SetActive(true));

            for (var i = 0; i < objectsToHide.Count; i++)
            {
                objectsToHide[i].SetActive(_lastActive[i]);
            }
            
            _matchables.ForEach(m => m.RollBack());
        }

        private IEnumerator Capture(bool wait = false)
        {
            Init();
            
            if (wait)
                yield return new WaitForSeconds(0.5f);
            
            PreCapture();
            
            foreach (var childPuzzle in childPuzzles)
            {
                yield return childPuzzle.Capture();
            }
            
            PreCapture();
            
            yield return new WaitForEndOfFrame();
            
            var sprite = snapViewer.TakePicture();
            
            PostCapture();
            
            _snapshot = new Snapshot(sprite);
            
            snapshotPickable.SetSnapshot(_snapshot);
        }

        private void StartAlign()
        {
            _duration = snapViewer.GetTime();
            _time = 0;
            snapViewer.BeforeAlign();
            _alignedMatchables.ForEach(mt => mt.BeforeAlign());
        }

        private void EndAlign()
        {
            _aligned = true;
            snapViewer.AfterAlign();
            _alignedMatchables.ForEach(mt => mt.AfterAlign());
            
            appearingObjects.ForEach(o => o.SetActive(true));
            disappearingObjects.ForEach(o => o.SetActive(false));
            
            _snapshotHand.EndAlign();

            foreach (var childPuzzle in childPuzzles)
            {
                childPuzzle.SolveParent();
            }
        }

        private void ProgressAlign()
        {
            _time += Time.deltaTime;
            if (_time >= _duration)
            {
                EndAlign();
                return;
            }
            var t = _time / _duration;
            snapViewer.ProgressAlign(t);
            _alignedMatchables.ForEach(mt => mt.ProgressAlign(t));
        }
    }
}