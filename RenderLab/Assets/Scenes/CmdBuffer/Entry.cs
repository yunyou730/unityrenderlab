using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace ayy.cmdbuffer
{
    public class Entry : MonoBehaviour
    {
        public Camera _camera = null;
        private CommandBuffer _buf = null;
        public Material _blitMaterial = null;

        void Start()
        {
            InitRenderCmd();
        }

        private void InitRenderCmd()
        {
            Debug.Assert(_camera != null);
            
            CommandBuffer buf = new CommandBuffer {name = "ayy cmd buf"};

            // Create RT
            int screenCopyID = Shader.PropertyToID("_ScreenCopyID");
            buf.GetTemporaryRT(screenCopyID,-1, -1, 0, FilterMode.Bilinear);
            //buf.GetTemporaryRT(screenCopyID,-2, -2, 0, FilterMode.Bilinear);
            
            // Blit drawing result to RT
            buf.Blit(BuiltinRenderTextureType.CurrentActive,screenCopyID);
            //buf.Blit(_camera.activeTexture,screenCopyID);
            
            
            buf.SetGlobalTexture("_Ayy_GrabTexture",screenCopyID);

            // if (_blitMaterial == null)
            // {
            //     // Blit RT to Global Texture
            //     buf.SetGlobalTexture("_Ayy_GrabTexture",screenCopyID);                
            // }
            // else
            // {
            //     int tempRt1 = Shader.PropertyToID("_Temp1");
            //     buf.GetTemporaryRT(tempRt1, -1, -1, 0, FilterMode.Bilinear);
            //     buf.Blit(screenCopyID,tempRt1,_blitMaterial);
            //     buf.SetGlobalTexture("_Ayy_GrabTexture",tempRt1);
            // }
            
            _camera.AddCommandBuffer(CameraEvent.AfterSkybox,buf);
            //_camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque,buf);

            _buf = buf;
        }
    }
}


