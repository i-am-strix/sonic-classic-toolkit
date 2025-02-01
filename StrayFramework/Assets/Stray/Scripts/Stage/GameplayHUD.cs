using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TMPro;
using Stray.World;

namespace Stray.Gameplay
{
    public sealed class GameplayHUD : MonoBehaviour
    {
        [Header("HUD Elements")]
        public TMP_Text ScoreCounter;
        public TMP_Text TimeCounter_Minutes, TimeCounter_Seconds;
        public TMP_Text RingsCounter;

        void Start()
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