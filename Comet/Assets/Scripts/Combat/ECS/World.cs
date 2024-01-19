using System;
using System.Collections.Generic;
using UnityEngine;

namespace comet.combat 
{
    public class World : IDisposable
    {
        private int _uuidSeed = 0;

        protected Dictionary<int, Entity> _entityMap = null;
        protected Dictionary<Type, BaseWorldComp> _worldComps = null;
        
        protected List<IUpdateSys> _updateSystems = null;
        protected List<ITickSys> _tickSystems = null;

        protected MapRecord _mapRecord = null;

        protected const int kTickFPS = 20;
        protected float _tickSpan = 1.0f / kTickFPS;
        protected float _elapsedTime = 0.0f;
        public float TickSpan
        {
            get { return _tickSpan; }
        }

        public World(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
        }

        public void Init()
        {
            _worldComps = new Dictionary<Type, BaseWorldComp>();
            _entityMap = new Dictionary<int, Entity>();
            
            _updateSystems = new List<IUpdateSys>();
            _tickSystems = new List<ITickSys>();
            
            RegisterWorldComps();
            RegisterSystems();
        }

        public void Start()
        {
            
        }

        public void OnUpdate(float deltaTime)
        {
            // Handle Tick
            _elapsedTime += deltaTime;
            if (_elapsedTime >= _tickSpan)
            {
                int ticks = (int)(_elapsedTime / _tickSpan);
                for (int i = 0;i < ticks;i++)
                {
                    OnTick();
                }
                _elapsedTime -= ticks * _tickSpan;
            }
    
            // Handle Update
            foreach (var sys in _updateSystems)
            {
                sys.OnUpdate(deltaTime);
            }
        }

        protected void OnTick()
        {
            foreach (var sys in _tickSystems)
            {
                sys.OnTick();
            }
        }

        public void Stop()
        {
            
        }

        public void Dispose()
        {
            _entityMap.Clear();
            
            _updateSystems.Clear();
            _tickSystems.Clear();
        }

        protected virtual void RegisterWorldComps()
        {
            RegisterWorldComp<MapComp>(new MapComp(_mapRecord));
            RegisterWorldComp<CreationComp>(new CreationComp());
            RegisterWorldComp<CmdComp>(new CmdComp());
            RegisterWorldComp<UserCtrlComp>(new UserCtrlComp());
        }

        protected virtual void RegisterSystems()
        {
            RegisterSys(new CreationSys(this));
            RegisterSys(new CmdSys(this));
            RegisterSys(new MovementSys(this));
        }

        protected void RegisterSys(BaseSys sys)
        {
            if (sys is ITickSys)
            {
                _tickSystems.Add((ITickSys)sys);
            }
            if (sys is IUpdateSys)
            {
                _updateSystems.Add((IUpdateSys)sys);
            }
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

        public Entity GetEntity(int uuid)
        {
            if (_entityMap.ContainsKey(uuid))
            {
                return _entityMap[uuid];
            }

            return null;
        }

        public List<Entity> GetEntities(Type[] requireComps)
        {
            List<Entity> result = new List<Entity>();
            foreach (var entity in _entityMap.Values)
            {
                bool bCheckOK = true;
                foreach (var compType in requireComps)
                {
                    if (!entity.HasComp(compType))
                    {
                        bCheckOK = false;
                        break;
                    }
                }

                if (bCheckOK)
                {
                    result.Add(entity);
                }
            }
            return result;
        }
    }
}