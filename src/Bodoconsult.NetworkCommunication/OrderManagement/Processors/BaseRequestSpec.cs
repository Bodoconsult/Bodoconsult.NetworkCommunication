// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Base class for request specs
/// </summary>
public abstract class BaseRequestSpec : IRequestSpec
{
    private bool _wasSuccessful;
    private readonly Lock _wasSuccessfulObject = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="name">Request name</param>
    /// <param name="parameterSet">Current parameter set</param>
    protected BaseRequestSpec(string name, IParameterSet? parameterSet)
    {
        ParameterSet = parameterSet;
        Name = name;
        OrderLoggerId = "Order";
    }

    ///// <summary>
    ///// The currently processed request step of the order
    ///// </summary>
    //public IRequestStepProcessor CurrentRequestStepProcessor { get; set; }

    /// <summary>
    /// Delegate to set the state for a <see cref="IRequestStepProcessor"/> instance
    /// </summary>
    public RequestStepProcessorSetResultDelegate? RequestStepProcessorSetResultDelegate { get; set; }

    /// <summary>
    /// Is the <see cref="IRequestStepProcessor"/> instance cancelled?
    /// </summary>
    public RequestStepProcessorIsCancelledDelegate? RequestStepProcessorIsCancelledDelegate { get; set; }

    /// <summary>
    /// Current app logger
    /// </summary>
    public IAppLoggerProxy? AppLogger { get; set; }

    ///// <summary>
    ///// Send an app notfication
    ///// </summary>
    //public DoNotifyDelegate DoNotifyDelegate { get; set; }

    /// <summary>
    /// Delegate to cancel running operation on comm adapter level
    /// </summary>
    public CancelRunningOperationDelegate? CancelRunningOperationDelegate { get; set; }

    /// <summary>
    /// Clear text name of the request
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Current parameter set to use for the request
    /// </summary>
    public IParameterSet? ParameterSet { get; private set; }

    /// <summary>
    /// Order logger ID
    /// </summary>
    public string OrderLoggerId { get; set; }

    /// <summary>
    /// Total calculated timeout for the answer(s) of a request in milliseconds
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
    /// An object to transferred from a predecessing request spec to the current one
    /// </summary>
    public object? TransportObject { get; set; }

    /// <summary>
    /// A resulting object to transferred from this request spec to the next
    /// </summary>
    public object? ResultTransportObject { get; set; }

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
    public List<IOrderExecutionResultState> ExpectedHandshakeForSentMessage { get; protected set; } = [OrderExecutionResultState.Successful];

    /// <summary>
    /// Does the request require only a (valid) handshake as answer to be successful
    /// </summary>
    public bool RequestRequiresOnlyAHandshakeAsAnswer { get; set; }

    /// <summary>
    /// A delegate for testing order executing. Not intended for production use
    /// </summary>
    public RequestAnswerStepIsStartedDelegate? RequestAnswerStepIsStartedDelegate { get; set; }

    /// <summary>
    /// Represents a timeline of request answers
    /// </summary>
    public List<IRequestAnswerStep> RequestAnswerSteps { get; } = new();

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
    public void SetTransportObject(object? transportObject)
    {
        if (!RequiresTransportObject)
        {
            return;
        }

        TransportObject = transportObject;
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
    public virtual void Dispose()
    {
        ResultTransportObject = null;
        TransportObject = null;
        ParameterSet = null;
    }
}