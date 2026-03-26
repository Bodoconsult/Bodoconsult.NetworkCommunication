// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for states performing an order based action resulting from jobs
/// </summary>
public interface IJobStateMachineState: IOrderBasedActionStateMachineState
{
    /// <summary>
    /// The UID of a source item like a joblist or a trial run the order is bound to
    /// </summary>
    Guid? SourceUid { get; set; }
}