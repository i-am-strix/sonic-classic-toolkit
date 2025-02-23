using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SonicCTK.Core;

namespace SonicCTK.Editor
{
    [CustomEditor(typeof(Transform), true)]
    public class PixelInspector : UnityEditor.Editor
    {
        public void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Vector2Field("Pixel Coordinates", (target as Transform).position.WToP());
        }
    }
}
