using UnityEngine;

namespace comet
{
    public class Config
    {
        public float CameraMoveSpeed = 20.0f;
        
        public Vector3 CameraInitPosition = new Vector3(22.0f,16.35f,19.9f);
        public Vector3 CameraInitEuler = new Vector3(66.66f,0,0);
        
        public int DefaultGridMapRows = 30;
        public int DefaultGridMapCols = 30;
        
        //public float DefaultGridSize = 5;
        public float DefaultGridSize = 1;

        public string kTerrainlayerName = "Comet_Terrain";
    }
}