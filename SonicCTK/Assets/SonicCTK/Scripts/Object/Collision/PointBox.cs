using System;
using System.Collections.Generic;
using UnityEngine.Events;
using SonicCTK.Physical;
using SonicCTK.World;
using static SonicCTK.CCommon;
using UnityEngine;


namespace SonicCTK.Core
{
    /// <summary>
    /// A point box is a type of collision box that doesn't have any solidity, but fires corresponding events whenever a
    /// player's center point enters its bounds. Compared to other collision boxes, it uses a Box Collider 2D's bounds
    /// area to define the bounds in which a player can be detected, and fires corresponding events when such thing happens
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D)), DefaultExecutionOrder(1)]
    public abstract class PointBox : MonoBehaviour
    {
        public BoxCollider2D thisCollider;
        public event CCollisionOverlapEvent OnBeginOverlap;
        public event CCollisionOverlapEvent OnOverlap;
        public event CCollisionOverlapEvent OnEndOverlap;

        private readonly List<Player> playersInBox = new();
        
        public virtual void Update()
        {
            foreach (Player player in Stage.ConnectedPlayers)
            {
                BeginOverlap(player);
                Overlap(player);
                EndOverlap(player);
            }
        }

        public virtual void BeginOverlap(Player Player)
        {
            if (playersInBox.Contains(Player)) return;

            if (thisCollider.OverlapPoint(Player.transform.position))
            {
                OnBeginOverlap?.Invoke(Player);
                playersInBox.Add(Player);
            }
        }

        public virtual void Overlap(Player Player)
        {
            if (thisCollider.OverlapPoint(Player.transform.position)) OnOverlap?.Invoke(Player);
        }
        
        public virtual void EndOverlap(Player Player)
        {
            if (!playersInBox.Contains(Player)) return;
            if (!thisCollider.OverlapPoint(Player.transform.position))
            {
                OnEndOverlap?.Invoke(Player);
                playersInBox.Remove(Player);
            }
        }
    }
}