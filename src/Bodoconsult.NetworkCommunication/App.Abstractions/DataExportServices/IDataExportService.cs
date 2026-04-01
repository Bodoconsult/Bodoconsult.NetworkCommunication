// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;

namespace Bodoconsult.NetworkCommunication.App.Abstractions.DataExportServices;

/// <summary>
/// Interface for generic data export services
/// </summary>
/// <typeparam name="T">Type of class to store in a file</typeparam>
public interface IDataExportService<in T> where T : class
{
    /// <summary>
    /// Encoding to use for string based exports like XML, JSON etc.
    /// </summary>
    Encoding Encoding { get; }

    /// <summary>
    /// Counts the rows since the service was started
    /// </summary>
    int RowCounter { get; }

    /// <summary>
    /// Maximum file size before rolling to next file. Default: 10 MB
    /// </summary>
    long MaxFileSize { get; set; }

    /// <summary>
    /// Cache size as number of T instances to cache before saving to file
    /// </summary>
    int CacheSize { get; set; }

    /// <summary>
    /// The directory path for the export target. Default: Path.GetTempPath();
    /// </summary>
    string? TargetPath { get; set; }

    /// <summary>
    /// The plain filename for the export file without extension, timestamp etc.
    /// </summary>
    string? FileName { get; set; }

    /// <summary>
    /// Pattern for the full filename including timestamp etc.
    /// {0} FileName
    /// {1} Timestamp
    /// {2} FileExtension
    /// </summary>
    string? FileNamePattern { get; set; }

    /// <summary>
    /// File extension to use for the export files without dot. Default: txt
    /// </summary>
    string? FileExtension { get; set; }

    /// <summary>
    /// The current file path the data are stored in
    /// </summary>
    string? CurrentFilePath { get; set; }

    /// <summary>
    /// Header data to add at the start of the file. Mainly intended for XML or JSON
    /// </summary>
    ReadOnlyMemory<byte>? HeaderData { get; set; }

    /// <summary>
    /// Footer data to add at the end of the file. Mainly intended for XML or JSON
    /// </summary>
    ReadOnlyMemory<byte>? FooterData { get; set; }

    /// <summary>
    /// Byte data separating tokens in the file. Default: null
    /// </summary>
    ReadOnlyMemory<byte>? TokenSeparatorData { get; set; }

    /// <summary>
    /// Create the current file path
    /// </summary>
    /// <returns>Current file path</returns>
    string CreateCurrentFilePath();

    /// <summary>
    /// Start the data export
    /// </summary>
    void Start();

    /// <summary>
    /// Save all data and then stop the data export
    /// </summary>
    void Stop();

    /// <summary>
    /// Add an item to store in the export file
    /// </summary>
    /// <param name="data"></param>
    void Add(T data);

    /// <summary>
    /// Converts an object of type T into a ReadOnlyMemory&lt;byte&gt; instance
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Thrown if type T is NOT string, ReadOnlyMemory&lt;byte&gt; or byte[]</exception>
    ReadOnlyMemory<byte> ToMemory(T data);
}