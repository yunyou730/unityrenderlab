namespace comet.combat
{
    public abstract class BaseSys
    {
        protected World _world = null;
        
        public BaseSys(World world)
        {
            _world = world;
        }

        public abstract void OnUpdate(float deltaTime);
        // public abstract void OnTick();
    }
}