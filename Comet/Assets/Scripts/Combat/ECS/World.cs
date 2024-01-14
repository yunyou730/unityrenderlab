using System;
using System.Collections.Generic;

namespace comet.combat 
{
    public class World : IDisposable
    {
        private int _uuidSeed = 0;

        protected Dictionary<int, Entity> _entityMap = null;
        protected List<BaseSys> _allSystems = null;
        protected Dictionary<Type, BaseWorldComp> _worldComps = null;
        
        public void Init()
        {
            _worldComps = new Dictionary<Type, BaseWorldComp>();
            _entityMap = new Dictionary<int, Entity>();
            _allSystems = new List<BaseSys>();
            
            RegisterWorldComps();
            RegisterSystems();
        }

        public void Start()
        {
            //var entity = CreateEntity();
            //entity.AttachComp(new GridPositionComp());
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var sys in _allSystems)
            {
                sys.OnUpdate(deltaTime);
            }
        }

        public void Stop()
        {
            
        }

        public void Dispose()
        {
            _entityMap.Clear();
            _allSystems.Clear();
        }

        protected virtual void RegisterWorldComps()
        {
            RegisterWorldComp<CreationComp>(new CreationComp());
        }

        protected virtual void RegisterSystems()
        {
            _allSystems.Add(new CreationSys(this));
        }


        public Entity CreateEntity()
        {
            Entity entity = null;
            _entityMap.Add(_uuidSeed,entity = new Entity(_uuidSeed));
            return entity;
        }
        
        protected T RegisterWorldComp<T>(T worldComp) where T : BaseWorldComp
        {
            _worldComps.Add(typeof(T),worldComp);
            return worldComp;
        }

        public T GetWorldComp<T>() where T : BaseWorldComp
        {
            if (_worldComps.ContainsKey(typeof(T)))
            {
                return (T)_worldComps[typeof(T)];
            }
            return null;
        }
    }
}