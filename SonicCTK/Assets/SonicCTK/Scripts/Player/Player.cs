using System;
using System.Collections;
using System.Collections.Generic;
using SonicCTK.Core;
using SonicCTK.World;
using UnityEngine;

namespace SonicCTK.Physical
{
    /// <summary>
    /// Contains the main parts of a Player's behaviour. You can extend this class and add your own custom 
    /// states for different characters, or disable pre-existing states from executing
    /// </summary>
    [AddComponentMenu(Constants.ContextName + "/Players/Player Core", 0)]
    public class Player : StateMachine<PlayerState>
    {
        public string CurrentStateName;
        
        [Header("States")] 
        public PStateMove MoveState = new();
        public PStateAir AirState = new();
        public PStateJump JumpState = new();
        public PStateRoll RollState = new();
        public PStateCrouch CrouchState = new();
        public PStateLookUp LookUpState = new();
        public PStateSpindash SpindashState = new();
        public PStateHurt HurtState = new();
        public PStateDead DeadState = new();
        
        [Space]
        [Header("References")]
        public PlayerPhysics Physics;
        public PlayerCamera Cam;
        public InputHandler PlayerInput;

        [Space]
        [Header("Utilities")]
        public PlayerCommon Common;
        public PlayerCollision Collision;
        public PlayerVisual Visual;
        public PlayerWorldInteraction Interactions;
        
        public delegate void StateChangeEvent(PlayerState PreviousState, PlayerState NewState);
        public delegate void PlayerUpdateEvent(Player This);
        
        public event StateChangeEvent OnStateChanged;
        public event PlayerUpdateEvent OnBeforeUpdate;
        
        public virtual void Start()
        {
            Application.targetFrameRate = 60;

            AddStates();
            
            Initialize<PStateMove>();

            Common.Player = Collision.Player = Visual.Player = Interactions.Player = this;
            Cam.PixelPosition = transform.position.WToP();

            if (!Stage.Current)
            {
                CDebug.Log("There is no Stage instance in this scene. This can lead to undefined behaviour, so be careful. Playback paused", CDebug.LogType.Important);
                Debug.Break();
            }
            
            Stage.ConnectedPlayers.Add(this);
            Common.CommonStart();
            PlayerStart();
        }

        public virtual void AddStates()
        {
            Add(MoveState);
            Add(AirState);
            Add(JumpState);
            Add(RollState);
            Add(CrouchState);
            Add(LookUpState);
            Add(SpindashState);
            Add(HurtState);
            Add(DeadState);
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
            OnBeforeUpdate?.Invoke(this);
            
            CurrentStateName = CurrentState.GetType().Name;

            transform.eulerAngles = Common.IsGrounded ? new Vector3(0, 0, (float)Collision.AngleMode * 90f) : Vector3.zero;
            transform.localScale = new Vector3(Collision.PlayerCollisionSize.x.PToW(), Collision.PlayerCollisionSize.y.PToW(), 1);
            
	        CurrentState.OnUpdate(Time.deltaTime);
            Common.CommonUpdate();
            Common.Gsp = Mathf.Clamp(Common.Gsp, -Physics.MaxSpeed, Physics.MaxSpeed);

            Visual.VisualUpdate();
            Interactions.InteractionUpdate();
            
            CDebug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(Common.SurfaceAngleRad), -Mathf.Cos(Common.SurfaceAngleRad)) * 0.15f, Color.magenta);
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

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(((Vector2)transform.position).WToP().Round().PToW(), 0.01f);
        }
    }
}