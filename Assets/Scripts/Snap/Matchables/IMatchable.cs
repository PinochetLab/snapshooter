namespace Snap.Matchables
{
    public interface IMatchable
    {
        public void Prepare();
        public void RollBack();
        public bool IsMatched();
    }
}