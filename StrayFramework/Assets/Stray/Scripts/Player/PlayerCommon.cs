using System;
using Stray.Core;
using Stray.World;
using UnityEngine;

namespace Stray.Physical
{
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
        public void GrabInput() => InputDirection = Player.PlayerInput.GetAxis2D("Move");

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
            if (Player.transform.position.WToP().x - Player.Col.WidthRadius < 0)
            {
                Gsp = 0;
                Xsp = 0;
                Player.transform.position = new Vector3(Player.Col.WidthRadius.PToW(), 
                    Player.transform.position.y, 
                    Player.transform.position.z);
            }

            if (Player.transform.position.WToP().y + Player.Col.HeightRadius > Stage.Current.StageSize.y) KillPlayer();
        }

        public void TakeDamage(GameObject Input = null)
        {
            if (Player.Is<WPStateHurt>() || Player.Is<WPStateDead>()) return;
            if (InvulnerabilityTimer != 0) return;

            if (Stage.Current.RingsCollected == 0)
            {
                Player.Set<WPStateDead>();
                WAudio.Play("Die");
                return;
            }

            Player.Interactions.IgnoreObjectsDuration = 64;
            Player.Interactions.IsStandingOnSolidBox = false;

            WUtility.SpawnRings(Player.transform.position, Stage.Current.RingsCollected);
            
            Player.Get<WPStateHurt>().Hazard = Input;
            Player.Set<WPStateHurt>();

            WAudio.Play("Lose Rings");

            Stage.Current.RingsCollected = 0;
        }

        public void KillPlayer()
        {
            if (Player.Is<WPStateDead>()) return;

            InvulnerabilityTimer = 0;

            Player.Set<WPStateDead>();
            WAudio.Play("Die");
        }
    }
}