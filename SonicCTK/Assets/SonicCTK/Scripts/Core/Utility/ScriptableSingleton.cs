using UnityEngine;

namespace SonicCTK.Core 
{
	public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
	{
		private static T instance;
		public static T Instance {
			get {
				if (instance == null) {
					T[] Assets = Resources.LoadAll<T>("");
					if (Assets == null && Assets.Length < 1) CDebug.Log($"No instances of {typeof(T).Name} discovered within Resources root", CDebug.LogType.Important);
					if (Assets.Length > 1) CDebug.Log($"Multiple instances of {typeof(T).Name} discovered within Resources root", CDebug.LogType.Important);
					
					instance = Assets[0];
				}
				return instance;
			}
		}
	}
}
