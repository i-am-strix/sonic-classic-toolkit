using UnityEngine;
using Stray.Core;

namespace Stray.Physical {
    public abstract class PlayerState : State
    {
        public Player Player;

        public PlayerCommon Cmn => Player.Cmn;
        public PlayerPhysics Phys => Player.Phys;
        public PlayerCollision Col => Player.Col;
        public PlayerCamera Cam => Player.Cam;
        public PlayerVisual Visual => Player.Visual;
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