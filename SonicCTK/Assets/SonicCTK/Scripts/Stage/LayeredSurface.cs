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
        }

        private void OnPlayerConnected(Player player)
        {
            CDebug.Log("test");
            player.OnBeforeUpdate += UpdateCollider;
        }

        private void UpdateCollider(Player player)
        {
			CDebug.Log("is this even updating");
            thisCollider.enabled = player.Interactions.CurrentPlayerLayer == LayerMode;
        }
    }
}