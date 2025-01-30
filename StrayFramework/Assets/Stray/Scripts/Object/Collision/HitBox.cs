using UnityEngine;
using System;
using UnityEngine.Events;
using Stray.Physical;

namespace Stray.Core
{
    public class HitBox : MonoBehaviour
    {
        [Tooltip("The half-size of the collision box")]
        public Vector2 radius;
        
        [Tooltip("Offset of the collision box position (in pixels)")]
        public Vector2 boxOffset;
        
        public Vector2 Radius
        {
            get
            {
                return Radius.Round();
            }
        }

        void Update()
        {
            foreach(Player p in FindObjectsOfType<Player>()) Collide(p);
        }

        // Note to self: The guide says hitbox collision happens on the Player end, but I think
        // that's dumb so I'm not doing it #lol
        public void Collide(Player Player)
        {
            var PixelPlayerPos = ((Vector2)Player.transform.position).WToP();
            var PixelObjectPos = ((Vector2)transform.position + boxOffset).WToP();
        }
    }
}