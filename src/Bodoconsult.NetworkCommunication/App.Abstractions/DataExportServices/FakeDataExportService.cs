// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;

namespace Bodoconsult.NetworkCommunication.App.Abstractions.DataExportServices;

/// <summary>
/// Fake implementation of IDataExportService&lt;byte[]&gt;
/// </summary>
public class FakeDataExportService : IDataExportService<byte[]>
{
    /// <summary>
    /// Data were logged
    /// </summary>
    public bool WasLogged { get; set; }

    /// <summary>
    /// Encoding to use for string based exports like XML, JSON etc.
    /// </summary>
    public Encoding Encoding => Encoding.UTF8;

    /// <summary>
    /// Counts the rows since the service was started
    /// </summary>
    public int RowCounter { get; set; }

    /// <summary>
    /// Maximum file size before rolling to next file. Default: 10 MB
    /// </summary>
    public long MaxFileSize { get; set; }

    /// <summary>
    /// Cache size as number of T instances to cache before saving to file
    /// </summary>
    public int CacheSize { get; set; }

    /// <summary>
    /// The directory path for the export target. Default: Path.GetTempPath();
    /// </summary>
    public string? TargetPath { get; set; }

    /// <summary>
    /// The plain filename for the export file without extension, timestamp etc.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Pattern for the full filename including timestamp etc.
    /// {0} FileName
    /// {1} Timestamp
    /// {2} FileExtension
    /// </summary>
    public string? FileNamePattern { get; set; }

    /// <summary>
    /// File extension to use for the export files without dot. Default: txt
    /// </summary>
    public string? FileExtension { get; set; }

    /// <summary>
    /// The current file path the data are stored in
    /// </summary>
    public string? CurrentFilePath { get; set; }

    /// <summary>
    /// Header data to add at the start of the file. Mainly intended for XML or JSON
    /// </summary>
    public ReadOnlyMemory<byte>? HeaderData { get; set; }

    /// <summary>
    /// Footer data to add at the end of the file. Mainly intended for XML or JSON
    /// </summary>
    public ReadOnlyMemory<byte>? FooterData { get; set; }

    /// <summary>
    /// Byte data separating tokens in the file. Default: null
    /// </summary>
    public ReadOnlyMemory<byte>? TokenSeparatorData { get; set; }

    /// <summary>
    /// Create the current file path
    /// </summary>
    /// <returns>Current file path</returns>
    public string CreateCurrentFilePath()
    {
        return $"{CurrentFilePath}\\data.log";
    }

    /// <summary>
    /// Start the data export
    /// </summary>
    public void Start()
    {
        // Do nothing
    }

    /// <summary>
    /// Save all data and then stop the data export
    /// </summary>
    public void Stop()
    {
        // Do nothing
    }

    /// <summary>
    /// Add an item to store in the export file
    /// </summary>
    /// <param name="data"></param>
    public void Add(byte[] data)
    {
        WasLogged = true;
    }

    /// <summary>
    /// Converts an object of type T into a ReadOnlyMemory&lt;byte&gt; instance
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Thrown if type T is NOT string, ReadOnlyMemory&lt;byte&gt; or byte[]</exception>
    public ReadOnlyMemory<byte> ToMemory(byte[] data)
    {
        return data.AsMemory();
    }
}