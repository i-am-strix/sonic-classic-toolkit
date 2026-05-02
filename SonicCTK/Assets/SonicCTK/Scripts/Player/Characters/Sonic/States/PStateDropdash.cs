using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;

namespace SonicCTK.Physical
{
    [Serializable]
    public class PStateDropdash : PStateAir
    {
        public float DropdashStrength;

        public override void OnInitialize()
        {
            Player.OnStateChanged += CheckOnCollision;
        }

        private void CheckOnCollision(PlayerState previousState, PlayerState newState)
        {
            if (previousState is not PStateDropdash) return;
            
            if (Cmn.IsGrounded)
            {
                Cam.FreezeCamera(16);
                
                Debug.Log("I mean you dropdashed???");

                bool isFacingVelocity = Visual.SpriteDirection * Mathf.Sign(Cmn.Xsp) == 1;

                if (isFacingVelocity)
                {
                    Cmn.Gsp = (Cmn.Gsp / 4) + (8 * Visual.SpriteDirection);
                }
                else
                {
                    if (Cmn.SurfaceAngle == 0) Cmn.Gsp = 8 * Visual.SpriteDirection;
                    else Cmn.Gsp = (Cmn.Gsp / 2) + (8 * Visual.SpriteDirection);
                }

                Set<PStateRoll>();
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            CAudio.Play("Dropdash");
            Visual.Play("Dropdash");
        }

        public override void OnUpdate(float Delta)
        {
	       base.OnUpdate(Delta);
           
           if (!Input.GetButton("Jump"))
           {
               Visual.Play("Spin");
               Set<PStateAir>();
           }
        }
    }
}