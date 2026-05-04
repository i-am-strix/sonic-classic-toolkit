using System;
using SonicCTK.Core;
using SonicCTK.Gameplay;
using UnityEditor;
using UnityEngine;

namespace SonicCTK.Editor
{
    [CustomEditor(typeof(SwingingPlatform), true)]
    public class SwingingPlatformEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        static void DrawSwingArc(Transform objectTransform, GizmoType gizmoType)
        {
            var obj = objectTransform.GetComponent<SwingingPlatform>();

            if (!obj) return;
            
            if (!obj.ChainSprite) return;
            float arcRadius = obj.ChainSprite.rect.width.PToW() / 2;

            var arcRoot = obj.transform.position + Vector3.down * arcRadius * 2 * (obj.ChainLength + 1);
            
            if (obj.Platform && !Application.isPlaying)
                obj.Platform.transform.position = arcRoot;
            
            Handles.DrawWireArc(arcRoot + Vector3.up * (arcRadius * 2 * (obj.ChainLength + 1)), 
                Vector3.forward, Quaternion.AngleAxis(-obj.SwingMaxAngle, Vector3.forward) * Vector3.down, obj.SwingMaxAngle * 2, arcRadius * 2 * (obj.ChainLength + 1), 3);
            
            var leftPivot = obj.transform.position + Quaternion.AngleAxis(obj.SwingMaxAngle, Vector3.forward) * 
                Vector3.down * arcRadius * 2 * (obj.ChainLength + 1);
            var rightPivot = obj.transform.position + Quaternion.AngleAxis(-obj.SwingMaxAngle, Vector3.forward) * 
                Vector3.down * arcRadius * 2 * (obj.ChainLength + 1);

            Handles.color = Constants.ColorConstants.SolidBoxColor;
            
            Handles.DrawWireCube(leftPivot, obj.Platform.Radius.PToW() * 2);
            Handles.DrawWireCube(rightPivot, obj.Platform.Radius.PToW() * 2);
        }
    }
}