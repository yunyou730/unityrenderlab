using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflectionManager : MonoBehaviour
{
    public Camera _mainCamera = null;
    public Camera _reflectionCamera = null;
    public Transform _planar = null;
    
    private Material _planarMaterial = null;
    private RenderTexture _reflectionRenderTarget = null;
    
    void Start()
    {
        // hold planar material
        _planarMaterial = _planar.GetComponent<MeshRenderer>().material;
        
        // create render texture for reflection camera 
        _reflectionRenderTarget = new RenderTexture(Screen.width,Screen.height,24);
        _reflectionCamera.targetTexture = _reflectionRenderTarget;
        _reflectionCamera.enabled = true;
        
        // Bind material & render texture
        _planarMaterial.SetTexture(Shader.PropertyToID("_ReflectionTex"),_reflectionRenderTarget);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("test1");
            //_planarMaterial.SetColor(Shader.PropertyToID("TestColor"),Color.red);
            _planarMaterial.SetColor(Shader.PropertyToID("_TestColor"),Color.yellow);
        }
        
        CalcReflectionCameraProperties();
    }


    private void CalcReflectionCameraProperties()
    {
        // Calculate properties
        Vector3 cameraPosWorldSpace = _mainCamera.transform.position;
        Vector3 cameraForwardWorldSpace = _mainCamera.transform.forward;
        Vector3 cameraUpDirWorldSpace = _mainCamera.transform.up;

        Vector3 cameraPosPlanarSpace = _planar.InverseTransformPoint(cameraPosWorldSpace);
        Vector3 cameraForwardPlanarSpace = _planar.InverseTransformDirection(cameraForwardWorldSpace);
        Vector3 cameraUpPlanarSpace = _planar.InverseTransformDirection(cameraUpDirWorldSpace);

        cameraPosPlanarSpace.y *= -1.0f;
        cameraForwardPlanarSpace.y *= -1.0f;
        cameraUpPlanarSpace.y *= -1.0f;

        cameraPosWorldSpace = _planar.TransformPoint(cameraPosPlanarSpace);
        cameraForwardWorldSpace = _planar.TransformDirection(cameraForwardPlanarSpace);
        cameraUpDirWorldSpace = _planar.TransformDirection(cameraUpPlanarSpace);
        
        // Apply properties to reflection camera
        _reflectionCamera.transform.position = cameraPosWorldSpace;
        _reflectionCamera.transform.LookAt(cameraPosWorldSpace + cameraForwardWorldSpace, cameraUpDirWorldSpace);
    }
}
