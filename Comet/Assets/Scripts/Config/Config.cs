using UnityEngine;

namespace comet
{
    public class Config
    {
        public float CameraMoveSpeed = 20.0f;
        
        public Vector3 CameraInitPosition = new Vector3(22.0f,27.7f,15f);
        public Vector3 CameraInitEuler = new Vector3(66.66f,0,0);
        
        public int DefaultGridMapRows = 50;
        public int DefaultGridMapCols = 50;
        public float DefaultGridSize = 1;
    }
}