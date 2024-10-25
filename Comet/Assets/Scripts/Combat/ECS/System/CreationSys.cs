namespace comet.combat
{
    public class CreationSys : BaseSys,ITickSys
    {
        private CreationComp _creation = null;
        private MapComp _mapComp = null;
        
        public CreationSys(World world) : base(world)
        {
            _creation = world.GetWorldComp<CreationComp>();
            _mapComp = world.GetWorldComp<MapComp>();
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
            
            // Actor Comp
            entity.AttachComp<ActorComp>(new ActorComp());
            
            // Position Comp
            var posComp = entity.AttachComp<PositionComp>(new PositionComp());
            posComp.SetGridPos(_mapComp.MapRecord,col,row);
            
            // Moveable Comp
            entity.AttachComp<MoveableComp>(new MoveableComp());
            
            // Route Plan Comp
            entity.AttachComp(new RoutePlanComp());
        }

    }
}