using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class VisionManager
    {
        private GameObject _visionPrefab = null;
        private List<VisionItem> _visionList = new List<VisionItem>();
        
        public VisionManager(GameObject prefab)
        {
            _visionPrefab = prefab;
        }

        public void RegisterVision(GameObject gameObject,float radius,float angle = 60.0f)
        {
            var item = new VisionItem();
            _visionList.Add(item);

            item.transform = gameObject.transform;
            item.radius = radius;
            item.angle = angle;

            VisionRange visionRange = GameObject.Instantiate(_visionPrefab).GetComponent<VisionRange>();
            visionRange.SetVisionParams(gameObject.transform,radius,angle,gameObject.transform.forward);
            item._visionRangeComp = visionRange;
            
            Material material = visionRange.GetComponent<MeshRenderer>().sharedMaterial;
            material.SetFloat(Shader.PropertyToID("Angle"),Mathf.Deg2Rad * angle);
            material.SetVector(Shader.PropertyToID("FrontDir"),new Vector3(0,0,1));
        }
    }
    
    public class VisionItem
    {
        public Transform transform = null;
        public float radius = 3.0f;
        public float angle = 60.0f; // degree, not radian
        public VisionRange _visionRangeComp = null;
    }
}
