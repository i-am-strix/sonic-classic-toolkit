using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Stray.Physical;
using System;

namespace Stray.Core
{
    [Serializable]
    public struct SolidBoxSolidity
    {
        public bool Top;
        public bool Bottom;
        public bool Left;
        public bool Right;
    }

    [Serializable]
    public class SolidBoxCollisionEvents
    {
        public UnityEvent<Player> Top;
        public UnityEvent<Player> Bottom;
        public UnityEvent<Player> Left;
        public UnityEvent<Player> Right;
    }
    [DefaultExecutionOrder(1)]
    public class SolidBox : MonoBehaviour
    {
        [Tooltip("The half-size of the collision box")]
        public Vector2 radius;
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

        public SolidBoxCollisionEvents SolidityEvents;

        void Update()
        {
            foreach (Player p in FindObjectsOfType<Player>()) Collide(p);
        }

        // This function gets called every frame for each player
        public void Collide(Player Player)
        {
            WDebug.DrawSquare(transform.position, radius.PToW(), Color.red);
            Vector2 CombinedRadius = Radius +
                new Vector2(Player.Col.WallRadius + 1, Player.Col.HeightRadius);
            WDebug.DrawSquare(transform.position, CombinedRadius.PToW(), Color.red);

            Vector2 PixelPlayerPos = ((Vector2)(Player.transform.position)).WToP();
            Vector2 PixelObjectPos = ((Vector2)(transform.position)).WToP();

            if (Player.Interactions.IsStandingOnSolidBox && Player.Interactions.CurrentlyStandingSolidBox == this)
            {
                var xLeftDist = (PixelPlayerPos.x - PixelObjectPos.x) + CombinedRadius.x;
                if (xLeftDist < 0 || xLeftDist >= CombinedRadius.x * 2)
                {
                    Player.Interactions.IsStandingOnSolidBox = false;
                    Player.Set<WPStateAir>();
                    return;
                }
            }

            var leftDifference = (PixelPlayerPos.x - PixelObjectPos.x) + CombinedRadius.x;
            var topDifference = (PixelPlayerPos.y - PixelObjectPos.y) + 4 + CombinedRadius.y;

            if (leftDifference < 0 || leftDifference > CombinedRadius.x * 2) return;
            if (topDifference < 0 || topDifference > CombinedRadius.y * 2) return;

            var xDistance = PixelPlayerPos.x < PixelObjectPos.x ? leftDifference : leftDifference - CombinedRadius.x * 2;
            var yDistance = PixelPlayerPos.y < PixelObjectPos.y ? topDifference : topDifference - CombinedRadius.y * 2;

            WDebug.DrawLine(transform.position, transform.position + Vector3.right * xDistance.PToW(), Color.green);
            WDebug.DrawLine(transform.position, transform.position + Vector3.up * yDistance.PToW(), Color.green);

            if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
            {
                if (PixelPlayerPos.y > PixelObjectPos.y)
                {
                    if (!AllowSolidityFrom.Bottom) return;

                    if (yDistance < 0 && Player.Cmn.Ysp < 0)
                    {
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

                            Player.transform.position -= new Vector3(xDistance.PToW(), 0);
                        }
                    }
                    else
                    {
                        if (Player.Cmn.Xsp < 0)
                        {
                            if (!AllowSolidityFrom.Right) return;

                            Player.Cmn.IsPushing = true;
                            Player.Cmn.Gsp = 0;
                            Player.Cmn.Xsp = 0;

                            Player.transform.position -= new Vector3(xDistance.PToW(), 0);
                        }
                    }
                }
            }
        }
    }
}
