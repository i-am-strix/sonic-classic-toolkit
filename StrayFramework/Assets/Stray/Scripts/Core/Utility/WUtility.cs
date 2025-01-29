using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Core
{
    /// <summary>
    /// Quick swizzle function collection
    /// </summary>
    public static class WVectorSwizzle
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

    public static class WMathUtility
    {
        public static Vector2 Round(this Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
        public static Vector2 Floor(this Vector2 v) => new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
        public static Vector2 Ceil(this Vector2 v) => new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));
    }

    /// <summary>
    /// Pixel transformation functions
    /// </summary>
    public static class WPixelConversion
    {
        private const float WPixelConversionFactor = 1f / 100;

        public static Vector2 WToP(this Vector2 v) => Vector2.Scale(v / WPixelConversionFactor, new Vector2(1, -1));
        public static Vector2 PToW(this Vector2 v) => Vector2.Scale(v * WPixelConversionFactor, new Vector2(1, -1));

        public static float WToP(this float f) => f / WPixelConversionFactor;
        public static float PToW(this float f) => f * WPixelConversionFactor;
    }

    public static class WUtility
    {
        public static void Split(this Vector3 v, Vector3 normal, out Vector3 lat, out Vector3 ver)
        {
            lat = Vector3.ProjectOnPlane(v, normal);
            ver = v - lat;
        }
    }
}
