using System;
using SonicCTK.Core;
using UnityEngine;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Stores collision information aswell as functions commonly used across <b>PlayerStates</b>
    /// </summary>
    [Serializable]
    public class PlayerCollision
    {
        public Player Player;

        public PlayerCommon Cmn
        {
            get
            {
                return Player.Common;
            }
        }

        [Header("Common")]
        public Vector2 DefaultCollisionSize = new Vector2(19, 39);
        public Vector2 SpinCollisionSize = new Vector2(15, 29);

        [HideInInspector] public Vector2 PlayerCollisionSize = new Vector2(19, 39);
        [HideInInspector] public Vector2 HitboxRadius => new Vector2(WidthRadius - 1, HeightRadius - 3);

        public float WidthRadius => Mathf.Ceil(PlayerCollisionSize.x / 2);
        public float HeightRadius => Mathf.Ceil(PlayerCollisionSize.y / 2);

        public LayerMask RayMask;

        private LayerMask RayMask_NoJumpThru => RayMask & ~(1 << LayerMask.NameToLayer("JumpThru"));

        public RaycastHit2D FloorHit, CeilingHit, WallHit;

        public byte AngleMode = 0b10; // 0b00 = floor, 0b01 = right, 0b11 = left, 0b10 = ceiling

        [Header("Params")] public float FloorRayLength = 32;
        public float FloorCeilRayLateralOffset = 9;

        public float WallRadius;

        public void FigureOutAngleMode()
        {
            float angle = Mathf.Round(Player.Common.SurfaceAngle);
            if ((angle >= 0 && angle <= 45) || (angle <= 360 && angle >= 315)) AngleMode = 0b00;
            if (angle >= 46 && angle <= 134) AngleMode = 0b01;
            if (angle >= 135 && angle <= 225) AngleMode = 0b10;
            if (angle >= 226 && angle <= 314) AngleMode = 0b11;
        }
        
        public bool FloorCollision()
        {
            FigureOutAngleMode();
            
            Transform transform = Player.transform;
            Vector2 leftOrigin = transform.position, rightOrigin = transform.position;

            Vector2 direction = Vector2.down;

            float sensorSpacing = Mathf.Floor(PlayerCollisionSize.x / 2);

            switch (AngleMode)
            {
                case 0b00: // Floor Mode
                    leftOrigin -= new Vector2(sensorSpacing, 0).PToW();
                    rightOrigin += new Vector2(sensorSpacing, 0).PToW();
                    
                    if (FindWinnerRay(leftOrigin, leftOrigin + direction * FloorRayLength.PToW(), 
                            rightOrigin, rightOrigin + direction * FloorRayLength.PToW(), 
                            RayMask, out FloorHit))
                    {
                        transform.position = new Vector3(transform.position.x, 
                            FloorHit.point.y + HeightRadius.PToW(), 
                            transform.position.z);
                        Player.Common.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;

                case 0b01: // Right Wall Mode
                    leftOrigin -= new Vector2(0, sensorSpacing).PToW();
                    rightOrigin += new Vector2(0, sensorSpacing).PToW();

                    direction = Vector2.right;
                    
                    if (FindWinnerRay(leftOrigin, leftOrigin + direction * FloorRayLength.PToW(), 
                            rightOrigin, rightOrigin + direction * FloorRayLength.PToW(), 
                            RayMask, out FloorHit))
                    {
                        transform.position = new Vector3(FloorHit.point.x - HeightRadius.PToW(), 
                            transform.position.y, 
                            transform.position.z);
                        Player.Common.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;

                case 0b10: // Ceiling Mode
                    leftOrigin += new Vector2(sensorSpacing, 0).PToW();
                    rightOrigin -= new Vector2(sensorSpacing, 0).PToW();

                    direction = Vector2.up;
                    
                    if (FindWinnerRay(leftOrigin, leftOrigin + direction * FloorRayLength.PToW(), 
                            rightOrigin, rightOrigin + direction * FloorRayLength.PToW(), 
                            RayMask, out FloorHit))
                    {
                        transform.position = new Vector3(transform.position.x, 
                            FloorHit.point.y - HeightRadius.PToW(), 
                            transform.position.z);
                        Player.Common.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;

                case 0b11: // Left Wall Mode
                    leftOrigin += new Vector2(0, sensorSpacing).PToW();
                    rightOrigin -= new Vector2(0, sensorSpacing).PToW();

                    direction = Vector2.left;
                    
                    if (FindWinnerRay(leftOrigin, leftOrigin + direction * FloorRayLength.PToW(), 
                            rightOrigin, rightOrigin + direction * FloorRayLength.PToW(),
                            RayMask, out FloorHit))
                    {
                        transform.position = new Vector3(FloorHit.point.x + HeightRadius.PToW(), 
                            transform.position.y, 
                            transform.position.z);
                        Player.Common.SurfaceAngle = CalculateAngle(FloorHit.normal);
                        return true;
                    }
                    break;
            }

            return false;
        }

        public bool WallCollision()
        {
            FigureOutAngleMode();
            
            Transform transform = Player.transform;
            Vector2 velocityOffset = new Vector2(Cmn.Xsp, Cmn.Ysp).PToW();
            Vector2 leftOrigin = transform.position, rightOrigin = transform.position;

            var verticalOffset = (Cmn.SurfaceAngle == 0 || Cmn.SurfaceAngle == 360) ? 8 : 0;

            leftOrigin += velocityOffset;
            rightOrigin += velocityOffset;

            // TODO: Implement checks for the other angle modes
            switch (AngleMode)
            {
                case 0b00:
                    if (FindWinnerRay(leftOrigin + new Vector2(0, verticalOffset).PToW(), leftOrigin - new Vector2(WallRadius, -verticalOffset).PToW(),
                            rightOrigin + new Vector2(0, verticalOffset).PToW(), rightOrigin + new Vector2(WallRadius, verticalOffset).PToW(), 
                            RayMask, out WallHit))
                    {
                        Vector2 WallNormal = WallHit.normal;
                        if (Cmn.Gsp < 0)
                            if (WallNormal.x > 0)
                            {
                                transform.position = new Vector3(WallHit.point.x + WallRadius.PToW(), transform.position.y, transform.position.z);
                                Cmn.Xsp = 0;
	                            Cmn.Gsp = 0;
	                            if (Cmn.InputDirection.x != 0) Cmn.IsPushing = true;
                            }
                        
                        if (Cmn.Gsp > 0)
                            if (WallNormal.x < 0)
                            {
                                transform.position = new Vector3(WallHit.point.x - WallRadius.PToW(), transform.position.y, transform.position.z);
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
       
        public bool CeilingCollision(byte velocityDirection = 0b10)
        {
            Transform transform = Player.transform;
            Vector2 leftOrigin = transform.position, rightOrigin = transform.position;

            Vector2 direction = Vector2.up;
            
            leftOrigin -= new Vector2(WidthRadius, 0).PToW();
            rightOrigin += new Vector2(WidthRadius, 0).PToW();
            
            if (FindWinnerRay(leftOrigin, leftOrigin + direction * HeightRadius.PToW(), 
                    rightOrigin, rightOrigin + direction * HeightRadius.PToW(),
                    RayMask_NoJumpThru, out CeilingHit))
            {
                Player.transform.position = new Vector3(Player.transform.position.x,
                    CeilingHit.point.y - Player.Collision.HeightRadius.PToW(), Player.transform.position.z);

                var surfaceAngle = Mathf.Round(CalculateAngle(CeilingHit.normal));

                if (surfaceAngle >= 91 && surfaceAngle <= 225)
                {
                    Cmn.Ysp = 0;
                    transform.position += (Vector3)(new Vector2(0, 1).PToW());
                    return false;
                }
                else
                {
                    if (velocityDirection != 0b10)
                    {
                        Cmn.Ysp = 0;
                        transform.position += (Vector3)(new Vector2(0, 1).PToW());
                        return false;
                    }
                    else
                    {
                        Cmn.SurfaceAngle = surfaceAngle;
                        Cmn.Gsp = Cmn.Ysp * -Mathf.Sign(Mathf.Sin(Cmn.SurfaceAngleRad));
                        return true;
                    }
                }
            }

            return false;
        }
        public bool AirFloorCollision(byte velocityDirection = 0b00)
        {
            Transform transform = Player.transform;
            Vector2 leftOrigin = transform.position, rightOrigin = transform.position;

            Vector2 direction = Vector2.down;

            float sensorDistance = HeightRadius;
            bool landCondition = (velocityDirection != 0b01 && velocityDirection != 0b11) || Cmn.Ysp > 0; 
            
            leftOrigin -= new Vector2(WidthRadius, 0).PToW();
            rightOrigin += new Vector2(WidthRadius, 0).PToW();
                    
            if (FindWinnerRay(leftOrigin, leftOrigin + direction * sensorDistance.PToW(), 
                    rightOrigin, rightOrigin + direction * sensorDistance.PToW(), 
                    RayMask, out FloorHit))
            {
                if (!landCondition) return false;
                
                Cmn.SurfaceAngle = Mathf.Round(CalculateAngle(FloorHit.normal));

                if ((Cmn.SurfaceAngle >= 0 && Cmn.SurfaceAngle <= 23) || (Cmn.SurfaceAngle <= 360 && Cmn.SurfaceAngle >= 339))
                {
                    Cmn.Gsp = Cmn.Xsp;
                }
                else if ((Cmn.SurfaceAngle >= 24 && Cmn.SurfaceAngle <= 45) || (Cmn.SurfaceAngle <= 338 && Cmn.SurfaceAngle >= 316))
                {
                    if (velocityDirection == 0b01 || velocityDirection == 0b11)
                    {
                        Cmn.Gsp = Cmn.Xsp;
                    }
                    else Cmn.Gsp = Cmn.Ysp * 0.5f * -Mathf.Sign(Mathf.Sin(Cmn.SurfaceAngleRad));
                }
                else
                {
                    if (velocityDirection == 0b01 || velocityDirection == 0b11)
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
            Transform transform = Player.transform;
            Vector2 leftOrigin = transform.position, RightOrigin = transform.position;
            
            if (FindWinnerRay(leftOrigin, leftOrigin - new Vector2(WallRadius, 0).PToW(),
                    RightOrigin , RightOrigin + new Vector2(WallRadius, 0).PToW(), RayMask_NoJumpThru, out WallHit))
            {
                Vector2 wallNormal = WallHit.normal;
                if (Cmn.Xsp < 0)
                    if (wallNormal.x > 0)
                    {
                        transform.position = new Vector3(WallHit.point.x + WallRadius.PToW(), transform.position.y, transform.position.z);
                        Cmn.Xsp = 0;
                    }
                
                if (Cmn.Xsp > 0)
                    if (wallNormal.x < 0)
                    {
                        transform.position = new Vector3(WallHit.point.x - WallRadius.PToW(), transform.position.y, transform.position.z);
                        Cmn.Xsp = 0;
                    }

                return true;
            }
            
            return false;
        }
        // ------------------- //
        // ----- UTILITY ----- //
        // ------------------- //
        
        public bool FindWinnerRay(Vector2 originLeft, Vector2 targetLeft,
            Vector2 originRight, Vector2 targetRight, LayerMask rayMask, out RaycastHit2D winner)
        {
            CDebug.DrawLine(originLeft, targetLeft, Color.red);
            CDebug.DrawLine(originRight, targetRight, Color.red);
            
            var leftHit = Physics2D.Linecast(originLeft, targetLeft,  rayMask);
            var rightHit = Physics2D.Linecast(originRight, targetRight,  rayMask);

            if (leftHit.collider != null && rightHit.collider != null)
            {
                CDebug.DrawLine(originLeft, targetLeft, Color.green);
                CDebug.DrawLine(originRight, targetRight, Color.green);
                
                if (leftHit.distance < rightHit.distance) winner = leftHit;
                else winner = rightHit;
                
                return true;
            }

            if (rightHit.collider == null && leftHit.collider != null)
            {
                CDebug.DrawLine(originLeft, targetLeft, Color.green);

                winner = leftHit;
                return true;
            }

            if (rightHit.collider != null && leftHit.collider == null)
            {
                CDebug.DrawLine(originRight, targetRight, Color.green);

                winner = rightHit;
                return true;
            }

            winner = new RaycastHit2D();
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