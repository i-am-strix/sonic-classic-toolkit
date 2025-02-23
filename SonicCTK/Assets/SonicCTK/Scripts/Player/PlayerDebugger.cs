using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using SonicCTK.Physical;

namespace SonicCTK.Core
{
    public class PlayerDebugger : MonoBehaviour
    {
        public TMP_Text DebugTxt;
	    public Player Player;

        void Start()
        {
        
        }

        void Update()
        {
            DebugTxt.text = string.Empty;
            AddNewLine($"Current State: {Player.CurrentStateName}");
	        AddNewLine("\n");
            
	        AddNewLine($"Gsp: {Player.Common.Gsp.ToString("00.00")}");
	        AddNewLine($"Xsp: {Player.Common.Xsp.ToString("00.00")}");
	        AddNewLine($"Ysp: {Player.Common.Ysp.ToString("00.00")}");
	        
	        AddNewLine($"Surface Angle: {Player.Common.SurfaceAngle.ToString("00.00")}");
	        AddNewLine($"Surface Angle Rad: {Player.Common.SurfaceAngleRad.ToString("00.00")}");
	        
	        AddNewLine($"Angle Mode: 0b{Convert.ToString(Player.Collision.AngleMode, 2)}");
	        AddNewLine($"Air Direction: 0b{Convert.ToString(Player.Get<PStateAir>().VelocityDirection, 2)}");

        }

        public void AddNewLine(string line)
        {
            DebugTxt.text += "\n";
            DebugTxt.text += line;
        }
    }
}
