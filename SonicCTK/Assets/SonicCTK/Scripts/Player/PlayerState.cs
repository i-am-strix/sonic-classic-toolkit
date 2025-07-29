using UnityEngine;
using SonicCTK.Core;

namespace SonicCTK.Physical {
    /// <summary>
    /// A player state is an extension of the <b>State</b> type, which is equipped with various references to core components of the player
    /// that this state is attached to
    /// </summary>
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