using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SonicCTK.Core
{
    /// <summary>
    /// Quick swizzle function collection
    /// </summary>
    public static class CVectorSwizzle
    {
        public static Vector3 xyz(this Vector3 v) => new Vector3(v.x, v.y, v.z);
        public static Vector3 xzy(this Vector3 v) => new Vector3(v.x, v.z, v.y);
        public static Vector3 yxz(this Vector3 v) => new Vector3(v.y, v.x, v.z);
        public static Vector3 yzx(this Vector3 v) => new Vector3(v.y, v.z, v.x);

        public static Vector3 xyz(this Vector2 v) => new Vector3(v.x, v.y, 0);
        public static Vector3 xzy(this Vector2 v) => new Vector3(v.x, 0, v.y);
        public static Vector3 yxz(this Vector2 v) => new Vector3(v.y, v.x, 0);
        public static Vector3 yzx(this Vector2 v) => new Vector3(v.y, 0, v.x);
    }

    public static class CMathUtility
    {
        public static Vector2 Round(this Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
        public static Vector2 Floor(this Vector2 v) => new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
        public static Vector2 Ceil(this Vector2 v) => new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));
    }

    /// <summary>
    /// Pixel transformation functions.
    ///
    /// This is essential for keeping <b><i>this</i></b> framework similar in function to other frameworks
    /// like those on Clickteam Fusion or Gamemaker, as it essentially makes the game use pixels for positions and stuff as opposed
    /// to Unity units (which are in meters), and in turn makes your life with handling stuff like sprites and positioning them
    /// much, much easier
    /// </summary>
    public static class CPixelConversion
    {
        /// <summary>
        /// The pixel transformation factor. By default, it uses the default Unity value of 100 PPU (pixels-per-unit),
        /// which is basically 100 pixels per 1 Unity unit (so a transform position of (1, 1, 1) is essentially (100, -100, 100) in pixels
        /// </summary>
        public const float CPixelConversionFactor = 1f / 100;

        /// <summary>
        /// Convert a Vector3 from world-space (0.01, 0.1, 1) to pixel-space (1, 10, 100)
        /// </summary>
        public static Vector3 WToP(this Vector3 v) => Vector3.Scale(v / CPixelConversionFactor, new Vector3(1, -1, 1));
        
        /// <summary>
        /// Convert a Vector3 from pixel-space units (1, 10, 100) to world-space units (0.01, 0.1, 1)
        /// </summary>
        public static Vector3 PToW(this Vector3 v) => Vector3.Scale(v * CPixelConversionFactor, new Vector3(1, -1, 1));

        /// <summary>
        /// Convert a Vector2 from world-space (0.01, 0.1, 1) to pixel-space (1, 10, 100)
        /// </summary>
        public static Vector2 WToP(this Vector2 v) => Vector2.Scale(v / CPixelConversionFactor, new Vector2(1, -1));
        
        /// <summary>
        /// Convert a Vector2 from pixel-space units (1, 10, 100) to world-space units (0.01, 0.1, 1)
        /// </summary>
        public static Vector2 PToW(this Vector2 v) => Vector2.Scale(v * CPixelConversionFactor, new Vector2(1, -1));

        /// <summary>
        /// Convert a float from world-space (0.01, 0.1, 1) to pixel-space (1, 10, 100)
        /// </summary>
        public static float WToP(this float f) => f / CPixelConversionFactor;
        /// <summary>
        /// Convert a float from pixel-space (1, 10, 100) to world-space (0.01, 0.1, 1)
        /// </summary>
        public static float PToW(this float f) => f * CPixelConversionFactor;
    }

    public static class CUtility
    {
        /// <summary>
        /// Converts a fixed duration from the Genesis games into an animation speed usable on an Animator
        /// </summary>
        /// <param name="Value">The duration of a frame in seconds</param>
        /// <returns></returns>
        public static float FDurToSpeed(float Value)
        {
            return Value != 0 ? (1f / Value) : 1f;
        }

        public static void SpawnRings(Vector3 Position, int Amount)
        {
            var RingCounter = 0;
            var StartAngle = 101.25f;
            var CurrentAngle = StartAngle;

            var ShouldFlip = false;
            var Velocity = 4;

            while(RingCounter < Mathf.Min(Amount, 32))
            {
                var RadAngle = CurrentAngle * Mathf.Deg2Rad;

                Gameplay.Ring Ring = Addressables.InstantiateAsync("/Core/Objects/Ring.prefab", Position, Quaternion.identity).WaitForCompletion().
                    GetComponent<Gameplay.Ring>();
                Ring.IsBouncingRing = true;

                Ring.Xsp = Mathf.Cos(RadAngle) * Velocity;
                Ring.Ysp = -Mathf.Sin(RadAngle) * Velocity;

                if (ShouldFlip)
                {
                    Ring.Xsp *= -1;
                    CurrentAngle += 22.5f;
                }

                ShouldFlip = !ShouldFlip;

                RingCounter++;

                if (RingCounter == 16)
                {
                    Velocity = 2;
                    CurrentAngle = StartAngle;
                }
            }
        }
    }
}
