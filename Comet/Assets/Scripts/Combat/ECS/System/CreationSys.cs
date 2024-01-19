namespace comet.combat
{
    public class CreationSys : BaseSys,ITickSys
    {
        private CreationComp _creation = null;
        
        public CreationSys(World world) : base(world)
        {
            _creation = world.GetWorldComp<CreationComp>();
        }

        public void OnTick()
        {
            foreach (var spawnInfo in _creation.SpawnInfo)
            {
                switch (spawnInfo.CreationType)
                {
                    case ECreationType.Actor:
                        CreateActor(spawnInfo.Row,spawnInfo.Col);
                        break;
                }
            }
            _creation.SpawnInfo.Clear();
        }
        
        private void CreateActor(int row,int col)
        {
            var entity = _world.CreateEntity();
            entity.AttachComp<ActorComp>(new ActorComp());
            var gridPosComp = entity.AttachComp<GridPositionComp>(new GridPositionComp());
            gridPosComp.X = col;
            gridPosComp.Y = row;
        }

    }
}