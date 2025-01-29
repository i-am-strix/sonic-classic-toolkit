using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace Stray.Core {
	public static class WInput{
		public static float GetAxis(string axisName) => GameInputManager.Instance.GetAxis(axisName);
		public static Vector2 GetAxis2D(string axisName) => GameInputManager.Instance.GetAxis2D(axisName);

		public static bool GetButton(string buttonName) => GameInputManager.Instance.GetButton(buttonName);
		public static bool GetButtonDown(string buttonName) => GameInputManager.Instance.GetButtonDown(buttonName);
		public static bool GetButtonUp(string buttonName) => GameInputManager.Instance.GetButtonUp(buttonName);

		//--------Functions prefixed with F = force functions, bypasses BoltInput.BlockInput--------//
		public static float FGetAxis(string axisName) => GameInputManager.Instance.FGetAxis(axisName);
		public static Vector2 FGetAxis2D(string axisName) => GameInputManager.Instance.FGetAxis2D(axisName);

		public static bool FGetButton(string buttonName) => GameInputManager.Instance.FGetButton(buttonName);
		public static bool FGetButtonDown(string buttonName) => GameInputManager.Instance.FGetButtonDown(buttonName);
		public static bool FGetButtonUp(string buttonName) => GameInputManager.Instance.FGetButtonUp(buttonName);
	
		//Local Functions

		public static float GetAxis(this GameInputManager Input, string axisName) => Input.GetAxis(axisName);
		public static Vector2 GetAxis2D(this GameInputManager Input,string axisName) => Input.GetAxis2D(axisName);

		public static bool GetButton(this GameInputManager Input,string buttonName) => Input.GetButton(buttonName);
		public static bool GetButtonDown(this GameInputManager Input,string buttonName) => Input.GetButtonDown(buttonName);
		public static bool GetButtonUp(this GameInputManager Input,string buttonName) => Input.GetButtonUp(buttonName);

		//--------Functions prefixed with F = force functions, bypasses BoltInput.BlockInput--------//
		public static float FGetAxis(this GameInputManager Input,string axisName) => Input.FGetAxis(axisName);
		public static Vector2 FGetAxis2D(this GameInputManager Input,string axisName) => Input.FGetAxis2D(axisName);

		public static bool FGetButton(this GameInputManager Input,string buttonName) => Input.FGetButton(buttonName);
		public static bool FGetButtonDown(this GameInputManager Input,string buttonName) => Input.FGetButtonDown(buttonName);
		public static bool FGetButtonUp(this GameInputManager Input,string buttonName) => Input.FGetButtonUp(buttonName);

		//-----------------------------------------------------------------------------------------//
		public static void BlockInput() => GameInputManager.Instance.BlockInput = true;
		public static void UnblockInput() => GameInputManager.Instance.BlockInput = false;
	
		public static void BlacklistInput(params string[] names){
			foreach(string s in names){
				if (GameInputManager.Instance.BlacklistedInputs.Contains(s)) continue;
				else GameInputManager.Instance.BlacklistedInputs.Add(s);
			}
		}
		
		public static void RemoveBlacklistInput(params string[] names){
			foreach(string s in names){
				if (GameInputManager.Instance.BlacklistedInputs.Contains(s)) GameInputManager.Instance.BlacklistedInputs.Remove(s);
			}
		}
		
		public static PlayerInput InputObject;
	}

	public enum InputDeviceType {
		Keyboard, Mouse, Dualsense, Xbox
	}

	public class GameInputManager : MonoBehaviour
	{
		public delegate void InputChangeEvent();
		
		public event InputChangeEvent OnInputChanged;
		
		public PlayerInput MainInput;
	
		public string LatestInputDevice;
		public string LatestInputDeviceFull;
		private InputDeviceType previousDeviceType;
		public InputDeviceType LatestDeviceType;
	
		public List<UButton> Buttons;
		public List<UAxis1D> Axis1D;
		public List<UAxis2D> Axis2D;
	
		public List<string> BlacklistedInputs = new List<string>();
		public List<string> BlacklistedMaps = new List<string>();
	
		public static GameInputManager Instance;
	
		public bool BlockInput;
		
		public string CurrentMap;
		public string CurrentMapOnInput;
	
		public void ChangeMap(string MapNameOrID){
			MainInput.SwitchCurrentActionMap(MapNameOrID);
			CurrentMap = MapNameOrID;
		}
		
		[Header("Input Display stuff")]
		public TMPro.TMP_SpriteAsset KBDAsset;
		public TMPro.TMP_SpriteAsset DualsenseAsset;
		public TMPro.TMP_SpriteAsset XboxAsset;
		
		void Awake()
		{
			//Create static bindings
			GameInputManager.Instance = this;
			MainInput.onActionTriggered += OnActionTriggered;

			CurrentMap = MainInput.defaultActionMap;
			Buttons = new List<UButton>(); Axis1D = new List<UAxis1D>(); Axis2D = new List<UAxis2D>();
			foreach(InputActionMap a in MainInput.actions.actionMaps){
				foreach(InputAction s in a.actions){
					switch(s.type){
					case InputActionType.Button:
						UButton button = new UButton();
						button.button = s;
						button.Name = s.name;
						button.MapName = a.name;
						Buttons.Add(button);
						break;
	    			
					case InputActionType.Value:
						switch(s.expectedControlType){
						case "Axis":
							UAxis1D axis1 = new UAxis1D();
							axis1.Name = s.name;
							axis1.MapName = a.name;
							Axis1D.Add(axis1);
							break;
						case "Vector2":
							UAxis2D axis2 = new UAxis2D();
							axis2.Name = s.name;
							axis2.MapName = a.name;
							Axis2D.Add(axis2);
							break;
						}
						break;
					}
				}
			}
		}
		
		void Start(){
		}
    
		internal UButton UBGet(string name) => Buttons.Find(s=>s.Name == name);
		internal string UBGetMap(string name) => Buttons.Find(s=>s.Name == name).MapName;
		internal UAxis1D UA1Get(string name) => Axis1D.Find(s=>s.Name == name);
		internal string UA1GetMap(string name) => Axis1D.Find(s=>s.Name == name).MapName;
		internal UAxis2D UA2Get(string name) => Axis2D.Find(s=>s.Name == name);
		internal string UA2GetMap(string name) => Axis2D.Find(s=>s.Name == name).MapName;
		
		void Update(){
			CurrentMapOnInput = MainInput.currentActionMap.name;
		}
	
		void OnActionTriggered(InputAction.CallbackContext callbackContext)
		{ 
			LatestInputDevice = callbackContext.action.activeControl.device.displayName.Split(' ')[0];
			LatestInputDeviceFull = callbackContext.action.activeControl.device.displayName;
			
			switch(LatestInputDevice){
			case "Mouse":
				LatestDeviceType = InputDeviceType.Mouse;
				break;
			case "Keyboard":
				LatestDeviceType = InputDeviceType.Keyboard;
				break;
			case "DualSense":
				LatestDeviceType = InputDeviceType.Dualsense;
				break;
			case "PS5":
				LatestDeviceType = InputDeviceType.Dualsense;
				break;
			case "PS4":
				LatestDeviceType = InputDeviceType.Dualsense;
				break;
			case "DualShock":
				LatestDeviceType = InputDeviceType.Dualsense;
				break;
			case "Xbox":
				LatestDeviceType = InputDeviceType.Xbox;
				break;
			case "Wireless":
				LatestDeviceType = InputDeviceType.Xbox;
				break;
			default:
				LatestDeviceType = InputDeviceType.Xbox;
				break;
			}
			
			if (previousDeviceType != LatestDeviceType){
				OnInputChanged?.Invoke();
				previousDeviceType = LatestDeviceType;
			}
			
			
			switch (callbackContext.action.type)
			{
			case InputActionType.Button:
				if (CurrentMap == UBGetMap(callbackContext.action.name)){
						UBGet(callbackContext.action.name).hold = callbackContext.ReadValueAsButton();
					}
					break;
				case InputActionType.Value:
					switch(callbackContext.action.expectedControlType){
					case "Axis":
						if (CurrentMap == UA1GetMap(callbackContext.action.name)){
							UA1Get(callbackContext.action.name).Value = callbackContext.ReadValue<float>();
						}
						break;
					case "Vector2":
						if (CurrentMap == UA2GetMap(callbackContext.action.name)){
							UA2Get(callbackContext.action.name).Value = callbackContext.ReadValue<Vector2>();
						}
						break;
					}
					break;
			}
		}
		public bool GetButtonDown(string name) { if (!BlacklistedInputs.Contains(name) && !BlockInput) return UBGet(name).pressed; else return false;}
		public bool GetButtonUp(string name) { if (!BlacklistedInputs.Contains(name) && !BlockInput) return UBGet(name).released; else return false;}
		public bool GetButton(string name) { if (!BlacklistedInputs.Contains(name) && !BlockInput) return UBGet(name).hold; else return false;}
	
		public float GetAxis(string name) {if (!BlacklistedInputs.Contains(name) && !BlockInput) return UA1Get(name).Value; else return 0;}
		public Vector2 GetAxis2D(string name) {if (!BlacklistedInputs.Contains(name) && !BlockInput) return UA2Get(name).Value; else return Vector2.zero;}
	
		//Force functions always return the value even if its name or parent map are blacklisted
		public bool FGetButtonDown(string name) => UBGet(name).pressed;
		public bool FGetButtonUp(string name) => UBGet(name).released;
		public bool FGetButton(string name) => UBGet(name).hold;
	
		public float FGetAxis(string name) => UA1Get(name).Value;
		public Vector2 FGetAxis2D(string name) => UA2Get(name).Value;
	}

	public class UAxis1D{
		public string Name;
		public string MapName;
		public float Value;
	}
	[System.Serializable]
	public class UAxis2D{
		public string Name;
		public string MapName;
		public Vector2 Value;
	}
	[System.Serializable]
	public class UButton
	{
		public string Name; public string MapName; //Define a public name that we can index later with helper functions
		public InputAction button;
		public Action press, release;

		public float PressedTime, FPressedTime, ReleasedTime, FReleasedTime;
		[HideInInspector] public bool hold
		{
			get => _hold;
			set{
				if (_hold == value) return;
				_hold = value;
				if (_hold){
					PressedTime = Time.unscaledTime;
					FPressedTime = Time.fixedUnscaledTime;
				}
				else{
					ReleasedTime = Time.unscaledTime;
					FReleasedTime = Time.fixedUnscaledTime;
				}
			}
		}
		public bool upressed;
	
		public bool pressed => !Time.inFixedTimeStep ? PressedTime == Time.unscaledTime : FPressedTime == Time.fixedUnscaledTime;
		public bool released =>!Time.inFixedTimeStep ? ReleasedTime == Time.unscaledTime : FReleasedTime == Time.fixedUnscaledTime;

		bool _hold;
	}
}
