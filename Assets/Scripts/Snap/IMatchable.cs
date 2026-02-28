namespace Snap
{
    public interface IMatchable
    {
        public bool IsMatched();
        public void BeforeAlign();
        public void AfterAlign();
        public void ProgressAlign(float t);
    }
}