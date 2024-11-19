using System;
using System.Collections.Generic;
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
        [SerializeField] private bool bEnableUVBleeding = true;

        private Transform _root = null;

        private void Start()
        {
            _root = transform;
        }

        public void Update()
        {
            List<Paintable> paintables = new List<Paintable>();
            GetPaintablesInChildren(ref paintables,_root);
            foreach (var paintable in paintables)
            {
                paintable.SyncBrushSettings(BrushSize,BrushColor,BrushSmooth,bEnableSmooth,bEnableUVBleeding);
            }
        }


        void GetPaintablesInChildren(ref List<Paintable> res,Transform node)
        {
            for(int i = 0;i < node.childCount;i++)
            {
                Transform child = node.GetChild(i);
                var paintable = child.GetComponent<Paintable>(); 
                if (paintable != null)
                {
                    res.Add(paintable);
                }
                GetPaintablesInChildren(ref res,child);
            }
        }
    }
}

