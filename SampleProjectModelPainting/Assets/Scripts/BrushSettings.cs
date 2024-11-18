using System;
using System.Security;
using UnityEngine;

namespace ayy
{
    public class BrushSettings : MonoBehaviour
    {
        [SerializeField,Range(0,2)] float BrushSize = 0.2f;
        [SerializeField] Color BrushColor = Color.magenta;
        [SerializeField,Range(0,0.1f)] float BrushSmooth = 0.05f;
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

