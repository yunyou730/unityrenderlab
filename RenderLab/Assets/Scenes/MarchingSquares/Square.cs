using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace ayy.ms
{
    public class Square : MonoBehaviour
    {
        [SerializeField] private float _topRightValue;
        [SerializeField] private float _bottomRightValue;
        [SerializeField] private float _bottomLeftValue;
        [SerializeField] private float _topLeftValue;

        [SerializeField] private float _borderSize = 3.0f;
        [SerializeField] private Vector3 _center = Vector3.zero;
        

        [SerializeField] private float _isoValue = 0.0f;
        
        private Vector3 _topRight;
        private Vector3 _bottomRight;
        private Vector3 _bottomLeft;
        private Vector3 _topLeft;

        private Vector3 _top;
        private Vector3 _right;
        private Vector3 _bottom;
        private Vector3 _left;
        
        private MeshFilter _meshFilter = null;
        private Mesh _mesh = null;
        
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
            
            
        }

        void Start()
        {
            
        }
        
        void Update()
        {
            CalcPointsPosition();
            int configValue = GetConfiguration();
            UpdateMesh(configValue);
        }

        private void CalcPointsPosition()
        {
            _topRight = _center + new Vector3(_borderSize * 0.5f,0,_borderSize * 0.5f);
            _bottomRight = _center + new Vector3(_borderSize * 0.5f,0,-_borderSize * 0.5f);
            _bottomLeft = _center + new Vector3(-_borderSize * 0.5f,0,-_borderSize * 0.5f);
            _topLeft = _center + new Vector3(-_borderSize * 0.5f,0,_borderSize * 0.5f);
            
            float topFactor = Mathf.Clamp01((_isoValue - _topLeftValue) / (_topRightValue - _topLeftValue));
            _top = _topLeft + (_topRight - _topLeft) * topFactor;

            float rightFactor = Mathf.Clamp01((_isoValue - _topRightValue)/(_bottomRightValue - _topRightValue));
            _right = _topRight + (_bottomRight - _topRight) * rightFactor;

            float bottomFactor = Mathf.Clamp01((_isoValue - _bottomLeftValue)/(_bottomRightValue - _bottomLeftValue));
            _bottom = _bottomLeft + (_bottomRight - _bottomLeft) * bottomFactor;
            
            float leftFactor = Mathf.Clamp01((_isoValue - _topLeftValue)/(_bottomLeftValue - _topLeftValue));
            _left = _topLeft + (_bottomLeft - _topLeft) * leftFactor;
        }

        private void UpdateMesh(int configValue)
        {
            _mesh.Clear();

            _vertices.Clear();
            _triangles.Clear();

            List<Vector3> verts = null;
            List<int> triangles = null;
            
            switch (configValue)
            {
                case 0:
                    // do nothing
                    break;
                case 1:
                    verts = new List<Vector3>(){_topRight,_right,_top};
                    triangles = new List<int>() { 0,1,2};
                    break;
                case 2:
                    verts = new List<Vector3>() { _right, _bottomRight, _bottom };
                    triangles = new List<int>() { 0,1,2};
                    break;
                case 3:
                    verts = new List<Vector3>() { _topRight, _bottomRight, _bottom, _top };
                    triangles = new List<int>() { 0, 1, 2, 0, 2, 3 };
                    break;
                case 4:
                    verts = new List<Vector3>() { _left,_bottom,_bottomLeft};
                    triangles = new List<int>() { 0,1,2};
                    break;
                case 5:
                    verts = new List<Vector3>() { _topRight, _right, _bottom, _bottomLeft, _left, _top };
                    triangles = new List<int>() { 0,1,2, 0,2,3, 0,3,4, 0,4,5};
                    break;   
                case 6:
                    verts = new List<Vector3>() { _right,_bottomRight,_bottomLeft,_left};
                    triangles = new List<int>() { 0,1,2,0,2,3};
                    break;
                case 7:
                    verts = new List<Vector3>() { _topRight, _bottomRight, _bottomLeft, _left, _top };
                    triangles = new List<int>() { 0,1,2,0,2,3,0,3,4};
                    break;
                case 8:
                    verts = new List<Vector3>() { _top,_left,_topLeft};
                    triangles = new List<int>() { 0,1,2};
                    break;
                case 9:
                    verts = new List<Vector3>() { _topRight, _right, _left, _topLeft };
                    triangles = new List<int>() { 0,1,2,0,2,3};                    
                    break;
                case 10:
                    verts = new List<Vector3>() { _top, _right, _bottomRight, _bottom,_left,_topLeft };
                    triangles = new List<int>() { 0,1,2, 0,2,3, 0,3,4, 0,4,5};   
                    break;
                case 11:
                    verts = new List<Vector3>() { _topRight,_bottomRight,_bottom,_left,_topLeft};
                    triangles = new List<int>() { 0, 1, 2, 0, 2, 3, 0, 3, 4, };
                    break;   
                case 12:
                    verts = new List<Vector3>() { _top, _bottom, _bottomLeft, _topLeft };
                    triangles = new List<int>() { 0,1,2, 0,2,3};
                    break;
                case 13:
                    verts = new List<Vector3>() { _topRight, _right, _bottom, _bottomLeft, _topLeft };
                    triangles = new List<int>() { 0,1,2, 0,2,3, 0,3,4};
                    break;
                case 14:
                    verts = new List<Vector3>() { _top, _right, _bottomRight, _bottomLeft, _topLeft };
                    triangles = new List<int>() { 0,1,2, 0,2,3, 0,3,4};                    
                    break;
                case 15:
                    verts = new List<Vector3>() { _topRight, _bottomRight, _bottomLeft, _topLeft };
                    triangles = new List<int>() { 0,1,2, 0,2,3 };
                    break;
                default:
                    break;
            }

            if (verts != null && triangles != null)
            {
                _vertices.AddRange(verts);
                _triangles.AddRange(triangles);
            }
            
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
        }
        
        private void OnDrawGizmos()
        {
            float kGizmoSizeFactor = 0.1f;
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_topRight,_borderSize * kGizmoSizeFactor);
            Gizmos.DrawSphere(_bottomRight,_borderSize * kGizmoSizeFactor);
            Gizmos.DrawSphere(_bottomLeft,_borderSize * kGizmoSizeFactor);
            Gizmos.DrawSphere(_topLeft,_borderSize * kGizmoSizeFactor);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_top,_borderSize * kGizmoSizeFactor * 0.5f);
            Gizmos.DrawSphere(_right,_borderSize * kGizmoSizeFactor * 0.5f);
            Gizmos.DrawSphere(_bottom,_borderSize * kGizmoSizeFactor * 0.5f);
            Gizmos.DrawSphere(_left,_borderSize * kGizmoSizeFactor * 0.5f);
        }

        int GetConfiguration()
        {
            int value = 0;
            if (_topRightValue > _isoValue)
                value += 1;
            if (_bottomRightValue > _isoValue)
                value += 2;
            if (_bottomLeftValue > _isoValue)
                value += 4;
            if (_topLeftValue > _isoValue)
                value += 8;
            return value;
        }
    }   
}
