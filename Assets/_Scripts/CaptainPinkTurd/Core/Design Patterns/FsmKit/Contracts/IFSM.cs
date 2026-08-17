using System;
using System.Collections.Generic;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts
{
    /// <summary>
    /// Defines the common business lifecycle for all FsmKit state machines; Editor/Tools builds additionally provide diagnostic read-only contracts.
    /// </summary>
    public interface IFSM : IState
    {
        /// <summary>Gets the machine's lifecycle stage.</summary>
        MachineState MachineState { get; }

#if UNITY_EDITOR || (GODOT && TOOLS)
        /// <summary>Gets the diagnostic name of the state machine.</summary>
        string Name { get; }

        /// <summary>Gets the enum type used for state identifiers.</summary>
        Type EnumType { get; }

        /// <summary>Gets the current or most recently focused state; null if there is no state.</summary>
        IState CurrentState { get; }

        /// <summary>Gets the integer identifier of the current or most recently focused state; -1 if there is no state.</summary>
        int CurrentStateId { get; }

        /// <summary>
        /// Gets an independent snapshot indexed by integer state identifiers; modifying the returned dictionary will not affect the state machine.
        /// </summary>
        /// <returns>Snapshot of the state dictionary.</returns>
        IReadOnlyDictionary<int, IState> GetAllStates();

        /// <summary>
        /// Gets the stable order in which a state was originally added to the state machine.
        /// </summary>
        /// <param name="stateId">State integer identifier.</param>
        /// <returns>Order index when added; returns stateId if the state does not exist.</returns>
        int GetStateOrderIndex(int stateId);
#endif
    }

    /// <summary>
    /// Defines the state machine entry identified by an enum.
    /// </summary>
    /// <typeparam name="TEnum">State enum type.</typeparam>
    public interface IFSM<TEnum> : IFSM where TEnum : System.Enum
    {
        /// <summary>Gets the current or most recently focused state's enum value.</summary>
        TEnum CurEnum { get; }

        /// <summary>
        /// Gets the specified state; returns null via out when the state does not exist.
        /// </summary>
        /// <param name="id">State identifier.</param>
        /// <param name="state">Found state.</param>
        void Get(TEnum id, out IState state);

        /// <summary>Starts the state machine from the specified state.</summary>
        /// <param name="id">State identifier.</param>
        void Start(TEnum id);

        /// <summary>Adds or replaces the specified state.</summary>
        /// <param name="id">State identifier.</param>
        /// <param name="state">State instance.</param>
        void Add(TEnum id, IState state);

        /// <summary>Removes and releases the specified state.</summary>
        /// <param name="id">State identifier.</param>
        void Remove(TEnum id);

        /// <summary>Switches to or starts the specified state.</summary>
        /// <param name="id">State identifier.</param>
        void Change(TEnum id);

        /// <summary>Switches to or starts the specified state with parameters.</summary>
        /// <typeparam name="TArgs">Type of enter parameters.</typeparam>
        /// <param name="id">State identifier.</param>
        /// <param name="args">Enter parameters.</param>
        void Change<TArgs>(TEnum id, TArgs args);

        /// <summary>Ends and releases all states, returning the state machine to the empty End state.</summary>
        void Clear();
    }

    /// <summary>
    /// Defines an entry for state machines that require parameters when starting.
    /// </summary>
    /// <typeparam name="TEnum">State enum type.</typeparam>
    /// <typeparam name="TArgs">Startup parameter type.</typeparam>
    public interface IFSM<TEnum, TArgs> : IFSM<TEnum>, IState<TArgs> where TEnum : System.Enum
    {
        /// <summary>Starts the state machine from the specified state using parameters.</summary>
        /// <param name="id">State identifier.</param>
        /// <param name="args">Startup parameters.</param>
        void Start(TEnum id, TArgs args);
    }
}