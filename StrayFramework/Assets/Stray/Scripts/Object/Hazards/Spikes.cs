using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stray.Core;
using Stray.Physical;

namespace Stray.Gameplay
{
    public class Spikes : SolidBox
    {
        public SolidBoxCollisionDirection AttackDirection;
        
        void Start()
        {
            OnCollision += OnSpikeHit;
        }

        private void OnSpikeHit(Player Player, SolidBoxCollisionDirection Direction)
        {
            if (Direction == AttackDirection)
            {
                Player.Cmn.TakeDamage(gameObject);
            }
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Vector3 LineDirection = Vector3.up;

            switch(AttackDirection)
            {
                case SolidBoxCollisionDirection.Down:
                    LineDirection = Vector3.down;
                    break;
                case SolidBoxCollisionDirection.Left:
                    LineDirection = Vector3.left;
                    break;
                case SolidBoxCollisionDirection.Right:
                    LineDirection = Vector3.right;
                    break;
            }

            Gizmos.color = Color.red;
            
            Gizmos.DrawLine(transform.position, transform.position + LineDirection * 0.16f);
        }

    }

}