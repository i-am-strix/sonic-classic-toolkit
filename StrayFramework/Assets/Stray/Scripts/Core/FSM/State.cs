namespace Stray.Core
{
    /// <summary>
    /// Low-level definition of a state. States are used to define logic for any object using a StateMachine. You can extend from this class
    /// to create specialized states, like a <b>PlayerState</b>
    /// </summary>
    public abstract class State
	{
        /// <summary>
        /// If a state is disabled, it cannot be transitioned to and from, but it can still be accessed
        /// by other states for utility methods and such
        /// </summary>
        public bool IsDisabled = false;

		public abstract void OnInitialize();
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate(float Delta);
        public abstract void OnLateUpdate(float Delta);
        public abstract void OnFixedUpdate(float Delta);
    }
}