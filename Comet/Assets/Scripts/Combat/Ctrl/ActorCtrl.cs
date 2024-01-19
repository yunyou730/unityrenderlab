using System;
using comet.input;
using UnityEngine;

namespace comet.combat
{
    public class ActorCtrl
    {
        private Camera _mainCamera = null;
        private World _world = null;

        private CombatManager _combat = null;
        private InputManager _input = null;

        private CmdComp _cmdComp = null; 

        public ActorCtrl(CombatManager combat)
        {
            _combat = combat;
            _cmdComp = _combat.World.GetWorldComp<CmdComp>();
        }

        public void Init(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            _input = Comet.Instance.ServiceLocator.Get<InputManager>();
        }

        public void OnUpdate(float deltaTime)
        {
            CheckSelectActor();
            CheckSelectGridCoord();
        }

        private void CheckSelectActor()
        {
            if (_input.IsMouseButtonDown(InputManager.EMouseBtn.Left))
            {
                Ray ray = _mainCamera.ScreenPointToRay(_input.MousePosition());
                RaycastHit hit;
                if (Physics.Raycast(ray,out hit))
                {
                    GfxActor gfxActor = hit.transform.GetComponent<GfxActor>();
                    if (gfxActor != null)
                    {
                        OnSelectActor(gfxActor);
                    }
                }
            }
        }

        private void CheckSelectGridCoord()
        {
            if (_input.IsMouseButtonDown(InputManager.EMouseBtn.Right))
            {
                Ray ray = _mainCamera.ScreenPointToRay(_input.MousePosition());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GridMap gfxGridMap = hit.transform.GetComponent<GridMap>();
                    if (gfxGridMap != null)
                    {
                        int x, y;
                        gfxGridMap.GetGridCoordBy3DPos(hit.point,out x,out y);
                        OnSelectGridCoord(x,y);
                    }
                }
            }
        }

        private void OnSelectActor(GfxActor gfxActor)
        {
            Debug.Log(gfxActor.UUID);

            var param = new ActorSelectionParam();
            param.UUIDs = new[] { gfxActor.UUID};
            
            Cmd cmd = new Cmd(ECmd.ActorSelection,param);
            _cmdComp.AddCmd(cmd);
        }

        private void OnSelectGridCoord(int gridX,int gridY)
        {
            Debug.Log("[" + gridX + "," + gridY + "]");

            var param = new ActorMoveToGridParam();
            param.GridX = gridX;
            param.GridY = gridY;
            
            Cmd cmd = new Cmd(ECmd.ActorMoveToGrid,param);
            _cmdComp.AddCmd(cmd);
        }
    }
}