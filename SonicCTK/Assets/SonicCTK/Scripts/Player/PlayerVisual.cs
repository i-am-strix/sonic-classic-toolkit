using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SonicCTK.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Contains visual info about the player, such as sprite direction, animation state etc
    /// </summary>
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
	    
	    private int skidSmokeSpawnTick = 4;

	    [Header("Flags")] 
	    [Tooltip("Use the old 8-rotation system that the Genesis games used instead of the smoother Mania rotation system")]
		public bool UseGenesisSpriteRotation;

        public void VisualUpdate()
        {
	        PlayerSprite.flipX = SpriteDirection < 0;

            if (Player.Common.InvulnerabilityTimer != 0)
                PlayerSprite.enabled = (Player.Common.InvulnerabilityTimer / 4) % 2 == 0;

        }

        public void VisualLateUpdate()
        {
		    SurfaceAngleLerp = Mathf.Lerp(SurfaceAngleLerp, (Player.Common.SurfaceAngle >= 35.16f && Player.Common.SurfaceAngle <= 360 - 35.16f) ? 1 : 0, 10 * Time.deltaTime);
        	
            PlayerSprite.transform.position = Player.transform.position;
		    if (CurrentAnimation != "Spin" && ((Player.Is<PStateMove>() && Player.Common.Gsp != 0) || Player.Is<PStateAir>())){
			    if (Player.Common.IsGrounded) VisualAngle = Mathf.LerpAngle(0, Player.Common.SurfaceAngle, SurfaceAngleLerp);
			    else VisualAngle = Mathf.MoveTowardsAngle(VisualAngle, 0, 2.8125f);
		    }
		    else VisualAngle = 0;
		    
		    PlayerSprite.transform.eulerAngles = new Vector3(0, 0, UseGenesisSpriteRotation ? 
			    Mathf.Round(VisualAngle / 45f) * 45f : VisualAngle);

		    if (Player.CurrentState is PStateMove move)
		    {
			    if (move.IsSkidding)
			    {
				    skidSmokeSpawnTick--;
				    if (skidSmokeSpawnTick <= 0)
				    {
					    Addressables.InstantiateAsync("/Player/FX/Skid Smoke",
							    Player.transform.position + Vector3.down * Player.Collision.HeightRadius.PToW(),
							    Quaternion.identity)
						    .WaitForCompletion();
					    skidSmokeSpawnTick = 4;
				    }
			    }
			    else skidSmokeSpawnTick = 0;
		    }
		    else skidSmokeSpawnTick = 0;
	    }

        public void Play(string Name)
        {
            Anim.Play(Name);
            CurrentAnimation = Name;
        }
        
        public void SetSpeedMultip(float Value) => 
            Anim.SetFloat("Animation Speed Multiplier", Value != 0 ? (1f / Value) : 1f);
        
        public IEnumerator HandleInvincibilityEffect()
        {
	        if (!Player.Common.IsInvincible) yield break;
	        
	        var invincibilitySparkles = new List<GameObject>();
	        for(int i = 0; i < 4; i++)
		        invincibilitySparkles.Add(Addressables.InstantiateAsync("/Player/FX/Invincibility Sparkles").WaitForCompletion());

	        int flashTick = 0, flashIndex = 0;
	        
	        while (Player.Common.IsInvincible)
	        {
		        for (int i = 0; i < 4; i++)
		        {
			        flashTick++;
			        if (flashTick > 8)
			        {
						invincibilitySparkles[i].transform.localScale = new Vector3(invincibilitySparkles[i].transform.localScale.x, 
							invincibilitySparkles[i].transform.localScale.y * -1);
				        flashTick = 0;
				        flashIndex++;
				        if (flashIndex > 3)
					        flashIndex = 0;
			        }
			        
			        invincibilitySparkles[i].SetActive(i <= flashIndex/* && (int)(Time.time * 128) % (i + 1) == 0*/);
			        invincibilitySparkles[i].transform.position = Player.previousFramePosition[31 - i * 3];
		        }
		        yield return null;
	        }
	        
	        for(int i = 0; i < 4; i++)
		        GameObject.Destroy(invincibilitySparkles[i]);
        }
    }
}