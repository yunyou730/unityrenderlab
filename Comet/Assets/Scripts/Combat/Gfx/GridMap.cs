using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace comet.combat
{
    public class GridMap : MonoBehaviour
    {
        private MapRecord _mapRecord = null;

        private MeshFilter _meshFilter = null;
        private MeshRenderer _meshRenderer = null;

        private float _gridSize = 1.0f;
        // private float _basePositionY = 0.0f;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        public void RefreshWithMapRecord(MapRecord mapRecord,float gridSize)
        {
            // hold data
            SetMapRecord(mapRecord);
            _gridSize = gridSize;
            
            // refresh gfx
            Mesh mesh = CreateMapMesh();
            _meshFilter.mesh = mesh;
        }

        private void SetMapRecord(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
        }
        
        private Mesh CreateMapMesh()
        {
            Vector3[] vertices;
            Vector2[] uvs;
            int[] indices;
            Color[] colors;
            
            GenerateMeshData(out vertices,out uvs,out indices,out colors);
            
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0,uvs);
            mesh.SetIndices(indices,MeshTopology.Triangles,0);
            mesh.SetColors(colors);
            
            return mesh;
        }

        private void GenerateMeshData(out Vector3[] vertices,out Vector2[] uvs,out int[] indices,out Color[] colors)
        {
            int verticesNum = _mapRecord.Rows * _mapRecord.Cols * 4;
            vertices = new Vector3[verticesNum]; 
            uvs = new Vector2[verticesNum];
            indices = new int[verticesNum * 6];
            colors = new Color[verticesNum];
            
            int vertIdx = 0;
            int indiceIdx = 0;
            for (int row = 0;row < _mapRecord.Rows;row++)
            {
                for (int col = 0; col < _mapRecord.Cols; col++)
                {
                    // decide height of grid each vertices
                    GridRecord gridRecord = _mapRecord.GetGridAt(row, col);

                    GridRecord north = _mapRecord.GetGridAt(row + 1, col);
                    GridRecord westNorth = _mapRecord.GetGridAt(row + 1, col - 1);
                    GridRecord eastNorth = _mapRecord.GetGridAt(row + 1, col + 1);
                    GridRecord west = _mapRecord.GetGridAt(row, col - 1);
                    GridRecord east = _mapRecord.GetGridAt(row, col + 1);
                    GridRecord south = _mapRecord.GetGridAt(row - 1, col);
                    GridRecord westSouth = _mapRecord.GetGridAt(row - 1, col - 1);
                    GridRecord eastSouth = _mapRecord.GetGridAt(row - 1, col + 1);

                    float y0 = GetGridsMaxHeight(gridRecord, west, westSouth, south);
                    float y1 = GetGridsMaxHeight(gridRecord, east, eastSouth, south);
                    float y2 = GetGridsMaxHeight(gridRecord, west, westNorth, north);
                    float y3 = GetGridsMaxHeight(gridRecord, east, eastNorth, north);
                    // y0 = y1 = y2 = y3 = gridRecord.Height 

                    // vert position
                    float baseX = col * _gridSize;
                    float baseZ = row * _gridSize;
                    vertices[vertIdx] = new Vector3(baseX, y0, baseZ);
                    vertices[vertIdx + 1] = new Vector3(baseX + _gridSize, y1, baseZ);
                    vertices[vertIdx + 2] = new Vector3(baseX, y2, baseZ + _gridSize);
                    vertices[vertIdx + 3] = new Vector3(baseX + _gridSize, y3, baseZ + _gridSize);

                    // vert uvs
                    uvs[vertIdx] = new Vector2(0, 0);
                    uvs[vertIdx + 1] = new Vector2(1, 0);
                    uvs[vertIdx + 2] = new Vector2(0, 1);
                    uvs[vertIdx + 3] = new Vector2(1, 1);

                    // vert color , pass Grid Type data to vertices attribute
                    Color color = Color.white;
                    
                    switch (gridRecord.GridType)
                    {
                        case GridRecord.EGridType.Grass:
                            color = Color.green;
                            break;
                        case GridRecord.EGridType.Plane:
                            color = Color.yellow;
                            break;
                        case GridRecord.EGridType.Water:
                            color = Color.cyan;
                            break;
                    }
                    colors[vertIdx] = colors[vertIdx + 1] = colors[vertIdx + 2] = colors[vertIdx + 3] = color;


                    // indices
                    indices[indiceIdx++] = vertIdx;
                    indices[indiceIdx++] = vertIdx + 2;
                    indices[indiceIdx++] = vertIdx + 1;
                    indices[indiceIdx++] = vertIdx + 1;
                    indices[indiceIdx++] = vertIdx + 2;
                    indices[indiceIdx++] = vertIdx + 3;
                    
                    // next vert group 
                    vertIdx += 4;
                }
            }
        }

        private float GetGridsMaxHeight(GridRecord g,GridRecord g1,GridRecord g2,GridRecord g3)
        {
            float h = g.Height;
            if (g1 != null && g1.Height > h) h = g1.Height;
            if (g2 != null && g2.Height > h) h = g2.Height;
            if (g3 != null && g3.Height > h) h = g3.Height;
            return h;
        }

        public Rect GetBounds()
        {
            Rect rect = Rect.zero;
            rect.x = 0;
            rect.y = 0;
            rect.width = _mapRecord.Cols * _gridSize;
            rect.height = _mapRecord.Rows * _gridSize;
            return rect;
        }
    }    
}


