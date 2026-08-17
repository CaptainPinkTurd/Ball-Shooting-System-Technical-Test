using System;
using System.Collections.Generic;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Diagnostics;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Machines
{
    /// <summary>
    /// Provides a generic finite state machine that runs only one state at a time.
    /// </summary>
    /// <typeparam name="TEnum">State enum type.</typeparam>
    public partial class FSM<TEnum> : IFSM<TEnum> where TEnum : System.Enum
    {
        /// <summary>The current or most recently selected state, accessible and controlled modification for derived state machines.</summary>
        public IState CurState { get; protected set; }

        /// <summary>The current or most recently selected state enum value.</summary>
        public TEnum CurEnum { get; protected set; }

        /// <summary>The state machine lifecycle phase.</summary>
        public MachineState MachineState => mMachineState;

        /// <summary>Lifecycle field accessible by derived state machines.</summary>
        protected MachineState mMachineState = MachineState.End;

        /// <summary>State dictionary accessible by derived state machines.</summary>
        protected readonly Dictionary<TEnum, IState> mStateDic;

        /// <summary>Initial capacity of the state dictionary, reflected only once per enclosed enum type.</summary>
        private static readonly int sStateCapacity = System.Enum.GetValues(typeof(TEnum)).Length;

        private bool mIsDisposed;
        private bool mIsTransitioning;

        /// <summary>
        /// Create an empty state machine and pre-allocate the state dictionary based on enum count.
        /// </summary>
        /// <param name="name">Optional diagnostic name for Editor/Tools use; not saved in Player.</param>
        public FSM(string name = null)
        {
#if UNITY_EDITOR || (GODOT && TOOLS)
            mName = NormalizeName(name);
#endif
            mStateDic = new(sStateCapacity);
#if UNITY_EDITOR || (GODOT && TOOLS)
            FsmKitRegistry.Register(this, Name);
            FsmEditorHook.RaiseFsmCreated(this);
#endif
        }

        /// <summary>Get the specified state.</summary>
        /// <param name="id">State identifier.</param>
        /// <param name="state">The found state.</param>
        public void Get(TEnum id, out IState state)
        {
            ThrowIfDisposed();
            mStateDic.TryGetValue(id, out state);
        }

        /// <summary>Add or replace a state; when replacing the currently running state, close the old lifecycle and attempt to start the new state.</summary>
        /// <param name="id">State identifier.</param>
        /// <param name="state">State instance.</param>
        public void Add(TEnum id, IState state)
        {
            EnsureMutationAllowed();
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (mStateDic.TryGetValue(id, out var previousState))
            {
                if (ReferenceEquals(previousState, state))
                {
                    return;
                }

                ReplaceState(id, previousState, state);
                return;
            }

            mStateDic.Add(id, state);
#if UNITY_EDITOR || (GODOT && TOOLS)
            RecordStateOrder(id);
#endif
            if (CurState == null)
            {
                CurState = state;
                CurEnum = id;
            }

#if UNITY_EDITOR || (GODOT && TOOLS)
            PublishStateAdded(id);
#endif
        }

        /// <summary>Remove and dispose a state; when removing the current state, reset the machine to empty End.</summary>
        /// <param name="id">State identifier.</param>
        public void Remove(TEnum id)
        {
            EnsureMutationAllowed();
            if (!mStateDic.TryGetValue(id, out var state))
            {
                return;
            }

            BeginLifecycleTransition();
            bool isCurrent;
            try
            {
                isCurrent = ReferenceEquals(CurState, state);
                if (isCurrent && mMachineState != MachineState.End)
                {
                    state.End();
                }

                state.Dispose();
                mStateDic.Remove(id);
#if UNITY_EDITOR || (GODOT && TOOLS)
                RemoveStateOrder(id);
#endif
                if (isCurrent)
                {
                    ResetSelection();
                }
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }

#if UNITY_EDITOR || (GODOT && TOOLS)
            PublishStateRemoved(id);
#endif
        }

        /// <summary>Switch to a different state that meets the entry condition during the Running phase.</summary>
        /// <param name="id">State identifier.</param>
        public void Change(TEnum id) => ChangeCore<object>(id, null, false);

        /// <summary>Switch with parameters during the Running phase; fall back to parameter-less entry if the target does not support parameters.</summary>
        /// <typeparam name="TArgs">Enter parameter type.</typeparam>
        /// <param name="id">State identifier.</param>
        /// <param name="args">Enter parameter.</param>
        public void Change<TArgs>(TEnum id, TArgs args) => ChangeCore(id, args, true);

        /// <summary>Execute the shared guards, lifecycle closure, and diagnostic publication for both types of switches.</summary>
        /// <typeparam name="TArgs">Enter parameter type.</typeparam>
        /// <param name="id">State identifier.</param>
        /// <param name="args">Enter parameter.</param>
        /// <param name="hasArgs">Whether to enter the target state according to the parameter contract.</param>
        private void ChangeCore<TArgs>(TEnum id, TArgs args, bool hasArgs)
        {
            EnsureMutationAllowed();
            if (mMachineState != MachineState.Running ||
                !mStateDic.TryGetValue(id, out var state) ||
                ReferenceEquals(state, CurState))
            {
                return;
            }

            TEnum previousId = CurEnum;
            BeginLifecycleTransition();
            try
            {
                if (!state.Condition())
                {
                    return;
                }

                CurState.End();
                CurState = state;
                CurEnum = id;
                if (hasArgs)
                {
                    StartState(state, args);
                }
                else
                {
                    state.Start();
                }
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }
#if UNITY_EDITOR || (GODOT && TOOLS)
            PublishStateChanged(previousId, id);
#endif
        }

        /// <summary>End the current active state, dispose of all states, and clear the selection and diagnostic records.</summary>
        public void Clear()
        {
            EnsureMutationAllowed();
            BeginLifecycleTransition();
            try
            {
                ClearStates();
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }
#if UNITY_EDITOR || (GODOT && TOOLS)
            FsmEditorHook.RaiseFsmCleared(this);
#endif
        }

        /// <summary>Forward custom updates to the current state only during the Running phase.</summary>
        public void CustomUpdate()
        {
            ThrowIfDisposed();
            if (mMachineState == MachineState.Running)
            {
                CurState?.CustomUpdate();
            }
        }

        /// <summary>End the current active state but retain the selection to support subsequent parameter-less restart.</summary>
        public void End()
        {
            EnsureMutationAllowed();
            if (mMachineState == MachineState.End)
            {
                return;
            }

            BeginLifecycleTransition();
            try
            {
                CurState?.End();
                mMachineState = MachineState.End;
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }
#if UNITY_EDITOR || (GODOT && TOOLS)
            FsmKitRegistry.NotifyStateChanged(this);
#endif
        }

        /// <summary>Forward fixed updates to the current state only during the Running phase.</summary>
        public void FixedUpdate()
        {
            ThrowIfDisposed();
            if (mMachineState == MachineState.Running)
            {
                CurState?.FixedUpdate();
            }
        }

        /// <summary>Resume the suspended current state and return to Running; no-op in non-Suspend phases, does not repeat trigger entry logic.</summary>
        public void Resume()
        {
            EnsureMutationAllowed();
            if (mMachineState != MachineState.Suspend)
            {
                return;
            }

            BeginLifecycleTransition();
            try
            {
                CurState?.Resume();
                mMachineState = MachineState.Running;
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }
#if UNITY_EDITOR || (GODOT && TOOLS)
            FsmKitRegistry.NotifyStateChanged(this);
#endif
        }

        /// <summary>Start from the current selection; no-op if running or entry condition fails.</summary>
        public void Start()
        {
            TryStartState(CurEnum, CurState);
        }

        /// <summary>Start from the specified state; no-op if target is missing, running, or condition fails.</summary>
        /// <param name="id">State identifier.</param>
        public void Start(TEnum id)
        {
            EnsureMutationAllowed();
            if (mStateDic.TryGetValue(id, out var state))
            {
                TryStartState(id, state);
            }
        }

        /// <summary>Suspend the currently running state and stop subsequent ticks and message forwarding.</summary>
        public void Suspend()
        {
            EnsureMutationAllowed();
            if (CurState == null || mMachineState != MachineState.Running)
            {
                return;
            }

            BeginLifecycleTransition();
            try
            {
                CurState.Suspend();
                mMachineState = MachineState.Suspend;
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }
#if UNITY_EDITOR || (GODOT && TOOLS)
            FsmKitRegistry.NotifyStateChanged(this);
#endif
        }

        /// <summary>Forward normal updates to the current state only during the Running phase.</summary>
        public void Update()
        {
            ThrowIfDisposed();
            if (mMachineState == MachineState.Running)
            {
                CurState?.Update();
            }
        }

        /// <summary>Forward strongly-typed messages to the current state only during the Running phase.</summary>
        /// <typeparam name="TMsg">Message type.</typeparam>
        /// <param name="message">Message value.</param>
        public void SendMessage<TMsg>(TMsg message)
        {
            ThrowIfDisposed();
            if (mMachineState == MachineState.Running)
            {
                CurState?.SendMessage(message);
            }
        }

        /// <summary>Publish disposal event, unregister stable instance, then close and clear all states; repeated calls remain idempotent.</summary>
        public void Dispose()
        {
            if (mIsDisposed)
            {
                return;
            }

            EnsureMutationAllowed();
            mIsDisposed = true;
            mIsTransitioning = true;
#if UNITY_EDITOR || (GODOT && TOOLS)
            FsmEditorHook.RaiseFsmDisposed(this);
            FsmKitRegistry.Unregister(this);
#endif
            try
            {
                ClearStates();
            }
            finally
            {
                mIsTransitioning = false;
#if UNITY_EDITOR || (GODOT && TOOLS)
                FsmEditorHook.RaiseFsmCleared(this);
#endif
            }
        }

        /// <summary>Replace an existing state and decide whether the new state continues to run based on the current machine phase.</summary>
        /// <param name="id">State identifier being replaced.</param>
        /// <param name="previousState">Old state about to be released.</param>
        /// <param name="replacement">New state taking over the identifier.</param>
        private void ReplaceState(TEnum id, IState previousState, IState replacement)
        {
            BeginLifecycleTransition();
            try
            {
                bool isCurrent = ReferenceEquals(CurState, previousState);
                bool shouldRestart = isCurrent && mMachineState == MachineState.Running;
                if (isCurrent && mMachineState != MachineState.End)
                {
                    previousState.End();
                }

                previousState.Dispose();
                mStateDic[id] = replacement;
#if UNITY_EDITOR || (GODOT && TOOLS)
                RecordStateOrder(id);
#endif
                if (isCurrent)
                {
                    CurState = replacement;
                    CurEnum = id;
                    mMachineState = MachineState.End;
                    if (shouldRestart && replacement.Condition())
                    {
                        mMachineState = MachineState.Running;
                        replacement.Start();
                    }
                }
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }
#if UNITY_EDITOR || (GODOT && TOOLS)
            PublishStateRemoved(id);
            PublishStateAdded(id);
#endif
        }

        /// <summary>Attempt to start the specified state without parameters, and publish diagnostic records after success.</summary>
        /// <param name="id">Target state identifier.</param>
        /// <param name="state">Target state instance.</param>
        protected void TryStartState(TEnum id, IState state) => TryStartStateCore<object>(id, state, null, false);

        /// <summary>Execute the shared guards, suspend closure, entry, and diagnostic publication for both types of startups.</summary>
        /// <typeparam name="TArgs">Enter parameter type.</typeparam>
        /// <param name="id">Target state identifier.</param>
        /// <param name="state">Target state instance.</param>
        /// <param name="args">Enter parameter.</param>
        /// <param name="hasArgs">Whether to enter the target state according to the parameter contract.</param>
        private void TryStartStateCore<TArgs>(TEnum id, IState state, TArgs args, bool hasArgs)
        {
            EnsureMutationAllowed();
            if (state == null || mMachineState == MachineState.Running)
            {
                return;
            }

            BeginLifecycleTransition();
            try
            {
                if (!state.Condition())
                {
                    return;
                }

                if (mMachineState == MachineState.Suspend && CurState != null && !ReferenceEquals(CurState, state))
                {
                    CurState.End();
                }

                mMachineState = MachineState.Running;
                CurState = state;
                CurEnum = id;
                if (hasArgs)
                {
                    StartState(state, args);
                }
                else
                {
                    state.Start();
                }
            }
            catch
            {
                mMachineState = MachineState.End;
                throw;
            }
            finally
            {
                EndLifecycleTransition();
            }
#if UNITY_EDITOR || (GODOT && TOOLS)
            PublishFsmStarted(id);
#endif
        }

    }
}