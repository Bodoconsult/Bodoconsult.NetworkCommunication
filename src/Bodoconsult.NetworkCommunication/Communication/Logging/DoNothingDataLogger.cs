// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Logging;

/// <summary>
/// <see cref="IDataLogger"/> implementation doing nothing. Intended mainly for testing
/// </summary>
public class DoNothingDataLogger: IDataLogger
{
    /// <summary>
    /// The directory path for the export target. Default: Path.GetTempPath();
    /// </summary>
    public string TargetPath { get; set; }

    /// <summary>
    /// The plain filename for the export file without extension, timestamp etc.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Pattern for the full filename including timestamp etc.. Default: "{0}_{1}.{2}";
    /// {0} FileName
    /// {1} Timestamp
    /// {2} FileExtension
    /// </summary>
    public string FileNamePattern { get; set; } = "{0}_{1}.{2}";

    /// <summary>
    /// File extension to use for the export files without dot. Default: txt
    /// </summary>
    public string FileExtension { get; set; } = "txt";

    /// <summary>
    /// Start the logging
    /// </summary>
    public void Start()
    {
        // Do nothing
    }

    /// <summary>
    /// Save all data and then stop the logging
    /// </summary>
    public void Stop()
    {
        // Do nothing
    }

    /// <summary>
    /// Add an item to log
    /// </summary>
    /// <param name="data">Byte data to log</param>
    public void Add(ReadOnlyMemory<byte> data)
    {
        // Do nothing
    }
}