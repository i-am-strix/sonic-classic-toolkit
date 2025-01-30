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

		// Local Functions

		public static float GetAxis(this GameInputManager Input, string axisName) => Input.GetAxis(axisName);
		public static Vector2 GetAxis2D(this GameInputManager Input,string axisName) => Input.GetAxis2D(axisName);

		public static bool GetButton(this GameInputManager Input,string buttonName) => Input.GetButton(buttonName);
		public static bool GetButtonDown(this GameInputManager Input,string buttonName) => Input.GetButtonDown(buttonName);
		public static bool GetButtonUp(this GameInputManager Input,string buttonName) => Input.GetButtonUp(buttonName);
			
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
	
		private string LatestInputDevice;
		private InputDeviceType previousDeviceType;
		private InputDeviceType LatestDeviceType;
	
		private List<WIButton> Buttons;
		private List<WIAxis> Axis1D;
		private List<WIAxis2D> Axis2D;
	
		public static GameInputManager Instance;
	
		public string CurrentMap;
		public string CurrentMapOnInput;
	
		public void ChangeMap(string MapNameOrID){
			MainInput.SwitchCurrentActionMap(MapNameOrID);
			CurrentMap = MapNameOrID;
		}
				
		void Awake()
		{
			//Create static bindings
			GameInputManager.Instance = this;
			MainInput.onActionTriggered += OnActionTriggered;

			CurrentMap = MainInput.defaultActionMap;
			Buttons = new List<WIButton>(); Axis1D = new List<WIAxis>(); Axis2D = new List<WIAxis2D>();
			foreach(InputActionMap a in MainInput.actions.actionMaps){
				foreach(InputAction s in a.actions){
					switch(s.type){
					case InputActionType.Button:
						WIButton button = new WIButton();
						button.button = s;
						button.Name = s.name;
						button.MapName = a.name;
						Buttons.Add(button);
						break;
	    			
					case InputActionType.Value:
						switch(s.expectedControlType){
						case "Axis":
							WIAxis axis1 = new WIAxis();
							axis1.Name = s.name;
							axis1.MapName = a.name;
							Axis1D.Add(axis1);
							break;
						case "Vector2":
							WIAxis2D axis2 = new WIAxis2D();
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
    
		internal WIButton WBGet(string name) => Buttons.Find(s=>s.Name == name);
		internal string UBGetMap(string name) => Buttons.Find(s=>s.Name == name).MapName;
		internal WIAxis WA1Get(string name) => Axis1D.Find(s=>s.Name == name);
		internal string UA1GetMap(string name) => Axis1D.Find(s=>s.Name == name).MapName;
		internal WIAxis2D WA2Get(string name) => Axis2D.Find(s=>s.Name == name);
		internal string UA2GetMap(string name) => Axis2D.Find(s=>s.Name == name).MapName;
		
		void Update(){
			CurrentMapOnInput = MainInput.currentActionMap.name;
		}
	
		void OnActionTriggered(InputAction.CallbackContext callbackContext)
		{ 
			LatestInputDevice = callbackContext.action.activeControl.device.displayName.Split(' ')[0];
			
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
						WBGet(callbackContext.action.name).hold = callbackContext.ReadValueAsButton();
					}
					break;
				case InputActionType.Value:
					switch(callbackContext.action.expectedControlType){
					case "Axis":
						if (CurrentMap == UA1GetMap(callbackContext.action.name)){
							WA1Get(callbackContext.action.name).Value = callbackContext.ReadValue<float>();
						}
						break;
					case "Vector2":
						if (CurrentMap == UA2GetMap(callbackContext.action.name)){
							WA2Get(callbackContext.action.name).Value = callbackContext.ReadValue<Vector2>();
						}
						break;
					}
					break;
			}
		}
		public bool GetButtonDown(string name) { return WBGet(name).pressed; }
		public bool GetButtonUp(string name) { return WBGet(name).released; }
		public bool GetButton(string name) { return WBGet(name).hold; }
	
		public float GetAxis(string name) { return WA1Get(name).Value; }
		public Vector2 GetAxis2D(string name) { return WA2Get(name).Value; }
	
		//Force functions always return the value even if its name or parent map are blacklisted
		public bool FGetButtonDown(string name) => WBGet(name).pressed;
		public bool FGetButtonUp(string name) => WBGet(name).released;
		public bool FGetButton(string name) => WBGet(name).hold;
	
		public float FGetAxis(string name) => WA1Get(name).Value;
		public Vector2 FGetAxis2D(string name) => WA2Get(name).Value;
	}

	public class WIAxis{
		public string Name;
		public string MapName;
		public float Value;
	}
	[System.Serializable]
	public class WIAxis2D{
		public string Name;
		public string MapName;
		public Vector2 Value;
	}
	[System.Serializable]
	public class WIButton
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
