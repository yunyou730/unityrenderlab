using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace comet.combat
{
    public class MiniMap
    {
        private Image _image = null;
        private Material _material = null;
        
        public MiniMap(Image image)
        {
            _image = image;
            _material = image.material;
        }

        public void BindGridMap(GfxGridMap gfxGridMap)
        {
            _material.SetTexture(Shader.PropertyToID("_WalkableTex"),gfxGridMap.BlockerAndHeightTexture);
            _material.SetTexture(Shader.PropertyToID("_TerrainLayer0Tex"),gfxGridMap.TerrainDataTextures[0]);
            _material.SetTexture(Shader.PropertyToID("_TerrainLayer1Tex"),gfxGridMap.TerrainDataTextures[1]);
        }

        public void Show()
        {
            _image.transform.parent.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _image.transform.parent.gameObject.SetActive(false);
        }

        public bool IsShowing()
        {
            return _image.transform.parent.gameObject.activeSelf;
        }
    }
}