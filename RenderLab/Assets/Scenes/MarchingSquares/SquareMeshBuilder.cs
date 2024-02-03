using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ayy.ms
{
    public struct SquareMeshBuilder
    {
        private Vector3 _center;
        private float _size;

        private Vector3 _topRight;
        private Vector3 _bottomRight;
        private Vector3 _bottomLeft;
        private Vector3 _topLeft;

        private Vector3 _top;
        private Vector3 _right;
        private Vector3 _bottom;
        private Vector3 _left;

        private List<Vector3> _verts;
        private List<int> _triangles;
        
        public SquareMeshBuilder(Vector3 center,float size) : this()
        {
            _center = center;
            _size = size;
            _verts = new List<Vector3>();
            _triangles = new List<int>();
            
            InitPointsPos();
        }

        private void InitPointsPos()
        {
            _topRight = _center + new Vector3(_size * 0.5f,0,_size * 0.5f);
            _bottomRight = _center + new Vector3(_size * 0.5f,0,-_size * 0.5f);
            _bottomLeft = _center + new Vector3(-_size * 0.5f,0,-_size * 0.5f);
            _topLeft = _center + new Vector3(-_size * 0.5f,0,_size * 0.5f);
            
            _top = _topLeft + (_topRight - _topLeft) * 0.5f;
            _right = _topRight + (_bottomRight - _topRight) * 0.5f;
            _bottom = _bottomLeft + (_bottomRight - _bottomLeft) * 0.5f;
            _left = _topLeft + (_bottomLeft - _topLeft) * 0.5f;
        }
        
        public void Triangulate(float isoValue,float[] values)
        {
            Interpolate(isoValue,values);
            int conf = GetConfiguration(isoValue, values);
            Triangulate(conf);
        }

        private void Triangulate(int configValue)
        {
            _verts.Clear();
            _triangles.Clear();
            
            switch (configValue)
            {
                case 0:
                    // do nothing
                    break;
                case 1:
                    _verts.AddRange(new Vector3[]{_topRight,_right,_top});
                    _triangles.AddRange(new int[]{0,1,2});
                    break;
                case 2:
                    _verts.AddRange(new Vector3[]{_right, _bottomRight, _bottom});
                    _triangles.AddRange(new int[]{0,1,2});
                    
                    break;
                case 3:
                    _verts.AddRange(new Vector3[] { _topRight, _bottomRight, _bottom, _top });
                    _triangles.AddRange(new int[]{ 0, 1, 2, 0, 2, 3 });
                    break;
                case 4:
                    _verts.AddRange(new Vector3[]{ _left,_bottom,_bottomLeft});
                    _triangles.AddRange(new int[]{ 0,1,2});
                    break;
                case 5:
                    _verts.AddRange(new Vector3[]{ _topRight, _right, _bottom, _bottomLeft, _left, _top });
                    _triangles.AddRange(new int[]{ 0,1,2, 0,2,3, 0,3,4, 0,4,5});
                    break;   
                case 6:
                    _verts.AddRange(new Vector3[]{ _right,_bottomRight,_bottomLeft,_left});
                    _triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 }); 
                    break;
                case 7:
                    _verts.AddRange(new Vector3[]{ _topRight, _bottomRight, _bottomLeft, _left, _top });
                    _triangles.AddRange(new int []{ 0,1,2,0,2,3,0,3,4});
                    break;
                case 8:
                    _verts.AddRange(new Vector3[] { _top, _left, _topLeft });
                    _triangles.AddRange(new int[]{ 0,1,2}); 
                    break;
                case 9:
                    _verts.AddRange(new Vector3[]{ _topRight, _right, _left, _topLeft });
                    _triangles.AddRange(new int[]{0,1,2,0,2,3});
                    break;
                case 10:
                    _verts.AddRange(new Vector3[]{_top, _right, _bottomRight, _bottom,_left,_topLeft});
                    _triangles.AddRange(new int[]{0,1,2, 0,2,3, 0,3,4, 0,4,5});
                    break;
                case 11:
                    _verts.AddRange(new Vector3[]{ _topRight,_bottomRight,_bottom,_left,_topLeft}); 
                    _triangles.AddRange(new int[]{ 0, 1, 2, 0, 2, 3, 0, 3, 4, });
                    break;   
                case 12:
                    _verts.AddRange(new Vector3[]{ _top, _bottom, _bottomLeft, _topLeft });
                    _triangles.AddRange(new int[]{ 0,1,2, 0,2,3});
                    break;
                case 13:
                    _verts.AddRange(new Vector3[]{ _topRight, _right, _bottom, _bottomLeft, _topLeft });
                    _triangles.AddRange(new int[]{ 0,1,2, 0,2,3, 0,3,4});
                    break;
                case 14:
                    _verts.AddRange(new Vector3[]{ _top, _right, _bottomRight, _bottomLeft, _topLeft });
                    _triangles.AddRange(new int[]{0,1,2, 0,2,3, 0,3,4});
                    break;
                case 15:
                    _verts.AddRange(new Vector3[]{ _topRight, _bottomRight, _bottomLeft, _topLeft });
                    _triangles.AddRange(new int[]{ 0,1,2, 0,2,3 });
                    break;
                default:
                    break;
            }
        }
        
        public List<Vector3> GetVertices()
        {
            return _verts;
        }

        public List<int> GetTriangles()
        {
            return _triangles;
        }

        private void Interpolate(float isoValue,float[] values)
        {
            _top = _topLeft + (_topRight - _topLeft) * 0.5f;
            _right = _topRight + (_bottomRight - _topRight) * 0.5f;
            _bottom = _bottomLeft + (_bottomRight - _bottomLeft) * 0.5f;
            _left = _topLeft + (_bottomLeft - _topLeft) * 0.5f;
            
            float topFactor = Mathf.Clamp01((isoValue - values[3]) / (values[0] - values[3]));
            _top = _topLeft + (_topRight - _topLeft) * topFactor;

            float rightFactor = Mathf.Clamp01((isoValue - values[0])/(values[1] - values[0]));
            _right = _topRight + (_bottomRight - _topRight) * rightFactor;

            float bottomFactor = Mathf.Clamp01((isoValue - values[2])/(values[1] - values[2]));
            _bottom = _bottomLeft + (_bottomRight - _bottomLeft) * bottomFactor;
            
            float leftFactor = Mathf.Clamp01((isoValue - values[3])/(values[2] - values[3]));
            _left = _topLeft + (_bottomLeft - _topLeft) * leftFactor;            
        }

        private int GetConfiguration(float isoValue,float[] values)
        {
            int value = 0;
            if (values[0] > isoValue)
                value += 1;
            if (values[1] > isoValue)
                value += 2;
            if (values[2] > isoValue)
                value += 4;
            if (values[3] > isoValue)
                value += 8;
            return value;
        }
        
    }
}
