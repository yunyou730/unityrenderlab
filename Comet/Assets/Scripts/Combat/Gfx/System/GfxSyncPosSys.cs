using System;
using UnityEngine;

namespace comet.combat
{
    public class GfxSyncPosSys : BaseSys,IUpdateSys
    {
        Type[] _comps = {typeof(PositionComp),typeof(GfxActorComp)};
        public GfxSyncPosSys(World world) : base(world)
        {
        }

        public void OnUpdate(float deltaTime)
        {
            var entityList = _world.GetEntities(_comps);
            foreach (var entity in entityList)
            {
                var gfxActorComp = entity.GetComp<GfxActorComp>();
                var posComp = entity.GetComp<PositionComp>();
                gfxActorComp.SetPosition(new Vector3(posComp.X,posComp.Y,posComp.Z));
            }
        }
    }
}