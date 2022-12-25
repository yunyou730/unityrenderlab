using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;


namespace lan.managers
{
    public class ABManager
    {
        private Dictionary<string, AssetBundle> _loadedBundles;

        private AssetBundle _mainBundle = null;
        private AssetBundleManifest _mainManifest = null;

        public string _rootPath;
        public string _platformKey;

        public ABManager(string rootPath)
        {
            if (!rootPath.EndsWith("/"))
            {
                rootPath = rootPath + "/";
            }

            _loadedBundles = new Dictionary<string, AssetBundle>();

            _rootPath = rootPath;
            _platformKey = GetPlatformKey();

            _mainBundle = AssetBundle.LoadFromFile(_rootPath + _platformKey);
            Debug.Assert(_mainBundle != null,"main bundle is null!");
            //_mainManifest = _mainBundle.mainAsset.ge;
        }
        
        private string GetPlatformKey()
        {
#if UNITY_STANDALONE_OSX
            return "OSX";
#elif UNITY_STANDALONE_WIN
            return "WIN";
#elif UNITY_IOS
            return "iOS";
#elif UNITY_ANROID
            return "Android";
#endif
            throw new Exception();
            return "";
        }

        bool LoadAB(string abName)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(abName);
            if (bundle == null)
                return false;

            
            
            return true;
        }

        void UnloadAB(string abName)
        {
            
        }

        void UnloadAllAB()
        {
            foreach (var ab in _loadedBundles.Values)
            {
                
            }
            _loadedBundles.Clear();
        }

    }

}
