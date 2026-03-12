// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Business substate
/// </summary>
public class BusinessSubState : IBusinessSubState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public BusinessSubState(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// The ID of the state
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The cleartext name of the state
    /// </summary>
    public string Name { get; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{Id} {Name}";

    protected bool Equals(BusinessSubState other)
    {
        return Id == other.Id && Name == other.Name;
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool Equals(IBusinessSubState? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other.GetType() == GetType() && Equals((BusinessSubState)other);
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object? obj)
    {
        return Equals((IBusinessSubState?)obj);
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}