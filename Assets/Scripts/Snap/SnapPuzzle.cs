using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Snapshots;
using SnapshotUI;
using UnityEngine;
using View;
using Zenject;

namespace Snap
{
    public class SnapPuzzle : MonoBehaviour
    {
        [SerializeField] private SnapViewer snapViewer;
        
        [SerializeField] private List<GameObject> objectsToShow;
        
        [SerializeField] private List<GameObject> objectsToHide;

        [SerializeField] private List<GameObject> matchables;

        [SerializeField] private SnapshotPickup snapshotPickup;

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

        private void SolveParent()
        {
            _parentSolved = true;
        }

        private void Init()
        {
            _matchables = matchables.Select(go => go.GetComponent<IMatchable>()).ToList();
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
                StartCoroutine(ProcessCapture());
        }

        private void PreCapture()
        {
            objectsToShow.ForEach(o => o.SetActive(true));
            objectsToHide.ForEach(o => o.SetActive(false));
            
            childPuzzles.ForEach(child => child.PreCapture());
        }

        private void PostCapture()
        {
            objectsToShow.ForEach(o => o.SetActive(false));
            objectsToHide.ForEach(o => o.SetActive(true));
            
            childPuzzles.ForEach(child => child.PostCapture());
        }

        private IEnumerator Capture()
        {
            yield return new WaitForSeconds(0.1f);
            
            var sprite = snapViewer.TakePicture();
            
            _snapshot = new Snapshot(sprite);
            
            snapshotPickup.SetSnapshot(_snapshot);

            foreach (var childPuzzle in childPuzzles)
            {
                yield return childPuzzle.Capture();
            }
        }

        private IEnumerator ProcessCapture()
        {
            Init();
            PreCapture();
            yield return Capture();
            PostCapture();
        }

        private void StartAlign()
        {
            _duration = snapViewer.GetTime();
            _time = 0;
            snapViewer.BeforeAlign();
            _matchables.ForEach(mt => mt.BeforeAlign());
        }

        private void EndAlign()
        {
            _aligned = true;
            snapViewer.AfterAlign();
            _matchables.ForEach(mt => mt.AfterAlign());
            
            _snapshotHand.EndAlign();
            
            objectsToShow.ForEach(o => o.SetActive(true));
            objectsToHide.ForEach(o => o.SetActive(false));

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
        }
    }
}