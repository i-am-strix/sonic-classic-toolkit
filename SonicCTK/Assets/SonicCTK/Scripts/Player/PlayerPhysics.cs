using UnityEngine;

namespace SonicCTK.Physical
{
    [CreateAssetMenu(fileName = "Player Physics", menuName = Constants.ContextName + "/Player/Physics Values", order = 0)]
    public class PlayerPhysics : ScriptableObject
    {
        [Header("Movement")] public float Acceleration = 0.046875f;
        public float Deceleration = 0.5f;
        public float Friction = 0.046875f;

        public float TopSpeed = 6;
        public float MaxSpeed = 16;

        public float SlopeRepel = 0.125f;

        [Header("Rolling")] public float RollSlopeRepelUp = 0.078125f;
        public float RollSlopeRepelDown = 0.3125f;
        public float RollFriction = 0.0234375f;
        public float RollDeceleration = 0.125f;

        [Header("Airborne")] public float Gravity = 0.21875f;
        public float AirControl = 0.09375f;

        public float JumpPower = 6.5f;
    }
}