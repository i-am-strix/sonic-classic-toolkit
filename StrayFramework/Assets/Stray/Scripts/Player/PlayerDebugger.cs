using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using Stray.Physical;

namespace Stray.Core
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
            AddNewLine($"Current State: {Player.currentState}");
	        AddNewLine("\n");
            
	        AddNewLine($"Gsp: {Player.Cmn.Gsp.ToString("00.00")}");
	        AddNewLine($"Xsp: {Player.Cmn.Xsp.ToString("00.00")}");
	        AddNewLine($"Ysp: {Player.Cmn.Ysp.ToString("00.00")}");
	        
	        AddNewLine($"Surface Angle: {Player.Cmn.SurfaceAngle.ToString("00.00")}");
	        AddNewLine($"Surface Angle Rad: {Player.Cmn.SurfaceAngleRad.ToString("00.00")}");
	        
	        AddNewLine($"Angle Mode: 0b{Convert.ToString(Player.Col.AngleMode, 2)}");
	        AddNewLine($"Air Direction: 0b{Convert.ToString(Player.Get<WPStateAir>().VelocityDirection, 2)}");

        }

        public void AddNewLine(string line)
        {
            DebugTxt.text += "\n";
            DebugTxt.text += line;
        }
    }
}
