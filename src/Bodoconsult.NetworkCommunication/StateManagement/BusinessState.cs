// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Current implementation of <see cref="IBusinessSubState"/>
/// </summary>
public class BusinessState : IBusinessState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="id">ID of the substate</param>
    /// <param name="name">Cleartext name of the substate</param>
    public BusinessState(int id, string name)
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

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool Equals(IBusinessState other)
    {
        if (other == null)
        {
            return false;
        }

        return Id == other.Id;
    }
}