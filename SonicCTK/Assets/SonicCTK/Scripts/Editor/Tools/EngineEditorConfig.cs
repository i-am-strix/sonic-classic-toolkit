using SonicCTK.Core;
using UnityEditor;

namespace SonicCTK
{
    public static class EngineEditorConfig
    {
        public static SynchronousConfig currentConfig;

        public static bool AttemptRefresh()
        {
            currentConfig =
                AssetDatabase.LoadAssetAtPath<SynchronousConfig>(
                    "Assets/Editor/SynchronousConfig.asset");
            return true;
        }
        
        
        [InitializeOnEnterPlayMode]
        public static void EnterPlay()
        {
            GamePersistence.CurrentCharacterID = currentConfig.CurrentCharacter;
        }
    }
}