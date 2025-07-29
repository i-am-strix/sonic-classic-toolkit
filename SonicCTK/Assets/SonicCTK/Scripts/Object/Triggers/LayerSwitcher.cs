using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.Physical;
using UnityEngine;

namespace SonicCTK
{
    public class LayerSwitcher : PointBox
    {
        private void Start()
        {
            OnOverlap += TestThing;   
        }

        private void TestThing(Player player)
        {
            Vector3 transformedPosition = transform.InverseTransformPoint(player.transform.position);

            if (transformedPosition.x < 0) player.Interactions.CurrentPlayerLayer = CLayerMode.Background;
            if (transformedPosition.x > 0) player.Interactions.CurrentPlayerLayer = CLayerMode.Foreground;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = Constants.ColorConstants.ForegroundLayerColor;
            Gizmos.DrawCube(new Vector3(0.25f, 0), new Vector3(0.5f, 1));
            
            Gizmos.color = Constants.ColorConstants.BackgroundLayerColor;
            Gizmos.DrawCube(new Vector3(-0.25f, 0), new Vector3(0.5f, 1));
        }
    }
}
