using System.Collections;
using System.Collections.Generic;
using comet.combat;
using UnityEngine;

namespace comet
{
    public class Comet : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            CreateGridMap();
        }


        void CreateGridMap()
        {
            var t = Resources.Load<GameObject>("Prefabs/GridMap");
            var gridMap = GameObject.Instantiate(t).GetComponent<GridMap>();
            
            MapRecord mapRecord = new MapRecord(100,150);
            
            
            gridMap.RefreshMap(mapRecord);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
    
}
