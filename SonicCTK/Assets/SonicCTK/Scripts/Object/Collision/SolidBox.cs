using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using SonicCTK.Physical;
using System;
using SonicCTK.World;
using UnityEngine;

namespace SonicCTK.Core
{
    public enum SolidBoxCollisionDirection
    {
        Up, Down, Left, Right
    };

    [Serializable]
    public struct SolidBoxSolidity
    {
        public bool Top;
        public bool Bottom;
        public bool Left;
        public bool Right;
    }

    /// <summary>
    /// A solid box is a type of collision box that acts solid when any player interacts with it. When that happens, it also
    /// fires corresponding events, giving information on which side of the box the player hit it from, the corresponding player etc.
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class SolidBox : MonoBehaviour
    {
        public delegate void SolidBoxCollisionEvent(Player Player, SolidBoxCollisionDirection Direction);
        
        [Tooltip("The half-size of the collision box")]
        public Vector2 radius;
        
        [Tooltip("Offset of the collision box position (in pixels)")]
        public Vector2 boxOffset;
        public Vector2 Radius
        {
            get
            {
                return radius.Ceil();
            }
        }

        public SolidBoxSolidity AllowSolidityFrom = new SolidBoxSolidity
        {
            Top = true,
            Bottom = true,
            Left = true,
            Right = true
        };
        
        /// <summary>
        /// This event gets fired whenever any player comes in the radius of this SolidBox
        /// </summary>
        public event SolidBoxCollisionEvent OnCollision;

        public virtual void Update()
        {
            foreach (Player p in Stage.ConnectedPlayers) Collide(p);
        }

        // This function gets called every frame for each player
        public virtual void Collide(Player Player)
        {
            if (Player.CurrentState is PStateDead) return;

            Vector2 PixelPlayerPos = ((Vector2)(Player.transform.position)).WToP();
            Vector2 PixelObjectPos = ((Vector2)(transform.position)).WToP() + boxOffset;
            
            CDebug.DrawSquare(PixelObjectPos.PToW(), radius.PToW(), Color.red);
            Vector2 CombinedRadius = Radius +
                new Vector2(Player.Collision.WallRadius + 1, Player.Collision.HeightRadius);
            CDebug.DrawSquare(PixelObjectPos.PToW(), CombinedRadius.PToW(), Color.red);
            

            if (Player.Interactions.IsStandingOnSolidBox && Player.Interactions.CurrentlyStandingSolidBox == this)
            {
                OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Up);

                if (Player.Common.Ysp < 0) return;

                var xLeftDist = (PixelPlayerPos.x - PixelObjectPos.x) + CombinedRadius.x;
                if (xLeftDist < 0 || xLeftDist >= CombinedRadius.x * 2)
                {
                    Player.Interactions.IsStandingOnSolidBox = false;
                    Player.Set<PStateAir>();
                    return;
                }

                OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Up);
            }

            var leftDifference = (PixelPlayerPos.x - PixelObjectPos.x) + CombinedRadius.x;
            var topDifference = (PixelPlayerPos.y - PixelObjectPos.y) + 4 + CombinedRadius.y;

            if (leftDifference < 0 || leftDifference > CombinedRadius.x * 2) return;
            if (topDifference < 0 || topDifference > CombinedRadius.y * 2) return;

            var xDistance = PixelPlayerPos.x < PixelObjectPos.x ? leftDifference : leftDifference - CombinedRadius.x * 2;
            var yDistance = PixelPlayerPos.y < PixelObjectPos.y ? topDifference : topDifference - CombinedRadius.y * 2;
            
            if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
            {
                if (PixelPlayerPos.y > PixelObjectPos.y)
                {
                    if (!AllowSolidityFrom.Bottom) return;

                    if (yDistance < 0 && Player.Common.Ysp < 0)
                    {
                        OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Down);
                        if (Player.Common.Ysp > 0) return;

                        Player.transform.position -= new Vector3(0, yDistance.PToW());
                        Player.Common.Ysp = 0;
                    }
                }
                else
                {
                    if (!AllowSolidityFrom.Top) return;

                    if (yDistance < 0 || yDistance >= 16) return;

                    yDistance -= 4;

                    var xComparison = PixelObjectPos.x - PixelPlayerPos.x + Radius.x;
                    if (xComparison < 0 || xComparison > Radius.x * 2) return;

                    OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Up);

                    if (Player.Common.Ysp <= 0) return;

                    Player.transform.position -= new Vector3(0, -yDistance.PToW());
                    Player.Cam.PixelPosition.y += yDistance;
                    Player.Common.IsGrounded = true;
                    Player.Set<PStateMove>();

                    Player.Interactions.IsStandingOnSolidBox = true;
                    Player.Interactions.CurrentlyStandingSolidBox = this;

                    Player.Common.Ysp = 0;
                    Player.Common.SurfaceAngle = 0;
                    Player.Common.Gsp = Player.Common.Xsp;
                    return;
                }
            }
            else
            {
                if (Mathf.Abs(yDistance) > 4)
                {
                    if (xDistance == 0) return;
                    if (PixelPlayerPos.x < PixelObjectPos.x)
                    {
                        if (!AllowSolidityFrom.Left) return;

                        if (Player.Common.Xsp > 0)
                        {
                            if (Player.Common.InputDirection.x > 0) Player.Common.IsPushing = true;
                            Player.Common.Gsp = 0;
                            Player.Common.Xsp = 0;
                            OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Left);

                            Player.transform.position -= new Vector3(xDistance.PToW(), 0);
                        }
                    }
                    else
                    {
                        if (!AllowSolidityFrom.Right) return;

                        if (Player.Common.Xsp < 0)
                        {
                            if (Player.Common.InputDirection.x < 0) Player.Common.IsPushing = true;
                            Player.Common.Gsp = 0;
                            Player.Common.Xsp = 0;
                            OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Right);

                            Player.transform.position -= new Vector3(xDistance.PToW(), 0);
                        }
                    }
                }
            }
        }

        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Constants.ColorConstants.SolidBoxColor;
            Gizmos.DrawCube(transform.position + (Vector3)boxOffset.PToW(), Radius.PToW() * 2);
            Gizmos.DrawWireCube(transform.position + (Vector3)boxOffset.PToW(), Radius.PToW() * 2);
        }
    }
}
