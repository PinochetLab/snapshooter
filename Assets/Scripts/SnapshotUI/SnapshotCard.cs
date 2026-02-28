using System.Collections;
using Snapshots;
using UnityEngine;
using UnityEngine.UI;

namespace SnapshotUI
{
    public class SnapshotCard : MonoBehaviour
    {
        [SerializeField] private Image snapshotImage;
        [SerializeField] private RectTransform imageRectTransform;

        private RectTransform _rectTransform;

        public Snapshot Snapshot { get; private set; }

        private SnapshotTransform _snapshotTransform;

        public SnapshotTransform SnapshotTransform
        {
            get => _snapshotTransform;
            set
            {
                _snapshotTransform = value;
                RectTransform.anchoredPosition = value.Offset;
                RectTransform.localRotation = Quaternion.Euler(0, 0, value.Angle);
                RectTransform.sizeDelta = Vector2.one * value.Size;
            }
        }

        private RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void SetSnapshot(Snapshot snapshot)
        {
            Snapshot = snapshot;
            snapshotImage.sprite = snapshot.Sprite;
        }

        public void Destroy()
        {
            DestroyImmediate(gameObject);
        }

        public IEnumerator MoveTo(SnapshotTransform snapshotTransform, float speed)
        {
            StopAllCoroutines();
            yield return MoveToTransform(snapshotTransform, speed, 0);
        }
        
        public IEnumerator MoveToWithFlip(SnapshotTransform a, SnapshotTransform b, float speed, bool backFlip)
        {
            StopAllCoroutines();
            yield return MoveToTransform(a, speed, backFlip ? 1 : -1);
            if (backFlip)
                RectTransform.SetAsFirstSibling();
            else
                RectTransform.SetAsLastSibling();
            yield return MoveToTransform(b, speed, backFlip ? 1 : -1);
        }

        private IEnumerator MoveToTransform(SnapshotTransform snapshotTransform, float speed, int flip)
        {
            var startTransform = SnapshotTransform;
            var startPosition = startTransform.Offset;
            var startAngle = imageRectTransform.localEulerAngles.z;
            var endAngle = startAngle + 180f * flip;
            var time = Vector2.Distance(startPosition, snapshotTransform.Offset) / speed;
            var dt = 0.02f;
            var n = (int)(time / dt) + 1;
            dt = time / n;
            
            for (var i = 0; i < n; i++)
            {
                var t = (float)i / n;
                SnapshotTransform = SnapshotTransform.Lerp(startTransform, snapshotTransform, t);
                var angle = Mathf.Lerp(startAngle, endAngle, t);
                imageRectTransform.localEulerAngles = Vector3.forward * angle;
                yield return new WaitForSeconds(dt);
            }

            SnapshotTransform = snapshotTransform;
            imageRectTransform.localEulerAngles = Vector3.forward * endAngle;
        }
    }
}