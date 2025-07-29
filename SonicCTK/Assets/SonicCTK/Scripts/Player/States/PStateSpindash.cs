using SonicCTK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Physical
{
    [Serializable]
    public class PStateSpindash : PlayerState
    {
        public float SpinRev;
        public GameObject SpindashSmoke;
        public override void OnInitialize()
        {
            SpindashSmoke.SetActive(false);
        }

        public override void OnEnter()
        {
            Visual.Play("Spindash");
            SpindashSmoke.SetActive(true);
            SpinRev = 2;
            SpindashSmoke.GetComponent<SpriteRenderer>().flipX = Visual.PlayerSprite.flipX;

            CAudio.GSetValueParam("SpindashRev", 0);
            CAudio.Play("Spindash");

            Cmn.IsCurledIntoBall = true;
        }

        public override void OnExit()
        {
            SpindashSmoke.SetActive(false);
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.GrabInput();

            Col.WallCollision();
            if (!Interactions.IsStandingOnSolidBox) Col.FloorCollision();
            
            if (Input.GetButtonDown("Jump"))
            {
                SpinRev += 2;
                CAudio.Stop("Spindash");
                CAudio.GSetValueParam("SpindashRev", CAudio.GGetValueParam("SpindashRev") + 0.1f);
                CAudio.Play("Spindash");
            }
            SpinRev -= (Mathf.Floor(SpinRev / 0.125f) / 256);

            if (Cmn.InputDirection.y >= 0)
            {
                CAudio.Stop("Spindash");
                CAudio.Play("SpindashRelease");
                Set<PStateRoll>();
                Cam.FreezeCamera(16 - (int)SpinRev);
                Cmn.Gsp = Visual.SpriteDirection * (8 + Mathf.Floor(SpinRev) / 2f);
            }
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