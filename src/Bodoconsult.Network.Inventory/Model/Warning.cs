namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Defines a warning message
/// </summary>
public class Warning
{
    /// <summary>
    /// default ctor
    /// </summary>
    public Warning()
    {
        WarningSourceType = WarningSourceType.Hardware;
    }

    /// <summary>
    /// Source type of warning: hardware, software, security
    /// </summary>
    public WarningSourceType WarningSourceType { get; set; }

    /// <summary>
    /// Severity level of the warning
    /// </summary>
    public WarningSeverityLevel WarningSeverityLevel { get; set; }

    /// <summary>
    /// Warning message
    /// </summary>
    public string Message { get; set; }

}