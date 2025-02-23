using UnityEngine;
using SonicCTK.Core;

namespace SonicCTK.Physical {
    public abstract class PlayerState : State
    {
        public Player Player;

        public PlayerCommon Cmn => Player.Common;
        public PlayerPhysics Phys => Player.Physics;
        public PlayerCollision Col => Player.Collision;
        public PlayerCamera Cam => Player.Cam;
        public PlayerVisual Visual => Player.Visual;
        public PlayerWorldInteraction Interactions => Player.Interactions;
        public GameInputManager Input => Player.PlayerInput;

        public bool Is<T>() where T : PlayerState
        {
            return Player.Is<T>();
        }

        public T Get<T>() where T : PlayerState
        {
            return Player.Get<T>();
        }

        public bool Set<T>() where T : PlayerState
        {
            return Player.Set<T>();
        }
    }
}