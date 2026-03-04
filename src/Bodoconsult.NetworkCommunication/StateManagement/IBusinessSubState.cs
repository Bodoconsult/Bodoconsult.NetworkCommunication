// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Business substate interface
/// </summary>
public interface IBusinessSubState :  IEquatable<IBusinessSubState>
{
    /// <summary>
    /// The ID of the business substate
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The cleartext name of the business substate
    /// </summary>
    string Name { get; }
}