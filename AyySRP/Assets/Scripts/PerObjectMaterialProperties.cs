using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor");
    private static int cutoffId = Shader.PropertyToID("_CutOff");

    [SerializeField]
    private Color _baseColor = Color.white;

    [SerializeField,Range(0f,1f)] 
    private float _cutoff = 0.5f;

    private static MaterialPropertyBlock _block;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
        }
        _block.SetColor(baseColorId,_baseColor);
        _block.SetFloat(cutoffId,_cutoff);
        GetComponent<Renderer>().SetPropertyBlock(_block);
    }
}
