using System.Collections;
using System.Collections.Generic;
using System.IO;
using ayy.jy.data;
using ayy.jy.record;
using Newtonsoft.Json;
using UnityEngine;

namespace ayy.jy
{
    public class JYBaseGridMono : MonoBehaviour
    {
        public bool _debugTilePos = true;
        public bool _enableGizmo = false;

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
            _meshFilter.mesh = JYMapUtils.CreateBaseGrid(_mapRecord.TileCountX, _mapRecord.TileCountY);
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
                            tileX * JYMapUtils.ISO_TILE_HALF_WIDTH,
                            0,
                            -tileX * JYMapUtils.ISO_TILE_HALF_HEIGHT);

                        float posX, posY;
                        JYMapUtils.GetCenterPositionAtTile(tileX, tileY,out posX, out posY);
                        Vector3 tileLeftTop = new Vector3(posX, 0,posY);
                        DrawGizmoTile(tileLeftTop,tileX,tileY);
                    }
                }
            }
        }

        void DrawGizmoTile(Vector3 tileLeftTop,int tileX,int tileY)
        {
            float ltx = tileLeftTop.x;
            float ltz = tileLeftTop.z;
            
            float rtx = tileLeftTop.x + JYMapUtils.ISO_TILE_WIDTH;
            float rtz = tileLeftTop.z;

            float lbx = tileLeftTop.x;
            float lbz = tileLeftTop.z - JYMapUtils.ISO_TILE_HEIGHT;
            
            float rbx = tileLeftTop.x + JYMapUtils.ISO_TILE_WIDTH;
            float rbz = tileLeftTop.z - JYMapUtils.ISO_TILE_HEIGHT;

            
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