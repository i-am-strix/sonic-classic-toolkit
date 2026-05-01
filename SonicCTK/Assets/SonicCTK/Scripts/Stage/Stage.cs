using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FMODUnity;
using FMOD.Studio;
using SonicCTK.Core;
using SonicCTK.Physical;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;

namespace SonicCTK.World
{
    public sealed class Stage : MonoBehaviour
    {
        /// <summary>
        /// Contains all of the current active players in the stage
        /// </summary>
        public static ObservableCollection<Player> ConnectedPlayers = new();
        public static Stage Current;

        [Header("Gameplay")]
        public float StageTime = 0;
        public bool CanUpdateStageTime = true;

        public int RingsCollected;

        [Header("Room Data")] 
        public Vector2 StageSize = new Vector2Int(1000, 800);
        public static EventInstance StageBGM;
        public static StageConfiguration.StageData StageData;

        [Header("Debug")]
        public bool ShowStageBorders = true;

        // Events
        public delegate void PlayerSignalEvent(Player Player);

        public static event PlayerSignalEvent OnPlayerConnected;
        public static event PlayerSignalEvent OnPlayerDisconnected;
        
        public static Player GetClosestPlayer(Vector2 position)
        {
            Player closest = null;
            var closestDistance = float.MaxValue;

            if (ConnectedPlayers.Count == 1)
            {
                closest = ConnectedPlayers[0];
                return closest;
            }
            
            for(int i = 0; i < ConnectedPlayers.Count; i++)
            {
                var dist = Vector2.Distance((Vector2)ConnectedPlayers[i].transform.position, position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = ConnectedPlayers[i];

                }
            }

            return closest;
        }

        private void Awake()
        {
            // Set this as the current main stage
            Stage.Current = this;
            
            // Flush any previously connected player, in the case of a stage reset or start
            ConnectedPlayers = new();
            ConnectedPlayers.CollectionChanged += ConnectedPlayersOnCollectionChanged;
        }

        private void ConnectedPlayersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CDebug.Log("New Player Connected");
                    OnPlayerConnected?.Invoke(e.NewItems[0] as Player);
                    break;
            }
        }

        private void Start()
        {
            StageData = StageConfiguration.Instance.FindFromScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            );
            if (StageBGM.isValid())
            {
                StageBGM.stop(STOP_MODE.IMMEDIATE);
                StageBGM.release();
            }
            
            StageBGM = RuntimeManager.CreateInstance(StageData.StageBGM);
            StageBGM.start();
        }

        private void Update()
        {
            if (CanUpdateStageTime) StageTime += Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            if (!ShowStageBorders) return;
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