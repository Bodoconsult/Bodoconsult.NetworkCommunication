// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Interface for states resulting from jobs
/// </summary>
public interface IJobStateMachineState: IStateMachineState
{
    /// <summary>
    /// The UID of a source item like a joblist or a trial run the order is bound to
    /// </summary>
    Guid? SourceUid { get; set; }
}