using System;
using UnityEngine;

namespace comet.res
{
    public class ResManager : IDisposable
    {
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            var result = Resources.Load<T>(path);
            return result;
        }

        public void Dispose()
        {
            
        }
    }
}