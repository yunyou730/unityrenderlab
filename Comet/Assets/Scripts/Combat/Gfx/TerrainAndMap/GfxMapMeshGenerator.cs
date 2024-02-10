using System;
using UnityEngine;

namespace comet.combat
{
    public class GfxMapMeshGenerator : IDisposable
    {
        private MapRecord _mapRecord = null;
        private float _gridSize = 0.0f;
        
        public GfxMapMeshGenerator(MapRecord mapRecord,float gridSize)
        {
            _mapRecord = mapRecord;
            _gridSize = gridSize;
        }

        public Mesh CreateMapMesh()
        {
            Vector3[] vertices;
            Vector2[] uvs;
            int[] indices;
            Color[] colors;
            Vector2[] uvs2;
            Vector2[] uvs3;
            
            GenerateMeshData(out vertices,out uvs,out uvs2,out uvs3,out indices,out colors);
            
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0,uvs);     // uv:  uv in one Grid
            mesh.SetUVs(1,uvs2);    // uv2: uv in whole mesh, in size of gridRows x gridCols 
            mesh.SetUVs(2,uvs3);    // uv3: uv in whole mesh, in size of pointRows x pointCols
            mesh.SetIndices(indices,MeshTopology.Triangles,0);
            mesh.SetColors(colors);
            
            return mesh;
        }
        
        private void GenerateMeshData(out Vector3[] vertices,
            out Vector2[] uvs,
            out Vector2[] uvs2,
            out Vector2[] uvs3,
            out int[] indices,
            out Color[] colors)
        {
            int verticesNum = _mapRecord.GridRows * _mapRecord.GridCols * 4;
            vertices = new Vector3[verticesNum]; 
            indices = new int[verticesNum * 6];
            colors = new Color[verticesNum];
            
            uvs = new Vector2[verticesNum];
            uvs2 = new Vector2[verticesNum];
            uvs3 = new Vector2[verticesNum];
            
            
            int vertIdx = 0;
            int indiceIdx = 0;
            for (int row = 0;row < _mapRecord.GridRows;row++)
            {
                for (int col = 0; col < _mapRecord.GridCols; col++)
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
                    
                    
                    /*
                     * 2 - 3
                     * |   |
                     * 0 - 1 
                     */

                    // vert position
                    float baseX = col * _gridSize;
                    float baseZ = row * _gridSize;
                    vertices[vertIdx] = new Vector3(baseX, y0, baseZ);
                    vertices[vertIdx + 1] = new Vector3(baseX + _gridSize, y1, baseZ);
                    vertices[vertIdx + 2] = new Vector3(baseX, y2, baseZ + _gridSize);
                    vertices[vertIdx + 3] = new Vector3(baseX + _gridSize, y3, baseZ + _gridSize);
                    
                    // vert uvs 1 , uv in grid
                    uvs[vertIdx] = new Vector2(0, 0);
                    uvs[vertIdx + 1] = new Vector2(1, 0);
                    uvs[vertIdx + 2] = new Vector2(0, 1);
                    uvs[vertIdx + 3] = new Vector2(1, 1);
                    
                    // vert uvs 2 , uv in whole mesh, gridRows x gridCols
                    uvs2[vertIdx] = new Vector2(
                        (float)col / (float)_mapRecord.GridCols,
                        (float)row / (float)_mapRecord.GridRows);
                    uvs2[vertIdx + 1] = new Vector2(
                        (float)(col + 1) / (float)_mapRecord.GridCols,
                        (float)row / (float)_mapRecord.GridRows);
                    uvs2[vertIdx + 2] = new Vector2(
                        (float)(col) / (float)_mapRecord.GridCols,
                        (float)(row + 1)/ (float)_mapRecord.GridRows);                    
                    uvs2[vertIdx + 3] = new Vector2(
                        (float)(col + 1) / (float)_mapRecord.GridCols,
                        (float)(row + 1)/ (float)_mapRecord.GridRows);  
                    
                    // vert uvs 3, uv in whole mesh, pointRows x pointCols
                    uvs3[vertIdx] = new Vector2(
                        (float)col / (float)_mapRecord.PointsInCol,
                        (float)row / (float)_mapRecord.PointsInRow);
                    uvs3[vertIdx + 1] = new Vector2(
                        (float)(col + 1) / (float)(_mapRecord.PointsInCol),
                        (float)(row) / (float)(_mapRecord.PointsInRow)
                    );
                    uvs3[vertIdx + 2] = new Vector2(
                        (float)col / (float)(_mapRecord.PointsInCol),
                        (float)(row + 1) / (float)(_mapRecord.PointsInRow));
                    uvs3[vertIdx + 3] = new Vector2(
                        (float)(col + 1) / (float)(_mapRecord.PointsInCol),
                        (float)(row + 1) / (float)(_mapRecord.PointsInRow));
                    

                    // vert color , pass Grid Type data to vertices attribute
                    Color color = Color.white;
                    
                    switch (gridRecord.GridType)
                    {
                        case EGridType.Grass:
                            color = Color.green;
                            break;
                        case EGridType.Ground:
                            color = Color.yellow;
                            break;
                        case EGridType.Wall:
                            color = Color.gray;
                            break;
                        case EGridType.Water:
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

        public void Dispose()
        {
            //_mapRecord?.Dispose();
        }
    }
}