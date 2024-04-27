using System;
using UnityEngine;
using UnityEngine.UI;

namespace comet.combat
{
    public class GfxMapMeshGenerator : IDisposable
    {
        private MapRecord _mapRecord = null;
        private float _gridSize = 0.0f;

        private Vector3[] _verticesBuffer;
        private Vector2[] _uvsBuffer;
        private int[] _indicesBuffer;
        private Color[] _colorsBuffer;
        private Vector2[] _uvs2Buffer;
        private Vector2[] _uvs3Buffer;
        
        public GfxMapMeshGenerator(MapRecord mapRecord,float gridSize)
        {
            _mapRecord = mapRecord;
            _gridSize = gridSize;
        }

        public Mesh CreateMapMesh()
        {
            GenerateMeshData(out _verticesBuffer,
                out _uvsBuffer,
                out _uvs2Buffer,
                out _uvs3Buffer,
                out _indicesBuffer,
                out _colorsBuffer);
            
            Mesh mesh = new Mesh();
            mesh.SetVertices(_verticesBuffer);
            mesh.SetUVs(0,_uvsBuffer);     // uv:  uv in one Grid
            mesh.SetUVs(1,_uvs2Buffer);    // uv2: uv in whole mesh, in size of gridRows x gridCols 
            mesh.SetUVs(2,_uvs3Buffer);    // uv3: uv in whole mesh, in size of pointRows x pointCols
            mesh.SetIndices(_indicesBuffer,MeshTopology.Triangles,0);
            mesh.SetColors(_colorsBuffer);
            
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
                    float y0, y1, y2, y3;
                    GetGridCornersHeight(row,col,out y0,out y1,out y2,out y3);
                    
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
                    GridRecord gridRecord = _mapRecord.GetGridAt(row, col);
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
        
        public void AdjustTerrainMeshVerticesHeight(Mesh mesh)
        {
            int vertIdx = 0;
            for (int row = 0; row < _mapRecord.GridRows; row++)
            {
                for (int col = 0; col < _mapRecord.GridCols; col++)
                {
                    float y0, y1, y2, y3;
                    GetGridCornersHeight(row,col,out y0,out y1,out y2,out y3);
                    /*
                     * 2 - 3
                     * |   |
                     * 0 - 1
                     */
                    // vert position
                    float baseX = col * _gridSize;
                    float baseZ = row * _gridSize;
                    
                    _verticesBuffer[vertIdx++] = new Vector3(baseX, y0, baseZ);
                    _verticesBuffer[vertIdx++] = new Vector3(baseX + _gridSize, y1, baseZ);
                    _verticesBuffer[vertIdx++] = new Vector3(baseX, y2, baseZ + _gridSize);
                    _verticesBuffer[vertIdx++] = new Vector3(baseX + _gridSize, y3, baseZ + _gridSize);
                }
            }
            
            mesh.SetVertices(_verticesBuffer);
        }

        private void GetGridCornersHeight(int gridRow,int gridCol,out float y0,out float y1,out float y2,out float y3)
        {
            PointRecord lowerLeftPoint = _mapRecord.GetPointAt(gridRow, gridCol);
            PointRecord upperLeftPoint = _mapRecord.GetPointAt(gridRow + 1, gridCol);
            PointRecord lowerRightPoint = _mapRecord.GetPointAt(gridRow, gridCol + 1);
            PointRecord upperRightPoint = _mapRecord.GetPointAt(gridRow + 1, gridCol + 1);

            y0 = lowerLeftPoint.TerrainHeight;
            y1 = lowerRightPoint.TerrainHeight;
            y2 = upperLeftPoint.TerrainHeight;
            y3 = upperRightPoint.TerrainHeight;
        }

        public void Dispose()
        {
            //_mapRecord?.Dispose();
        }
    }
}