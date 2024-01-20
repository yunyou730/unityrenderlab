

namespace comet.combat
{
    public class PositionComp : BaseComp
    {
        public int GridX = 0;
        public int GridY = 0;
        
        public float X = 0;
        public float Y = 0;
        public float Z = 0;

        public void SetGridPos(MapRecord mapRecord,int gridX,int gridY)
        {
            GridX = gridX;
            GridY = gridY;
            
            Metrics.GetGridCenterPos(mapRecord,gridX,gridY,out X,out Y,out Z);
        }

        public void SetPos(MapRecord mapRecord,float x,float z)
        {
            X = x;
            Z = z;
            GridX = (int)(X / mapRecord.GridSize);
            GridY = (int)(Z / mapRecord.GridSize);
        }

        public void SetY(float y)
        {
            Y = y;
        }
    }
}