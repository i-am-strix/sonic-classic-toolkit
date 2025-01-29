using UnityEngine;

namespace Stray.Core 
{
	public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
	{
		private static T instance;
		public static T Instance {
			get {
				if (instance == null) {
					T[] Assets = Resources.LoadAll<T>("");
					if (Assets == null && Assets.Length < 1) WDebug.Log($"No instances of {typeof(T).Name} discovered within Resources root", WDebug.LogType.Important);
					if (Assets.Length > 1) WDebug.Log($"Multiple instances of {typeof(T).Name} discovered within Resources root", WDebug.LogType.Important);
					
					instance = Assets[0];
				}
				return instance;
			}
		}
	}
}
