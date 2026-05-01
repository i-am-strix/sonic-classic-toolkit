using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;

namespace SonicCTK
{
    public class Motobug : EnemyBase
    {
        public float MovementSpeed = 1f;
        public override void Update()
        {
            transform.position += (Vector3.left * MovementSpeed).PToW();

            var hit = Physics2D.Raycast(transform.position, Vector2.down, (PlayerHazardousHitbox.Radius.y + 2).PToW());
            if (hit)
            {
                transform.position =
                    new Vector3(transform.position.x, hit.point.y + PlayerHazardousHitbox.Radius.y.PToW());
            }
        }
    }
}
