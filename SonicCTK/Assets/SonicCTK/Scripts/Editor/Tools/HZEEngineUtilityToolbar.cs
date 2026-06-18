using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using SonicCTK.Core;
using UnityToolbarExtender;


namespace SonicCTK.Editor
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			EngineEditorConfig.AttemptRefresh();
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUIRight);
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUILeft);
		}

		static void OnToolbarGUILeft()
		{
			GUIStyle selectorStyle = new GUIStyle("dropdown");
			GUI.backgroundColor = Color.white;
			selectorStyle.fixedWidth = 150;
			EngineEditorConfig.currentConfig.CurrentCharacter = (CharacterID)EditorGUILayout.EnumPopup("Entering Play Mode as", 
				EngineEditorConfig.currentConfig.CurrentCharacter, 
				selectorStyle);
		}

		static void OnToolbarGUIRight()
		{
			GUIStyle style = new GUIStyle(ToolbarStyles.commandButtonStyle);
			style.fixedWidth = 72;
			style.fontSize = 13;
	
			if(GUILayout.Button(new GUIContent("Stages"), style))
			{
				SonicCTK.Editor.StageConfigurationEditor.Create();
			}
		}
	}

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void StartScene(string sceneName)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			sceneToOpen = sceneName;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (sceneToOpen == null ||
			    EditorApplication.isPlaying || EditorApplication.isPaused ||
			    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			EditorApplication.update -= OnUpdate;

			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				// need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time
				string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
				if (guids.Length == 0)
				{
					UnityEngine.Debug.LogWarning("Couldn't find scene file");
				}
				else
				{
					/*string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
					EditorSceneManager.OpenScene(scenePath);
					EditorApplication.isPlaying = true;*/
				}
			}
			sceneToOpen = null;
		}
	}
}