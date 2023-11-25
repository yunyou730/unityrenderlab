using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    public class SampleCompute : MonoBehaviour
    {
        public ComputeShader compute = null;

        public RenderTexture result = null;
        public Color color;

        private int _kernel = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            _kernel = compute.FindKernel("CSMain");
            result = new RenderTexture(512,512,24);
            result.enableRandomWrite = true;    // writable
            result.Create();
            compute.SetTexture(_kernel,"Result",result);
        }

        // Update is called once per frame
        void Update()
        {
            compute.SetVector("Color",color);
            compute.Dispatch(_kernel,512/8,512/8,1);
            //compute.Dispatch(_kernel,8,8,1);
        }
    }    
}

