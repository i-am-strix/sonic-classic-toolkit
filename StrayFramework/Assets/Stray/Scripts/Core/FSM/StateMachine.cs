#pragma warning disable CS0693

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stray.Core
{
    /// <summary>
    /// Low-level definiton for a state machine. Must be inherited from to produce anything worthwhile
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public abstract class StateMachine<T> : MonoBehaviour where T : State
	{
		[Header("Debug")]
        public T CurrentState;
        public T PreviousState;

        public List<T> AvailableStates
        {
            get;
            set;
        }

        public virtual void Add(T state)
        {
            if (AvailableStates == null) AvailableStates = new List<T>();
            else {
                foreach(T fState in AvailableStates){
                    if (fState.GetType() == typeof(T)){
	                    //BKLog.Print($"State of type {typeof(T).Name} already exists. Cannot add.", BKLog.LogType.Ignorable);
                        return; //failsafe
                    }
                }
            }
	        AvailableStates.Add(state);
	        state.OnInitialize();
        }
        
        public virtual void Initialize<T>()
        {
            CurrentState = PreviousState = AvailableStates.Find(s=>s.GetType() == typeof(T));
        }

        public virtual bool IsCurrentState<T>() where T : State
        {
            return CurrentState.GetType() == typeof(T);
        }

        public virtual bool Set<T>() where T : State
        {
            var NewState = AvailableStates.Find(s => s.GetType() == typeof(T));
            if (NewState != null)
            {
                CurrentState.OnExit();
                PreviousState = CurrentState;
                CurrentState = NewState;
                CurrentState.OnEnter();

                return true;
            }
            else
            {
                WDebug.Log($"{gameObject.name}: The {typeof(T).Name} state either hasn't been added to this object or it doesn't exist. You cannot transition into it.", 
                    WDebug.LogType.Important);

                return false;
            }
        }
        
        public virtual T Get<T>() where T : State
        {
            return AvailableStates.Find(s => s.GetType() == typeof(T)) as T;
        }
        
    }
}
