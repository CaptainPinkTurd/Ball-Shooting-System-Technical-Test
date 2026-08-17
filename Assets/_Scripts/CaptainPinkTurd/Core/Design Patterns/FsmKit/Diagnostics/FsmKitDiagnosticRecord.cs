#if UNITY_EDITOR || (GODOT && TOOLS)
using System;
using System.Collections.Generic;
using System.Globalization;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Diagnostics
{
    /// <summary>
    /// Stores the stable diagnostic identity and a bounded history for a single FSM; holds only a weak reference to the state machine instance so it does not prevent garbage collection.
    /// </summary>
    internal sealed class FsmKitDiagnosticRecord
    {
        private const int MAX_RECORD_COUNT = 200;

        private readonly FsmKitBoundedBuffer<FsmKitTransitionRecord> mHistory =
            new FsmKitBoundedBuffer<FsmKitTransitionRecord>(MAX_RECORD_COUNT);
        private readonly FsmKitBoundedBuffer<FsmKitStateEventRecord> mStateEvents =
            new FsmKitBoundedBuffer<FsmKitStateEventRecord>(MAX_RECORD_COUNT);
        private readonly Dictionary<string, long> mEntryCounts =
            new Dictionary<string, long>(StringComparer.Ordinal);
        private readonly WeakReference<IFSM> mFsmRef;

        /// <summary>
        /// Creates a diagnostic record bound to the specified state machine instance.
        /// </summary>
        /// <param name="instanceId">A stable and unique instance identifier within the current process.</param>
        /// <param name="name">Diagnostic name used for display and for compatibility queries.</param>
        /// <param name="fsm">The state machine instance.</param>
        internal FsmKitDiagnosticRecord(string instanceId, string name, IFSM fsm)
        {
            InstanceId = instanceId;
            Name = name;
            mFsmRef = new WeakReference<IFSM>(fsm);
            Version = 1L;
        }

        /// <summary>Gets the stable instance identifier within the current process.</summary>
        internal string InstanceId { get; }

        /// <summary>Gets the current diagnostic name.</summary>
        internal string Name { get; private set; }

        /// <summary>Gets the monotonic version number of the current instance's diagnostic facts.</summary>
        internal long Version { get; private set; }

        /// <summary>
        /// Attempts to resolve the (still) alive state machine instance.
        /// </summary>
        /// <param name="fsm">The resolved strong reference; null if the instance has been collected.</param>
        /// <returns>Returns true if the instance is still alive.</returns>
        internal bool TryGetFsm(out IFSM fsm)
        {
            return mFsmRef.TryGetTarget(out fsm);
        }

        /// <summary>
        /// Updates the diagnostic name; the instance identifier and existing history remain unchanged.
        /// </summary>
        /// <param name="name">The new non-empty diagnostic name.</param>
        internal void Rename(string name)
        {
            Name = name;
            MarkChanged();
        }

        /// <summary>
        /// Append a record of a successful start or state transition, keeping only the latest 200 entries.
        /// </summary>
        /// <param name="from">Source state.</param>
        /// <param name="to">Target state.</param>
        internal void RecordTransition(string from, string to)
        {
            mHistory.Add(new FsmKitTransitionRecord(from, to, CreateTimestamp()));
            if (!string.IsNullOrEmpty(to))
            {
                mEntryCounts.TryGetValue(to, out long current);
                mEntryCounts[to] = current + 1L;
            }

            MarkChanged();
        }

        /// <summary>
        /// Append a record of a state being added or removed, keeping only the latest 200 entries.
        /// </summary>
        /// <param name="eventName">Stable event name.</param>
        /// <param name="state">State enum name.</param>
        internal void RecordStateEvent(string eventName, string state)
        {
            mStateEvents.Add(new FsmKitStateEventRecord(eventName, state, CreateTimestamp()));
            MarkChanged();
        }

        /// <summary>
        /// Clears history and state events, but preserves the FSM registration identity for later reuse.
        /// </summary>
        internal void ClearRecords()
        {
            mHistory.Clear();
            mStateEvents.Clear();
            mEntryCounts.Clear();
            MarkChanged();
        }

        /// <summary>
        /// Mark state machine lifecycle changes that do not produce history records.
        /// </summary>
        internal void NotifyStateChanged()
        {
            MarkChanged();
        }

        /// <summary>
        /// Create a diagnostic snapshot that can be read without holding the registry lock; the snapshot holds a temporary strong reference to the resolved instance.
        /// </summary>
        /// <returns>A snapshot containing the current state machine reference and copies of both history types; null if the instance has been collected.</returns>
        internal FsmKitDiagnosticSnapshot CreateSnapshot()
        {
            if (!mFsmRef.TryGetTarget(out IFSM fsm))
            {
                return null;
            }

            return new FsmKitDiagnosticSnapshot(
                InstanceId,
                Name,
                Version,
                fsm,
                mHistory.ToArray(),
                mStateEvents.ToArray(),
                new Dictionary<string, long>(mEntryCounts, StringComparer.Ordinal));
        }

        /// <summary>
        /// Generate a millisecond-precision local time string compatible with the old workbench.
        /// </summary>
        /// <returns>Time formatted as HH:mm:ss.fff.</returns>
        private static string CreateTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Advance the current instance's version; the version is only modified while holding the registry lock.
        /// </summary>
        private void MarkChanged()
        {
            Version++;
        }
    }

    /// <summary>
    /// Provides a fixed-capacity ring buffer to prevent diagnostic history from growing without bound.
    /// </summary>
    /// <typeparam name="T">The record value type.</typeparam>
    internal sealed class FsmKitBoundedBuffer<T>
    {
        private readonly T[] mItems;
        private int mHead;
        private int mCount;

        /// <summary>
        /// Create a buffer with the specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum number of records; must be greater than zero.</param>
        internal FsmKitBoundedBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            mItems = new T[capacity];
        }

        /// <summary>
        /// Append a record; when capacity is full, overwrite the oldest record.
        /// </summary>
        /// <param name="item">The record to append.</param>
        internal void Add(T item)
        {
            if (mCount == mItems.Length)
            {
                mItems[mHead] = item;
                mHead = (mHead + 1) % mItems.Length;
                return;
            }

            int writeIndex = (mHead + mCount) % mItems.Length;
            mItems[writeIndex] = item;
            mCount++;
        }

        /// <summary>
        /// Clear all records and release any object references that may be held in the buffer.
        /// </summary>
        internal void Clear()
        {
            Array.Clear(mItems, 0, mItems.Length);
            mHead = 0;
            mCount = 0;
        }

        /// <summary>
        /// Create a copy of the records in order from oldest to newest.
        /// </summary>
        /// <returns>An independent array that does not expose the internal buffer.</returns>
        internal T[] ToArray()
        {
            T[] snapshot = new T[mCount];
            for (var index = 0; index < mCount; index++)
            {
                snapshot[index] = mItems[(mHead + index) % mItems.Length];
            }

            return snapshot;
        }
    }

    /// <summary>Represents a single FSM start or state transition.</summary>
    internal readonly struct FsmKitTransitionRecord
    {
        /// <summary>Create an immutable transition record.</summary>
        internal FsmKitTransitionRecord(string from, string to, string time)
        {
            From = from ?? string.Empty;
            To = to ?? string.Empty;
            Time = time ?? string.Empty;
        }

        internal string From { get; }
        internal string To { get; }
        internal string Time { get; }
    }

    /// <summary>Represents a single state addition or removal.</summary>
    internal readonly struct FsmKitStateEventRecord
    {
        /// <summary>Create an immutable state lifecycle record.</summary>
        internal FsmKitStateEventRecord(string eventName, string state, string time)
        {
            EventName = eventName ?? string.Empty;
            State = state ?? string.Empty;
            Time = time ?? string.Empty;
        }

        internal string EventName { get; }
        internal string State { get; }
        internal string Time { get; }
    }

    /// <summary>
    /// Provides a single-instance diagnostic snapshot usable outside the registry lock.
    /// </summary>
    internal sealed class FsmKitDiagnosticSnapshot
    {
        /// <summary>Create a single-instance diagnostic snapshot.</summary>
        internal FsmKitDiagnosticSnapshot(
            string instanceId,
            string name,
            long version,
            IFSM fsm,
            FsmKitTransitionRecord[] history,
            FsmKitStateEventRecord[] stateEvents,
            IReadOnlyDictionary<string, long> entryCounts)
        {
            InstanceId = instanceId;
            Name = name;
            Version = version;
            Fsm = fsm;
            History = history;
            StateEvents = stateEvents;
            EntryCounts = entryCounts;
        }

        internal string InstanceId { get; }
        internal string Name { get; }
        internal long Version { get; }
        internal IFSM Fsm { get; }
        internal FsmKitTransitionRecord[] History { get; }
        internal FsmKitStateEventRecord[] StateEvents { get; }
        internal IReadOnlyDictionary<string, long> EntryCounts { get; }
    }
}
#endif