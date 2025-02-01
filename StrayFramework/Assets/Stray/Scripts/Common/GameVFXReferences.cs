using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Core
{
    [CreateAssetMenu(fileName = "VFX Container", menuName = Constants.ContextName + "/Common/VFX Container")]
    public sealed class GameVFXReferences : ScriptableSingleton<GameVFXReferences>
    {       
        [Serializable]
        public struct VFXContainer
        {
            public string ID;
            public GameObject VFX;
        }

        [Header("Make sure to have this container in your Resources folder")]
        public VFXContainer[] AvailableVFX;

        public static GameObject SpawnVFX(string ID, Vector3 Position)
        {
            return GameObject.Instantiate(
                Array.Find(Instance.AvailableVFX, s => s.ID == ID).VFX, 
                (Vector2)Position, 
                Quaternion.identity);
        }
    }

}