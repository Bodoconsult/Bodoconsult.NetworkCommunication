// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for data logging for received byte messages
/// </summary>
public interface IDataLogger
{
    /// <summary>
    /// The directory path for the export target. Default: Path.GetTempPath();
    /// </summary>
    string TargetPath { get; set; }

    /// <summary>
    /// The plain filename for the export file without extension, timestamp etc.
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// Pattern for the full filename including timestamp etc.
    /// {0} FileName
    /// {1} Timestamp
    /// {2} FileExtension
    /// </summary>
    string FileNamePattern { get; set; }

    /// <summary>
    /// File extension to use for the export files without dot. Default: txt
    /// </summary>
    string FileExtension { get; set; }

    /// <summary>
    /// Start the logging
    /// </summary>
    void Start();

    /// <summary>
    /// Save all data and then stop the logging
    /// </summary>
    void Stop();

    /// <summary>
    /// Add an item to log
    /// </summary>
    /// <param name="data">Byte data to log</param>
    void Add(ReadOnlyMemory<byte> data);

}