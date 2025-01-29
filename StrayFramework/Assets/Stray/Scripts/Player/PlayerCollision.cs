using System;
using Stray.Core;
using UnityEngine;

namespace Stray.Physical
{
    [Serializable]
    public class PlayerCollision
    {
        public Player Player;

        public PlayerCommon Cmn
        {
            get
            {
                return Player.Cmn;
            }
        }

        [Header("Common")] public Vector2 DefaultCollisionSize = new Vector2(19, 39);
        public Vector2 SpinCollisionSize = new Vector2(15, 29);
        
        [HideInInspector] public Vector2 PlayerCollisionSize = new Vector2(19, 39);

        public float WidthRadius
        {
            get
            {
                return Mathf.Ceil(PlayerCollisionSize.x / 2);
            }
        }
        public float HeightRadius
        {
            get
            {
                return Mathf.Ceil(PlayerCollisionSize.y / 2);
            }
        }

        public LayerMask RayMask;

        public RaycastHit2D FloorHit, CeilingHit, WallHit;

        public byte AngleMode = 0b10; // 0b00 = floor, 0b01 = right, 0b11 = left, 0b10 = ceiling

        [Header("Params")] public float FloorRayLength = 32;
        public float FloorCeilRayLateralOffset = 9;

        public float WallRadius;

        public void FigureOutAngleMode()
        {
            float Angle = Mathf.Round(Player.Cmn.SurfaceAngle);
            if ((Angle >= 0 && Angle <= 45) || (Angle <= 360 && Angle >= 315)) AngleMode = 0b00;
            if (Angle >= 46 && Angle <= 134) AngleMode = 0b01;
            if (Angle >= 135 && Angle <= 225) AngleMode = 0b10;
            if (Angle >= 226 && Angle <= 314) AngleMode = 0b11;
        }
        
        public bool FloorCollision()
        {
            FigureOutAngleMode();
            
            Transform Transform = Player.transform;
            Vector2 LeftOrigin = Transform.position, RightOrigin = Transform.position;

            Vector2 Direction = Vector2.down;

            float SensorSpacing = Mathf.Floor(PlayerCollisionSize.x / 2);

            switch (AngleMode)
            {
                case 0b00:
                    LeftOrigin -= new Vector2(SensorSpacing, 0).PToW();
                    RightOrigin += new Vector2(SensorSpacing, 0).PToW();
                    
                    if (FindWinnerRay(LeftOrigin, LeftOrigin + Direction * FloorRayLength.PToW(), 
                            RightOrigin, RightOrigin + Direction * FloorRayLength.PToW(), 
                            out FloorHit))
                    {
                        Transform.position = new Vector3(Transform.position.x, 
                            FloorHit.point.y + HeightRadius.PToW(), 
                            Transform.position.z);
                        Player.Cmn.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;
                case 0b01:
                    LeftOrigin -= new Vector2(0, SensorSpacing).PToW();
                    RightOrigin += new Vector2(0, SensorSpacing).PToW();

                    Direction = Vector2.right;
                    
                    if (FindWinnerRay(LeftOrigin, LeftOrigin + Direction * FloorRayLength.PToW(), 
                            RightOrigin, RightOrigin + Direction * FloorRayLength.PToW(), 
                            out FloorHit))
                    {
                        Transform.position = new Vector3(FloorHit.point.x - HeightRadius.PToW(), 
                            Transform.position.y, 
                            Transform.position.z);
                        Player.Cmn.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;
                case 0b10:
                    LeftOrigin += new Vector2(SensorSpacing, 0).PToW();
                    RightOrigin -= new Vector2(SensorSpacing, 0).PToW();

                    Direction = Vector2.up;
                    
                    if (FindWinnerRay(LeftOrigin, LeftOrigin + Direction * FloorRayLength.PToW(), 
                            RightOrigin, RightOrigin + Direction * FloorRayLength.PToW(), 
                            out FloorHit))
                    {
                        Transform.position = new Vector3(Transform.position.x, 
                            FloorHit.point.y - HeightRadius.PToW(), 
                            Transform.position.z);
                        Player.Cmn.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;
                case 0b11:
                    LeftOrigin += new Vector2(0, SensorSpacing).PToW();
                    RightOrigin -= new Vector2(0, SensorSpacing).PToW();

                    Direction = Vector2.left;
                    
                    if (FindWinnerRay(LeftOrigin, LeftOrigin + Direction * FloorRayLength.PToW(), 
                            RightOrigin, RightOrigin + Direction * FloorRayLength.PToW(), 
                            out FloorHit))
                    {
                        Transform.position = new Vector3(FloorHit.point.x + HeightRadius.PToW(), 
                            Transform.position.y, 
                            Transform.position.z);
                        Player.Cmn.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;
            }

            return false;
        }
        public bool WallCollision()
        {
            FigureOutAngleMode();
            
            Transform Transform = Player.transform;
            Vector2 VelocityOffset = new Vector2(Cmn.Xsp, Cmn.Ysp).PToW();
            Vector2 LeftOrigin = Transform.position, RightOrigin = Transform.position;
            var VerticalOffset = (Cmn.SurfaceAngle == 0 || Cmn.SurfaceAngle == 360) ? 8 : 0;

            LeftOrigin += VelocityOffset;
            RightOrigin += VelocityOffset;

            switch (AngleMode)
            {
                case 0b00:
                    if (FindWinnerRay(LeftOrigin + new Vector2(0, VerticalOffset).PToW(), LeftOrigin - new Vector2(WallRadius, -VerticalOffset).PToW(),
                            RightOrigin + new Vector2(0, VerticalOffset).PToW(), RightOrigin + new Vector2(WallRadius, VerticalOffset).PToW(), out WallHit))
                    {
                        Vector2 WallNormal = WallHit.normal;
                        if (Cmn.Gsp < 0)
                            if (WallNormal.x > 0)
                            {
                                Transform.position = new Vector3(WallHit.point.x + WallRadius.PToW(), Transform.position.y, Transform.position.z);
                                Cmn.Xsp = 0;
	                            Cmn.Gsp = 0;
	                            if (Cmn.InputDirection.x != 0) Cmn.IsPushing = true;
                            }
                        
                        if (Cmn.Gsp > 0)
                            if (WallNormal.x < 0)
                            {
                                Transform.position = new Vector3(WallHit.point.x - WallRadius.PToW(), Transform.position.y, Transform.position.z);
                                Cmn.Xsp = 0;
	                            Cmn.Gsp = 0;
	                            if (Cmn.InputDirection.x != 0) Cmn.IsPushing = true;
                            }

                        return true;
                    }

                    break;
            }
            
            return false;
        }
       
        public bool CeilingCollision(byte VelocityDirection = 0b10)
        {
            Transform Transform = Player.transform;
            Vector2 LeftOrigin = Transform.position, RightOrigin = Transform.position;

            Vector2 Direction = Vector2.up;
            
            LeftOrigin -= new Vector2(WidthRadius, 0).PToW();
            RightOrigin += new Vector2(WidthRadius, 0).PToW();
                    
            if (FindWinnerRay(LeftOrigin, LeftOrigin + Direction * HeightRadius.PToW(), 
                    RightOrigin, RightOrigin + Direction * HeightRadius.PToW(), 
                    out CeilingHit))
            {
                var SurfaceAngle = Mathf.Round(CalculateAngle(CeilingHit.normal));

                if (SurfaceAngle >= 91 && SurfaceAngle <= 225)
                {
                    Cmn.Ysp = 0;
                    Transform.position += (Vector3)(new Vector2(0, 1).PToW());
                    return false;
                }
                else
                {
                    if (VelocityDirection != 0b10)
                    {
                        Cmn.Ysp = 0;
                        Transform.position += (Vector3)(new Vector2(0, 1).PToW());
                        return false;
                    }
                    else
                    {
                        Cmn.SurfaceAngle = SurfaceAngle;
                        Cmn.Gsp = Cmn.Ysp * -Mathf.Sign(Mathf.Sin(Cmn.SurfaceAngleRad));
                        return true;
                    }
                }
            }

            return false;
        }
        public bool AirFloorCollision(byte VelocityDirection = 0b00)
        {
            Transform Transform = Player.transform;
            Vector2 LeftOrigin = Transform.position, RightOrigin = Transform.position;

            Vector2 Direction = Vector2.down;
            
            LeftOrigin -= new Vector2(WidthRadius, 0).PToW();
            RightOrigin += new Vector2(WidthRadius, 0).PToW();
                    
            if (FindWinnerRay(LeftOrigin, LeftOrigin + Direction * HeightRadius.PToW(), 
                    RightOrigin, RightOrigin + Direction * HeightRadius.PToW(), 
                    out FloorHit))
            {
                Cmn.SurfaceAngle = Mathf.Round(CalculateAngle(FloorHit.normal));

                if ((Cmn.SurfaceAngle >= 0 && Cmn.SurfaceAngle <= 23) || (Cmn.SurfaceAngle <= 360 && Cmn.SurfaceAngle >= 339))
                {
                    Cmn.Gsp = Cmn.Xsp;
                }
                else if ((Cmn.SurfaceAngle >= 24 && Cmn.SurfaceAngle <= 45) || (Cmn.SurfaceAngle <= 338 && Cmn.SurfaceAngle >= 316))
                {
                    if (VelocityDirection == 0b01 || VelocityDirection == 0b11)
                    {
                        Cmn.Gsp = Cmn.Xsp;
                    }
                    else Cmn.Gsp = Cmn.Ysp * 0.5f * -Mathf.Sign(Mathf.Sin(Cmn.SurfaceAngleRad));
                }
                else
                {
                    if (VelocityDirection == 0b01 || VelocityDirection == 0b11)
                    {
                        Cmn.Gsp = Cmn.Xsp;
                    }
                    else Cmn.Gsp = Cmn.Ysp * -Mathf.Sign(Mathf.Sin(Cmn.SurfaceAngleRad));
                }
                
                FloorCollision();
                
                return true;
            }

            return false;
        }
        public bool AirWallCollision()
        {
            Transform Transform = Player.transform;
            Vector2 LeftOrigin = Transform.position, RightOrigin = Transform.position;
            
            if (FindWinnerRay(LeftOrigin, LeftOrigin - new Vector2(WallRadius, 0).PToW(),
                    RightOrigin , RightOrigin + new Vector2(WallRadius, 0).PToW(), out WallHit))
            {
                Vector2 WallNormal = WallHit.normal;
                if (Cmn.Xsp < 0)
                    if (WallNormal.x > 0)
                    {
                        Transform.position = new Vector3(WallHit.point.x + WallRadius.PToW(), Transform.position.y, Transform.position.z);
                        Cmn.Xsp = 0;
                        Cmn.Gsp = 0;
                    }
                
                if (Cmn.Xsp > 0)
                    if (WallNormal.x < 0)
                    {
                        Transform.position = new Vector3(WallHit.point.x - WallRadius.PToW(), Transform.position.y, Transform.position.z);
                        Cmn.Xsp = 0;
                        Cmn.Gsp = 0;
                    }

                return true;
            }
            
            return false;
        }
        // ------------------- //
        // ----- UTILITY ----- //
        // ------------------- //
        
        public bool FindWinnerRay(Vector2 OriginLeft, Vector2 TargetLeft,
            Vector2 OriginRight, Vector2 TargetRight, out RaycastHit2D Winner)
        {
            WDebug.DrawLine(OriginLeft, TargetLeft, Color.red);
            WDebug.DrawLine(OriginRight, TargetRight, Color.red);
            
            var LeftHit = Physics2D.Linecast(OriginLeft, TargetLeft,  RayMask);
            var RightHit = Physics2D.Linecast(OriginRight, TargetRight,  RayMask);

            if (LeftHit.collider != null && RightHit.collider != null)
            {
                WDebug.DrawLine(OriginLeft, TargetLeft, Color.green);
                WDebug.DrawLine(OriginRight, TargetRight, Color.green);
                
                if (LeftHit.distance < RightHit.distance) Winner = LeftHit;
                else Winner = RightHit;
                
                return true;
            }

            if (RightHit.collider == null && LeftHit.collider != null)
            {
                WDebug.DrawLine(OriginLeft, TargetLeft, Color.green);

                Winner = LeftHit;
                return true;
            }

            if (RightHit.collider != null && LeftHit.collider == null)
            {
                WDebug.DrawLine(OriginRight, TargetRight, Color.green);

                Winner = RightHit;
                return true;
            }

            Winner = new RaycastHit2D();
            return false;
        }
        
        public float CalculateAngle(Vector2 normal)
        {
            float angle = Vector2.SignedAngle(Vector2.up, normal) > 0 ? 
                Vector2.SignedAngle(Vector2.up, normal) : 
                360 + Vector2.SignedAngle(Vector2.up, normal);
            return angle;
        }
    }
}