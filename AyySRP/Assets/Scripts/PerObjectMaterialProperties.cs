using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private Color _baseColor = Color.white;


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
        GetComponent<Renderer>().SetPropertyBlock(_block);
    }
}
