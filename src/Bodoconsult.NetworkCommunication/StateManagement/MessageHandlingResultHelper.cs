// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Helper class to create <see cref="MessageHandlingResult"/> instances
/// </summary>
public static class MessageHandlingResultHelper
{
    /// <summary>
    /// Current app logger
    /// </summary>
    public static IAppLoggerProxy AppLogger { get; set; }

    #region Result handling

    /// <summary>
    /// Result: Error
    /// </summary>
    /// <param name="message">Message or null</param>
    /// <returns><see cref="MessageHandlingResult"/> instance</returns>
    public static MessageHandlingResult Error(string message = null)
    {
        if (!string.IsNullOrEmpty(message))
        {
            AppLogger?.LogError($"Error: {message}");
        }

        return new MessageHandlingResult
        {
            Error = 1,
            ExecutionResult = OrderExecutionResultState.Error,
            ErrorDescription = message
        };
    }

    /// <summary>
    /// Result: NotProcessed
    /// </summary>
    /// <param name="message">Message or null</param>
    /// <returns><see cref="MessageHandlingResult"/> instance</returns>
    public static MessageHandlingResult NotProcessed(string message = null)
    {
        if (!string.IsNullOrEmpty(message))
        {
            AppLogger?.LogError($"NotProcessed: {message}");
        }

        return new MessageHandlingResult
        {
            Error = 1,
            ExecutionResult = OrderExecutionResultState.NotProcessed,
            ErrorDescription = message
        };
    }

    /// <summary>
    /// Result: Unsuccessful
    /// </summary>
    /// <param name="message">Message or null</param>
    /// <returns><see cref="MessageHandlingResult"/> instance</returns>
    public static MessageHandlingResult Unsuccessful(string message = null)
    {
        if (!string.IsNullOrEmpty(message))
        {
            AppLogger?.LogError($"Unsuccessful: {message}");
        }

        return new MessageHandlingResult
        {
            Error = 1,
            ExecutionResult = OrderExecutionResultState.Unsuccessful,
            ErrorDescription = message
        };
    }

    /// <summary>
    /// Result: Successful
    /// </summary>
    /// <param name="message">Message or null</param>
    /// <returns><see cref="MessageHandlingResult"/> instance</returns>
    public static MessageHandlingResult Success(string message = null)
    {
        return new MessageHandlingResult
        {
            Error = 0,
            ExecutionResult = OrderExecutionResultState.Successful,
            ErrorDescription = message ?? ""
        };
    }

    #endregion
}