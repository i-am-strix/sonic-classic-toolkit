using System;
using Stray.Core;
using UnityEngine;

namespace Stray.Physical
{
    [Serializable]
    public class PlayerCommon
    {
        public Player Player;
        
        [Header("Physics")] 
        public float Gsp;

        public float Xsp;
        public float Ysp;

        public float SurfaceAngle;

        public float SurfaceAngleRad
        {
            get => SurfaceAngle * Mathf.Deg2Rad;
            set => SurfaceAngle = value * Mathf.Rad2Deg;
        }

        public Vector2 InputDirection;

        [Header("Flags")] 
	    public bool IsGrounded;
	    public bool IsPushing;

        public int ControlLock;
        public void GrabInput() => InputDirection = Player.PlayerInput.GetAxis2D("Move");
        public void ApplyTransformations()
        {
            Player.transform.position += (Vector3)(new Vector2(Xsp, Ysp).PToW());
        }
    }
}