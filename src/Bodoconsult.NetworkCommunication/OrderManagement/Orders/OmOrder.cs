// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Orders;

/// <summary>
/// Order management order
/// </summary>
public class OmOrder : IOrder
{
    private string _name;
    private string _loggerId;
    private bool _isCancelled;
    private readonly Lock _isCancelledObject = new();

    /// <summary>
    /// Base class default ctor
    /// </summary>
    public OmOrder(long id, string name, IParameterSet parameterSet)
    {
        ParameterSet = parameterSet;
        Id = id;
        Name = name;
        TypeName = Name;
        ParameterSet.LoadOrder(this);
    }

    #region New TOM

    #region Order meta data

    /// <summary>
    /// Unique Order-ID
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// The next order in a chain of orders like trial run
    /// </summary>
    public IOrder NextOrder { get; set; }

    /// <summary>
    /// Name of the order
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _name = TypeName;
                _loggerId = $"{_name} ID {Id}: ";
                return;
            }

            if (_name == value)
            {
                return;
            }

            _name = value;

            _loggerId = _name == TypeName ? $"{_name} ID {Id}: " : $"{TypeName} ID {Id} Name {_name}: ";
        }
    }

    /// <summary>
    /// Type name of the order
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Order source
    /// </summary>
    public OrderSource OrderSource { get; protected set; }

    /// <summary>
    /// The UID of a source item like a joblist the order is bound to
    /// </summary>
    public Guid? SourceUid { get; set; }

    /// <summary>
    /// Type of order sent to client
    /// </summary>
    public int OrderTypeCode { get; set; }

    /// <summary>
    /// Formatted ID for logging
    /// </summary>
    public virtual string LoggerId => _loggerId;

    /// <summary>
    /// Current request specs of the order
    /// </summary>
    public IList<IRequestSpec> RequestSpecs { get; } = new List<IRequestSpec>();

    /// <summary>
    /// The names of allowed order types for running parallel to the current order
    /// </summary>
    public IList<string> AllowedParallelOrderTypes { get; } = new List<string>();

    /// <summary>
    /// Send a CAN command to the device if order fails or is cancelled
    /// </summary>
    public bool SendCancelTodeviceIfCancelledOrUnsuccessful { get; set; }

    ///// <summary>
    ///// Does the order require a device hardware init in case on a hardware error
    ///// </summary>
    //public bool IsdeviceHardwareInitRequiredOnHardwareError { get; set; }

    /// <summary>
    /// Action running after successful processing of the order
    /// </summary>
    public ActionRequestStepDelegate OrderFinishedSuccessfulAction { get; protected set; }

    /// <summary>
    /// Action running after unsuccessful processing of the order
    /// </summary>
    public ActionRequestStepDelegate OrderFinishedUnsuccessfulAction { get; protected set; }

    /// <summary>
    /// The state to notify the app in case of order was successfully running
    /// </summary>
    public IDeviceState StateToNotifyOnSuccess { get; protected set; }

    /// <summary>
    /// Is the order running in sync mode? Default: false means async
    /// </summary>
    public bool IsRunningSync { get; set; }

    /// <summary>
    /// Is the order ready for disposing
    /// </summary>
    public bool IsDisposable { get; set; }

    /// <summary>
    /// If client notification is turned off, the client will not be informed on this order
    /// </summary>
    public bool IsClientNotificationTurnedOff { get; protected set; }

    /// <summary>
    /// Start the next order of the same type (used for ExpressUnload i.e.)
    /// </summary>
    public bool IsNextOrderStartAllowed { get; set; }

    /// <summary>
    /// If the orders fail, a ComDevCose is required
    /// </summary>
    public bool FailingOrderRequiresComDevClose { get; set; }

    /// <summary>
    /// Should the order be cancelled on a ComDevClose. Valid only for normal orders. Priority orders to not pay attention to this field
    /// </summary>
    public bool IsCancelledOnComDevClose { get; set; }

    /// <summary>
    /// Should the order be cancelled on a device properties update
    /// </summary>
    public bool IsCancelledOndevicePropertyUpdate { get; set; }


    protected string GetFormattedUser()
    {
        return string.IsNullOrEmpty(ParameterSet.UserRequestingTheOrderRun) ?
            "" :
            ParameterSet.UserRequestingTheOrderRun;
    }

    #endregion

    #region Processing information

    /// <summary>
    /// Is the order a high priority order
    /// </summary>
    public bool IsHighPriorityOrder { get; set; }

    /// <summary>
    /// Is this order prioritized when receiving messages
    /// </summary>
    public bool IsCheckedWithPriority { get; set; }

    /// <summary>
    /// Current parameter set to use for the order. Must include the parameter sets of all contained requests by interface.
    /// </summary>
    public IParameterSet ParameterSet { get; set; }

    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    public bool WasSuccessful { get; set; }

    /// <summary>
    /// The order is cancelled
    /// </summary>
    public bool IsCancelled
    {
        get
        {
            lock (_isCancelledObject)
            {
                return _isCancelled;
            }
        }
        set
        {
            lock (_isCancelledObject)
            {
                _isCancelled = value;
            }
        }
    }

    /// <summary>
    /// An object to lock the property <see cref="IOrder.IsCancelled"/>
    /// </summary>
    public object IsCancelledLockObject { get; set; }

    /// <summary>
    /// Order processing start time
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Order processing end time
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// The execution result of the order
    /// </summary>
    public IOrderExecutionResultState ExecutionResult { get; set; } = OrderExecutionResultState.Unsuccessful;

    /// <summary>
    /// Current execution state
    /// </summary>
    public IOrderState ExecutionState { get; set; } = OrderState.Unknown; // State unknown means order is created but not given to TOM

    /// <summary>
    /// Is the order already finished
    /// </summary>
    public bool IsFinished { get; set; }

    #endregion


    #region Tracing

    /// <summary>
    /// ID of a not TOM related transaction the order is part of. Used for trialrun
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Current device ID
    /// </summary>
    public string DeviceId { get; set; }

    /// <summary>
    /// Trace code to log on success
    /// </summary>
    public int TraceCodeSuccess { get; set; }

    /// <summary>
    /// Trace code to log on error
    /// </summary>
    public int TraceCodeError { get; set; }

    /// <summary>
    /// Trace message to log
    /// </summary>
    public string TraceMessage { get; set; }

    /// <summary>
    /// ErrorCode to trace
    /// </summary>
    public int OrderError { get; set; }

    /// <summary>
    /// Trace entry to create for the order
    /// </summary>
    public ITraceEntry TraceEntry { get; set; }

    /// <summary>
    /// Create a trace entry (do not use directly, public only for unit testing)
    /// </summary>
    /// <param name="lastStepExecutionResult">Execution result</param>
    /// <param name="communicationAdapterError">Error on the communication adapter</param>
    /// <param name="timeStamp">App date service</param>
    public virtual void CreateTraceEntry(IOrderExecutionResultState lastStepExecutionResult, in int communicationAdapterError, DateTime timeStamp)
    {
        if (lastStepExecutionResult.Id == OrderExecutionResultState.Successful.Id)
        {
            TraceEntry.MessageCode = TraceCodeSuccess;
            TraceEntry.Message = TraceMessage;
        }
        else
        {
            TraceEntry.MessageCode = TraceCodeError;
            TraceEntry.Message = communicationAdapterError > 0 ? $"{TraceMessage}: Result: {lastStepExecutionResult} Error: {communicationAdapterError}" : "";
        }

        TraceEntry.TraceDate = timeStamp;
        //TraceEntry.Uid = GetFormattedUser();
    }

    /// <summary>
    /// This order will be benchmarked? True or false
    /// </summary>
    public bool IsBenchable { get; set; } = false;

    /// <summary>
    /// Benchmark object (see output in Benchmark.csv)
    /// Make sure to create it, addStep, and dispose it 
    /// </summary>
    public Bench Benchmark { get; set; }


    #endregion

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public virtual void Dispose()
    {
        TraceEntry = null;
        TraceMessage = null;


        foreach (var requestSpec in RequestSpecs)
        {
            requestSpec.Dispose();
        }

        RequestSpecs.Clear();

        ParameterSet.Dispose();
        ParameterSet = null;

        OrderFinishedUnsuccessfulAction = null;
        OrderFinishedSuccessfulAction = null;

        NextOrder = null;

        Benchmark?.Dispose();
    }

    #endregion

}