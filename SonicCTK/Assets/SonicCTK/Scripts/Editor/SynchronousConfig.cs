#if UNITY_EDITOR
using SonicCTK.Core;
using UnityEditor;
using UnityEngine;

namespace SonicCTK
{
    /// <summary>
    /// This is used for interoping between the game scripts and the editor scripts (the editor scripts can modify the game ones, not the other way around)
    /// </summary>
    [CreateAssetMenu(menuName = Constants.ContextName + " Editor/Synchronous Config Handler")]
    public class SynchronousConfig : ScriptableObject
    {
        public CharacterID CurrentCharacter;
    }
}
#endif
