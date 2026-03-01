// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement;

/// <summary>
/// Current implementation of <see cref="IBusinessSubState"/>
/// </summary>
public class BusinessSubState : IBusinessSubState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="id">ID of the substate</param>
    /// <param name="name">Cleartext name of the substate</param>
    public BusinessSubState(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// ID of the substate
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Cleartext name of the substate
    /// </summary>
    public string Name { get; }
}