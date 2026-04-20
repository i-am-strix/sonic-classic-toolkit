using System;
using System.Collections;
using System.Collections.Generic;

namespace SonicCTK.Physical
{
    [Serializable]
    public class PStateLookUp : PlayerState
    { 
        public override void OnInitialize()
        {
            
        }

        public override void OnEnter()
        {
            Visual.SetSpeedMultip(0);
            Visual.Play("Look Up");
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.GrabInput();
            if (Cmn.InputDirection.y <= 0) Set<PStateMove>();
            Col.WallCollision();
            if (!Interactions.IsStandingOnSolidBox) Col.FloorCollision();
            Cam.Follow();
        }

        public override void OnLateUpdate(float Delta)
        {
            if (Input.GetButtonDown("Jump"))
            {
                // TODO: Add PeelOut action here
                Set<PStateJump>();
            }
        }

        public override void OnFixedUpdate(float Delta)
        {
            
        }
    }

}