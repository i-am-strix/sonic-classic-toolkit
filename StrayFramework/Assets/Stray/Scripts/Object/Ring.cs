using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stray.Core;
using Stray.Physical;
using Stray.World;

namespace Stray.Gameplay
{
    public class Ring : HitBox
    {
        public bool IsBouncingRing;
        private bool WasBouncing;
        private const float RingGravity = 0.09375f;

        public float Xsp, Ysp;

        private int RingLifeSpan = 256;

        private Animator RingAnimator;

        private void Reset()
        {
            radius = Vector2.one * 6.5f;
            boxOffset = Vector2.one * 0.5f;
        }

        void Start()
        {
            RingAnimator = GetComponent<Animator>();
            OnOverlap += OnCollect;

            RingAnimator.SetFloat("Speed", 1f / 8);
        }

        private IEnumerator RingCollisionSequence()
        {
            var frameCounter = 4;
            while (true)
            {
                frameCounter--;

                if (frameCounter == 0)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, Radius.y.PToW()) && Ysp > 0)
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
                RingLifeSpan--;

                RingAnimator.SetFloat("Speed", WUtility.FDurToSpeed(256f * 2 / RingLifeSpan));
                
                if (!WasBouncing)
                {
                    WasBouncing = true;
                    StartCoroutine(RingCollisionSequence());
                }

                Ysp += RingGravity;

                transform.position += new Vector3(Xsp, Ysp).PToW();

                if (RingLifeSpan <= 0) Destroy(gameObject);
            }
        }

        private void OnCollect(Player Player)
        {
            if (Player.Interactions.IgnoreObjectsDuration != 0) return;

            Destroy(gameObject);
            WAudio.PlayOneShot("Ring");
            Stage.Current.RingsCollected++;
            ObjectReferenceContainer.Spawn("Ring Sparkle", transform.position);
        }
    }
}
