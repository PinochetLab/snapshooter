using Electricity;
using UnityEngine;

namespace Locks
{
    public class ElectricLock : MonoBehaviour, ILoad, ILock
    {
        private static readonly int Unlock = Animator.StringToHash("Unlock");
        [SerializeField] private GameObject lamp;
        [SerializeField] private Animator animator;
        
        [SerializeField] protected MonoBehaviour lockableTarget;
        
        private ILockable Lockable => lockableTarget as ILockable;

        private void Awake()
        {
            lamp.SetActive(false);
        }
        
        public void PowerUp()
        {
            lamp.SetActive(true);
            animator.SetTrigger(Unlock);
            Lockable.Unlock();
        }

        public void PowerDown()
        {
            lamp.SetActive(false);
        }

        public void ShowLocked()
        {
            //TODO
        }
    }
}