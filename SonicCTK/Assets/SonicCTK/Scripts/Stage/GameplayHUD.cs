using System.Collections;
using System.Collections.Generic;
using System;

using TMPro;
using SonicCTK.World;

using UnityEngine;
using UnityEngine.UI;

namespace SonicCTK.Gameplay
{
    public sealed class GameplayHUD : MonoBehaviour
    {
        [Header("HUD Elements")]
        public TMP_Text ScoreCounter;
        public TMP_Text TimeCounter_Minutes, TimeCounter_Seconds;
        public TMP_Text RingsCounter;

        private void Awake()
        {
            var canvasScaler = GetComponentInParent<CanvasScaler>();
            canvasScaler.referenceResolution = GameConfiguration.Instance.GameReferenceResolution;
        }
        
        private void Start()
        {

        }

        private void Update()
        {
            if (Stage.Current)
            {
                TimeSpan StageTime = TimeSpan.FromSeconds(Stage.Current.StageTime);

                TimeCounter_Minutes.text = StageTime.Minutes.ToString("0");
                TimeCounter_Seconds.text = StageTime.Seconds.ToString("00");

                RingsCounter.text = Stage.Current.RingsCollected.ToString();
            }
        }
    }

}