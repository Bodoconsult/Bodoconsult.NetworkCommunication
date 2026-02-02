// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface order execution result states
/// </summary>
public interface IOrderExecutionResultState
{
    /// <summary>
    /// The ID of the state
    /// </summary>
    int Id { get;  }

    /// <summary>
    /// The cleartext name of the state
    /// </summary>
    string Name { get; set; }
}