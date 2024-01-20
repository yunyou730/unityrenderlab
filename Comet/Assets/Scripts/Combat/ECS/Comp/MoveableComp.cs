namespace comet.combat
{
    public class MoveableComp : BaseComp
    {
        public int TargetGridX = -1;
        public int TargetGridY = -1;
        public bool IsMoving = false;

        public void MoveTo(int gridX,int gridY)
        {
            IsMoving = true;
            TargetGridX = gridX;
            TargetGridY = gridY;
        }
        
        public void StopMove()
        {
            IsMoving = false;
            TargetGridX = -1;
            TargetGridY = -1;
        }
    }
}