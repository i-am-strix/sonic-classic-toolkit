using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.Physical;
using UnityEngine;

namespace SonicCTK
{
    public class Bumper : HitBox
    {
        public float Strength = 7f;
        private Animator animator;
        
        private void Reset()
        {
            Radius = Vector2.one * 9;
        }
        private void Start()
        {
            animator = GetComponent<Animator>();
            OnOverlap += WatchOnOverlap;
        }

        private void WatchOnOverlap(Player player)
        {
            float angle = Vector2.SignedAngle(Vector2.right, (player.transform.position - transform.position).normalized) * Mathf.Deg2Rad;

            animator.Play("Trigger", 0, 0);
            CAudio.Play("Bumper");
            Debug.Log(angle * Mathf.Rad2Deg);
            player.Common.Xsp = player.Common.Gsp = Strength * Mathf.Cos(angle);
            player.Common.Ysp = Strength * -Mathf.Sin(angle);
            
            player.Common.GrantScore(ScoreGrantAmount.Score100, transform);
        }
    }
}
