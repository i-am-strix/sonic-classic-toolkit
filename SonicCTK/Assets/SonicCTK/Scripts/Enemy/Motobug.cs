using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;

namespace SonicCTK
{
    public class Motobug : EnemyBase
    {
        public float MovementSpeed = 1f;

        private float movementDirection = 1;
        private bool canCheckForMissingCollision = true;

        private bool holdingMovement = false;

        private SpriteRenderer sprite;
        private Animator animator;

        public override void Start()
        {
            base.Start();
            sprite = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public override void Update()
        {
            base.Update();
            
            if (!holdingMovement)
                transform.position += (Vector3.left * MovementSpeed * movementDirection).PToW();

            var hit = Physics2D.Raycast(transform.position, Vector2.down, (PlayerHazardousHitbox.Radius.y + 2).PToW());
            if (!canCheckForMissingCollision) return;
            
            if (hit)
            {
                transform.position =
                    new Vector3(transform.position.x, hit.point.y + PlayerHazardousHitbox.Radius.y.PToW());
            }
            else
            {
                StartCoroutine(WaitForSwitchDirection());
            }
        }

        private IEnumerator WaitForSwitchDirection()
        {
            animator.enabled = false;
            canCheckForMissingCollision = false;
            holdingMovement = true;
            
            yield return new WaitForSeconds(1);
            movementDirection *= -1;
            holdingMovement = false;
            animator.enabled = true;

            transform.position += Vector3.left * movementDirection * 1f.PToW();
            sprite.flipX = !sprite.flipX;
            
            canCheckForMissingCollision = true;
        }
    }
}
