using Player;
using UnityEngine;

namespace Doors
{
    public class SwingDoorCollider : MonoBehaviour
    {
        [SerializeField] private SwingDoor door;
        
        private Collider _collider;
        private bool _isMoving;

        private Collider Collider
        {
            get
            {
                if (!_collider)
                    _collider = GetComponent<Collider>();
                return _collider;
            }
        }

        public void StartMove()
        {
            _isMoving = true;
            Collider.isTrigger = true;
        }
        
        public void EndMove()
        {
            _isMoving = false;
            Collider.isTrigger = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isMoving)
                return;
            if (other.CompareTag("PlayerHead"))
                door.OnPlayerEnter();
        }
    }
}