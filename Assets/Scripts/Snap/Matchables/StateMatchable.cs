using UnityEngine;

namespace Snap.Matchables
{
    public class StateMatchable : MonoBehaviour, IMatchable
    {
        [SerializeField] private GameObject statableGameObject;
        [SerializeField] private string stateName;
        
        private IStatable _statable;

        private IStatable Statable
        {
            get
            {
                _statable ??= statableGameObject.GetComponent<IStatable>();
                return _statable;
            }
        }
        
        public void Prepare()
        {
            Statable.SetState(stateName);
        }

        public void RollBack()
        {
            Statable.RollBack();
        }

        public bool IsMatched()
        {
            return Statable.CompareState(stateName);
        }

        public void BeforeAlign()
        {
            
        }

        public void AfterAlign()
        {
            
        }

        public void ProgressAlign(float t)
        {
            
        }
    }
}