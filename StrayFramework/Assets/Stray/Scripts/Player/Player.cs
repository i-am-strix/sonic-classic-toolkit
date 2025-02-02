using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stray.Core;
using Stray.World;

namespace Stray.Physical
{
    /// <summary>
    /// Contains the main parts of a Player's behaviour. You can extend this class and add your own custom 
    /// states for different characters, or disable pre-existing states from executing
    /// </summary>
    [AddComponentMenu(Constants.ContextName + "/Players/Player Core", 0)]
    public class Player : StateMachine<PlayerState>
    {
        public string currentState;
        
        [Header("States")] 
        public WPStateMove MoveState = new WPStateMove();
        public WPStateAir AirState = new WPStateAir();
        public WPStateJump JumpState = new WPStateJump();
        public WPStateRoll RollState = new WPStateRoll();
        public WPStateCrouch CrouchState = new WPStateCrouch();
        public WPStateSpindash SpindashState = new WPStateSpindash();
        public WPStateHurt HurtState = new WPStateHurt();
        public WPStateDead DeadState = new WPStateDead();
        
        [Space]
        [Header("References")]
        public PlayerPhysics Phys;
        public PlayerCamera Cam;
        public GameInputManager PlayerInput;

        [Space]
        [Header("Utilities")]
        public PlayerCommon Cmn;
        public PlayerCollision Col;
        public PlayerVisual Visual;
        public PlayerWorldInteraction Interactions;

        public delegate void StateChangeEvent(PlayerState PreviousState, PlayerState NewState);
        public event StateChangeEvent OnStateChanged;
        
        public void Start()
        {
            Application.targetFrameRate = 60;

            Add(MoveState);
            Add(AirState);
            Add(JumpState);
            Add(RollState);
            Add(CrouchState);
            Add(SpindashState);
            Add(HurtState);
            Add(DeadState);

            Initialize<WPStateMove>();

            Cmn.Player = Col.Player = Visual.Player = Interactions.Player = this;

            if (!Stage.Current)
            {
                WDebug.Log("There is no Stage instance in this scene. This can lead to undefined behaviour, so be careful. Playback paused", WDebug.LogType.Important);
                Debug.Break();
            }
            
            Stage.ConnectedPlayers.Add(this);
            PlayerStart();
        }

        public virtual void PlayerStart()
        {

        }

        public virtual void OnDestroy()
        {
            Stage.ConnectedPlayers.Remove(this);
        }

        // ------------------- //
        // ------ EVENTS ----- //
        // ------------------- //

        public virtual void Update()
        {
            currentState = CurrentState.GetType().Name;

            transform.eulerAngles = Cmn.IsGrounded ? new Vector3(0, 0, (float)Col.AngleMode * 90f) : Vector3.zero;
            transform.localScale = new Vector3(Col.PlayerCollisionSize.x.PToW(), Col.PlayerCollisionSize.y.PToW(), 1);
            
	        CurrentState.OnUpdate(Time.deltaTime);
            Cmn.CommonUpdate();
            Cmn.Gsp = Mathf.Clamp(Cmn.Gsp, -Phys.MaxSpeed, Phys.MaxSpeed);

            Visual.VisualUpdate();
            Interactions.InteractionUpdate();
            
            WDebug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(Cmn.SurfaceAngleRad), -Mathf.Cos(Cmn.SurfaceAngleRad)) * 0.15f, Color.magenta);
        }
        
        public virtual void LateUpdate()
        {
            CurrentState.OnLateUpdate(Time.deltaTime);
            Visual.VisualLateUpdate();
        }
        
        public virtual void FixedUpdate()
        {
            CurrentState.OnFixedUpdate(Time.deltaTime);
        }

        // ------------------- //
        // ----- UTILITY ----- //
        // ------------------- //

        public override void Add(PlayerState state)
        {
            state.Player = this;
            base.Add(state);
        }
        
        public override bool Set<T>()
        {
            var PreviousState = CurrentState; 
            if (base.Set<T>())
            {
                OnStateChanged?.Invoke(PreviousState, 
                    AvailableStates.Find(s => s.GetType() == typeof(T)));
                return true;
            }
            else return false;
        }

        public bool Is<T>() where T : PlayerState
        {
            return CurrentState.GetType() == typeof(T);
        }
    }
}