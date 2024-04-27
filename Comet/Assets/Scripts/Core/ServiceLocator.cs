using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace comet.core
{
    public class ServiceLocator
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public T Get<T>()
        {
            if (!_services.ContainsKey(typeof(T)))
            {
                string error = $"{typeof(T)} is not registered with {GetType().Name}";
                throw new InvalidOperationException(error);
            }
            return (T)_services[typeof(T)];
        }

        public T Register<T>(T service)
        {
            _services.Add(typeof(T),service);
            return service;
        }
    }
    
}

