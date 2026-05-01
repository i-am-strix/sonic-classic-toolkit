using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Physical;
using UnityEngine;

namespace SonicCTK.Core
{
    /// <summary>
    /// A hazard box is simply a variation of a <b>HitBox</b> that automatically
    /// attaches a hurt event to any player that hits it on startup, from any direction
    /// </summary>
    public class HazardBox : HitBox
    {
        public virtual void Start()
        {
            OnOverlap += OnHazardHit;
        }

        public virtual void OnHazardHit(Player player)
        {
            player.Common.TakeDamage(gameObject);
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Constants.ColorConstants.HazardColor;
            Gizmos.DrawCube(transform.position + (Vector3)BoxOffset.PToW(), ObjectRadius.PToW() * 2);
            Gizmos.DrawWireCube(transform.position + (Vector3)BoxOffset.PToW(), ObjectRadius.PToW() * 2);
        }
    }
}
