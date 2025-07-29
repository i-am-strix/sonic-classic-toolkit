using UnityEngine;
using System;
using UnityEngine.Events;
using SonicCTK.Physical;
using SonicCTK.World;
using static SonicCTK.CCommon;

namespace SonicCTK.Core
{
    /// <summary>
    /// A hit box is a type of collision box that doesn't have any solidity, but fires corresponding events whenever a
    /// player enters its bounds (using similar calculations to how a solid box determines its radius)
    /// </summary>
    public class HitBox : MonoBehaviour
    {
        [Header("Hitbox Properties")]
        [Tooltip("The half-size of the hitbox")]
        public Vector2 Radius;
        
        [Tooltip("Offset of the collision box position (in pixels)")]
        public Vector2 BoxOffset;

        public Vector2 ObjectRadius => Radius;

        public event CCollisionOverlapEvent OnOverlap;

        public virtual void Update()
        {
            foreach(Player player in Stage.ConnectedPlayers) Overlap(player);
        }

        // Note to self: The guide says hitbox collision happens on the Player end, but I think
        // that's dumb so I'm not doing it #lol
        public virtual void Overlap(Player Player)
        {
            if (Player.CurrentState is PStateDead) return;

            var PixelPlayerPos = ((Vector2)Player.transform.position).WToP();
            var PixelObjectPos = ((Vector2)transform.position).WToP() + BoxOffset;

            var PlayerHitboxRadius = Player.Collision.HitboxRadius;

            if (PixelPlayerPos.x + PlayerHitboxRadius.x > PixelObjectPos.x - ObjectRadius.x &&
                PixelPlayerPos.x - PlayerHitboxRadius.x < PixelObjectPos.x + ObjectRadius.x)
            {
                if (PixelPlayerPos.y + PlayerHitboxRadius.y > PixelObjectPos.y - ObjectRadius.y &&
                    PixelPlayerPos.y - PlayerHitboxRadius.y < PixelObjectPos.y + ObjectRadius.y)
                {
                    OnOverlap?.Invoke(Player);
                    return;
                }
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Constants.ColorConstants.HitBoxColor;
            Gizmos.DrawCube(transform.position + (Vector3)BoxOffset.PToW(), ObjectRadius.PToW() * 2);
            Gizmos.DrawWireCube(transform.position + (Vector3)BoxOffset.PToW(), ObjectRadius.PToW() * 2);
        }
    }
}