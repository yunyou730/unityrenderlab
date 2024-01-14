using Unity.VisualScripting;

namespace comet.combat
{
    public class MapStateWorldComp : BaseWorldComp
    {
        public enum EGridOccupyState
        {
            EMPTY,
            OCCUPYIED
        }
        
        public int Rows = 0;
        public int Cols = 0;
        public EGridOccupyState[] OccupyStates;
        
        private MapRecord _mapRecord = null;
        
        MapStateWorldComp(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
            
            // size
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