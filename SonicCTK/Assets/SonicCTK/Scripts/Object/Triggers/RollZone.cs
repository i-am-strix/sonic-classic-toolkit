using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.Physical;
using UnityEngine;

namespace SonicCTK
{
    public class RollZone : PointBox
    {
        private void Start()
        {
            OnBeginOverlap += EnterArea;
            OnEndOverlap += ExitArea;
        }

        private void ExitArea(Player player)
        {
            player.Interactions.IsSTunnelMode = false;
        }

        private void EnterArea(Player player)
        {
            player.Interactions.IsSTunnelMode = true;
        }
        

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = new Color32(212, 89, 255, 100);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}