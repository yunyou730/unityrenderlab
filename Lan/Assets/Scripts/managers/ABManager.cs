using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using UnityEditor.VersionControl;
using UnityEngine;
using Object = System.Object;


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
#else
            throw new Exception();
            return "";
#endif
        }

        public AssetBundle LoadBundle(string abName)
        {
            if (_loadedBundles.Keys.Contains(abName))
            {
                Debug.Log("ab:" + abName + " has been loaded,don't do that again.");
                return _loadedBundles[abName];
            }
            AssetBundle bundle = AssetBundle.LoadFromFile(_rootPath + abName);
            Debug.Assert(bundle != null,"Load AB " + abName + "is null!");
            _loadedBundles.Add(abName,bundle);

            return bundle;
        }
        
        public void UnloadBundle(string abName)
        {
            if (_loadedBundles.ContainsKey(abName))
            {
                AssetBundle bundle = _loadedBundles[abName];
                bundle.Unload(false);
            }
        }

        public void UnloadAllBundles()
        {
            foreach (var ab in _loadedBundles.Values)
            {
                ab.Unload(false);
            }
            _loadedBundles.Clear();
        }

        public bool HasBundleLoaded(string abName)
        {
            return _loadedBundles.ContainsKey(abName);
        }

        public T LoadRes<T>(string abName,string resName) where T:UnityEngine.Object
        {
            AssetBundle bundle = null;
            if (!HasBundleLoaded(abName))
            {
                bundle = LoadBundle(abName);
            }
            else
            {
                bundle = _loadedBundles[abName];
            }

            if (bundle != null)
            {
                T asset = bundle.LoadAsset<T>(resName);
                return asset;
            }

            return null;
        }

    }

}
