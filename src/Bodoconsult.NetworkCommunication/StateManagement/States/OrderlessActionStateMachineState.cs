// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// Current implementation of <see cref="IOrderlessActionStateMachineState"/>
/// </summary>
public class OrderlessActionStateMachineState : BaseStateMachineState, IOrderlessActionStateMachineState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="id">ID of the current state</param>
    /// <param name="name">Name of the current state</param>
    public OrderlessActionStateMachineState(IStateMachineDevice currentContext, int id, string name) : base(currentContext, id, name)
    { }

    /// <summary>
    /// Delegate to be executed from an orderless state machine state
    /// </summary>
    public ExecuteActionForStateDelegate? ExecuteActionForStateDelegate { get; set; }

    /// <summary>
    /// Execute the action defined with <see cref="IOrderlessActionStateMachineState.ExecuteActionForStateDelegate"/> for this state
    /// </summary>
    public void Execute()
    {
        if (ExecuteActionForStateDelegate == null)
        {
            return;
        }

        ExecuteActionForStateDelegate.Invoke(this);
    }
}