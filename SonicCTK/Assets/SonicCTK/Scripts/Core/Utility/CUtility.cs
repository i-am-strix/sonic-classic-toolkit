using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// Pixel transformation functions
    /// </summary>
    public static class CPixelConversion
    {
        public const float CPixelConversionFactor = 1f / 100;

        public static Vector3 WToP(this Vector3 v) => Vector3.Scale(v / CPixelConversionFactor, new Vector3(1, -1, 1));
        public static Vector3 PToW(this Vector3 v) => Vector3.Scale(v * CPixelConversionFactor, new Vector3(1, -1, 1));

        public static Vector2 WToP(this Vector2 v) => Vector2.Scale(v / CPixelConversionFactor, new Vector2(1, -1));
        public static Vector2 PToW(this Vector2 v) => Vector2.Scale(v * CPixelConversionFactor, new Vector2(1, -1));

        public static float WToP(this float f) => f / CPixelConversionFactor;
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

                Gameplay.Ring Ring = ObjectReferenceContainer.Spawn("Ring", Position).GetComponent<Gameplay.Ring>();
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
