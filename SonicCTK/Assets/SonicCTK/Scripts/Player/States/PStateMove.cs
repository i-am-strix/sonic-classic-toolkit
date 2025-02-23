using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;

namespace SonicCTK.Physical
{
    [Serializable]
    public sealed class PStateMove : PlayerState
    {
        public bool IsSkidding;

        public override void OnInitialize()
        {
        }

        public override void OnEnter()
        {
            Col.PlayerCollisionSize = Col.DefaultCollisionSize;
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float delta)
        {
            SlopeRepel();
            Cmn.GrabInput();
            CheckForJump();
            if (!Cmn.IsGrounded) 
                return;
            GroundMovement();
            Col.WallCollision();
            CheckForRoll();
            Cmn.ApplyTransformations();
            if (!Interactions.IsStandingOnSolidBox) 
                if (!Col.FloorCollision()) 
                    Set<PStateAir>();
            FallFromGround();

            Debug.Log("Uhhhh");
        }

        public override void OnLateUpdate(float delta)
        {
	        Cam.Follow();
            
	        if (Mathf.Abs(Cmn.Gsp) == 0 && Cmn.InputDirection.x == 0)
	        {
		        Cmn.IsPushing = false;
	        }

            if (Cmn.InputDirection.x != 0)
            {
                if (Cmn.InputDirection.x != Mathf.Sign(Cmn.Gsp))
                {
                    if (Mathf.Abs(Cmn.Gsp) > 4 && Col.AngleMode == 0b00)
                    {
                        if (!IsSkidding) CAudio.Play("Skid");
                        IsSkidding = true;
                       
                    }
                }
                else IsSkidding = false;
            }
            else IsSkidding = false;

            if (IsSkidding)
            {
                Visual.Play("Skid");
                Visual.SetSpeedMultip(8);
                return;
            }

	        if (!Cmn.IsPushing){
	            if (Mathf.Abs(Cmn.Gsp) == 0)
	            {	                
		            Visual.Play("Idle");
	                Visual.SetSpeedMultip(1);
	            }
	            else if (Mathf.Abs(Cmn.Gsp) < Phys.TopSpeed)
	            {
	                Visual.Play("Walk"); 
	                Visual.SetSpeedMultip(Mathf.Floor(Mathf.Max(0, 8-Mathf.Abs(Cmn.Gsp))));
	            }
	            else if (Mathf.Abs(Cmn.Gsp) < 12)
	            {
	                Visual.Play("Run");
	                Visual.SetSpeedMultip(Mathf.Floor(Mathf.Max(0, 8-Mathf.Abs(Cmn.Gsp))));
	            }
                else
                {
                    Visual.Play("Peelout");
                    Visual.SetSpeedMultip(Mathf.Floor(Mathf.Max(0, 8-Mathf.Abs(Cmn.Gsp))));
                }
	        }
	        else
            {
                Visual.Play("Push");
	        	Visual.SetSpeedMultip(Mathf.Floor(Mathf.Max(0, 8-Mathf.Abs(Cmn.Gsp))) * 4);
	        }
        }

        public override void OnFixedUpdate(float delta)
        {

        }

        public void CheckForJump()
        {
            if (Interactions.IsSTunnelMode) return;

            if (Input.GetButtonDown("Jump"))
            {
                Set<PStateJump>();
            }
        }

        public void CheckForRoll()
        {
            if (Mathf.Abs(Cmn.Gsp) < 0.5f)
            {
                if (Cmn.InputDirection.y < 0)
                {
                    Set<PStateCrouch>();
                }
                return;
            }
            if (Cmn.InputDirection.y < 0)
            {
                Set<PStateRoll>();
                CAudio.Play("Spin");
            }
        }
        
        public void GroundMovement()
        {
            var inputDir = Cmn.InputDirection;

            if (inputDir.x < 0)
            {
                if (Cmn.ControlLock == 0)
                {
                    if (Cmn.Gsp > 0)
                    {
                        Cmn.Gsp -= Phys.Deceleration;
                        if (Cmn.Gsp <= 0) Cmn.Gsp = -0.5f;
                    }
                    else if (Cmn.Gsp > -Phys.TopSpeed)
                    {
	                    if (Visual.SpriteDirection != -1) {
	                    	Visual.SpriteDirection = -1;
	                    	Cmn.IsPushing = false;
	                    }
                        Cmn.Gsp = Mathf.Max(Cmn.Gsp - Phys.Acceleration, -Phys.TopSpeed);
                    }
                }
            }
            else if (inputDir.x > 0)
            {
                if (Cmn.ControlLock == 0)
                {
                    if (Cmn.Gsp < 0)
                    {
                        Cmn.Gsp += Phys.Deceleration;
                        if (Cmn.Gsp >= 0) Cmn.Gsp = 0.5f;
                    }
                    else if (Cmn.Gsp < Phys.TopSpeed)
                    {
	                    if (Visual.SpriteDirection != 1) {
	                    	Visual.SpriteDirection = 1;
	                    	Cmn.IsPushing = false;
	                    }                        
	                    Cmn.Gsp = Mathf.Min(Cmn.Gsp + Phys.Acceleration, Phys.TopSpeed);
                    }
                }
            }
            else Cmn.Gsp -= Mathf.Min(Mathf.Abs(Cmn.Gsp), Phys.Friction) * Mathf.Sign(Cmn.Gsp);
            
            Cmn.Xsp = Cmn.Gsp * Mathf.Cos(Cmn.SurfaceAngleRad);
            Cmn.Ysp = Cmn.Gsp * -Mathf.Sin(Cmn.SurfaceAngleRad);
        }

        public void SlopeRepel()
        {
            var RepelCondition = Cmn.ControlLock != 0 || Cmn.Gsp != 0;
            if (RepelCondition) Cmn.Gsp -= Mathf.Sin(Cmn.SurfaceAngleRad) * Phys.SlopeRepel;
        }

        public void FallFromGround()
        {
            if (Cmn.ControlLock == 0)
            {
                if (Mathf.Abs(Cmn.Gsp) < 2.5f && (Cmn.SurfaceAngle >= 46 && Cmn.SurfaceAngle <= 315))
                {
                    Set<PStateAir>();
                    Cmn.Gsp = 0;
                    Cmn.ControlLock = 30;
                }
            }
            else
            {
                Cmn.ControlLock = Mathf.Max(Cmn.ControlLock - 1, 0);
            }
        }
    }
}