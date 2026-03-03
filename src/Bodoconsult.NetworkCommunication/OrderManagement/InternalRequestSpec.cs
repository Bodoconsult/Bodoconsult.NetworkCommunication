// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement;

/// <summary>
/// Request spec doing an internal action
/// </summary>
public class InternalRequestSpec : BaseRequestSpec, IInternalRequestSpec
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="parameterSet">Current parameter set</param>
    public InternalRequestSpec(IParameterSet parameterSet) : base(parameterSet)
    {
    }
}