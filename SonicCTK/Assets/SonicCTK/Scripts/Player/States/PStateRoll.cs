using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Physical
{
    public class PStateRoll : PlayerState
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
            Get<PStateMove>().CheckForJump();
            if (!Cmn.IsGrounded) 
                return;

            GroundMovement();
            Col.WallCollision();
            Cmn.ApplyTransformations();
            if (!Interactions.IsStandingOnSolidBox) 
                if (!Col.FloorCollision()) 
                    Set<PStateAir>();

            if (!Interactions.IsSTunnelMode) 
                Get<PStateMove>().FallFromGround();
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

            Cmn.Gsp -= Mathf.Min(Mathf.Abs(Cmn.Gsp), Phys.RollFriction) * Mathf.Sign(Cmn.Gsp);

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

            if (!Interactions.IsSTunnelMode)
            {
                if (Cmn.Gsp == 0) Set<PStateMove>();
            }
            else if (Cmn.Gsp == 0) Cmn.Gsp = 2 * Visual.SpriteDirection;
            
            Cmn.Xsp = Cmn.Gsp * Mathf.Cos(Cmn.SurfaceAngleRad);
            Cmn.Ysp = Cmn.Gsp * -Mathf.Sin(Cmn.SurfaceAngleRad);
        }

        public void SlopeRepel()
        {
            Cmn.Gsp -= Mathf.Sin(Cmn.SurfaceAngleRad) * 
                (Mathf.Sign(Cmn.Gsp) == Mathf.Sign(Mathf.Sin(Cmn.SurfaceAngleRad)) ? Phys.RollSlopeRepelUp : Phys.RollSlopeRepelDown);
        }
    }

}