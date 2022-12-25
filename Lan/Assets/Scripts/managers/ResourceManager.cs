using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using lan.game;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


namespace lan.managers
{
    public class ResourceManager
    {
        private ABManager _abManager = null;

        private static string kPrefix = "Assets/AssetLib/";
        private static int kPrefixLen = kPrefix.Length;

        private bool _bEnableAssetDataBase = false;

        public ResourceManager()
        {
            _abManager = Entry.AB();
            
        }
        
        public GameObject GetPrefab(string prefabPath)
        {
            GameObject result = null;
            if (_bEnableAssetDataBase)
            {
                #if UNITY_EDITOR
                result = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                #endif
            }

            if (result != null)
            {
                return result;
            }
            
            var abPath = GetABAssetPath(prefabPath);
            result = _abManager.LoadRes<GameObject>(abPath.Item1,abPath.Item2);
            return result;
        }

        Tuple<string, string> GetABAssetPath(string assetPath)
        {
            int lastSplash = assetPath.LastIndexOf('/');
            int lastDot = assetPath.LastIndexOf('.');
            
            string assetName = assetPath.Substring(lastSplash + 1,lastDot - lastSplash - 1);
            string abName = assetPath.Substring(kPrefixLen,lastSplash - kPrefixLen).ToLower();
            string[] splitPath = abName.Split('/');
            if (splitPath.Length > 2)
            {
                abName = splitPath[0] + '/' + splitPath[1];
            }

            Debug.Log("abName:" + abName);
            Debug.Log("assetName:" + assetName);
            
            Tuple<string, string> result = new Tuple<string, string>(abName,assetName);
            return result;
        }

    }

}

