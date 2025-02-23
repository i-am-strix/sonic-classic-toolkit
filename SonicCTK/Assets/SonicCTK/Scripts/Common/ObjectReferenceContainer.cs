using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Core
{
    [CreateAssetMenu(fileName = "Generic Reference Container", menuName = Constants.ContextName + "/Common/Container/Generic")]
    public  class ObjectReferenceContainer : ScriptableSingleton<ObjectReferenceContainer>
    {       
        [Serializable]
        public struct ReferenceContainer
        {
            public string ID;
            public GameObject VFX;
        }

        [Header("Make sure to have this container in your Resources folder")]
        public ReferenceContainer[] References;

        public static GameObject Spawn(string ID, Vector3 Position)
        {
            return GameObject.Instantiate(
                Array.Find(Instance.References, s => s.ID == ID).VFX,
                (Vector2)Position,
                Quaternion.identity);
        }
    }

}