using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Physical
{
    [Serializable]
    public class PStateAir : PlayerState
    { 
        public byte VelocityDirection = 0b10; // 0b00 = floor, 0b01 = right, 0b11 = left, 0b10 = ceiling

        public float VelocityDirectionAngle;
        public override void OnInitialize()
        {
            
        }

        public override void OnEnter()
        {
	        Cmn.IsGrounded = false;
            Col.PlayerCollisionSize = Col.SpinCollisionSize;
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float Delta)
        {
	        Cmn.SurfaceAngle = 0;
            Cmn.IsPushing = false;

            Cmn.GrabInput();
            ApplyGravity();
            AirControl();
            Cmn.ApplyTransformations();
            HandleCollision();
        }

        public override void OnLateUpdate(float Delta)
        {
            Cam.Follow();
        }

        public override void OnFixedUpdate(float Delta)
        {

        }

        public void ApplyGravity() => Cmn.Ysp += Phys.Gravity;

        public void AirControl()
        {
            bool applyRollLock = GameConfiguration.Instance.ApplyRollLock && Player.PreviousState is PStateRoll;
            if (Mathf.Abs(Cmn.Xsp) < Phys.TopSpeed)
            {
                if (!applyRollLock) Cmn.Xsp += Cmn.InputDirection.x * Phys.AirControl;
                Cmn.Xsp = Mathf.Clamp(Cmn.Xsp, -Phys.TopSpeed, Phys.TopSpeed);
            }
            
            if (!applyRollLock)
                if (Cmn.InputDirection.x != 0) Visual.SpriteDirection = (int)Mathf.Sign(Cmn.InputDirection.x);

            if (Cmn.Ysp < 0 && Cmn.Ysp > -4) // Air Drag
                Cmn.Xsp -= Mathf.Floor(Cmn.Xsp / 0.125f) / 256;
        }

        public void HandleCollision()
        {
            if (Mathf.Abs(Cmn.Xsp) >= Mathf.Abs(Cmn.Ysp)) VelocityDirection = (byte)(Cmn.Xsp > 0 ? 0b01 : 0b11);
            else VelocityDirection = (byte)(Cmn.Ysp > 0 ? 0b00 : 0b10);

            Col.AirWallCollision();
            
            if (VelocityDirection != 0b10)
            {
                if (Col.AirFloorCollision(VelocityDirection))
                {
	                Cmn.IsGrounded = true;
	                Col.PlayerCollisionSize = Col.DefaultCollisionSize;
	                Col.FloorCollision();
                    Set<PStateMove>();
                }
            }

            if (VelocityDirection != 0b00)
            {
                if (Col.CeilingCollision(VelocityDirection))
                {
                    Cmn.IsGrounded = true;
                    Set<PStateMove>();
                }
            }
        }
    }
}