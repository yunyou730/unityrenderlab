using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cameraTransform = null;
    private Material _material = null;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.GetComponent<Transform>();
        _material = GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        _material.SetVector(Shader.PropertyToID("_CameraPosition"),_cameraTransform.position);
    }
}
