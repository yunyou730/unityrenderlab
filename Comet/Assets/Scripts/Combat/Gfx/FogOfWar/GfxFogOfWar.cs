using System;
using comet.combat.PostEffect;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace comet.combat
{
    public class GfxFogOfWar : IDisposable
    {
        private Camera _mainCamera = null;
        private TerrainDepthTextureProvider _terrainDepthTextureProvider = null;

        private Config _config = null;

        private CombatManager _combat = null;

        private posteffect.PostEffect _postEffect = null;

        private GfxGridMap _gfxGridMap = null;

        public GfxFogOfWar(CombatManager combatManager)
        {
            _combat = combatManager;
        }

        public void Init(Camera mainCamera,TerrainDepthTextureProvider terrainDepthTextureProvider)
        {
            _mainCamera = mainCamera;
            _terrainDepthTextureProvider = terrainDepthTextureProvider;
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            
            InitPostEffectMaterial();
            DebugShowTerrainDepthTexture();
        }

        private void DebugShowTerrainDepthTexture()
        {
            // Debug display depth texture
            var testDepthGO = GameObject.Find("TestDepthTexture");
            if (testDepthGO != null)
            {
                var testDepthImage = testDepthGO.GetComponent<Image>();
                testDepthImage.material.SetTexture(Shader.PropertyToID("_MainTex"), _terrainDepthTextureProvider.GetTerrainDepthTexture());
            }
        }
        
        private void InitPostEffectMaterial()
        {
            _postEffect = _mainCamera.GetComponent<posteffect.PostEffect>();
            var material = _postEffect.GetFogOfWarMaterial();
            
            // Terrain Depth Texture 
            material.SetTexture(Shader.PropertyToID("_TerrainDepthTexture"),_terrainDepthTextureProvider.GetTerrainDepthTexture());
            
            // MapRecord Grid Size Params
            MapRecord mapRecord = _combat.GetMapRecord();
            Vector4 gridSizeParam = new Vector4(mapRecord.GridCols, mapRecord.GridRows, mapRecord.GridSize, 0);
            material.SetVector(Shader.PropertyToID("_TerrainSizeParam"),gridSizeParam);
            
        }
        
        public void OnUpdate()
        {
            UpdatePostEffectMaterial();
        }


        private void UpdatePostEffectMaterial()
        {
            if (_gfxGridMap == null)
            {
                _gfxGridMap = _combat.World.GfxGridMap;
            }

            if (_gfxGridMap == null)
                return;
            
            var material = _postEffect.GetFogOfWarMaterial();
            
            Camera depthCamera = _terrainDepthTextureProvider.GetDepthCamera();
            // @miao @todo
            material.SetFloat(Shader.PropertyToID("_CameraFarPlane"),depthCamera.farClipPlane);
            material.SetMatrix(Shader.PropertyToID("_CameraInvProjMatrix"),depthCamera.projectionMatrix.inverse);
            material.SetMatrix(Shader.PropertyToID("_CameraViewToWorldMatrix"), depthCamera.cameraToWorldMatrix);
            material.SetMatrix(Shader.PropertyToID("_CameraWorldToViewMatrix"),depthCamera.worldToCameraMatrix);
            
            //depthCamera.GetStereoViewMatrix()
        }

        public void Dispose()
        {
            
        }
    }
}