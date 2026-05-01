using System;
using SonicCTK.Core;
using SonicCTK.World;
using UnityEngine;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Contains common functions and variables used by all <b>PlayerStates</b>
    /// </summary>
    [Serializable]
    public class PlayerCommon
    {
        public Player Player;
        
        /// <summary>
        /// When grounded, <b>Gsp</b> (Ground Speed) controls both Xsp and Ysp based on the terrain angle
        /// </summary>
        [Header("Physics")] 
        public float Gsp;

        /// <summary>
        /// Horizontal Player Speed
        /// </summary>
        public float Xsp;
        
        /// <summary>
        /// Vertical Player Speed (negative = upwards)
        /// </summary>
        public float Ysp;

        public float SurfaceAngle;

        public float SurfaceAngleRad
        {
            get => SurfaceAngle * Mathf.Deg2Rad;
            set => SurfaceAngle = value * Mathf.Rad2Deg;
        }

        public Vector2 InputDirection;

        [Header("Damage")]
        public int InvulnerabilityTimer;
        

        [Header("Flags")] 
	    public bool IsGrounded;
	    public bool IsPushing;
        public bool IsCurledIntoBall;

        public int ControlLock;

        [Header("Camera Offsets")] public float LookUpCameraOffset = 104;
        public float CrouchCameraOffset = 88;

        private int cameraVerticalOffsetTicks;
        private float cameraVerticalOffsetSpeed = 2;

        /// <summary>
        /// Use this flag to block character specific abilities from being
        /// usable in certain scenarios
        /// </summary>
        public bool CanCharacterPerformSpecialAbilities = true;
        
        public void GrabInput() => InputDirection = Player.PlayerInput.GetAxis2D("Move").Round();

        public void CommonStart()
        {
            Player.OnStateChanged += (state, newState) =>
            {
                if (newState is PStateLookUp or PStateCrouch)
                {
                    cameraVerticalOffsetTicks = 120;
                }
                else cameraVerticalOffsetTicks = 0;
            };
        }
        
        public void CommonUpdate()
        {
            InvulnerabilityTimer = Mathf.Max(0, InvulnerabilityTimer - 1);
            CheckPlayerBounds();
            HandleCameraOffset();
        }

        public void ApplyTransformations()
        {
            Player.transform.position += (Vector3)(new Vector2(Xsp, Ysp).PToW());
        }

        public void CheckPlayerBounds()
        {
            if (Player.transform.position.WToP().x - Player.Collision.WidthRadius < 0)
            {
                Gsp = 0;
                Xsp = 0;
                Player.transform.position = new Vector3(Player.Collision.WidthRadius.PToW(), 
                    Player.transform.position.y, 
                    Player.transform.position.z);
            }

            if (Player.transform.position.WToP().y + Player.Collision.HeightRadius > Stage.Current.StageSize.y) 
                KillPlayer();
        }

        public void TakeDamage(GameObject objectDealingDamage = null)
        {
            if (Player.Is<PStateHurt>() || Player.Is<PStateDead>()) return;
            if (InvulnerabilityTimer != 0) return;

            if (Stage.Current.RingsCollected == 0)
            {
                Player.Set<PStateDead>();
                CAudio.Play("Die");
                return;
            }

            Player.Interactions.IgnoreObjectsDuration = 64;
            Player.Interactions.IsStandingOnSolidBox = false;

            if (Player.Interactions.ShieldType == PlayerShieldType.None)
            {
                CUtility.SpawnRings(Player.transform.position, Stage.Current.RingsCollected);
                Stage.Current.RingsCollected = 0;
                CAudio.Play("Lose Rings");
            }
            else
            {
                Player.Interactions.CurrentShieldScript?.OnShieldLost(Player);
                Player.Interactions.CurrentShieldScript = null;
                Player.Interactions.SetShield(PlayerShieldType.None);
            }
            
            Player.Get<PStateHurt>().Hazard = objectDealingDamage;
            Player.Set<PStateHurt>();
        }

        public void HandleCameraOffset()
        {
            if (cameraVerticalOffsetTicks != 0) cameraVerticalOffsetTicks--;
            else
            {
                if (Player.CurrentState is PStateCrouch)
                    Player.Cam.CameraOffset.y = Mathf.MoveTowards(Player.Cam.CameraOffset.y, CrouchCameraOffset, cameraVerticalOffsetSpeed);
                else if (Player.CurrentState is PStateLookUp) 
                    Player.Cam.CameraOffset.y = Mathf.MoveTowards(Player.Cam.CameraOffset.y, -LookUpCameraOffset, cameraVerticalOffsetSpeed);
                else 
                    Player.Cam.CameraOffset.y = Mathf.MoveTowards(Player.Cam.CameraOffset.y, 0, cameraVerticalOffsetSpeed);;
            }
        }

        public void KillPlayer()
        {
            if (Player.Is<PStateDead>()) return;

            InvulnerabilityTimer = 0;

            Player.Set<PStateDead>();
            CAudio.Play("Die");
        }
    }
}