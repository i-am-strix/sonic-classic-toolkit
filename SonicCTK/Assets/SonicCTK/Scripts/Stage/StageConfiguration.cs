using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

namespace SonicCTK.Core{
	[CreateAssetMenu(fileName = "New Stage Configuration Container", menuName = Constants.ContextName + "/Stage/Stage Configurations")]
	public class StageConfiguration : ScriptableSingleton<StageConfiguration>
	{
		public const string RelativePath = "Stage Configuration";
		
		[System.Serializable]
		public class StageData {
			public string PublicName;
			
			[Range(1,3)] public uint Act = 1;
			[Space]
			public string InternalName;
			[Scene] public string TargetScene;
			
			[Header("BGM")]
			
			public EventReference StageBGM;

			[Range(0, 100)] public float StageBGMVolume = 100f;
			
		}
		
		public List<StageData> Stages;
		
		public StageData FindFromInternal(string ID) => Stages.Find(s=>s.InternalName == ID);
		public StageData FindFromScene(Scene ID) => Stages.Find(s=>s.TargetScene == ID.name);
		public StageData GetCurrentStageMeta() => Stages.Find(s=>s.TargetScene == SceneManager.GetActiveScene().name);
	}
}