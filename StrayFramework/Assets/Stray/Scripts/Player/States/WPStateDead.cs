using UnityEngine;

namespace Stray.Physical
{
    public class WPStateDead : PlayerState
    {
        public float DeathYSpeed = -7f;
        public override void OnInitialize()
        {
        }

        public override void OnEnter()
        {
            Cmn.Xsp = 0;
            Cmn.Ysp = DeathYSpeed;
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.Ysp += Phys.Gravity;
        }

        public override void OnLateUpdate(float Delta)
        {
        }

        public override void OnFixedUpdate(float Delta)
        {
        }
    }
}