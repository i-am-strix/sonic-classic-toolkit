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
        
        [Header("Physics")] 
        public float Gsp;

        public float Xsp;
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

        public int ControlLock;
        public void GrabInput() => InputDirection = Player.PlayerInput.GetAxis2D("Move").Round();

        public void CommonUpdate()
        {
            InvulnerabilityTimer = Mathf.Max(0, InvulnerabilityTimer - 1);
            CheckPlayerBounds();
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

            CUtility.SpawnRings(Player.transform.position, Stage.Current.RingsCollected);
            
            Player.Get<PStateHurt>().Hazard = objectDealingDamage;
            Player.Set<PStateHurt>();

            CAudio.Play("Lose Rings");

            Stage.Current.RingsCollected = 0;
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