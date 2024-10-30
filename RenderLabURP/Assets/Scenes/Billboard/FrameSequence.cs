using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSequence : MonoBehaviour
{
    public int FrameCountInRow;
    public int FrameCountInCol;
    public float FrameRate = 16.0f;

    private int _frameIndex = 0;
    private int _totalFrame = 1;
    private Material _material = null;
    private float _elapsedTime = 0.0f;
    private float _frameDuration = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        _totalFrame = FrameCountInCol * FrameCountInRow;
        _frameDuration = 1.0f / FrameRate;
        _material = GetComponent<MeshRenderer>().sharedMaterial;
        
        _material.SetFloat(Shader.PropertyToID("_FrameCountInRow"),FrameCountInRow);
        _material.SetFloat(Shader.PropertyToID("_FrameCountInCol"),FrameCountInCol);
        _material.SetFloat(Shader.PropertyToID("_FrameIndex"),_frameIndex);
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _frameDuration)
        {
            _elapsedTime -= _frameDuration;
            _frameIndex++;
            if (_frameIndex >= _totalFrame)
            {
                _frameIndex = 0;
            }
            _material.SetFloat(Shader.PropertyToID("_FrameIndex"),_frameIndex);
        }
    }
}
