namespace CaptainPinkTurd.Core.DesignPatterns.FsmKit.Contracts
{
    /// <summary>
    /// Represents the lifecycle stage the state machine or parallel sub-state is currently in.
    /// </summary>
    public enum MachineState
    {
        /// <summary>The state machine has ended and no longer forwards ticks or messages.</summary>
        End = 0,

        /// <summary>The state machine is suspended, preserving the current selection but not forwarding ticks or messages.</summary>
        Suspend = 1,

        /// <summary>The state machine is running and forwards ticks and messages to active states.</summary>
        Running = 2
    }
}