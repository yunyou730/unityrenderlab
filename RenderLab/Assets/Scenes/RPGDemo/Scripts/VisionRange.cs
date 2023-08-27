using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class VisionRange : MonoBehaviour
    {
        [SerializeField][Range(1,20)] 
        float _radius;
        
        [SerializeField][Range(30,120)] 
        public float _angle;
        
        public Transform _parent = null;
        private Vector3 _faceDir = Vector3.one;
         
        void Start()
        {
            transform.SetParent(_parent);
            transform.localPosition = new Vector3(0, 0.1f, 0);
            transform.localScale = new Vector3(_radius * 2.0f, _radius * 2.0f, 1);
        }
        
        public void SetVisionParams(Transform parent,float radius,float angle,Vector3 faceDir)
        {
            _parent = parent;
            _radius = radius;
            _angle = angle;
            _faceDir = faceDir;
        }
        
        public void SyncRadius()
        {
            transform.localScale = new Vector3(_radius * 2.0f, _radius * 2.0f, 1);
        }
    }    
}

