// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for tower request specifications
/// </summary>
public interface IRequestSpec: IDisposable
{
    /// <summary>
    /// The currently processed request step of the order
    /// </summary>
    IRequestStepProcessor CurrentRequestStepProcessor { get; set; }

    /// <summary>
    /// Clear text name of the request
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Current parameter set to use for the request
    /// </summary>
    IParameterSet ParameterSet { get; }

    /// <summary>
    /// Command sent to the tower
    /// </summary>
    char Command { get; }

    /// <summary>
    /// Total calculated timeout for the answer(s) of an request in milliseconds
    /// </summary>
    int Timeout { get; }

    /// <summary>
    /// The number of repeatings in case the step was not successful
    /// </summary>
    int NumberOfRepeatsInCaseOfNoSuccess { get; }

    /// <summary>
    /// Is the request internal and does not send a message to the tower
    /// </summary>
    bool IsInternalRequest { get; }
        
    /// <summary>
    /// A object to transferred from a predecessing request spec to the the current one
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
    /// The messages to send. This message are processed all in the same way
    /// defined by the request
    /// </summary>
    IList<ICommandDataMessage> SentMessage { get; }

    /// <summary>
    /// The expected handshake if the message was sent
    /// </summary>
    IList<IOrderExecutionResultState> ExpectedHandshakeForSentMessage { get; }

    /// <summary>
    /// Does the request require only a (valid) handshake as answer to be successful
    /// </summary>
    bool RequestRequiresOnlyAHandshakeAsAnswer { get; set; }

    /// <summary>
    /// Represents a timeline of request answers
    /// </summary>
    IList<IRequestAnswerStep> RequestAnswerSteps { get; }


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

    #endregion
}