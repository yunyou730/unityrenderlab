using System;
using System.Security;
using UnityEngine;

namespace ayy
{
    public class BrushSettings : MonoBehaviour
    {
        [SerializeField,Range(0,2)] float BrushSize = 0.2f;
        [SerializeField] Color BrushColor = Color.magenta;
        //[SerializeField,Range(0,0.08f)] float BrushSmooth = 0.03f;
        [SerializeField,Range(0,1)] float BrushSmooth = 0.4f;
        [SerializeField] private bool bEnableSmooth = true;

        private Transform _root = null;

        private void Start()
        {
            _root = transform;
        }

        public void Update()
        {
            foreach (var paintable in _root.GetComponentsInChildren<Paintable>())
            {
                paintable.SyncBrushSettings(BrushSize,BrushColor,BrushSmooth,bEnableSmooth);
            }
        }
    }
}

