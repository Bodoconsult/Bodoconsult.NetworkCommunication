// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for device request specifications
/// </summary>
public interface IRequestSpec: IDisposable
{
    /// <summary>
    /// Delegate to set the state for a <see cref="IRequestStepProcessor"/> instance
    /// </summary>
    RequestStepProcessorSetResultDelegate RequestStepProcessorSetResultDelegate { get; set; }

    /// <summary>
    /// Is the <see cref="IRequestStepProcessor"/> instance cancelled?
    /// </summary>
    RequestStepProcessorIsCancelledDelegate RequestStepProcessorIsCancelledDelegate { get; set; }

    /// <summary>
    /// Current app logger
    /// </summary>
    IAppLoggerProxy AppLogger { get; set; }

    /// <summary>
    /// Send an app notfication
    /// </summary>
    DoNotifyDelegate DoNotifyDelegate { get; set; }

    /// <summary>
    /// Delegate to cancel running operation on comm adapter level
    /// </summary>
    CancelRunningOperationDelegate CancelRunningOperationDelegate { get; set; }

    /// <summary>
    /// Clear text name of the request
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Current parameter set to use for the request
    /// </summary>
    IParameterSet ParameterSet { get; }

    /// <summary>
    /// Total calculated timeout for the answer(s) of a request in milliseconds
    /// </summary>
    int Timeout { get; }

    /// <summary>
    /// The number of repeatings in case the step was not successful
    /// </summary>
    int NumberOfRepeatsInCaseOfNoSuccess { get; }

    /// <summary>
    /// An object to transferred from a predecessing request spec to the current one
    /// </summary>
    object TransportObject { get; set; }

    /// <summary>
    /// A resulting object to transferred from this request spec to the next
    /// </summary>
    object ResultTransportObject { get; set; }

    /// <summary>
    /// Requires transport object from predecessing request injected via <see cref="SetTransportObject"/>
    /// </summary>
    bool RequiresTransportObject { get; }

    /// <summary>
    /// Do not run this request spec if there is the same order type in the queue already. Use for ExpressUnload currently
    /// </summary>
    bool DoNotRunRequestSpecIfThereIsSameOrderTypeInQueue { get; }

    /// <summary>
    /// The expected handshake if the message was sent
    /// </summary>
    List<IOrderExecutionResultState> ExpectedHandshakeForSentMessage { get; }

    /// <summary>
    /// Does the request require only a (valid) handshake as answer to be successful
    /// </summary>
    bool RequestRequiresOnlyAHandshakeAsAnswer { get; set; }

    /// <summary>
    /// Represents a timeline of request answers
    /// </summary>
    List<IRequestAnswerStep> RequestAnswerSteps { get; }

    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    bool WasSuccessful { get; set; }

    /// <summary>
    /// Set  the <see cref="TransportObject"/> with an object from predecessing request
    /// </summary>
    /// <param name="transportObject">Object from predecessing request</param>
    void SetTransportObject(object transportObject);

    /// <summary>
    /// Create all messages to process in the step. This message are processed all in the same way
    /// defined by the request
    /// </summary>
    void CreateMessagesToSend();

    /// <summary>
    /// Calculate the total timeout and store it in <see cref="Timeout"/>
    /// </summary>
    void CalculateTotalTimeout();

    #region For testing

    /// <summary>
    /// A delegate for testing order executing. Not intended for production use
    /// </summary>
    public RequestAnswerStepIsStartedDelegate RequestAnswerStepIsStartedDelegate { get; set; }

    /// <summary>
    /// Order logger ID
    /// </summary>
    string OrderLoggerId { get; set; }

    #endregion
}