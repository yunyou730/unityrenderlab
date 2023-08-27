using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class VisionManager
    {
        private GameObject _visionPrefab = null;
        private List<VisionItem> _visionList = new List<VisionItem>();
        
        public VisionManager(GameObject prefab)
        {
            _visionPrefab = prefab;
        }

        public void RegisterVision(GameObject gameObject,float radius,float angle = 60.0f)
        {
            var item = new VisionItem();
            _visionList.Add(item);

            item.transform = gameObject.transform;
            item.radius = radius;
            item.angle = angle;

            VisionRange visionRange = GameObject.Instantiate(_visionPrefab).GetComponent<VisionRange>();
            visionRange.SetVisionParams(gameObject.transform,radius,angle,gameObject.transform.forward);
            item._visionRangeComp = visionRange;
    
            
            Material mat = new Material(Shader.Find("ayy/rpg/VisionRange"));
            // Material material = visionRange.GetComponent<MeshRenderer>().sharedMaterial;
            mat.SetFloat(Shader.PropertyToID("_Angle"),Mathf.Deg2Rad * angle);
            mat.SetVector(Shader.PropertyToID("_FrontDir"),new Vector3(0,0,1));
            visionRange.GetComponent<MeshRenderer>().material = mat;
            item.material = mat;
            
            var camera = SetupDepthCameraSettings(gameObject);
            item._visionCamera = camera;
        }
        
        protected Camera SetupDepthCameraSettings(GameObject parentGameObject)
        {
            var gameObject = new GameObject("[depth camera]");
            gameObject.transform.SetParent(parentGameObject.transform);
            gameObject.transform.localPosition = Vector3.zero;
            
            var camera = gameObject.AddComponent<Camera>();
            camera.depthTextureMode = DepthTextureMode.Depth;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.nearClipPlane = 0.3f;
            //camera.farClipPlane = 10.0f;    // @miao @Temp
            camera.farClipPlane = 100.0f;
            //camera.farClipPlane = 1000.0f;    // @miao @Temp

            RenderTexture rt = new RenderTexture(
                Screen.width,
                Screen.height,
                32,
                // RenderTextureFormat.Default);
                RenderTextureFormat.Depth);            
            camera.targetTexture = rt;
            
            return camera;
        }


        public void Update()
        {
            foreach (var item in _visionList)
            {
                var camera = item._visionCamera;
                // camera depth texture
                item.material.SetTexture(Shader.PropertyToID("_DepthTex"),item._visionCamera.targetTexture);
                
                // camera view matrix
                item.material.SetMatrix(
                    Shader.PropertyToID("_depthCameraViewMatrix"),
                    camera.worldToCameraMatrix);
                
                item.material.SetMatrix(
                    Shader.PropertyToID("_depthCameraInvViewMatrix"),
                    Matrix4x4.Inverse(camera.worldToCameraMatrix));

                // camera projection matrix
                item.material.SetMatrix(
                    Shader.PropertyToID("_depthCameraProjMatrix"),
                    camera.projectionMatrix);
                
                item.material.SetMatrix(
                    Shader.PropertyToID("_depthCameraInvProjMatrix"),
                    Matrix4x4.Inverse(camera.projectionMatrix));
                
                // camera pos
                float x = camera.transform.position.x;
                float y = camera.transform.position.y;
                float z = camera.transform.position.z;
                Vector4 homoPos = new Vector4(x,y,z,1.0f);
                item.material.SetVector(Shader.PropertyToID("_depthCameraPos"),homoPos);
                
                // camera near/far plane value
                item.material.SetFloat(Shader.PropertyToID("_depthCameraNear"),camera.nearClipPlane);
                item.material.SetFloat(Shader.PropertyToID("_depthCameraFar"),camera.farClipPlane);
                
                // Fov
                item.material.SetFloat(Shader.PropertyToID("_depthCameraFovY"),camera.fieldOfView);

                // camera dir
                // x = camera.transform.forward.x;
                // y = camera.transform.forward.y;
                // z = camera.transform.forward.z;
                // Vector4 camDir = new Vector4(x, y, z, 0.0f);
                // item.material.SetVector(Shader.PropertyToID("_depthCameraDir"),camDir);
            }
        }
    }
    
    public class VisionItem
    {
        public Transform transform = null;
        public float radius = 3.0f;
        public float angle = 60.0f; // degree, not radian
        public VisionRange _visionRangeComp = null;
        public Camera _visionCamera = null;
        public Material material = null;
    }
}
