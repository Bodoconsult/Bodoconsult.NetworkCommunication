// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerAuditLog : IDhcpServerAuditLog
{
    /// <summary>
    /// The associated DHCP Server
    /// </summary>
    public IDhcpServer Server { get; }

    /// <summary>
    /// The directory where the audit log is stored as an absolute path within the file system.
    /// </summary>
    public string AuditLogDirectory { get; }

    /// <summary>
    /// The disk check interval for attempting to write the audit log to the specified file as the number of logged DHCP server events that should occur between checks.
    /// </summary>
    public int DiskCheckInterval { get; }

    /// <summary>
    /// The maximum log file size, in bytes.
    /// </summary>
    public int MaxLogFilesSize { get; }

    /// <summary>
    /// The minimum required disk space, in bytes, for audit log storage.
    /// </summary>
    public int MinSpaceOnDisk { get; }

    private DhcpServerAuditLog(DhcpServer server, string auditLogDir, int diskCheckInterval, int maxLogFilesSize, int minSpaceOnDisk)
    {
        Server = server;
        AuditLogDirectory = auditLogDir;
        DiskCheckInterval = diskCheckInterval;
        MaxLogFilesSize = maxLogFilesSize;
        MinSpaceOnDisk = minSpaceOnDisk;
    }

    internal static DhcpServerAuditLog GetAuditLog(DhcpServer server)
    {
        var result = Api.DhcpAuditLogGetParams(serverIpAddress: server.Address,
            flags: 0,
            auditLogDir: out var auditLogDir,
            diskCheckInterval: out var diskCheckInterval,
            maxLogFilesSize: out var maxLogFilesSize,
            minSpaceOnDisk: out var minSpaceOnDisk);

        if (result != DhcpErrors.Success)
            throw new DhcpServerException(nameof(Api.DhcpAuditLogGetParams), result);

        return new DhcpServerAuditLog(server, auditLogDir, diskCheckInterval, maxLogFilesSize, minSpaceOnDisk);
    }
}