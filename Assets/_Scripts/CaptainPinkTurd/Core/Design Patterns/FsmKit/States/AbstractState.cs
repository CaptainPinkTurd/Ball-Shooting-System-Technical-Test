using System;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Machines;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.States
{
    /// <summary>
    /// Provides overridable lifecycle methods for states with a shared blackboard and hides explicit implementation boilerplate of IState.
    /// </summary>
    /// <typeparam name="TEnum">The state enum type of the FSM this state belongs to.</typeparam>
    /// <typeparam name="TBlack">The shared blackboard type.</typeparam>
    public abstract class AbstractState<TEnum, TBlack> : IState where TEnum : System.Enum
    {
        /// <summary>The state machine this state belongs to, allowing derived states to initiate transitions.</summary>
        protected FSM<TEnum> FSM;

        /// <summary>The business blackboard shared by the state.</summary>
        protected TBlack blackboard;

        /// <summary>
        /// Creates a state that binds the state machine and blackboard.
        /// </summary>
        /// <param name="fsm">The state machine this state belongs to.</param>
        /// <param name="black">The shared blackboard.</param>
        protected AbstractState(FSM<TEnum> fsm, TBlack black)
        {
            FSM = fsm ?? throw new ArgumentNullException(nameof(fsm));
            blackboard = black;
        }

        /// <summary>Determines whether the state is allowed to enter; allows entry by default.</summary>
        /// <returns>Returns true when entry is allowed.</returns>
        protected virtual bool OnCondition() => true;

        /// <summary>Handles state entry.</summary>
        protected virtual void OnEnter() { }

        /// <summary>Handles normal frame update.</summary>
        protected virtual void OnUpdate() { }

        /// <summary>Handles fixed time step update.</summary>
        protected virtual void OnFixedUpdate() { }

        /// <summary>Handles custom updates defined by the caller.</summary>
        protected virtual void OnCustomUpdate() { }

        /// <summary>Handles state exit.</summary>
        protected virtual void OnExit() { }

        /// <summary>Handles state suspension.</summary>
        protected virtual void OnSuspend() { }

        /// <summary>Handles state resumption.</summary>
        protected virtual void OnResume() { }

        /// <summary>Releases state resources.</summary>
        protected virtual void OnDispose() { }

        /// <summary>Handles strongly-typed messages.</summary>
        /// <typeparam name="TMsg">The message type.</typeparam>
        /// <param name="message">The message value.</param>
        protected virtual void OnMessage<TMsg>(TMsg message) { }

        /// <summary>Forwards the entry condition to the derived state.</summary>
        /// <returns>The derived state's evaluation result.</returns>
        bool IState.Condition() => OnCondition();

        /// <summary>Forwards parameterless entry to the derived state.</summary>
        void IState.Start() => OnEnter();

        /// <summary>Forwards suspension to the derived state.</summary>
        void IState.Suspend() => OnSuspend();

        /// <summary>Forwards resumption to the derived state.</summary>
        void IState.Resume() => OnResume();

        /// <summary>Forwards normal update to the derived state.</summary>
        void IState.Update() => OnUpdate();

        /// <summary>Forwards fixed update to the derived state.</summary>
        void IState.FixedUpdate() => OnFixedUpdate();

        /// <summary>Forwards custom update to the derived state.</summary>
        void IState.CustomUpdate() => OnCustomUpdate();

        /// <summary>Forwards exit to the derived state.</summary>
        void IState.End() => OnExit();

        /// <summary>Forwards release to the derived state.</summary>
        void IState.Dispose() => OnDispose();

        /// <summary>Forwards messages to the derived state; messages are uniformly delivered through the FSM's Running gate control.</summary>
        /// <typeparam name="TMsg">The message type.</typeparam>
        /// <param name="message">The message value.</param>
        void IState.SendMessage<TMsg>(TMsg message) => OnMessage(message);
    }

    /// <summary>
    /// Provides a strongly-typed OnEnter callback for states with a shared blackboard that require entry parameters.
    /// </summary>
    /// <typeparam name="TEnum">The state enum type of the FSM this state belongs to.</typeparam>
    /// <typeparam name="TBlack">The shared blackboard type.</typeparam>
    /// <typeparam name="TArgs">The entry parameter type.</typeparam>
    public abstract class AbstractState<TEnum, TBlack, TArgs> : AbstractState<TEnum, TBlack>, IState<TArgs>
        where TEnum : System.Enum
    {
        /// <summary>Creates a parameterized state that binds the state machine and blackboard.</summary>
        /// <param name="fsm">The state machine this state belongs to.</param>
        /// <param name="black">The shared blackboard.</param>
        protected AbstractState(FSM<TEnum> fsm, TBlack black) : base(fsm, black) { }

        /// <summary>Seals parameterless entry as default parameter entry.</summary>
        protected sealed override void OnEnter() => OnEnter(default(TArgs));

        /// <summary>Handles state entry with parameters.</summary>
        /// <param name="args">The entry parameters.</param>
        protected virtual void OnEnter(TArgs args) { }

        /// <summary>Forwards strongly-typed entry to the derived state.</summary>
        /// <param name="args">The entry parameters.</param>
        void IState<TArgs>.Start(TArgs args) => OnEnter(args);
    }
}