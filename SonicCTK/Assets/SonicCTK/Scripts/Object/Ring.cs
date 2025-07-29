using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicCTK.Core;
using SonicCTK.Physical;
using SonicCTK.World;

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

        public override void Update()
        {
            base.Update();
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

                transform.position += new Vector3(Xsp, Ysp).PToW();

                if (ringLifeSpan <= 0) Destroy(gameObject);
            }
        }

        private void OnCollect(Player Player)
        {
            if (Player.Interactions.IgnoreObjectsDuration != 0) return;

            Destroy(gameObject);
            CAudio.PlayOneShot("Ring");
            Stage.Current.RingsCollected++;
            ObjectReferenceContainer.Spawn("Ring Sparkle", transform.position);
        }
    }
}
