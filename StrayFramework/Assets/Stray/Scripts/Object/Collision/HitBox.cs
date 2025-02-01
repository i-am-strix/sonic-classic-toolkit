using UnityEngine;
using System;
using UnityEngine.Events;
using Stray.Physical;
using Stray.World;

namespace Stray.Core
{
    public class HitBox : MonoBehaviour
    {
        [Tooltip("The half-size of the hitbox")]
        public Vector2 radius;
        
        [Tooltip("Offset of the collision box position (in pixels)")]
        public Vector2 boxOffset;
        
        public Vector2 Radius
        {
            get
            {
                return radius;
            }
        }

        public delegate void HitBoxOverlapEvent(Player Player);
        public event HitBoxOverlapEvent OnOverlap;

        public virtual void Update()
        {
            foreach(Player p in Stage.ConnectedPlayers) Overlap(p);
        }

        // Note to self: The guide says hitbox collision happens on the Player end, but I think
        // that's dumb so I'm not doing it #lol
        public virtual void Overlap(Player Player)
        {
            var PixelPlayerPos = ((Vector2)Player.transform.position).WToP();
            var PixelObjectPos = ((Vector2)transform.position).WToP() + boxOffset;

            var PlayerHitboxRadius = Player.Col.HitboxRadius;

            if (PixelPlayerPos.x + PlayerHitboxRadius.x > PixelObjectPos.x - Radius.x &&
                PixelPlayerPos.x - PlayerHitboxRadius.x < PixelObjectPos.x + Radius.x)
            {
                if (PixelPlayerPos.y + PlayerHitboxRadius.y > PixelObjectPos.y - Radius.y &&
                    PixelPlayerPos.y - PlayerHitboxRadius.y < PixelObjectPos.y + Radius.y)
                {
                    OnOverlap?.Invoke(Player);
                    return;
                }
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Constants.ColorConstants.HitBoxColor;
            Gizmos.DrawCube(transform.position + (Vector3)boxOffset.PToW(), Radius.PToW() * 2);
            Gizmos.DrawWireCube(transform.position + (Vector3)boxOffset.PToW(), Radius.PToW() * 2);
        }
    }
}