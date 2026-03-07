// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

public interface IStateMachineStateFactory
{

    /// <summary>
    /// Current context
    /// </summary>
    ITowerServer CurrentContext { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IStateMachineState CreateInstance(string stateName);

}