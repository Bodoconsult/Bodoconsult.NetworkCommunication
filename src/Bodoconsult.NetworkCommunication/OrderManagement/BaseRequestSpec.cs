// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement;

/// <summary>
/// Base class for request specs
/// </summary>
public class BaseRequestSpec : IRequestSpec
{
    private bool _wasSuccessful;
    private readonly Lock _wasSuccessfulObject = new();

    /// <summary>
    /// The currently processed request step of the order
    /// </summary>
    public IRequestStepProcessor CurrentRequestStepProcessor { get; set; }

    /// <summary>
    /// Clear text name of the request
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Current parameter set to use for the request
    /// </summary>
    public IParameterSet ParameterSet { get; protected set; }

    /// <summary>
    /// Command sent to the device
    /// </summary>
    public char Command { get; protected set; }

    /// <summary>
    /// Total calculated timeout for the answer(s) of an request in milliseconds
    /// </summary>
    public int Timeout { get; protected set; }


    /// <summary>
    /// The number of repeatings in case the step was not successful
    /// </summary>
    public int NumberOfRepeatsInCaseOfNoSuccess { get; protected set; } = 0;

    /// <summary>
    /// Is the request internal and does not send a message to the device
    /// </summary>
    public bool IsInternalRequest { get; protected set; }


    /// <summary>
    /// A object to transferred from a predecessing request spec to the the current one
    /// </summary>
    public object TransportObject { get; set; }

    /// <summary>
    /// A resulting object to transferred from this request spec to the next
    /// </summary>
    public object ResultTransportObject { get; set; }

    /// <summary>
    /// Requires transport object from predecessing request injected via <see cref="IRequestSpec.SetTransportObject"/>
    /// </summary>
    public bool RequiresTransportObject { get; protected set; }

    /// <summary>
    /// Do not run this request spec if there is the same order type in the queue already. Use for ExpressUnload currently
    /// </summary>
    public bool DoNotRunRequestSpecIfThereIsSameOrderTypeInQueue { get; protected set; }

    /// <summary>
    /// The expected handshake if the message was sent
    /// </summary>
    public IList<IOrderExecutionResultState> ExpectedHandshakeForSentMessage { get; protected set; } = new List<IOrderExecutionResultState> { OrderExecutionResultState.Successful};

    /// <summary>
    /// Does the request require only a (valid) handshake as answer to be successful
    /// </summary>
    public bool RequestRequiresOnlyAHandshakeAsAnswer { get; set; }

    /// <summary>
    /// A delegate for testing order executing. Not intended for production use
    /// </summary>
    public RequestAnswerStepIsStartedDelegate RequestAnswerStepIsStartedDelegate { get; set; }

    /// <summary>
    /// The messages to send. These messages are processed all in the same way
    /// defined by the request
    /// </summary>
    public IList<IOutboundDataMessage> SentMessage { get; } = new List<IOutboundDataMessage>();

    /// <summary>
    /// Represents a timeline of request answers
    /// </summary>
    public IList<IRequestAnswerStep> RequestAnswerSteps { get; } = new List<IRequestAnswerStep>();

    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    public bool WasSuccessful
    {
        get
        {
            lock (_wasSuccessfulObject)
            {
                return _wasSuccessful;
            }
        }
        set
        {
            lock (_wasSuccessfulObject)
            {
                _wasSuccessful = value;
            }
        }
    }

    /// <summary>
    /// Set  the <see cref="IRequestSpec.TransportObject"/> with an object from predecessing request
    /// </summary>
    /// <param name="transportObject">Object from predecessing request</param>
    public void SetTransportObject(object transportObject)
    {
        if (!RequiresTransportObject)
        {
            return;
        }

        TransportObject = transportObject;
    }

    /// <summary>
    /// Create all messages to process in the step. This message are processed all in the same way
    /// defined by the request
    /// </summary>
    public virtual void CreateMessagesToSend()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Calculate the total timeout and store it in <see cref="IRequestSpec.Timeout"/>
    /// </summary>
    public void CalculateTotalTimeout()
    {
        Timeout = 0;
        foreach (var steps in RequestAnswerSteps)
        {
            if (steps.Timeout < int.MaxValue && Timeout + steps.Timeout < int.MaxValue)
            {
                Timeout += steps.Timeout;
            }
            else
            {
                Timeout = int.MaxValue;
                return;
            }
        }
    }

    /// <summary>
    /// Check if the parameter is valid
    /// </summary>
    /// <param name="interfaceName">Name of the required interface name the parameter set has to implement</param>
    /// <exception cref="ArgumentException">Exception thrown if the parameterset is valid</exception>
    protected void CheckParameterSet(string interfaceName)
    {
        if (ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(ParameterSet));
        }

        var t = ParameterSet.GetType();

        if (t.GetInterface(interfaceName) == null)
        {
            throw new ArgumentException($"Wrong type of parameter set: should implement {interfaceName} but was {t.Name}");
        }

        var result = ParameterSet.IsValid;

        if (!result.Any())
        {
            return;
        }

        var s = ValidationHelper.FormatValidationErrors(result);

        throw new ArgumentException($"No valid parameter set: {s}");
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        ResultTransportObject = null;
        TransportObject = null;
        ParameterSet = null;

        //foreach (var requestAnswerStep in RequestAnswerSteps)
        //{
        //    requestAnswerStep.Dispose();
        //}

        //RequestAnswerSteps.Clear();

        SentMessage.Clear();
        CurrentRequestStepProcessor = null;
    }
}