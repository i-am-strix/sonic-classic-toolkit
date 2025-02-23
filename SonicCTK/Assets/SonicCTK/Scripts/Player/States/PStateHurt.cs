using System;
using UnityEngine;

namespace SonicCTK.Physical
{
    [Serializable]
    public class PStateHurt : PlayerState
    {
        public float YHurtSpeed = -4f;
        public float XAbsHurtSpeed = 2f;

        public float Gravity = 0.1875f;

        public GameObject Hazard;

        public override void OnInitialize()
        {
            Player.OnStateChanged += (p, c) =>
            {
                if (p is PStateHurt && c is PStateMove)
                {
                    Player.Common.Xsp = 0;
                    Player.Common.Gsp = 0;

                    Cmn.InvulnerabilityTimer = 120;
                }
            };
        }

        public override void OnEnter()
        {
            Cmn.IsGrounded = false;
            Col.PlayerCollisionSize = Col.SpinCollisionSize;

            Cmn.Xsp = XAbsHurtSpeed;
            if (Hazard) Cmn.Xsp *= Mathf.Sign(Player.transform.position.x - Hazard.transform.position.x);

            Cmn.Ysp = YHurtSpeed;

            Visual.Play("Hurt");
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float Delta)
        {
            Get<PStateAir>().HandleCollision();

            Cmn.Ysp += Gravity;
            Cmn.ApplyTransformations();
        }

        public override void OnLateUpdate(float Delta)
        {
            Cam.Follow();
        }

        public override void OnFixedUpdate(float Delta)
        {

        }
    }
}