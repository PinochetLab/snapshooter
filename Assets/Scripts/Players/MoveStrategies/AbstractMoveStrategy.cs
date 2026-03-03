namespace Players.MoveStrategies
{
    public abstract class AbstractMoveStrategy
    {
        protected Player Player;

        public void Init(Player player)
        {
            Player = player;
            Init();
        }

        protected abstract void Init();
        public abstract void ProcessInput();
        public abstract void Update();
        public abstract void FixedUpdate();
    }
}