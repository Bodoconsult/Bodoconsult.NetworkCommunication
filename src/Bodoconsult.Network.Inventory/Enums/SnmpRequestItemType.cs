namespace Bodoconsult.Inventory.Enums;

/// <summary>
/// Type of request to map from SNMP to WMI data
/// </summary>
public enum SnmpRequestItemType
{
    Name,
    Caption,
    Model,
    Manufacturer,
    SerialNumber,
    TemperaturStatus,
    SystemStatus,
    Hostname,
    OperatingSystem,
    DomainRole
}