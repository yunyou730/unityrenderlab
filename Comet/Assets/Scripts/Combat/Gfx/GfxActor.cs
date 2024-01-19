using UnityEngine;

namespace comet.combat
{
    public class GfxActor : MonoBehaviour
    {
        private int _uuid = 0;
        public int UUID
        {
            get { return _uuid;}
            set { _uuid = value; }
        }
    }
}