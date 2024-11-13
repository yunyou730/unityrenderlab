using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class ModelPainting : MonoBehaviour
{
    [SerializeField]
    private Color _brushColor = Color.red;
    [SerializeField,Range(0.03f,0.3f)]
    private float _brushSize = 0.05f;
    
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CheckMouseCollision();            
        }
        else
        {
            var paintableGameObjects = GameObject.FindGameObjectsWithTag("ayy.paintable");
            foreach (var go in paintableGameObjects)
            {
                PaintableMesh paintalbe = null;
                if (go.TryGetComponent<PaintableMesh>(out paintalbe))
                {
                    paintalbe.ClearDrawPoints();
                }
            }
        }
    }
    
    PaintableMesh CheckMouseCollision()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit))
        {
            PaintableMesh paintable = null;
            if (hit.collider.TryGetComponent<PaintableMesh>(out paintable))
            {
                paintable.SetCurrentDrawPointWS(hit.point);
                paintable.SetBrushSize(_brushSize);
                paintable.SetBrushColor(_brushColor);
                
                return paintable;
            }
        }

        return null;
    }
}
