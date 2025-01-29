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
        }

        public override void OnExit()
        {
            SpindashSmoke.SetActive(false);
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.GrabInput();
            
            if (Input.GetButtonDown("Jump")) SpinRev += 2;
            SpinRev -= (Mathf.Floor(SpinRev / 0.125f) / 256);

            if (Cmn.InputDirection.y >= 0)
            {
                Set<WPStateRoll>();
                Cam.FreezeCamera(32 - (int)SpinRev);
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