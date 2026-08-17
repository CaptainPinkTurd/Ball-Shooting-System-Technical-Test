using System;
using System.Collections.Generic;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Diagnostics;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Machines
{
    /// <summary>Provides parameterized start for regular FSMs, termination/disposal of terminal states, and guards against lifecycle reentrancy.</summary>
    public partial class FSM<TEnum> where TEnum : System.Enum
    {
        /// <summary>Attempt to start the specified state with parameters; if the state does not support that parameter type, fall back to parameterless entry.</summary>
        /// <typeparam name="TArgs">Type of entry arguments.</typeparam>
        /// <param name="id">Target state identifier.</param>
        /// <param name="state">Target state instance.</param>
        /// <param name="args">Entry arguments.</param>
        protected void TryStartState<TArgs>(TEnum id, IState state, TArgs args) =>
            TryStartStateCore(id, state, args, true);

        /// <summary>Enter the state according to the parameter contract; if parameters are not supported, fall back to parameterless entry.</summary>
        /// <typeparam name="TArgs">Type of entry arguments.</typeparam>
        /// <param name="state">Target state.</param>
        /// <param name="args">Entry arguments.</param>
        protected static void StartState<TArgs>(IState state, TArgs args)
        {
            if (state is IState<TArgs> stateWithArgs)
            {
                stateWithArgs.Start(args);
                return;
            }

            state.Start();
        }

        /// <summary>Clear the current selection and reset the machine to End.</summary>
        private void ResetSelection()
        {
            CurState = null;
            CurEnum = default(TEnum);
            mMachineState = MachineState.End;
        }

        /// <summary>Try to end and dispose all states, then reset the selection; on failure, aggregate and throw after completing cleanup.</summary>
        private void ClearStates()
        {
            List<Exception> errors = null;
            try
            {
                TryEndCurrentState(ref errors);
                DisposeStates(ref errors);
            }
            finally
            {
                mStateDic.Clear();
#if UNITY_EDITOR || (GODOT && TOOLS)
                ClearStateOrder();
#endif
                ResetSelection();
#if UNITY_EDITOR || (GODOT && TOOLS)
                FsmKitRegistry.ClearRecords(this);
#endif
            }

            if (errors != null)
            {
                throw new AggregateException("One or more FSM state cleanup operations failed.", errors);
            }
        }

        /// <summary>End the current active state and collect exceptions so that subsequent states can still be released.</summary>
        /// <param name="errors">Cleanup exceptions collected in order of occurrence.</param>
        private void TryEndCurrentState(ref List<Exception> errors)
        {
            if (CurState == null || mMachineState == MachineState.End)
            {
                return;
            }

            try
            {
                CurState.End();
            }
            catch (Exception exception)
            {
                AddCleanupError(ref errors, exception);
            }
        }

        /// <summary>Dispose all states one by one and isolate individual state exceptions to ensure subsequent states are still disposed.</summary>
        /// <param name="errors">Cleanup exceptions collected in order of occurrence.</param>
        private void DisposeStates(ref List<Exception> errors)
        {
            foreach (var state in mStateDic.Values)
            {
                try
                {
                    state.Dispose();
                }
                catch (Exception exception)
                {
                    AddCleanupError(ref errors, exception);
                }
            }
        }

        /// <summary>Create the exception collection as needed and preserve original exceptions to avoid losing contextual failure information.</summary>
        /// <param name="errors">The collection of cleanup exceptions.</param>
        /// <param name="exception">The state exception caught this time.</param>
        private static void AddCleanupError(ref List<Exception> errors, Exception exception)
        {
            errors ??= new List<Exception>();
            errors.Add(exception);
        }

        /// <summary>Reject access to a state machine that has already been disposed to prevent diagnostic instances from being re-registered.</summary>
        private void ThrowIfDisposed()
        {
            if (mIsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>Reject reuse after disposal or initiating nested state changes within lifecycle callbacks.</summary>
        protected void EnsureMutationAllowed()
        {
            ThrowIfDisposed();
            if (mIsTransitioning)
            {
                throw new InvalidOperationException("状态生命周期回调执行期间不能再次修改 FSM。");
            }
        }

        /// <summary>Enter the lifecycle transition region, making Start, End, Suspend, Resume, and Dispose callbacks non-reentrant.</summary>
        private void BeginLifecycleTransition()
        {
            EnsureMutationAllowed();
            mIsTransitioning = true;
        }

        /// <summary>Exit the lifecycle transition region, restoring the ability for normal Update callbacks to trigger state changes.</summary>
        private void EndLifecycleTransition()
        {
            mIsTransitioning = false;
        }
    }

    /// <summary>A generic finite state machine that supports start arguments for the machine itself.</summary>
    /// <typeparam name="TEnum">The enum type for states.</typeparam>
    /// <typeparam name="TArgs">Start argument type.</typeparam>
    public class FSM<TEnum, TArgs> : FSM<TEnum>, IFSM<TEnum, TArgs> where TEnum : System.Enum
    {
        /// <summary>Create an empty parameterized state machine.</summary>
        /// <param name="name">Optional diagnostic name used by Editor/Tools; not persisted in Player.</param>
        public FSM(string name = null) : base(name)
        {
        }

        /// <summary>Start from the current selection using arguments; remains a no-op if conditions fail or already running.</summary>
        /// <param name="args">Start arguments.</param>
        public void Start(TArgs args) => TryStartState(CurEnum, CurState, args);

        /// <summary>Start from the specified state using arguments; fall back to parameterless entry if parameters are not supported.</summary>
        /// <param name="id">State identifier.</param>
        /// <param name="args">Start arguments.</param>
        public void Start(TEnum id, TArgs args)
        {
            EnsureMutationAllowed();
            if (mStateDic.TryGetValue(id, out var state))
            {
                TryStartState(id, state, args);
            }
        }
    }
}