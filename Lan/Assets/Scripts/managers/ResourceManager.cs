using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace lan.managers
{
    public class ResourceManager
    {
        public GameObject GetPrefab(string prefabPath)
        {
            GameObject result = null;
            #if UNITY_EDITOR
            result = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            #endif
            return result;
        }
    }

}

