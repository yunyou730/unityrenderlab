using System;
using System.Collections.Generic;

namespace comet.combat
{
    public class Entity : IDisposable
    {
        private int _uuid;

        private Dictionary<Type, BaseComp> _compMap = null;
        
        public int UUID
        {
            get { return _uuid; }
        }
        
        public Entity(int uuid)
        {
            _uuid = uuid;
            _compMap = new Dictionary<Type, BaseComp>();
        }

        public T AttachComp<T>(T comp) where T: BaseComp
        {
            Type type = typeof(T);
            if (!_compMap.ContainsKey(type))
            {
                _compMap.Add(type,comp);
            }
            return comp;
        }

        public bool HasComp(Type type)
        {
            return _compMap.ContainsKey(type);
        }

        public T GetComp<T>() where T : BaseComp
        {
            if (_compMap.ContainsKey(typeof(T)))
            {
                return (T)_compMap[typeof(T)];
            }
            return null;
        }

        public void Dispose()
        {
            _compMap.Clear();
            _compMap = null;
        }
    }
}