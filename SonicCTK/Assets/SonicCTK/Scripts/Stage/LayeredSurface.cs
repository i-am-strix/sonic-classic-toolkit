using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.World;
using Object = SonicCTK.Gameplay.Object;
using UnityEngine;

namespace SonicCTK.Physical
{
    public enum CLayerMode
    {
        Foreground, Background
    }
    
    [DefaultExecutionOrder(2)]
    public sealed class LayeredSurface : Object
    {
        public CLayerMode LayerMode = CLayerMode.Foreground;

        private Collider2D thisCollider;

        private void Awake()
        {
            thisCollider = GetComponent<Collider2D>();
            Stage.OnPlayerConnected += OnPlayerConnected;
            Stage.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        private void OnPlayerDisconnected(Player player)
        {
            player.OnBeforeUpdate -= UpdateCollider;
        }

        private void OnPlayerConnected(Player player)
        {
            player.OnBeforeUpdate += UpdateCollider;
        }

        private void UpdateCollider(Player player)
        {
            if (!thisCollider) return;
            thisCollider.enabled = player.Interactions.CurrentPlayerLayer == LayerMode;
        }
    }
}