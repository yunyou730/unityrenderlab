using System.Collections;
using System.Collections.Generic;
using System.IO;
using ayy.jy.data;
using ayy.jy.record;
using Newtonsoft.Json;
using UnityEngine;

namespace ayy.jy
{
    public class JYMapMono : MonoBehaviour
    {
        private JYSubscene _subscene = null;
        private JYTileAtlas _tileAtlas = null;
        private JYMapRecord _mapRecord = null;

        void Awake()
        {
            string text = File.ReadAllText(JYPaths.FullPath("scene001/scene001.json"));
            _subscene = JsonConvert.DeserializeObject<JYSubscene>(text);
                
            text = File.ReadAllText(JYPaths.FullPath("scene001/atlas_scene001.json"));
            _tileAtlas = JsonConvert.DeserializeObject<JYTileAtlas>(text);
                
            _mapRecord = new JYMapRecord();
            _mapRecord.TileCountX = _subscene.Size;
            _mapRecord.TileCountY = _subscene.Size;
        }

        void Start()
        {
            BuildMesh();
            
            
        }
        
        void Update()
        {
        
        }

        private void BuildMesh()
        {
            // @miao @todo
            // 参考 JYTilePreview, 加载 texture, 做对应 tile 的渲染
        }
    }
    
}