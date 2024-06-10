using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MeshBall : MonoBehaviour
{
    private static int s_baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField] private Mesh _mesh = null;
    [SerializeField] private Material _material = null;

    private Matrix4x4[] _matrices = new Matrix4x4[1023];
    private Vector4[] _baseColors = new Vector4[1023];
    private MaterialPropertyBlock _block;

    private void Awake()
    {
        for (int i = 0;i < _matrices.Length;i++)
        {
            _matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 10.0f,Quaternion.identity,Vector3.one);
            _baseColors[i] = new Vector4(Random.value,Random.value,Random.value,1.0f);
        }
    }
    
    void Update()
    {
        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
            _block.SetVectorArray(s_baseColorId,_baseColors);
        }
        Graphics.DrawMeshInstanced(_mesh,0,_material,_matrices,123,_block);
    }
}
