using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SonicCTK
{
    [CreateAssetMenu(fileName = "Game Configuration", menuName = Constants.ContextName + "/Game Configuration")]
    public sealed class GameConfiguration : ScriptableSingleton<GameConfiguration>
    {
        [Header("Window")] 
        [Tooltip("The resolution that you're \"technically\" making the game for. The game's resolution in editor will be set to this, " +
                 "and all the corresponding camera/UI configurations will pull their resolution from this")]
        public Vector2Int GameReferenceResolution = new Vector2Int(320, 224);
        
        [Header("Game Loop")] 
        public bool UseLives = true;
        public int StartingLifeCountPerSaveFile = 3;

        [Header("Player Physics")] 
        public bool ApplySonic1SpeedCap = false;
        
        /// <summary>
        /// Why would you do this to yourself
        /// </summary>
        public bool ApplyRollLock = false;

        [Header("Defaults")] 
        public AssetReferenceGameObject DefaultPlayerHUD;
    }
}
