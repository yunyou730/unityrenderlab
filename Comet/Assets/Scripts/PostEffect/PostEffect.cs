using System;
using System.Collections;
using System.Collections.Generic;
using comet.combat;
using comet.res;
using Unity.VisualScripting;
using UnityEngine;

namespace comet.posteffect
{
    public class PostEffect : MonoBehaviour
    {
        private ResManager _res = null;
        private CombatManager _combat = null;
        private Material _fogOfWarMaterial = null;
        
        void Start()
        {
            _res = Comet.Instance.ServiceLocator.Get<ResManager>();
            Shader shader = _res.Load<Shader>("PostEffectMaterials/FogOfWar");
            _fogOfWarMaterial = new Material(shader);
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
        }

        private void Update()
        {
            // if (_res == null)
            // {
            //     
            // }
            //
            // if (_combat == null)
            // {
            //     
            // }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            
            if (_combat != null && _combat.GetGfxGridMap() != null)
            {
                Graphics.Blit(source,destination,_fogOfWarMaterial);    
            }
            else
            {
                Graphics.Blit(source,destination);
            }
        }


        public Material GetFogOfWarMaterial()
        {
            return _fogOfWarMaterial;
        }

    }
}

