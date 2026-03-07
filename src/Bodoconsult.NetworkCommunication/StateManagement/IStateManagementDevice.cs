// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Interface for state management devices 
/// </summary>
public interface IStateManagementDevice : IStateMachineContext, IOrderManagementDevice
{
    
}