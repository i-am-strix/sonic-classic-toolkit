using SonicCTK.Core;

namespace SonicCTK.Physical {
    /// <summary>
    /// A player state is an extension of the <b>State</b> type, which is equipped with various references to core components of the player
    /// that this state is attached to
    /// </summary>
    public abstract class PlayerState : State
    {
        public Player Player;

        protected PlayerCommon Cmn => Player.Common;
        protected PlayerPhysics Phys => Player.Physics;
        protected PlayerCollision Col => Player.Collision;
        protected PlayerCamera Cam => Player.Cam;
        protected PlayerVisual Visual => Player.Visual;
        protected PlayerWorldInteraction Interactions => Player.Interactions;
        protected InputHandler Input => Player.PlayerInput;

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