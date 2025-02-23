using UnityEngine;

namespace SonicCTK.Physical
{
    public class PStateDead : PlayerState
    {
        public float DeathYSpeed = -7f;
        public override void OnInitialize()
        {

        }

        public override void OnEnter()
        {
            Cmn.Xsp = 0;
            Visual.SpriteDirection = 1;
            Cmn.Ysp = DeathYSpeed;
            Visual.Play("Dead");
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float Delta)
        {
            Cmn.Ysp += Phys.Gravity;
            Cmn.ApplyTransformations();
        }

        public override void OnLateUpdate(float Delta)
        {
        }

        public override void OnFixedUpdate(float Delta)
        {
        }
    }
}