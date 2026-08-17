#if UNITY_EDITOR || (GODOT && TOOLS)
using System;
using System.Collections.Generic;
using System.Globalization;
using CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts;

namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Diagnostics
{
    /// <summary>
    /// Maintains stable diagnostic identity per FSM instance; instances with the same name remain independent and do not depend on the host or reflection scanning.
    /// </summary>
    internal static class FsmKitRegistry
    {
        private const string INSTANCE_ID_PREFIX = "fsm-";
        private const string UNNAMED_FSM = "UnnamedFSM";

        private static readonly object sGate = new object();
        private static readonly List<FsmKitDiagnosticRecord> sRecords =
            new List<FsmKitDiagnosticRecord>();

        private static long sNextInstanceId;
        private static long sStateVersion;

        /// <summary>
        /// Gets the monotonic version of diagnostic state; the host publishes Telemetry only after domain facts change based on this.
        /// </summary>
        internal static long StateVersion
        {
            get
            {
                lock (sGate)
                {
                    return sStateVersion;
                }
            }
        }

        /// <summary>
        /// Registers a state machine; re-registering the same instance only updates the name without changing instanceId or history.
        /// </summary>
        /// <param name="fsm">The state machine instance to register.</param>
        /// <param name="name">The diagnostic name; falls back to the state machine's own name when null.</param>
        internal static void Register(IFSM fsm, string name)
        {
            EnsureFsm(fsm);
            string resolvedName = ResolveRegistrationName(fsm, name);
            lock (sGate)
            {
                FsmKitDiagnosticRecord record = FindByFsm(fsm);
                if (record != null)
                {
                    record.Rename(resolvedName);
                    MarkChanged();
                    return;
                }

                sRecords.Add(CreateRecord(fsm, resolvedName));
                MarkChanged();
            }
        }

        /// <summary>
        /// Renames the specified state machine; if not yet registered, completes the first registration with the new name.
        /// </summary>
        /// <param name="fsm">The target state machine instance.</param>
        /// <param name="name">The new non-empty diagnostic name.</param>
        internal static void Rename(IFSM fsm, string name)
        {
            EnsureFsm(fsm);
            EnsureName(name);
            lock (sGate)
            {
                FsmKitDiagnosticRecord record = FindByFsm(fsm);
                if (record == null)
                {
                    sRecords.Add(CreateRecord(fsm, name));
                    MarkChanged();
                    return;
                }

                record.Rename(name);
                MarkChanged();
            }
        }

        /// <summary>
        /// Unregisters a state machine and all its diagnostic records by instance.
        /// </summary>
        /// <param name="fsm">The target state machine instance.</param>
        internal static void Unregister(IFSM fsm)
        {
            if (fsm == null)
            {
                return;
            }

            lock (sGate)
            {
                int index = FindIndexByFsm(fsm);
                if (index >= 0)
                {
                    sRecords.RemoveAt(index);
                    MarkChanged();
                }
            }
        }

        /// <summary>
        /// Records a successful state transition.
        /// </summary>
        /// <param name="fsm">The state machine instance.</param>
        /// <param name="from">The source state name.</param>
        /// <param name="to">The target state name.</param>
        internal static void RecordTransition(IFSM fsm, string from, string to)
        {
            lock (sGate)
            {
                GetOrCreateRecord(fsm).RecordTransition(from, to);
                MarkChanged();
            }
        }

        /// <summary>
        /// Records state entry or exit events.
        /// </summary>
        /// <param name="fsm">The state machine instance.</param>
        /// <param name="eventName">The stable event name.</param>
        /// <param name="state">The state name.</param>
        internal static void RecordStateEvent(IFSM fsm, string eventName, string state)
        {
            lock (sGate)
            {
                GetOrCreateRecord(fsm).RecordStateEvent(eventName, state);
                MarkChanged();
            }
        }

        /// <summary>
        /// Clears transition and state event records for the specified instance while preserving the stable registration identity.
        /// </summary>
        /// <param name="fsm">The state machine instance.</param>
        internal static void ClearRecords(IFSM fsm)
        {
            if (fsm == null)
            {
                return;
            }

            lock (sGate)
            {
                FsmKitDiagnosticRecord record = FindByFsm(fsm);
                if (record != null)
                {
                    record.ClearRecords();
                    MarkChanged();
                }
            }
        }

        /// <summary>
        /// Marks that the state machine lifecycle phase has changed, but no new transition or state event records were added.
        /// </summary>
        /// <param name="fsm">The registered state machine that changed.</param>
        internal static void NotifyStateChanged(IFSM fsm)
        {
            if (fsm == null)
            {
                return;
            }

            lock (sGate)
            {
                FsmKitDiagnosticRecord record = FindByFsm(fsm);
                if (record != null)
                {
                    record.NotifyStateChanged();
                    MarkChanged();
                }
            }
        }

        /// <summary>
        /// Gets all independent diagnostic snapshots in registration order and removes records for state machines that have been collected.
        /// </summary>
        /// <returns>A snapshot array that is safe for the caller to enumerate.</returns>
        internal static FsmKitDiagnosticSnapshot[] GetAllSnapshots()
        {
            lock (sGate)
            {
                PruneDeadRecords();
                List<FsmKitDiagnosticSnapshot> snapshots =
                    new List<FsmKitDiagnosticSnapshot>(sRecords.Count);
                for (var index = 0; index < sRecords.Count; index++)
                {
                    FsmKitDiagnosticSnapshot snapshot = sRecords[index].CreateSnapshot();
                    if (snapshot != null)
                    {
                        snapshots.Add(snapshot);
                    }
                }

                return snapshots.ToArray();
            }
        }

        /// <summary>
        /// Gets all current active instance identifiers for the host to establish per-instance split Shared Memory latest frames.
        /// </summary>
        /// <returns>A safe instanceId array arranged in registration order.</returns>
        internal static string[] GetInstanceIds()
        {
            lock (sGate)
            {
                PruneDeadRecords();
                string[] instanceIds = new string[sRecords.Count];
                for (var index = 0; index < sRecords.Count; index++)
                {
                    instanceIds[index] = sRecords[index].InstanceId;
                }

                return instanceIds;
            }
        }

        /// <summary>
        /// Reads a single instance diagnostic version by instance identifier without creating a complete diagnostic snapshot.
        /// </summary>
        /// <param name="instanceId">The stable instance identifier generated by the registry.</param>
        /// <returns>The current version of the active instance; returns zero if the instance is invalid or does not exist.</returns>
        internal static long GetInstanceVersion(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                return 0L;
            }

            lock (sGate)
            {
                FsmKitDiagnosticRecord record = FindByInstanceId(instanceId);
                return record == null ? 0L : record.Version;
            }
        }

        /// <summary>
        /// Prioritizes exact lookup by instanceId, otherwise returns the last registered compatible target by name; removes collected records before searching.
        /// </summary>
        /// <param name="instanceId">The exact instance identifier.</param>
        /// <param name="name">The compatible diagnostic name.</param>
        /// <returns>The found independent snapshot; null if no match or the instance has been collected.</returns>
        internal static FsmKitDiagnosticSnapshot FindSnapshot(string instanceId, string name)
        {
            lock (sGate)
            {
                PruneDeadRecords();
                FsmKitDiagnosticRecord record = !string.IsNullOrEmpty(instanceId)
                    ? FindByInstanceId(instanceId)
                    : FindByName(name);
                return record?.CreateSnapshot();
            }
        }

        /// <summary>
        /// Unregisters the last registered instance with the compatible name to avoid accidentally deleting other FSMs with the same name in a single call.
        /// </summary>
        /// <param name="name">The diagnostic name.</param>
        internal static void UnregisterByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            lock (sGate)
            {
                int index = FindIndexByName(name);
                if (index >= 0)
                {
                    sRecords.RemoveAt(index);
                    MarkChanged();
                }
            }
        }

        /// <summary>
        /// Clears all instances and history; the instanceId counter does not roll back to prevent old references from matching new instances.
        /// </summary>
        internal static void ClearAll()
        {
            lock (sGate)
            {
                if (sRecords.Count > 0)
                {
                    sRecords.Clear();
                    MarkChanged();
                }
            }
        }

        /// <summary>Advances the diagnostic version; the caller must hold the registry lock.</summary>
        private static void MarkChanged()
        {
            sStateVersion++;
        }

        /// <summary>Gets an existing record; if missing, creates one using the state machine's own name.</summary>
        private static FsmKitDiagnosticRecord GetOrCreateRecord(IFSM fsm)
        {
            EnsureFsm(fsm);
            FsmKitDiagnosticRecord record = FindByFsm(fsm);
            if (record != null)
            {
                return record;
            }

            record = CreateRecord(fsm, ResolveRegistrationName(fsm, fsm.Name));
            sRecords.Add(record);
            return record;
        }

        /// <summary>Creates a record with a new instanceId; the caller must hold the registry lock.</summary>
        private static FsmKitDiagnosticRecord CreateRecord(IFSM fsm, string name)
        {
            sNextInstanceId++;
            string suffix = sNextInstanceId.ToString("D8", CultureInfo.InvariantCulture);
            return new FsmKitDiagnosticRecord(INSTANCE_ID_PREFIX + suffix, name, fsm);
        }

        /// <summary>Finds a record by reference to avoid merging instances after the state machine overrides Equals.</summary>
        private static FsmKitDiagnosticRecord FindByFsm(IFSM fsm)
        {
            int index = FindIndexByFsm(fsm);
            return index >= 0 ? sRecords[index] : null;
        }

        /// <summary>Finds the index by state machine reference; the caller must hold the registry lock.</summary>
        private static int FindIndexByFsm(IFSM fsm)
        {
            for (var index = 0; index < sRecords.Count; index++)
            {
                if (sRecords[index].TryGetFsm(out IFSM candidate) && ReferenceEquals(candidate, fsm))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>Removes records for state machines that have been collected; the caller must hold the registry lock.</summary>
        private static void PruneDeadRecords()
        {
            bool removed = false;
            for (var index = sRecords.Count - 1; index >= 0; index--)
            {
                if (!sRecords[index].TryGetFsm(out _))
                {
                    sRecords.RemoveAt(index);
                    removed = true;
                }
            }

            if (removed)
            {
                MarkChanged();
            }
        }

        /// <summary>Finds a record by instance identifier.</summary>
        private static FsmKitDiagnosticRecord FindByInstanceId(string instanceId)
        {
            int index = FindIndexByInstanceId(instanceId);
            return index >= 0 ? sRecords[index] : null;
        }

        /// <summary>Finds the index by instance identifier.</summary>
        private static int FindIndexByInstanceId(string instanceId)
        {
            for (var index = 0; index < sRecords.Count; index++)
            {
                if (string.Equals(sRecords[index].InstanceId, instanceId, StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>Returns the last registered record by name, maintaining the legacy same-name override query experience.</summary>
        private static FsmKitDiagnosticRecord FindByName(string name)
        {
            int index = FindIndexByName(name);
            return index >= 0 ? sRecords[index] : null;
        }

        /// <summary>Finds the index by name searching backward.</summary>
        private static int FindIndexByName(string name)
        {
            for (var index = sRecords.Count - 1; index >= 0; index--)
            {
                if (string.Equals(sRecords[index].Name, name, StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>Resolves the first registration name to ensure all instances can be queried by name.</summary>
        private static string ResolveRegistrationName(IFSM fsm, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            return string.IsNullOrEmpty(fsm.Name) ? UNNAMED_FSM : fsm.Name;
        }

        /// <summary>Rejects null state machine instances.</summary>
        private static void EnsureFsm(IFSM fsm)
        {
            if (fsm == null)
            {
                throw new ArgumentNullException(nameof(fsm));
            }
        }

        /// <summary>Rejects null diagnostic names or instance identifiers.</summary>
        private static void EnsureName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("FSM identity must not be empty.", nameof(name));
            }
        }
    }
}
#endif