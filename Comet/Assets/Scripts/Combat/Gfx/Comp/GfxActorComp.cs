using UnityEngine;

namespace comet.combat
{
    public class GfxActorComp : BaseComp
    {
        public int UUID = -1;
        public GfxActor _gfxActor = null;

        public void Init(int uuid,GameObject prefab,GridMap gridMap,int row,int col)
        {
            UUID = uuid;
            GameObject gameObject = GameObject.Instantiate(prefab);

            float x, y, z;
            Metrics.GetGridCenterPos(gridMap.MapRecord, col, row,out x,out y,out z);
            gameObject.transform.position = new Vector3(x, y, z);
            _gfxActor = gameObject.AddComponent<GfxActor>();
            _gfxActor.UUID = uuid;
        }

        public void SetPosition(Vector3 pos)
        {
            _gfxActor.transform.position = pos;
        }

    }
}