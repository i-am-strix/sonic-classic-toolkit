using Stray.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Gameplay
{
    public enum SpringDirection
    {
        Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft
    };
    public class Spring : SolidBox
    {
        public float Strength = 10;
        public SpringDirection Direction;

        void Start()
        {
            OnCollision += OnSpringCollision;
        }

        private void OnSpringCollision(Physical.Player Player, SolidBoxCollisionDirection Direction)
        {
            switch(this.Direction)
            {
                case SpringDirection.Up:
                    if (Direction != SolidBoxCollisionDirection.Up) return;

                    Player.Set<Physical.WPStateAir>();
                    WAudio.Play("Spring");

                    Player.transform.position += new Vector3(0, 0.08f, 0);
                    Player.Cmn.Ysp = -Strength;
                    Player.Visual.Play("Spring");
                    Player.Visual.SetSpeedMultip(0);
                    break;
                
                case SpringDirection.Left:
                    if (Direction != SolidBoxCollisionDirection.Left) return;
                    WAudio.Play("Spring");

                    Player.Cmn.Gsp = -Strength;
                    break;
                
                case SpringDirection.Right:
                    if (Direction != SolidBoxCollisionDirection.Right) return;
                    WAudio.Play("Spring");

                    Player.Cmn.Gsp = Strength;
                    break;
            }
        }
    }
}
