#if UNITY_EDITOR || (GODOT && TOOLS)
using System;
using System.Collections.Generic;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Diagnostics;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Machines
{
    /// <summary>Contains FSM observation and publish logic that exists only in Editor/Tools builds.</summary>
    public partial class FSM<TEnum> where TEnum : System.Enum
    {
        private readonly Dictionary<TEnum, int> mStateOrder = new();
        private string mName;
        private int mNextStateOrder;

        /// <summary>Name used for FSM diagnostics; when modified the stable instance registry is updated accordingly.</summary>
        public string Name
        {
            get => mName;
            set
            {
                mName = NormalizeName(value);
                if (!mIsDisposed) FsmKitRegistry.Rename(this, mName);
            }
        }

        /// <summary>Gets the enum type used for state identifiers.</summary>
        public Type EnumType => typeof(TEnum);

        /// <summary>Gets the current state via the non-generic diagnostic contract.</summary>
        public IState CurrentState => CurState;

        /// <summary>Gets the current state's integer identifier via the non-generic diagnostic contract.</summary>
        public int CurrentStateId => CurState == null ? -1 : Convert.ToInt32(CurEnum);

        /// <summary>Gets a snapshot of the state dictionary with independent integer keys.</summary>
        /// <returns>Returns: snapshot of the state dictionary.</returns>
        public IReadOnlyDictionary<int, IState> GetAllStates()
        {
            Dictionary<int, IState> snapshot = new(mStateDic.Count);
            foreach (var pair in mStateDic) snapshot[Convert.ToInt32(pair.Key)] = pair.Value;
            return snapshot;
        }

        /// <summary>Gets the order index when the state was first added.</summary>
        /// <param name="stateId">stateId: the state's integer identifier.</param>
        /// <returns>The addition order; returns stateId if missing.</returns>
        public int GetStateOrderIndex(int stateId)
        {
            var id = (TEnum)System.Enum.ToObject(typeof(TEnum), stateId);
            return mStateOrder.TryGetValue(id, out var order) ? order : stateId;
        }

        /// <summary>Normalize an empty diagnostic name to a stable generic state machine name.</summary>
        /// <param name="name">name: name provided by the caller.</param>
        /// <returns>Returns: a non-empty name suitable for the registry.</returns>
        private static string NormalizeName(string name) =>
            string.IsNullOrEmpty(name) ? "FSM<" + typeof(TEnum).Name + ">" : name;

        /// <summary>Record the order when a state is first added.</summary>
        /// <param name="id">id: state identifier.</param>
        protected void RecordStateOrder(TEnum id)
        {
            if (!mStateOrder.ContainsKey(id)) mStateOrder.Add(id, mNextStateOrder++);
        }

        /// <summary>Remove the state order record; re-adding will get a new order.</summary>
        protected void RemoveStateOrder(TEnum id) => mStateOrder.Remove(id);

        /// <summary>Clear all state order records.</summary>
        protected void ClearStateOrder()
        {
            mStateOrder.Clear();
            mNextStateOrder = 0;
        }

        /// <summary>Record a state addition and notify observation subscribers.</summary>
        /// <param name="id">id: the added state's identifier.</param>
        private void PublishStateAdded(TEnum id)
        {
            string stateName = id.ToString();
            FsmKitRegistry.RecordStateEvent(this, "added", stateName);
            FsmEditorHook.RaiseStateAdded(this, stateName);
        }

        /// <summary>Record a state removal and notify observation subscribers.</summary>
        /// <param name="id">id: the removed state's identifier.</param>
        private void PublishStateRemoved(TEnum id)
        {
            string stateName = id.ToString();
            FsmKitRegistry.RecordStateEvent(this, "removed", stateName);
            FsmEditorHook.RaiseStateRemoved(this, stateName);
        }

        /// <summary>Record the FSM start and notify observation subscribers.</summary>
        /// <param name="id">id: the identifier of the successfully started state.</param>
        private void PublishFsmStarted(TEnum id)
        {
            string stateName = id.ToString();
            FsmKitRegistry.RecordTransition(this, "Start", stateName);
            FsmEditorHook.RaiseFsmStarted(this, stateName);
        }

        /// <summary>Record a normal state transition and notify observation subscribers.</summary>
        /// <param name="previousId">previousId: source state identifier.</param>
        /// <param name="currentId">currentId: target state identifier.</param>
        private void PublishStateChanged(TEnum previousId, TEnum currentId)
        {
            string previousName = previousId.ToString();
            string currentName = currentId.ToString();
            FsmKitRegistry.RecordTransition(this, previousName, currentName);
            FsmEditorHook.RaiseStateChanged(this, previousName, currentName);
        }
    }
}
#endif