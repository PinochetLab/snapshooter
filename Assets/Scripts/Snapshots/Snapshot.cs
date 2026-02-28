using UnityEngine;

namespace Snapshots
{
    public class Snapshot
    {
        private static int _nextId;

        public int Id { get; private set; }

        public Sprite Sprite { get; private set; }

        public Snapshot(Sprite sprite)
        {
            Id = _nextId++;
            Sprite = sprite;
        }
    }
}