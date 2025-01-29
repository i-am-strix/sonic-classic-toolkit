using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Physical
{
    public class WPStateRoll : PlayerState
    {
        public override void OnInitialize()
        {
            
        }

        public override void OnEnter()
        {
            Col.PlayerCollisionSize = Col.SpinCollisionSize;
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.GrabInput();
            SlopeRepel();
            Get<WPStateMove>().CheckForJump();
            if (!Cmn.IsGrounded) return;
            GroundMovement();
            Col.WallCollision();
            Cmn.ApplyTransformations();
            if (!Col.FloorCollision()) Set<WPStateAir>();
            Get<WPStateMove>().FallFromGround();
        }

        public override void OnLateUpdate(float Delta)
        {
            Cam.Follow();
            Visual.Play("Spin");
            Visual.SetSpeedMultip(Mathf.Floor(Mathf.Max(0, 4 - Mathf.Abs(Cmn.Gsp))));
        }

        public override void OnFixedUpdate(float Delta)
        {
            
        }
        
        public void GroundMovement()
        {
            var inputDir = Cmn.InputDirection;

            if (inputDir.x < 0)
            {
                if (Cmn.Gsp > 0)
                {
                    Cmn.Gsp -= Phys.RollDeceleration;
                    if (Cmn.Gsp <= 0) Cmn.Gsp = -0.5f;
                }
                else
                {
                    Visual.SpriteDirection = -1;
                }
            }
            else if (inputDir.x > 0)
            {
                if (Cmn.Gsp < 0)
                {
                    Cmn.Gsp += Phys.RollDeceleration;
                    if (Cmn.Gsp >= 0) Cmn.Gsp = 0.5f;
                }
                else
                {
                    Visual.SpriteDirection = 1;
                }
            }
            Cmn.Gsp -= Mathf.Min(Mathf.Abs(Cmn.Gsp), Phys.RollFriction) * Mathf.Sign(Cmn.Gsp);

            if (Cmn.Gsp == 0) Set<WPStateMove>();
            
            Cmn.Xsp = Cmn.Gsp * Mathf.Cos(Cmn.SurfaceAngleRad);
            Cmn.Ysp = Cmn.Gsp * -Mathf.Sin(Cmn.SurfaceAngleRad);
        }

        public void SlopeRepel()
        {
            if (Cmn.Gsp != 0)
            {
                Cmn.Gsp -= Mathf.Sin(Cmn.SurfaceAngleRad) * 
                           (Mathf.Sign(Cmn.Gsp) == Mathf.Sign(Mathf.Sin(Cmn.SurfaceAngleRad)) ? Phys.RollSlopeRepelUp : Phys.RollSlopeRepelDown);
            }
        }
    }

}