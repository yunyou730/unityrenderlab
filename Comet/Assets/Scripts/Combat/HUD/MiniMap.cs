using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace comet.combat
{
    public class MiniMap
    {
        private Image _image = null;
        
        public MiniMap(Image image)
        {
            _image = image; 
        }

        public void BindTexture(Texture2D texture)
        {
            _image.material.mainTexture = texture;
        }
    }
}