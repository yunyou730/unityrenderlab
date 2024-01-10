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

        public const float GridSize = 1;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        void Start()
        {
            
        }
        
        
        public void RefreshMap(MapRecord mapRecord)
        {
            SetMapRecord(mapRecord);
            Mesh mesh = CreateMapMesh();
            _meshFilter.mesh = mesh;
        }

        private void SetMapRecord(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
        }


        private Mesh CreateMapMesh()
        {
            int verticesNum = _mapRecord.Rows * _mapRecord.Cols * 4;
            var vertices = new Vector3[verticesNum];
            var uvs = new Vector2[verticesNum];
            var indices = new int[verticesNum * 6];

            int vertIdx = 0;
            int indiceIdx = 0;
            for (int row = 0;row < _mapRecord.Rows;row++)
            {
                for (int col = 0;col < _mapRecord.Cols;col++)
                {
                    // vert position
                    float baseX = col * GridSize;
                    float baseZ = row * GridSize;
                    vertices[vertIdx] = new Vector3(baseX, 0, baseZ);
                    vertices[vertIdx+1] = new Vector3(baseX + GridSize, 0, baseZ);
                    vertices[vertIdx+2] = new Vector3(baseX, 0, baseZ + GridSize);
                    vertices[vertIdx+3] = new Vector3(baseX + GridSize, 0, baseZ + GridSize);
                    
                    // vert uvs
                    uvs[vertIdx] = new Vector2(0, 0);
                    uvs[vertIdx + 1] = new Vector2(1, 0);
                    uvs[vertIdx + 2] = new Vector2(0, 1);
                    uvs[vertIdx + 3] = new Vector2(1, 1);
                    
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
            
            
            
            // var vertices = new Vector3[4];
            // vertices[0] = new Vector3(0, 0, 0);
            // vertices[1] = new Vector3(1, 0, 0);
            // vertices[2] = new Vector3(0, 0, 1);
            // vertices[3] = new Vector3(1, 0, 1);

            // var uvs = new Vector2[4];
            // uvs[0] = new Vector2(0, 0);
            // uvs[1] = new Vector2(1, 0);
            // uvs[2] = new Vector2(0, 1);
            // uvs[3] = new Vector2(1, 1);
            
            // var indices = new int[6];
            // indices[0] = 0;
            // indices[1] = 2;
            // indices[2] = 1;
            // indices[3] = 1;
            // indices[4] = 2;
            // indices[5] = 3;
            
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0,uvs);
            mesh.SetIndices(indices,MeshTopology.Triangles,0);
            
            return mesh;
        }
    }    
}


