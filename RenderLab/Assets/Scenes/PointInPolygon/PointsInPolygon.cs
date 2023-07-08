using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ayy.pointsinpolygon
{
    //[ExecuteInEditMode]
    public class PointsInPolygon : MonoBehaviour
    {
        [SerializeField]
        private Material _material = null;

        private bool _bRecording = false;

        private const int MAX_LEN = 1000;
        private const float NEAR_DIS_SQ = 0.05f * 0.05f;
        private float[] _mousePosList = new float[MAX_LEN];
        private int _mousePosLen = 0;
        private int _shallFill = 0;     // flag of shall we fill the shape
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_material != null)
            {
                Graphics.Blit(src,dest,_material);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }

        private void Update()
        {
            RecordedPointsToShader();
            
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Mouse LB Down");
                _bRecording = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Mouse LB Up");

                //RecordedPointsToShader();
                _bRecording = false;
                _mousePosLen = 0;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _shallFill = 1 - _shallFill;
            }

            if (_bRecording)
            {
                RecordingPoints();                
            }

            
            
        }

        private void RecordingPoints()
        {
            Vector2 cur = Input.mousePosition;
            cur /= new Vector2(Screen.width, Screen.height);
            
            
            bool bCheckEnoughFar = true;
            if (_mousePosLen > 0)
            {
                int xIdx = _mousePosLen - 2;
                int yIdx = _mousePosLen - 1;
                Vector2 prev = new Vector2(_mousePosList[xIdx], _mousePosList[yIdx]);
                if ((prev - cur).sqrMagnitude < NEAR_DIS_SQ)
                {
                    bCheckEnoughFar = false;
                }
            }

            if (bCheckEnoughFar && _mousePosLen < MAX_LEN)
            //if (_mousePosLen < MAX_LEN)
            {
                _mousePosList[_mousePosLen++] = cur.x;
                _mousePosList[_mousePosLen++] = cur.y;
            }

        }

        private void RecordedPointsToShader()
        {
            if (_material != null)
            {
                if (_mousePosLen > 0)
                {
                    _material.SetFloatArray(Shader.PropertyToID("_polygonPoints"),_mousePosList);
                    _material.SetInt(Shader.PropertyToID("_polygonPointCount"),_mousePosLen / 2);
                }
                _material.SetInt(Shader.PropertyToID("_bFill"),_shallFill);
            }
        }
    }   
}
