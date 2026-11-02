using System.Collections;
using System.Collections.Generic;
using System.IO;
using ayy.jy.data;
using ayy.jy.record;
using Newtonsoft.Json;
using UnityEngine;

namespace ayy.jy
{
    public class JYMap : MonoBehaviour
    {
        // Tile [0,0] 的位置在哪
        public Vector3 _baseTilePosition = Vector3.zero;
        public bool _debugTilePos = true;
        public bool _enableGizmo = false;
        
        // 一个菱形 tile图 本身是矩形的, 这个矩形贴图的 宽,高
        private const int ISO_TILE_WIDTH = 36;
        private const int ISO_TILE_HEIGHT = 18;
        
        private const float ISO_TILE_HALF_WIDTH = ISO_TILE_WIDTH / 2.0f;
        private const float ISO_TILE_HALF_HEIGHT = ISO_TILE_HEIGHT / 2.0f;
        
        private JYMapRecord _mapRecord = null;
        
        
        private MeshFilter _meshFilter = null;
        private MeshRenderer _meshRenderer = null;
        private Material _material = null;

        void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = _meshRenderer.material;
        }

        void Start()
        {
            BuildMapRecord();
            CreateBaseGrid(_mapRecord.TileCountX, _mapRecord.TileCountY);
        }
        

        private void BuildMapRecord()
        {
            string text = File.ReadAllText(JYPaths.FullPath("scene001/scene001.json"));
            JYSubscene subscene = JsonConvert.DeserializeObject<JYSubscene>(text);
            
            text = File.ReadAllText(JYPaths.FullPath("scene001/atlas_scene001.json"));
            JYTileAtlas tileAtlas = JsonConvert.DeserializeObject<JYTileAtlas>(text);
            
            _mapRecord = new JYMapRecord();
            _mapRecord.TileCountX = subscene.Size;
            _mapRecord.TileCountY = subscene.Size;

            // @miao @test
            //_mapRecord.TileCountX = 1;
            //_mapRecord.TileCountY = 1;
        }

        void CreateBaseGrid(int tileCountX,int tileCountY)
        {
            float currentY = 0.0f;
            
            int quadCount = tileCountX * tileCountY;
            
            Vector3[] verts = new Vector3[quadCount * 4];
            Vector2[] uvs = new Vector2[quadCount * 4];
            int[] tris = new int[quadCount * 6];

            Color[] colors = new Color[quadCount * 4];
            //new Color();
            

            int vi = 0;
            int ti = 0;
            for (int tileY = 0; tileY < tileCountY; tileY++)
            {
                for (int tileX = 0; tileX < tileCountX; tileX++)
                {
                    float px, py;
                    GetCenterPositionAtTile(tileX,tileY,out px, out py);
                    
                    /*
                     * p0  ---  p1
                     * |        |
                     * |        |
                     * p2  ---  p3 
                     */
                    verts[vi + 0] = new Vector3(px, currentY, py);
                    verts[vi + 1] = new Vector3(px + ISO_TILE_WIDTH, currentY, py);
                    verts[vi + 2] = new Vector3(px, currentY, py - ISO_TILE_HEIGHT);
                    verts[vi + 3] = new Vector3(px + ISO_TILE_WIDTH, currentY, py - ISO_TILE_HEIGHT);

                    uvs[vi + 0] = new Vector2(0, 1);
                    uvs[vi + 1] = new Vector2(1, 1);
                    uvs[vi + 2] = new Vector2(0, 0);
                    uvs[vi + 3] = new Vector2(1, 0);

                    Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1.0f);
                    colors[vi + 0] = color;
                    colors[vi + 1] = color;
                    colors[vi + 2] = color;
                    colors[vi + 3] = color;

                    tris[ti++] = vi + 0;
                    tris[ti++] = vi + 1;
                    tris[ti++] = vi + 2;
                    tris[ti++] = vi + 1;
                    tris[ti++] = vi + 3;
                    tris[ti++] = vi + 2;
                    
                    

                    vi += 4;
                    
                    currentY += 0.001f;
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "JYBaseGrid";
            
            mesh.SetVertices(verts);
            mesh.SetUVs(0,uvs);
            mesh.SetTriangles(tris, 0);
            mesh.SetColors(colors);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            //mesh.UploadMeshData(false);
            
            _meshFilter.mesh = mesh;
        }

        void OnDrawGizmos()
        {
            if (!_enableGizmo)
            {
                return;
            }

            if (_mapRecord != null)
            {
                for (int tileY = 0; tileY < _mapRecord.TileCountY; tileY++)
                {
                    for (int tileX = 0; tileX < _mapRecord.TileCountX; tileX++)
                    {
                        Vector3 offset = new Vector3(
                            tileX * ISO_TILE_HALF_WIDTH,
                            0,
                            -tileX * ISO_TILE_HALF_HEIGHT);

                        float posX, posY;
                        GetCenterPositionAtTile(tileX, tileY,out posX, out posY);
                        Vector3 tileLeftTop = new Vector3(posX, 0,posY);
                        DrawGizmoTile(tileLeftTop,tileX,tileY);
                    }
                }
            }
        }

        // 原本意图是用来描绘 tile 中心
        // 但是时间上, 有可能这个描述的是 tile 的左上角, 和 anchor 配合起来也许是更对的
        // 返回值, 先理解为 "左上角" 吧
        void GetCenterPositionAtTile(int tileX, int tileY,out float posX,out float posY)
        {
            float baseX = -tileY * ISO_TILE_HALF_WIDTH;
            float baseY = -tileY * ISO_TILE_HALF_HEIGHT;
            
            posX = baseX + tileX * ISO_TILE_HALF_WIDTH;
            posY = baseY - tileX * ISO_TILE_HALF_HEIGHT;
        }

        void DrawGizmoTile(Vector3 tileLeftTop,int tileX,int tileY)
        {
            float ltx = tileLeftTop.x;
            float ltz = tileLeftTop.z;
            
            float rtx = tileLeftTop.x + ISO_TILE_WIDTH;
            float rtz = tileLeftTop.z;

            float lbx = tileLeftTop.x;
            float lbz = tileLeftTop.z - ISO_TILE_HEIGHT;
            
            float rbx = tileLeftTop.x + ISO_TILE_WIDTH;
            float rbz = tileLeftTop.z - ISO_TILE_HEIGHT;

            
            Vector3 lt = new Vector3(ltx, tileLeftTop.y, ltz);
            Vector3 rt = new Vector3(rtx, tileLeftTop.y, rtz);
            Vector3 lb = new Vector3(lbx, tileLeftTop.y, lbz);
            Vector3 rb = new Vector3(rbx, tileLeftTop.y, rbz);
            if (_debugTilePos)
            {
                lt.y = rt.y = lb.y = rb.y = tileX;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(lt,rt);
            Gizmos.DrawLine(rt,rb);
            Gizmos.DrawLine(rb,lb);
            Gizmos.DrawLine(lb,lt);
        }
    }
    
}

