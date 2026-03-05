// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for internal request specifications
/// </summary>
public interface IInternalRequestSpec: IRequestSpec
{

    /// <summary>
    /// Represents a timeline of request answers
    /// </summary>
    List<IRequestAnswerStep> RequestAnswerSteps { get; }
}