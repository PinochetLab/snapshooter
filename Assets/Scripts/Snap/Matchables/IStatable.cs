namespace Snap.Matchables
{
    public interface IStatable
    {
        public bool CompareState(string stateName);
        public void SetState(string stateName);
        public void RollBack();
    }
}