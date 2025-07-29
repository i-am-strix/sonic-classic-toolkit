namespace SonicCTK.Core
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
        
        /// <summary>
        /// This gets called when the state is first added to the state machine
        /// </summary>
		public abstract void OnInitialize();
        
        /// <summary>
        /// This gets called when this state becomes the state machine's current state
        /// </summary>
        public abstract void OnEnter();
        
        /// <summary>
        /// This gets called when the state machine is changed to another state
        /// </summary>
        public abstract void OnExit();
        
        /// <summary>
        /// This is the equivalent of the Update() method in a MonoBehaviour
        /// </summary>
        /// <param name="delta">The delta time parameter</param>
        public abstract void OnUpdate(float delta);
        
        /// <summary>
        /// This is the equivalent of the LateUpdate() method in a MonoBehaviour
        /// </summary>
        /// <param name="delta">The delta time parameter</param>
        public abstract void OnLateUpdate(float delta);
        
        /// <summary>
        /// This is the equivalent of the FixedUpdate() method in a MonoBehaviour
        /// </summary>
        /// <param name="delta">The fixed delta time parameter</param>
        public abstract void OnFixedUpdate(float delta);
    }
}