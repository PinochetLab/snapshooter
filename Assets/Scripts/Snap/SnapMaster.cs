using UnityEngine;

namespace Snap
{
    public static class SnapMaster
    {
        private const float SnapshotHeightFraction = 0.74f;

        public static int SnapshotSize => (int)(SnapshotHeightFraction * Screen.height);
    }
}