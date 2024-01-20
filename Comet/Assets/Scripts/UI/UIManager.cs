using System;

namespace comet
{
    public class UIManager : IDisposable
    {
        private GridDebug _gridDebug = null;

        public void Init()
        {
            _gridDebug = new GridDebug();
            _gridDebug.Init();
        }

        public void OnGUI()
        {
            _gridDebug?.OnGUI();
        }

        public void Dispose()
        {
            _gridDebug = null;
        }
    }
}