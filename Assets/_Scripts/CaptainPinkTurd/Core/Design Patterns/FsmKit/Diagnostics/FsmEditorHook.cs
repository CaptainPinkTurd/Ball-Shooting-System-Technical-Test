#if UNITY_EDITOR || (GODOT && TOOLS)
using System;
using System.Diagnostics;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Diagnostics
{
    /// <summary>
    /// Publishes FsmKit lifecycle diagnostic events; the events are editor-agnostic, so Unity, Godot, and tool hosts share the same contract.
    /// </summary>
    public static class FsmEditorHook
    {
        /// <summary>Raised after an FSM is created.</summary>
        public static event Action<IFSM> OnFsmCreated;

        /// <summary>Raised before an FSM is explicitly disposed.</summary>
        public static event Action<IFSM> OnFsmDisposed;

        /// <summary>Raised after an FSM has been cleared.</summary>
        public static event Action<IFSM> OnFsmCleared;

        /// <summary>Raised after an FSM has successfully started.</summary>
        public static event Action<IFSM, string> OnFsmStarted;

        /// <summary>Raised after a normal FSM successfully changes state.</summary>
        public static event Action<IFSM, string, string> OnStateChanged;

        /// <summary>Raised after a state has been successfully added.</summary>
        public static event Action<IFSM, string> OnStateAdded;

        /// <summary>Raised before a state is removed.</summary>
        public static event Action<IFSM, string> OnStateRemoved;

        /// <summary>Notifies that the FSM has been created.</summary>
        /// <param name="fsm">The FSM instance.</param>
        internal static void RaiseFsmCreated(IFSM fsm) => InvokeSafely(OnFsmCreated, fsm);

        /// <summary>Notifies that the FSM is about to be disposed.</summary>
        /// <param name="fsm">The FSM instance.</param>
        internal static void RaiseFsmDisposed(IFSM fsm) => InvokeSafely(OnFsmDisposed, fsm);

        /// <summary>Notifies that the FSM has been cleared.</summary>
        /// <param name="fsm">The FSM instance.</param>
        internal static void RaiseFsmCleared(IFSM fsm) => InvokeSafely(OnFsmCleared, fsm);

        /// <summary>Notifies that the FSM has been started.</summary>
        /// <param name="fsm">The FSM instance.</param>
        /// <param name="state">The initial state's name.</param>
        internal static void RaiseFsmStarted(IFSM fsm, string state) => InvokeSafely(OnFsmStarted, fsm, state);

        /// <summary>Notifies that a normal FSM has changed state.</summary>
        /// <param name="fsm">The FSM instance.</param>
        /// <param name="from">The name of the source state.</param>
        /// <param name="to">The name of the target state.</param>
        internal static void RaiseStateChanged(IFSM fsm, string from, string to) =>
            InvokeSafely(OnStateChanged, fsm, from, to);

        /// <summary>Notifies that a state has been added.</summary>
        /// <param name="fsm">The FSM instance.</param>
        /// <param name="state">The name of the state.</param>
        internal static void RaiseStateAdded(IFSM fsm, string state) => InvokeSafely(OnStateAdded, fsm, state);

        /// <summary>Notifies that a state is about to be removed.</summary>
        /// <param name="fsm">The FSM instance.</param>
        /// <param name="state">The name of the state.</param>
        internal static void RaiseStateRemoved(IFSM fsm, string state) => InvokeSafely(OnStateRemoved, fsm, state);

        /// <summary>Invokes stateless observers one by one; if an individual observer fails, the exception is written only to the debug output.</summary>
        /// <param name="callbacks">A snapshot of the current event subscribers.</param>
        /// <param name="fsm">The FSM instance.</param>
        private static void InvokeSafely(Action<IFSM> callbacks, IFSM fsm)
        {
            if (callbacks == null)
            {
                return;
            }

            foreach (Delegate subscriber in callbacks.GetInvocationList())
            {
                try
                {
                    ((Action<IFSM>)subscriber)(fsm);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        /// <summary>Invokes single-state-parameter observers one by one, ensuring subsequent observers still receive the event.</summary>
        /// <param name="callbacks">A snapshot of the current event subscribers.</param>
        /// <param name="fsm">The FSM instance.</param>
        /// <param name="state">The state name associated with the event.</param>
        private static void InvokeSafely(Action<IFSM, string> callbacks, IFSM fsm, string state)
        {
            if (callbacks == null)
            {
                return;
            }

            foreach (Delegate subscriber in callbacks.GetInvocationList())
            {
                try
                {
                    ((Action<IFSM, string>)subscriber)(fsm, state);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        /// <summary>Invokes state-change observers one by one, isolating exceptions thrown by any subscriber.</summary>
        /// <param name="callbacks">A snapshot of the current event subscribers.</param>
        /// <param name="fsm">The FSM instance.</param>
        /// <param name="from">The name of the source state.</param>
        /// <param name="to">The name of the target state.</param>
        private static void InvokeSafely(
            Action<IFSM, string, string> callbacks,
            IFSM fsm,
            string from,
            string to)
        {
            if (callbacks == null)
            {
                return;
            }

            foreach (Delegate subscriber in callbacks.GetInvocationList())
            {
                try
                {
                    ((Action<IFSM, string, string>)subscriber)(fsm, from, to);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }
    }
}
#endif