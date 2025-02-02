using Stray.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Physical
{
    [Serializable]
    public class WPStateSpindash : PlayerState
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

            WAudio.GSetValueParam("SpindashRev", 0);
            WAudio.Play("Spindash");
        }

        public override void OnExit()
        {
            SpindashSmoke.SetActive(false);
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.GrabInput();

            if (Input.GetButtonDown("Jump"))
            {
                SpinRev += 2;
                WAudio.Stop("Spindash");
                WAudio.GSetValueParam("SpindashRev", WAudio.GGetValueParam("SpindashRev") + 0.1f);
                WAudio.Play("Spindash");
            }
            SpinRev -= (Mathf.Floor(SpinRev / 0.125f) / 256);

            if (Cmn.InputDirection.y >= 0)
            {
                WAudio.Stop("Spindash");
                WAudio.Play("SpindashRelease");
                Set<WPStateRoll>();
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