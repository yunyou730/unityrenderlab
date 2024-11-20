using UnityEngine;

namespace ayy.showcase
{
    class SwitchMaterialBetweenNormalAndUnWrap : MonoBehaviour
    {
        public Material _mat1 = null;
        public Material _mat2 = null;
        private bool _switchFlag = true;
        
        public void OnClickSwitchMaterial()
        {
            Material target = _switchFlag ? _mat2 : _mat1;
            GetComponent<MeshRenderer>().material = target;
            _switchFlag = !_switchFlag;
        }
    }
    
}
