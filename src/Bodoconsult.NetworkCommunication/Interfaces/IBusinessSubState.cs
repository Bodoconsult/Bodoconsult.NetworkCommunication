// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for business substates
/// </summary>
public interface IBusinessSubState: IEquatable<IBusinessSubState>
{
    /// <summary>
    /// The ID of the state
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The cleartext name of the state
    /// </summary>
    string Name { get; }
}