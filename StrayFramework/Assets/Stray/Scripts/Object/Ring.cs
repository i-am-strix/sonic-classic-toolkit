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
        private void Reset()
        {
            radius = Vector2.one * 6.5f;
            boxOffset = Vector2.one * 0.5f;
        }

        void Start()
        {
            OnOverlap += OnCollect;
        }

        private void OnCollect(Player Player)
        {
            Destroy(gameObject);
            WAudio.PlayOneShot("Ring");
            Stage.Current.RingsCollected++;
        }
    }
}
