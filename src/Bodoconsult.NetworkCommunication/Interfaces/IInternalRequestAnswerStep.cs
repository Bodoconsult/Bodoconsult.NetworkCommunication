// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for defining an internal request answer step
/// </summary>
public interface IInternalRequestAnswerStep : IRequestAnswerStep
{
    /// <summary>
    /// The internal request spec instance
    /// </summary>
    IInternalRequestSpec InternalRequestSpec { get; }
}