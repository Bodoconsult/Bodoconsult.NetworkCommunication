// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

public interface IJoblistUnloadProcessingState: IJobStateMachineState
{

    /// <summary>
    /// Name of the joblist
    /// </summary>
    public string JoblistName { get; set; }

}