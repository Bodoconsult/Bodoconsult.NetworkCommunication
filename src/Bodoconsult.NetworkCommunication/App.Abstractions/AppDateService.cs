// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Concrete implementation returning system DateTime
/// </summary>
public class AppDateService : IAppDateService
{
    /// <summary>
    /// Factory method for <see cref="AppDateService"/>
    /// </summary>
    /// <returns>Fresh instance of <see cref="AppDateService"/></returns>
    public IAppDateService CreateInstance() {
        var dts = new AppDateService();
        return dts;
    }

    /// <summary>
    /// Minimum date MS Access can handle
    /// </summary>
    public readonly DateTime AccessMinDate = new(1900, 1, 1);

    private readonly DateTime _noDateGivenDate = DateTime.Now;

    private readonly Lock _lock = new();

    /// <summary>
    /// return current date and time
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// return the current date only
    /// </summary>
    public DateTime Today => DateTime.Today;

    /// <summary>
    /// Get a valid MS Access datetime value. Values before 1/1/1900 are set to this value
    /// </summary>
    /// <param name="date"></param>
    /// <returns>Valid MS Access datetime value</returns>
    public DateTime GetValidAccessDate(DateTime? date)
    {

        // No date given: use current date
        if (!date.HasValue)
        {
            return _noDateGivenDate;
        }

        // Dates below 1900/1/1 cannot be handled by MS Access
        return date.Value >= AccessMinDate ? date.Value : AccessMinDate;
    }
        
    /// <summary>
    /// Get the number of ticks from the beginning of time. Only one access per time possible!
    /// </summary>
    /// <returns>Ticks from the beginning of time</returns>
    public long GetCurrentTicks()
    {
        lock (_lock)
        {
            return DateTime.Now.Ticks;
        }
    }
}