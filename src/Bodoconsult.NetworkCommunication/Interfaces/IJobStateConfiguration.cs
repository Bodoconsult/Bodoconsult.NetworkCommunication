// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Configuration for a <see cref="IJobStateMachineState"/>. State machine state configurations have to set per device
/// </summary>
public interface IJobStateConfiguration : IOrderBasedActionStateConfiguration
{
    /// <summary>
    /// The UID of a source item like a joblist or a trial run the order is bound to
    /// </summary>
    Guid? SourceUid { get; set; }
}