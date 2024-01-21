using Unity.VisualScripting;

namespace comet.combat
{
    public enum EGridOccupyState
    {
        EMPTY,
        OCCUPYIED
    }
    
    public enum EFogOfWarState
    {
        NotExplorer,
        Explorered,
        Visible
    }
    
    public class MapComp : BaseWorldComp
    {
        public int Rows = 0;
        public int Cols = 0;
        public EGridOccupyState[,] OccupyState;
        public EFogOfWarState[,] FogOfWarState;
        
        private MapRecord _mapRecord = null;
        public MapRecord MapRecord { get { return _mapRecord; } }

        public float GridSize;

        public MapComp(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
            
            GridSize = mapRecord.GridSize;
            Rows = _mapRecord.Rows;
            Cols = _mapRecord.Cols;
            
            OccupyState = new EGridOccupyState[Rows, Cols];
            FogOfWarState = new EFogOfWarState[Rows, Cols];
            
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0;x < Cols;x++)
                {
                    OccupyState[y,x] = EGridOccupyState.EMPTY;
                    FogOfWarState[y,x] = EFogOfWarState.NotExplorer;
                }
            }
        }
    }
}