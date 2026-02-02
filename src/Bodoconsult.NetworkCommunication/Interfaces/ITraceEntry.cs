// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for tracing events
/// </summary>
public interface ITraceEntry
{
    /// <summary>
    /// Primary key column with unique values
    /// </summary>
    Guid Uid { get; set; }

    /// <summary>
    /// Trace date
    /// </summary>
    public DateTime TraceDate { get; set; } 

    /// <summary>
    /// Trace message content
    /// </summary>
    public string Message{ get; set; }

    /// <summary>
    /// Message code for identifying different types of traces
    /// </summary>
    public int MessageCode { get; set; }
}