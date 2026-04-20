using System;
using System.Collections;
using System.Collections.Generic;

namespace SonicCTK.Physical
{
    [Serializable]
    public class PStateCrouch : PlayerState
    { 
        public override void OnInitialize()
        {
            
        }

        public override void OnEnter()
        {
            Visual.SetSpeedMultip(0);
            Visual.Play("Crouch");
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.GrabInput();
            if (Cmn.InputDirection.y >= 0) Set<PStateMove>();
            Col.WallCollision();
            if (!Interactions.IsStandingOnSolidBox) Col.FloorCollision();
            Cam.Follow();
        }

        public override void OnLateUpdate(float Delta)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (!Set<PStateSpindash>()) 
                    Set<PStateJump>(); // This is in case the Spindash state hasn't been added to your player character
            }
        }

        public override void OnFixedUpdate(float Delta)
        {
            
        }
    }

}