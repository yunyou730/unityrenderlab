using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PlanarReflection : MonoBehaviour
{
    private Camera _reflectionCamera = null;
    private Camera _mainCamera;

    [SerializeField]
    private GameObject _reflectionPlane = null;
    
    private RenderTexture _renderTarget = null;


    [SerializeField] private Material _floorMaterial = null;

    [SerializeField][Range(0.0f,1.0f)] 
    private float _reflectionFactor;
    
    void Start()
    {
        // Hold main camera
        _mainCamera = Camera.main;
        
        // Create reflection camera
        GameObject reflectionCameraGo = new GameObject("reflection_camera");
        _reflectionCamera = reflectionCameraGo.AddComponent<Camera>();
        _reflectionCamera.enabled = false;        
        
        // set render target for the reflection camera
        _renderTarget = new RenderTexture(Screen.width, Screen.height, 24);
    }
    
    void Update()
    {
        Shader.SetGlobalFloat(Shader.PropertyToID("_reflectionFactor"),_reflectionFactor);
    }

    private void OnPostRender()
    {
        RenderReflection();
    }


    void RenderReflection()
    {
        _reflectionCamera.CopyFrom(_mainCamera);

        // World Space 
        Vector3 cameraDirectionWorldSpace = _mainCamera.transform.forward;
        Vector3 cameraUpWorldSpace = _mainCamera.transform.up;
        Vector3 cameraPositionWorldSpace = _mainCamera.transform.position;
        
        // Plane Space
        Vector3 cameraDirectionPlaneSpace = _reflectionPlane.transform.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = _reflectionPlane.transform.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = _reflectionPlane.transform.InverseTransformPoint(cameraPositionWorldSpace);
        
        // Mirror in plane space
        cameraDirectionPlaneSpace.y *= -1.0f;
        cameraUpPlaneSpace.y *= -1.0f;
        cameraPositionPlaneSpace.y *= -1.0f;
        
        // Transform from plane space  ,back to world space
        cameraDirectionWorldSpace = _reflectionPlane.transform.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = _reflectionPlane.transform.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = _reflectionPlane.transform.TransformPoint(cameraPositionPlaneSpace);
        
        
        // Apply direction,up,position ,to reflection camera
        _reflectionCamera.transform.position = cameraPositionWorldSpace;
        _reflectionCamera.transform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace,cameraUpWorldSpace);

          
        // render the reflection camera
        _reflectionCamera.targetTexture = _renderTarget;
        _reflectionCamera.Render();
        
        // Draw full screen quad
        DrawQuad();
    }


    void DrawQuad()
    {
        GL.PushMatrix();
        
        // User floo material to draw the quad
        _floorMaterial.SetPass(0);
        _floorMaterial.SetTexture(Shader.PropertyToID("_ReflectionTex"),_renderTarget);
        
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        
        GL.TexCoord2(1.0f,0.0f);
        GL.Vertex3(0.0f,0.0f,0.0f);
        
        GL.TexCoord2(1.0f,1.0f);        
        GL.Vertex3(0.0f,1.0f,0.0f);
        
        GL.TexCoord2(0.0f,1.0f);
        GL.Vertex3(1.0f,1.0f,0.0f);
        
        GL.TexCoord2(0.0f,0.0f);
        GL.Vertex3(1.0f,0.0f,0.0f);
        
        GL.End();
        GL.PopMatrix();
    }
}
