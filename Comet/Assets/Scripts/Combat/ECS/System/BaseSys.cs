namespace comet.combat
{
    public abstract class BaseSys
    {
        protected World _world = null;
        
        public BaseSys(World world)
        {
            _world = world;
        }
    }
    
    public interface IUpdateSys
    {
        public void OnUpdate(float deltaTime);
    }
    
    public interface ITickSys
    {
        public void OnTick();
    }

    public interface ILateUpdateSys
    {
        public void OnLateUpdate();
    }

}