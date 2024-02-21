using UnityEngine;
using UnityEngine.UI;

namespace comet.combat
{
    public class TerrainDepthTextureProvider
    {
        private Camera _mainCamera = null;
        private Camera _terrainDepthCamera = null;

        private Config _config = null;
        
        public void Init(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            CreateTerrainDepthCamera();
            //PassTerrainDepthTextureToMaterial();
            
            // Debug display depth texture
            var testDepthGO = GameObject.Find("TestDepthTexture");
            if (testDepthGO != null)
            {
                var testDepthImage = testDepthGO.GetComponent<Image>();
                testDepthImage.material.SetTexture(Shader.PropertyToID("_MainTex"),_terrainDepthCamera.targetTexture);
            }
        }
        
        private void CreateTerrainDepthCamera()
        {
            var depthTerrainCameraGameObject = new GameObject("[depthTerrainCameraGameObject]");
            _terrainDepthCamera = depthTerrainCameraGameObject.AddComponent<Camera>();
            _terrainDepthCamera.cullingMask = (1 << LayerMask.NameToLayer(_config.kTerrainlayerName));
            SetupTerrainDepthCamera(_mainCamera,_terrainDepthCamera);
        }
        
        // private void PassTerrainDepthTextureToMaterial()
        // {
        //     var postEffect = _mainCamera.GetComponent<posteffect.PostEffect>();
        //     var material = postEffect.GetFogOfWarMaterial();
        //     material.SetTexture(Shader.PropertyToID("_TerrainDepthTexture"),_terrainDepthCamera.targetTexture);
        // }

        private void SetupTerrainDepthCamera(Camera src,Camera dest)
        {
            dest.fieldOfView = src.fieldOfView;
            dest.nearClipPlane = src.nearClipPlane;
            dest.farClipPlane = src.farClipPlane;

            dest.transform.position = src.transform.position;
            dest.transform.rotation = src.transform.rotation;
            
            
            dest.depthTextureMode = DepthTextureMode.Depth;
            dest.clearFlags = CameraClearFlags.Depth;

            int width = Screen.width;
            int height = Screen.height;
            
            RenderTexture rt = new RenderTexture(
                width,
                height,
                32,
                RenderTextureFormat.Depth);
            dest.targetTexture = rt;
        }
        
        public void OnUpdate()
        {
            _terrainDepthCamera.transform.position = _mainCamera.transform.position;
            _terrainDepthCamera.transform.rotation = _mainCamera.transform.rotation;
            
            // _terrainDepthCamera.targetTexture
        }

        public RenderTexture GetTerrainDepthTexture()
        {
            return _terrainDepthCamera.targetTexture;
        }

        public void Dispose()
        {
            //GameObject.Destroy(_terrainDepthCamera.gameObject);
            //_terrainDepthCamera = null;
        }
    }
}