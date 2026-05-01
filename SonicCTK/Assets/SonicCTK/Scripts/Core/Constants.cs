using UnityEngine;

namespace SonicCTK
{
    /// <summary>
    /// Constants used across the framework
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The context name (in editor)
        /// </summary>
        public const string ContextName = "Sonic Classic Toolkit";

        public static class ColorConstants
        {
            public static readonly Color32 SolidBoxColor = new Color32(100, 150, 250, 100);
            public static readonly Color32 HitBoxColor = new Color32(250, 150, 100, 100);
            public static readonly Color32 HazardColor = new(250, 0, 3, 100);
            
            public static readonly Color32 BackgroundLayerColor = new Color32(14, 0, 255, 149);
            public static readonly Color32 ForegroundLayerColor = new Color32(255, 123, 0, 149);
        }
    }
}