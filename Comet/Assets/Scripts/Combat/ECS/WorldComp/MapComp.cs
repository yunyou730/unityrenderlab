using Unity.VisualScripting;

namespace comet.combat
{
    public enum EGridOccupyState
    {
        EMPTY,
        OCCUPYIED
    }
    
    public class MapComp : BaseWorldComp
    {
        public int Rows = 0;
        public int Cols = 0;
        public EGridOccupyState[] OccupyStates;
        
        private MapRecord _mapRecord = null;
        public MapRecord MapRecord { get { return _mapRecord; } }

        public float GridSize = 1.0f;

        public MapComp(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
            
            GridSize = mapRecord.GridSize;
            Rows = _mapRecord.Rows;
            Cols = _mapRecord.Cols;
            
            // occupy states
            OccupyStates = new EGridOccupyState[Rows * Cols];
            for (int i = 0;i < Rows * Cols;i++)
            {
                OccupyStates[i] = EGridOccupyState.EMPTY;
            }
        }
    }
}