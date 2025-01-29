using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Core
{
    public static class WDebug
    {
        public enum LogType{
            Normal, Important, Ignorable
        }
        public static void Log(string message, LogType type = LogType.Normal)
        {
            Color color;
            switch(type)
            {
                case LogType.Important: color = new Color(1, 0.6295775f, 0.3915094f, 1); break;
                case LogType.Ignorable: color = Color.gray; break;
                default: color = new Color(1, 1, 1, .5f); break;
            };
            Debug.Log (string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), $"StrixTools Log - {message}"));
        }

        public static void DrawLine(Vector2 Origin, Vector2 Target, Color Col)
        {
            Vector2 OrgPos = Origin.WToP();
            Vector2 TarPos = Target.WToP();
            
            Debug.DrawLine(OrgPos.Round().PToW(), TarPos.Round().PToW(), Col);
        }
    }
}
