// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// Current implementation of <see cref="IOrderBasedActionStateMachineState"/>
/// </summary>
public class OrderBasedStateMachineState : BaseOrderBasedStateMachineState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="id">ID of the current state</param>
    /// <param name="name">Name of the current state</param>
    public OrderBasedStateMachineState(IStateManagementDevice currentContext, int id, string name) : base(currentContext, id, name)
    {
    }
}