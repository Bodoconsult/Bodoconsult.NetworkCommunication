// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// Current implementation of <see cref="IOrderBasedActionStateMachineState"/>
/// </summary>
public class JobStateMachineState : BaseOrderBasedStateMachineState, IJobStateMachineState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="id">ID of the current state</param>
    /// <param name="name">Name of the current state</param>
    public JobStateMachineState(IStateMachineDevice currentContext, int id, string name) : base(currentContext, id, name)
    { }

    /// <summary>
    /// The UID of a source item like a joblist or a trial run the order is bound to
    /// </summary>
    public Guid? SourceUid { get; set; }
}