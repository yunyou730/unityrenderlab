using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using ayy.ms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.XR;
using Random = UnityEngine.Random;

namespace ayy.marchingsquare
{
    class FieldRecord
    {
        public float Value;
    }

    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private bool _enableGizmo = false;
        [SerializeField] private bool _enableInterpolate = true;
        [SerializeField] private bool _buildMode = false;   // whether we build mesh from zero,or clip from full mesh
        
        [SerializeField] private int _rows = 50;
        [SerializeField] private int _cols = 50;
        [SerializeField] private float _gridSize = 1.0f;

        [SerializeField] private int _brushSize = 1;
        [SerializeField] private float _brushStrength = 1.0f;

        [SerializeField] private float _isoValue = 0.5f;
        
        private GameObject _terrainGameObject = null;
        private MeshFilter _terrainMeshFilter = null;
        private Mesh _terrainMesh = null;
        
        private SquareMeshBuilder[,] _squares = null;
        private FieldRecord[,] _fields = null;

        private Vector3? _prevMousePos = null;
        
        void Awake()
        {
            _terrainGameObject = transform.Find("Terrain").gameObject;
            _terrainMeshFilter = _terrainGameObject.GetComponent<MeshFilter>();
            _terrainMesh = new Mesh();
            _terrainMeshFilter.sharedMesh = _terrainMesh;
            
            InitGridsAndSquares();
        }
        
        void Start()
        {
            BuildTerrainMesh();
        }
        
        void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit))
                {
                    Vector2Int gridPos = WorldPosToGridPos(hit.point);
                    Debug.Log("[" + gridPos + "]");
                    if (gridPos.x >= 0 && gridPos.x < _cols && gridPos.y >= 0 && gridPos.y < _rows)
                    {
                        HandleBrushGrid(gridPos.x,gridPos.y);
                        //BuildTerrainMesh();
                    }
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                _prevMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                Vector3 delta = Input.mousePosition - _prevMousePos.Value;
                delta *= -Time.deltaTime * 3.0f;
                Camera.main.transform.position += new Vector3(delta.x,0,delta.y);
        
                _prevMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(2))
            {
                _prevMousePos = null;
            }
            
            BuildTerrainMesh();
            
        }

        private void HandleBrushGrid(int x,int y)
        {
            for (int gridY = y - _brushSize;gridY <= y + _brushSize;gridY++)
            {
                for (int gridX = x - _brushSize;gridX <= x + _brushSize;gridX++)
                {
                    if (gridY >= 0 && gridY <= _rows && gridX >= 0 && gridX <= _cols)
                    {
                        FieldRecord field = _fields[gridY, gridX];
                        if (_buildMode)
                            field.Value += _brushStrength;
                        else
                            field.Value -= _brushStrength;    
                    }
                }
            }
        }

        private void InitGridsAndSquares()
        {
            _fields = new FieldRecord[_rows + 1, _cols + 1];
            _squares = new SquareMeshBuilder[_rows, _cols];
            for (int y = 0;y < _rows;y++)
            {
                for (int x = 0;x < _cols;x++)
                {
                    var square = new SquareMeshBuilder(GridPosToWorldPos(x,y),_gridSize);
                    _squares[y, x] = square;
                }
            }
            
            for (int y = 0;y <= _rows;y++)
            {
                for (int x = 0;x <= _cols;x++)
                {
                    _fields[y, x] = new FieldRecord();
                    _fields[y, x].Value = _buildMode ? 0.0f : 10.0f; 
                }
            }
        }

        Vector3 GridPosToWorldPos(int x,int y)
        {
            Vector3 pos = new Vector3(x * _gridSize + 0.5f * _gridSize,0,y * _gridSize + 0.5f * _gridSize);
            return pos;
        }

        Vector2Int WorldPosToGridPos(Vector3 worldPos)
        {
            int x = (int)(worldPos.x / _gridSize);
            int y = (int)(worldPos.z / _gridSize);
            return new Vector2Int(x,y);
        }

        private void BuildTerrainMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            
            for (int y = 0;y < _rows;y++)
            {
                for (int x = 0;x < _cols;x++)
                {
                    SquareMeshBuilder square = _squares[y, x];
                    float[] values = new float[4];
                    values[0] = _fields[y + 1, x + 1].Value;
                    values[1] = _fields[y, x + 1].Value;
                    values[2] = _fields[y, x].Value;
                    values[3] = _fields[y + 1, x].Value;
                    square.Triangulate(_isoValue,values,_enableInterpolate);
                    
                    List<Vector3> gridVertices = square.GetVertices();
                    List<int> gridTriangles = square.GetTriangles();
                    
                    // map Square triangle index to whole mesh triangle index 
                    int baseVertIndex = vertices.Count;
                    for(int i = 0;i < gridTriangles.Count;i++)
                    {
                        int countedTriangleIndex = baseVertIndex + gridTriangles[i];
                        gridTriangles[i] = countedTriangleIndex;
                    }
                    
                    // Do Add to result vertices data 
                    vertices.AddRange(gridVertices);
                    triangles.AddRange(gridTriangles);
                }
            }
            
            _terrainMesh.Clear();
            _terrainMesh.vertices = vertices.ToArray();
            _terrainMesh.triangles = triangles.ToArray();
        }

        private void OnDestroy()
        {
            GameObject.Destroy(_terrainMesh);
        }

        private void OnDrawGizmos()
        {
            if (!_enableGizmo)
                return;
            
            Gizmos.color = Color.red;
            for (int y = 0;y <= _rows;y++)
            {
                for (int x = 0;x <= _cols;x++)
                {
                    Vector3 worldPos = GridPosToWorldPos(x, y);
                    worldPos.x -= _gridSize * 0.5f;
                    worldPos.z -= _gridSize * 0.5f;
                    
                    Gizmos.DrawSphere(worldPos,_gridSize * 0.2f);
                    if (EditorApplication.isPlaying)
                    {
                        FieldRecord fieldRecord = _fields[y, x];
                        Handles.Label(worldPos + Vector3.up * _gridSize * 0.4f,fieldRecord.Value.ToString());
                    }
                }
            }
        }
    }

}
