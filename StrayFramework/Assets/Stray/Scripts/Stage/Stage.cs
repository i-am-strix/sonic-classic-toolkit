using UnityEngine;
using System;
using System.Collections.Generic;
using Stray.Core;
using Stray.Physical;

namespace Stray.World
{
    public sealed class Stage : MonoBehaviour
    {
        public static List<Player> ConnectedPlayers = new List<Player>();
        public static Stage Current;

        [Header("Room Data")] public Vector2 StageSize = new Vector2Int(1000, 800);

        private void Awake()
        {
            // Set this as the current main stage
            Stage.Current = this;
            
            // Flush any previously connected player, in the case of a stage reset or start
            ConnectedPlayers = new List<Player>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(new Vector3(-100, StageSize.PToW().y / 2), new Vector3(200, StageSize.PToW().y + 1000));
            Gizmos.DrawCube(new Vector3(StageSize.PToW().x + 100, StageSize.PToW().y / 2), new Vector3(200, StageSize.PToW().y + 1000));
            Gizmos.DrawCube(new Vector3(StageSize.PToW().x / 2, 100), new Vector3(StageSize.PToW().x, 200));
            Gizmos.DrawCube(new Vector3(StageSize.PToW().x / 2, StageSize.PToW().y - 100), new Vector3(StageSize.PToW().x, 200));
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(0, 0), new Vector3(0, StageSize.PToW().y));
            Gizmos.DrawLine(new Vector3(StageSize.PToW().x, 0), new Vector3(StageSize.PToW().x, StageSize.PToW().y));
            Gizmos.DrawLine(new Vector3(0, 0), new Vector3(StageSize.PToW().x, 0));
            Gizmos.DrawLine(new Vector3(0, StageSize.PToW().y), new Vector3(StageSize.PToW().x, StageSize.PToW().y));
        }
    }
}