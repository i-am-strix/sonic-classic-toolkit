using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.Physical;
using SonicCTK.World;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SonicCTK.Gameplay
{
    public class Ring : HitBox
    {
        public bool IsBouncingRing;
        private bool wasBouncing;

        public float Xsp, Ysp;

        private int ringLifeSpan = 256;
        private Animator ringAnimator;
        private SpriteRenderer ringSprite;
        
        private const float RING_GRAVITY = 0.09375f;

        private bool isMagnetised;
        private Player magnetizedPlayer;

        private void Reset()
        {
            Radius = Vector2.one * 6.5f;
            BoxOffset = Vector2.one * 0.5f;

            ringSprite = GetComponent<SpriteRenderer>();
            ringAnimator = GetComponent<Animator>();
        }

        void Start()
        {
            ringSprite = GetComponent<SpriteRenderer>();
            ringAnimator = GetComponent<Animator>();
            
            OnOverlap += OnCollect;

            ringAnimator.SetFloat("Speed", 1f / 8);
        }

        private IEnumerator RingCollisionSequence()
        {
            var frameCounter = 4;
            while (true)
            {
                frameCounter--;

                if (frameCounter == 0)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ObjectRadius.y.PToW()) && Ysp > 0)
                    {
                        Ysp *= -.75f;
                    }
                    frameCounter = 4;
                }

                yield return null;
            }
        }

        private readonly float[] magnetizedRingAcceleration = new []
        {
            0.75f, 0.1875f
        };
        
        public override void Update()
        {
            base.Update();

            if (isMagnetised)
            {
                var s = new Vector2(Math.Sign(magnetizedPlayer.transform.position.x - transform.position.x),
                    Math.Sign(magnetizedPlayer.transform.position.y - transform.position.y));
                
                var tx = Math.Sign(Xsp) == s.x;
                var ty = Math.Sign(Ysp) == s.y;
	
                Xsp += magnetizedRingAcceleration[tx ? 1 : 0] * s.x;
                Ysp -= magnetizedRingAcceleration[ty ? 1 : 0] * s.y;
            }
            
            if (IsBouncingRing)
            {
                ringLifeSpan--;

                ringAnimator.SetFloat("Speed", CUtility.FDurToSpeed(256f * 2 / ringLifeSpan));

                if (ringLifeSpan < 64) ringSprite.enabled = ringLifeSpan / 4 % 2 == 0;
                
                if (!wasBouncing)
                {
                    wasBouncing = true;
                    StartCoroutine(RingCollisionSequence());
                }

                Ysp += RING_GRAVITY;

                if (ringLifeSpan <= 0) Destroy(gameObject);
            }
            transform.position += new Vector3(Xsp, Ysp).PToW();

            CheckForMagnetization();
        }

        private void CheckForMagnetization()
        {
            if (isMagnetised || IsBouncingRing) return;
            var player = Stage.GetClosestPlayer(transform.position);

            if (player.Interactions.CurrentShieldScript is not PlayerElectricShield) return;
            
            if (Vector3.Distance(player.transform.position, transform.position) < 1.28f)
            {
                isMagnetised = true;
                magnetizedPlayer = player;
            }
        }

        private void OnCollect(Player Player)
        {
            if (Player.Interactions.IgnoreObjectsDuration != 0) return;

            Destroy(gameObject);
            CAudio.PlayOneShot("Ring");
            Stage.Current.RingsCollected++;
            Addressables.InstantiateAsync("S_FX_RingSparkle.prefab", transform.position, Quaternion.identity);
        }
    }
}
