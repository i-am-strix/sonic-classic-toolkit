using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Stray.Physical;
using System;
using Stray.World;

namespace Stray.Core
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

    [DefaultExecutionOrder(1)]
    public class SolidBox : MonoBehaviour
    {
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

        public delegate void SolidBoxCollisionEvent(Player Player, SolidBoxCollisionDirection Direction);
        public event SolidBoxCollisionEvent OnCollision;

        public virtual void Update()
        {
            foreach (Player p in Stage.ConnectedPlayers) Collide(p);
        }

        // This function gets called every frame for each player
        public virtual void Collide(Player Player)
        {
            if (Player.CurrentState is WPStateDead) return;

            Vector2 PixelPlayerPos = ((Vector2)(Player.transform.position)).WToP();
            Vector2 PixelObjectPos = ((Vector2)(transform.position)).WToP() + boxOffset;
            
            WDebug.DrawSquare(PixelObjectPos.PToW(), radius.PToW(), Color.red);
            Vector2 CombinedRadius = Radius +
                new Vector2(Player.Col.WallRadius + 1, Player.Col.HeightRadius);
            WDebug.DrawSquare(PixelObjectPos.PToW(), CombinedRadius.PToW(), Color.red);
            

            if (Player.Interactions.IsStandingOnSolidBox && Player.Interactions.CurrentlyStandingSolidBox == this)
            {
                OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Up);

                if (Player.Cmn.Ysp < 0) return;

                var xLeftDist = (PixelPlayerPos.x - PixelObjectPos.x) + CombinedRadius.x;
                if (xLeftDist < 0 || xLeftDist >= CombinedRadius.x * 2)
                {
                    Player.Interactions.IsStandingOnSolidBox = false;
                    Player.Set<WPStateAir>();
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

                    if (yDistance < 0 && Player.Cmn.Ysp < 0)
                    {
                        OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Down);
                        if (Player.Cmn.Ysp > 0) return;

                        Player.transform.position -= new Vector3(0, yDistance.PToW());
                        Player.Cmn.Ysp = 0;
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

                    if (Player.Cmn.Ysp <= 0) return;

                    Player.transform.position -= new Vector3(0, -yDistance.PToW());
                    Player.Cam.PixelPosition.y += yDistance;
                    Player.Cmn.IsGrounded = true;
                    Player.Set<WPStateMove>();

                    Player.Interactions.IsStandingOnSolidBox = true;
                    Player.Interactions.CurrentlyStandingSolidBox = this;

                    Player.Cmn.Ysp = 0;
                    Player.Cmn.SurfaceAngle = 0;
                    Player.Cmn.Gsp = Player.Cmn.Xsp;
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

                        if (Player.Cmn.Xsp > 0)
                        {
                            Player.Cmn.IsPushing = true;
                            Player.Cmn.Gsp = 0;
                            Player.Cmn.Xsp = 0;
                            OnCollision?.Invoke(Player, SolidBoxCollisionDirection.Left);

                            Player.transform.position -= new Vector3(xDistance.PToW(), 0);
                        }
                    }
                    else
                    {
                        if (!AllowSolidityFrom.Right) return;

                        if (Player.Cmn.Xsp < 0)
                        {
                            Player.Cmn.IsPushing = true;
                            Player.Cmn.Gsp = 0;
                            Player.Cmn.Xsp = 0;
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
