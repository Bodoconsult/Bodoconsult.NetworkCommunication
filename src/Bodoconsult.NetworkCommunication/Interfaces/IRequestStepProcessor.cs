// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for executing request steps of a device request
/// </summary>
public interface IInternalRequestStepProcessor : IRequestStepProcessor
{
    /// <summary>
    /// Currentinternal request spec
    /// </summary>
    IInternalRequestSpec InternalRequestSpec { get; }
}

/// <summary>
/// Interface for executing request steps of a device request
/// </summary>
public interface IRequestStepProcessor : IDisposable
{
    /// <summary>
    /// Current execution result
    /// </summary>
    IOrderExecutionResultState Result { get; set; }

    /// <summary>
    /// Current request spec to use for the processor
    /// </summary>
    IRequestSpec RequestSpec { get; set; }

    /// <summary>
    /// Is the request processor cancelled
    /// </summary>
    bool IsCancelled { get; }



    /// <summary>
    /// Execute the request
    /// </summary>
    /// <returns>Execution result</returns>
    IOrderExecutionResultState ExecuteRequest();

    /// <summary>
    /// Cancel the current request step processor
    /// </summary>
    void Cancel();

    /// <summary>
    /// Set the result state
    /// </summary>
    /// <param name="state">State to set as result</param>
    void SetResult(IOrderExecutionResultState state);

    /// <summary>
    /// Check if cancelled
    /// </summary>
    /// <returns>True if cancelled else false</returns>
    bool CheckIsCancelled();
}