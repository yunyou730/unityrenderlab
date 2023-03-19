using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;

public class Brush2D : MonoBehaviour
{
    private RenderTexture _rt1 = null;
    private RenderTexture _rt2 = null;
    private RenderTexture _activeRT = null;
    private bool _pingpongFlag = true;
    
    public Material _presentMaterial = null;
    public Material _blitAndBrushMaterial = null;

    private Vector2? _prevFramePoint = null;
    private Vector2? _curFramePoint = null;

    public float _brushSize = 0.03f;
    public Color _brushColor = Color.white;

    void Start()
    {
        PrepareRT();
        HandleWindowSizeChange();
        PassInvalidTouchPoint();
    }

    private void Update()
    {
        HandleTouchPoint();
        PassBrushArgsToMaterial();
    }

    private void HandleTouchPoint()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 pt = new Vector2(Input.mousePosition.x/GetScreenSize().x, Input.mousePosition.y/GetScreenSize().y);
            
            if (_prevFramePoint == null)
            {
                _prevFramePoint = pt;
            }
            else
            {
                _prevFramePoint = _curFramePoint;
            }
            _curFramePoint = pt;
        }
        else
        {
            _prevFramePoint = null;
            _curFramePoint = null;
        }
    }

    private void PassBrushArgsToMaterial()
    {
        if (_prevFramePoint != null && _curFramePoint != null)
        {
            Vector4 touchPos = new Vector4(_prevFramePoint.Value.x, _prevFramePoint.Value.y, _curFramePoint.Value.x, _curFramePoint.Value.y); 
            _blitAndBrushMaterial.SetVector(Shader.PropertyToID("_FromTo"),touchPos);
        }
        else
        {
            PassInvalidTouchPoint();
        }
        _blitAndBrushMaterial.SetFloat(Shader.PropertyToID("_BrushSize"),_brushSize);
    }

    private void PassInvalidTouchPoint()
    {
        _blitAndBrushMaterial.SetVector(Shader.PropertyToID("_FromTo"),new Vector4(-1.0f,-1.0f,-1.0f,-1.0f));
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //if (_prevFramePoint != null && _curFramePoint != null)
        SwapRenderTexture();
        _presentMaterial.SetTexture(Shader.PropertyToID("_BrushTex"), _activeRT);
        _presentMaterial.SetColor(Shader.PropertyToID("_BrushColor"), _brushColor);
        Graphics.Blit(src,dest,_presentMaterial);
    }

    private void PrepareRT()
    {
        // Create RTs
        int width = (int)GetScreenSize().x;
        int height = (int)GetScreenSize().y;
        _rt1 = new RenderTexture(width,height,0,RenderTextureFormat.Default);
        _rt2 = new RenderTexture(width, height, 0, RenderTextureFormat.Default);

        // Clear RTs
        Color clearColor = new Vector4(1.0f,1.0f,1.0f,0.0f);
        CommandBuffer cmdClearRT = new CommandBuffer();
        cmdClearRT.SetRenderTarget(_rt1);
        cmdClearRT.ClearRenderTarget(RTClearFlags.All, clearColor,0,0);
        cmdClearRT.SetRenderTarget(_rt2);
        cmdClearRT.ClearRenderTarget(RTClearFlags.All, clearColor,0,0);
        Graphics.ExecuteCommandBuffer(cmdClearRT);
        
        // init for ping pong 
        _pingpongFlag = true;
        _activeRT = _rt1;
    }
    
    private void SwapRenderTexture()
    {
        if (_pingpongFlag)
        {
            Graphics.Blit(_rt1,_rt2,_blitAndBrushMaterial);
            _activeRT = _rt2;
        }
        else
        {
            Graphics.Blit(_rt2, _rt1, _blitAndBrushMaterial);
            _activeRT = _rt1;
        }

        _pingpongFlag = !_pingpongFlag;
    }

    private void HandleWindowSizeChange()
    {
        _blitAndBrushMaterial.SetFloat(Shader.PropertyToID("_ScreenWidth"),GetScreenSize().x);
        _blitAndBrushMaterial.SetFloat(Shader.PropertyToID("_ScreenHeight"),GetScreenSize().y);
    }


    Vector2 GetScreenSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }
}
