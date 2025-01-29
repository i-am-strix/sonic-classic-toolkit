using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stray.Core;

namespace Stray.Physical
{
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
        
        public PlayerCommon Cmn;
        public PlayerPhysics Phys;
        public PlayerCollision Col;
        public PlayerCamera Cam;
        public PlayerVisual Visual;
        public PlayerWorldInteraction Interactions;
        public GameInputManager PlayerInput;

        public delegate void StateChangeEvent(PlayerState state);
        public event StateChangeEvent OnStateChanged;
        
        void Start()
        {
            Application.targetFrameRate = 60;
            Add(MoveState);
            Add(AirState);
            Add(JumpState);
            Add(RollState);
            Add(CrouchState);
            Add(SpindashState);
            Initialize<WPStateMove>();
        }
        
        // ------------------- //
        // ------ EVENTS ----- //
        // ------------------- //

        void Update()
        {
            currentState = CurrentState.GetType().Name;
            transform.eulerAngles = Cmn.IsGrounded ? new Vector3(0, 0, (float)Col.AngleMode * 90f) : Vector3.zero;
            transform.localScale = new Vector3(Col.PlayerCollisionSize.x.PToW(), Col.PlayerCollisionSize.y.PToW(), 1);
            
	        Cmn.Gsp = Mathf.Clamp(Cmn.Gsp, -Phys.MaxSpeed, Phys.MaxSpeed);
	        
	        CurrentState.OnUpdate(Time.deltaTime);
            Visual.VisualUpdate();
            
            WDebug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(Cmn.SurfaceAngleRad), -Mathf.Cos(Cmn.SurfaceAngleRad)) * 0.15f, Color.magenta);
        }
        
        void LateUpdate()
        {
            CurrentState.OnLateUpdate(Time.deltaTime);
            Visual.VisualLateUpdate();
        }
        
        void FixedUpdate()
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
            if (base.Set<T>())
            {
                OnStateChanged?.Invoke(AvailableStates.Find(s => s.GetType() == typeof(T)));
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