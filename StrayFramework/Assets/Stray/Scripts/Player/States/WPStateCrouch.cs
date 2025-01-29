using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Physical
{
    [Serializable]
    public class WPStateCrouch : PlayerState
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
            if (Cmn.InputDirection.y >= 0) Set<WPStateMove>();
            Col.WallCollision();
            Col.FloorCollision();
            Cam.Follow();
        }

        public override void OnLateUpdate(float Delta)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (!Set<WPStateSpindash>()) 
                    Set<WPStateJump>(); // This is in case the Spindash state hasn't been added to your player character
            }
        }

        public override void OnFixedUpdate(float Delta)
        {
            
        }
    }

}