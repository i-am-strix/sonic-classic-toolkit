using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;

namespace SonicCTK.Physical
{
    [Serializable]
    public class PStateJump : PlayerState
    {
        public bool HoldingJump;

        private bool canDropdash;
        private const int DROPDASH_CHARGE_DURATION = 20;
        private int dropdashChargeTime = 0;
        
        public override void OnInitialize()
        {
        }

        public override void OnEnter()
        {
	        Cmn.IsGrounded = false;
	        Cmn.IsPushing = false;
            Interactions.IsStandingOnSolidBox = false;

            Cmn.Xsp -= Phys.JumpPower * Mathf.Sin(Cmn.SurfaceAngleRad);
            Cmn.Ysp -= Phys.JumpPower * Mathf.Cos(Cmn.SurfaceAngleRad);
            HoldingJump = true;
            
	        Col.PlayerCollisionSize = Col.SpinCollisionSize;
            Visual.Play("Spin");
            Cmn.IsCurledIntoBall = true;

            CAudio.PlayOneShot("Jump");

            dropdashChargeTime = 0;
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float Delta)
        {
            Get<PStateAir>().OnUpdate(Delta);
            if (HoldingJump && !Input.GetButton("Jump"))
            {
                HoldingJump = false;
                if (Cmn.Ysp <= -4f) Cmn.Ysp = -4f;
            }

            CheckForDropdash(Delta);
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
        
        #region Sonic

        public void CheckForDropdash(float Delta)
        {
            // Prevent this block of code from working if the current character is not Sonic
            if (Player is not Sonic) return;

            if (!HoldingJump)
            {
                if (Input.GetButton("Jump"))
                {
                    dropdashChargeTime += 1;
                    if (dropdashChargeTime >= DROPDASH_CHARGE_DURATION) Set<PStateDropdash>();
                }
                else dropdashChargeTime = 0;
            }
        }

        #endregion
    }
}