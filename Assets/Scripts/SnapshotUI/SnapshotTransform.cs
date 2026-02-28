using UnityEngine;

namespace SnapshotUI
{
    public struct SnapshotTransform
    {
        public float Angle { get; private set; }
        public Vector2 Offset { get; private set; }
        public float Size { get; set; }
    
        public SnapshotTransform(float angle, Vector2 offset, float size)
        {
            Angle = angle;
            Offset = offset;
            Size = size;
        }

        public static SnapshotTransform Lerp(SnapshotTransform a, SnapshotTransform b, float t)
        {
            var angle = Mathf.LerpAngle(a.Angle, b.Angle, t);
            var offset = Vector2.Lerp(a.Offset, b.Offset, t);
            var size = Mathf.Lerp(a.Size, b.Size, t);
            var st = new SnapshotTransform(angle, offset, size);
            return st;
        }
    }

}