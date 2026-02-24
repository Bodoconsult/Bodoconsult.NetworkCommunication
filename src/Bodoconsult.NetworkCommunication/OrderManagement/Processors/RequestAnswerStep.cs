// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Defines a request step
/// </summary>
public class RequestAnswerStep : BaseRequestAnswerStep
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestSpec">The request spec this object is bound to</param>
    public RequestAnswerStep(IRequestSpec requestSpec) : base(requestSpec)
    { }
}