using UnityEditor;
using UnityEngine;
using Stray.Core;

namespace Stray.Editor
{
	public class StageConfigurationEditor : EditorWindow
	{
		private Object asset;
		private UnityEditor.Editor assetEditor;
		
        [MenuItem(Constants.ContextName + "/Stage Configuration")]
        public static StageConfigurationEditor Create()
        {
	        var window = CreateWindow<StageConfigurationEditor>($"Stage Configuration Editor | {nameof(StageConfiguration)}");
	        window.asset = Resources.Load<StageConfiguration>("Stage Configuration");
	        window.assetEditor = UnityEditor.Editor.CreateEditor(Resources.Load<StageConfiguration>("Stage Configuration"));
	        return window;        
        }
        
		private void OnGUI(){
			GUI.enabled = false;
			asset = EditorGUILayout.ObjectField("Asset", asset, asset.GetType(), false);
			GUI.enabled = true;

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			assetEditor.OnInspectorGUI();
			EditorGUILayout.EndVertical();
		}
    }
}