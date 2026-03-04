// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Business state interface
/// </summary>
public interface IBusinessState : IEquatable<IBusinessState>
{
    /// <summary>
    /// The ID of the business state
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The cleartext name of the business state
    /// </summary>
    string Name { get; }
}