using SonicCTK.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Gameplay
{
    public enum SpringDirection
    {
        Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft
    };
    public class Spring : SolidBox
    {
        public float Strength = 10;
        public SpringDirection Direction;

        private Animator _animator;

        void Start()
        {
            _animator = GetComponent<Animator>();
            OnCollision += OnSpringCollision;
        }

        private void OnSpringCollision(Physical.Player Player, SolidBoxCollisionDirection Direction)
        {
            switch(this.Direction)
            {
                case SpringDirection.Up:
                    if (Direction != SolidBoxCollisionDirection.Up) return;

                    Player.Set<Physical.PStateAir>();
                    CAudio.Play("Spring");
                    _animator.Play("Hit", 0, 0);

                    Player.transform.position += new Vector3(0, 0.08f, 0);
                    Player.Common.Ysp = -Strength;
                    Player.Visual.Play("Spring");
                    Player.Visual.SetSpeedMultip(0);
                    break;
                
                case SpringDirection.Left:
                    if (Direction != SolidBoxCollisionDirection.Left) return;
                    CAudio.Play("Spring");
                    _animator.Play("Hit", 0, 0);

                    Player.Common.Gsp = -Strength;
                    Player.Common.IsPushing = false;
                    break;
                
                case SpringDirection.Right:
                    if (Direction != SolidBoxCollisionDirection.Right) return;
                    CAudio.Play("Spring");
                    _animator.Play("Hit", 0, 0);

                    Player.Common.Gsp = Strength;
                    Player.Common.IsPushing = false;
                    break;
            }
        }
    }
}
