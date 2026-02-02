// Copyright (c) Mycronic. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// A delegate for testing order executing. Not intended for production use
/// </summary>
public delegate bool RequestAnswerStepIsStartedDelegate();

/// <summary>
/// This interface is for representing process related
/// to the tower 
/// </summary>
public interface IOrder : IDisposable
{


    #region Order meta data

    /// <summary>
    /// Unique Order-ID
    /// </summary>
    long Id { get; }

    /// <summary>
    /// The next order in a chain of orders like trial run
    /// </summary>
    IOrder NextOrder { get; set; }

    /// <summary>
    /// Name of the order
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Type name of the order
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// Type of order sent to client
    /// </summary>
    int OrderTypeCode { get; set; }

    /// <summary>
    /// Order source
    /// </summary>
    OrderSource OrderSource { get; }

    /// <summary>
    /// The UID of a source item like a joblist or a trial run the order is bound to
    /// </summary>
    Guid? SourceUid { get; set; }


    /// <summary>
    /// Current ID to log 
    /// </summary>
    string LoggerId { get; }


    /// <summary>
    /// Current request specs of the order
    /// </summary>
    IList<IRequestSpec> RequestSpecs { get; }

    /// <summary>
    /// The names of allowed order types for running parallel to the current order
    /// </summary>
    IList<string> AllowedParallelOrderTypes { get; }

    /// <summary>
    /// Send a CAN command to the tower if order fails or is cancelled
    /// </summary>
    bool SendCancelToTowerIfCancelledOrUnsuccessful { get; set; }

    // ToDo: RL: remove it IsTowerHardwareInitRequiredOnHardwareError as it is not needed anymor

    ///// <summary>
    ///// Does the order require a tower hardware init in case on a hardware error
    ///// </summary>
    //bool IsTowerHardwareInitRequiredOnHardwareError { get; }

    /// <summary>
    /// Action running after successful processing of the order
    /// </summary>
    ActionRequestStepDelegate OrderFinishedSuccessfulAction { get; }

    /// <summary>
    /// Action running after unsuccessful processing of the order
    /// </summary>
    ActionRequestStepDelegate OrderFinishedUnsuccessfulAction { get; }

    /// <summary>
    /// The state to notify the app in case of order was successfully running
    /// </summary>
    IDeviceState StateToNotifyOnSuccess { get; }

    /// <summary>
    /// Is the order running in sync mode? Default: false means async
    /// </summary>
    bool IsRunningSync { get; set; }

    /// <summary>
    /// Is the order ready for disposing
    /// </summary>
    bool IsDisposable { get; set; }

    /// <summary>
    /// If client notification is turned off, the client will not be informed on this order
    /// </summary>
    bool IsClientNotificationTurnedOff { get; }

    /// <summary>
    /// Start the next order of the same type (used for ExpressUnload i.e.)
    /// </summary>
    bool IsNextOrderStartAllowed { get; set; }

    /// <summary>
    /// If the order fails a ComDevClose has to be initialized
    /// </summary>
    bool FailingOrderRequiresComDevClose { get; set; }

    /// <summary>
    /// Should the order be cancelled on a ComDevClose. Valid only for normal orders. Priority orders to not pay attention to this field
    /// </summary>
    bool IsCancelledOnComDevClose  { get; set; }

    /// <summary>
    /// Should the order be cancelled on a tower properties update
    /// </summary>
    bool IsCancelledOnTowerPropertyUpdate  { get; set; }

    ///// <summary>
    ///// A list of tower error codes which should should not influence the order running
    ///// </summary>
    //List<byte> TowerErrorCodesToIgnore { get; }

    #endregion

    #region Processing information

    /// <summary>
    /// Is the order a high priority order
    /// </summary>
    bool IsHighPriorityOrder { get; set; }

    /// <summary>
    /// Is this order prioritized when receiving messages
    /// </summary>
    bool IsCheckedWithPriority { get; set; }

    /// <summary>
    /// Is the order already finished
    /// </summary>
    bool IsFinished { get; set; }

    /// <summary>
    /// Current runtime parameter set to use for the order. Must include the parameter sets of all contained requests by interface.
    /// </summary>
    IParameterSet ParameterSet { get; }

    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    bool WasSuccessful { get; set; }

    /// <summary>
    /// The order is cancelled
    /// </summary>
    bool IsCancelled { get; set; }

    /// <summary>
    /// An object to lock the property <see cref="IsCancelled"/>
    /// </summary>
    object IsCancelledLockObject { get; }


    /// <summary>
    /// Order processing start time
    /// </summary>
    DateTime StartTime { get; set; }

    /// <summary>
    /// Order processing end time
    /// </summary>
    DateTime EndTime { get; set; }

    /// <summary>
    /// The execution result of the order
    /// </summary>
    IOrderExecutionResultState ExecutionResult { get; set; }

    /// <summary>
    /// Current execution state
    /// </summary>
    IOrderState ExecutionState  { get; set; }

    /// <summary>
    /// ErrorCode to trace
    /// </summary>
    public int OrderError { get; set; }

    #endregion


    #region Tracing

    /// <summary>
    /// ID of a not OM related transaction the order is part of. Used for trialrun steps
    /// </summary>
    int TransactionId { get; set; }

    /// <summary>
    /// Current device ID
    /// </summary>
    string DeviceId { get; }

    /// <summary>
    /// Trace code to log on success
    /// </summary>
    int TraceCodeSuccess { get; }

    /// <summary>
    /// Trace code to log on error
    /// </summary>
    int TraceCodeError { get; }

    /// <summary>
    /// Trace message to log
    /// </summary>
    string TraceMessage { get; }

    /// <summary>
    /// Trace entry to create for the order
    /// </summary>
    ITraceEntry TraceEntry { get; set; }


    /// <summary>
    /// Create a trace entry (do not use directly, public only for unit testing)
    /// </summary>
    /// <param name="lastStepExecutionResult">Execution result</param>
    /// <param name="communicationAdapterError">Error on the communication adapter</param>
    void CreateTraceEntry(IOrderExecutionResultState lastStepExecutionResult, in int communicationAdapterError);
        
    /// <summary>
    /// Benchmark object (see output in StSys_Benchmark.csv)
    /// Make sure to create it, addStep, and dispose it 
    /// </summary>
    Bench Benchmark { get; set; }

    #endregion

}