namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts
{
    /// <summary>
    /// Defines the synchronous lifecycle of an FsmKit state; states are driven manually by the caller or host.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Determines whether the current state is allowed to enter; allowed by default.
        /// </summary>
        /// <returns>Returns true when entering is allowed.</returns>
        bool Condition()
        {
            return true;
        }

        /// <summary>Enter the state.</summary>
        void Start();

        /// <summary>Suspend the state.</summary>
        void Suspend();

        /// <summary>
        /// Resume a suspended state; does nothing by default to avoid re-triggering entry side effects.
        /// </summary>
        void Resume()
        {
        }

        /// <summary>Perform normal frame update.</summary>
        void Update();

        /// <summary>Perform fixed-step update.</summary>
        void FixedUpdate();

        /// <summary>Perform caller-defined custom update.</summary>
        void CustomUpdate();

        /// <summary>End the state.</summary>
        void End();

        /// <summary>Release resources held by the state; the state machine guarantees this is called only once per removal.</summary>
        void Dispose();

        /// <summary>
        /// Send a strongly-typed message to the state.
        /// </summary>
        /// <typeparam name="TMsg">Message type.</typeparam>
        /// <param name="message">Message value.</param>
        void SendMessage<TMsg>(TMsg message);
    }

    /// <summary>
    /// Defines a state that requires entry parameters; parameterless entry will pass default values according to 2.0-pre semantics.
    /// </summary>
    /// <typeparam name="TArgs">Type of the entry arguments.</typeparam>
    public interface IState<TArgs> : IState
    {
        /// <summary>
        /// Map parameterless entry to entry with default parameters, keeping plain FSM and parameterized FSM interchangeable.
        /// </summary>
        void IState.Start()
        {
            Start(default(TArgs));
        }

        /// <summary>
        /// Enter the state using the specified parameters.
        /// </summary>
        /// <param name="args">Entry arguments.</param>
        void Start(TArgs args);
    }
}