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
            PaintableMesh paintable = null;
            if (hit.collider.TryGetComponent<PaintableMesh>(out paintable))
            {
                //paintable.DrawPointWorldPos = hit.point;
                paintable.SetCurrentDrawPointWS(hit.point);
            }
        }
    }
}
