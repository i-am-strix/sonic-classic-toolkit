using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Physical
{
    [Serializable]
    public class WPStateJump : PlayerState
    {
        public bool HoldingJump;
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
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float Delta)
        {
            Get<WPStateAir>().OnUpdate(Delta);
            if (HoldingJump && !Input.GetButton("Jump"))
            {
                HoldingJump = false;
                if (Cmn.Ysp <= -4f) Cmn.Ysp = -4f;
            }
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
    }

}