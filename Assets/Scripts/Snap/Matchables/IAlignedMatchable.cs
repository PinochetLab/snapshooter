namespace Snap.Matchables
{
    public interface IAlignedMatchable : IMatchable
    {
        public void BeforeAlign();
        public void AfterAlign();
        public void ProgressAlign(float t);
    }
}