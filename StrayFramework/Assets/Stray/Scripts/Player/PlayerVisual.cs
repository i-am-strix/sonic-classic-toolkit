using System;
using Stray.Core;
using UnityEngine;

namespace Stray.Physical
{
    [Serializable]
    public class PlayerVisual
    {
        public Player Player;
        public SpriteRenderer PlayerSprite;

        public Animator Anim;
        public int SpriteDirection = 1;

	    public string CurrentAnimation;
        
	    public float SurfaceAngleLerp;
	    public float VisualAngle;

	    [Header("Flags")] 
	    [Tooltip("Use the old 8-rotation system that the Genesis games used instead of the smoother Mania rotation system")]
		public bool UseGenesisSpriteRotation;

        public void VisualUpdate()
        {
	        PlayerSprite.flipX = SpriteDirection < 0;

            if (Player.Cmn.InvulnerabilityTimer != 0)
                PlayerSprite.enabled = (Player.Cmn.InvulnerabilityTimer / 4) % 2 == 0;

        }

        public void VisualLateUpdate()
	    {
		    SurfaceAngleLerp = Mathf.Lerp(SurfaceAngleLerp, (Player.Cmn.SurfaceAngle >= 35.16f && Player.Cmn.SurfaceAngle <= 360 - 35.16f) ? 1 : 0, 10 * Time.deltaTime);
        	
            PlayerSprite.transform.position = Player.transform.position;
		    if (CurrentAnimation != "Spin"){
			    if (Player.Cmn.IsGrounded) VisualAngle = Mathf.LerpAngle(0, Player.Cmn.SurfaceAngle, SurfaceAngleLerp);
			    else VisualAngle = Mathf.MoveTowardsAngle(VisualAngle, 0, 2.8125f);
		    }
		    else VisualAngle = 0;
		    
		    PlayerSprite.transform.eulerAngles = new Vector3(0, 0, UseGenesisSpriteRotation ? 
			    Mathf.Round(VisualAngle / 45f) * 45f : VisualAngle);
        }

        public void Play(string Name)
        {
            Anim.Play(Name);
            CurrentAnimation = Name;
        }
        
        public void SetSpeedMultip(float Value) => 
            Anim.SetFloat("Animation Speed Multiplier", Value != 0 ? (1f / Value) : 1f); 
    }
}