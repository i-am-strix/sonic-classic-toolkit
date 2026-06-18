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

        private bool showUnitySpaceCoords;
        public override void OnInspectorGUI()
        {
            GUIStyle style = new("label");
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("2D Space", style);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();

            var transform = (Transform)target;
            
            var pixelPosition = EditorGUILayout.Vector2Field("Pixel Coordinates", transform.localPosition.WToP());
            var scale = EditorGUILayout.Vector2Field("Scale", transform.localScale);
            var rotation = EditorGUILayout.FloatField("Z Rotation", transform.localEulerAngles.z);

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Round to Nearest Pixel"))
            {
                pixelPosition = pixelPosition.Round();
            }

            showUnitySpaceCoords = EditorGUILayout.Foldout(showUnitySpaceCoords, "Unity Space");
            
            if (showUnitySpaceCoords)
            {
                base.OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Transform");
                transform.localPosition = pixelPosition.PToW();
                transform.localEulerAngles =
                    new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, rotation);
                transform.localScale = new Vector3(scale.x, scale.y, transform.localScale.z);
            }
        }
    }
}
