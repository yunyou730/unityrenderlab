using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class ModelPainting : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CheckMouseCollision();            
        }
    }
    
    void CheckMouseCollision()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit))
        {
            Debug.Log("hit name:" + hit.collider.gameObject.name);
            Debug.Log("hit uv:1" + hit.textureCoord);
            // Debug.Log("hit uv:2" + hit.textureCoord2);
            
            GameObject targetGameObject = hit.collider.gameObject;
            Material targetMaterial = targetGameObject.GetComponent<MeshRenderer>().material;
            
            Vector4 paintingPoints = new Vector4(hit.textureCoord.x,hit.textureCoord.y,0,0);
            targetMaterial.SetVector(Shader.PropertyToID("_PaintingPoints"),paintingPoints);

        }
    }
}
