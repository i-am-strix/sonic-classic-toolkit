using UnityEngine;
using System;
using UnityEngine.Events;

namespace Stray.Core
{
    public class HitBox : MonoBehaviour
    {
        public Vector2 radius;
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

        public void Collide(Player Player)
        {
            var PixelPlayerPos = ((Vector2)Player.transform.position).WToP();
            var PixelObjectPos = ((Vector2)transform.position).WToP();
        }
    }
}