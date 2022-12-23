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
            var result = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            return result;
        }
    }

}

