// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DATE_TIME structure defines a 64-bit integer value that contains a date/time, expressed as the number of ticks (100-nanosecond increments) since 12:00 midnight, January 1, 1 C.E. in the Gregorian calendar. 
/// </summary>
[StructLayout(LayoutKind.Explicit)]
internal readonly struct DateTime
{
    [FieldOffset(0)]
    public readonly long DwDateTime;

    private DateTime(long fileTimeUtc)
    {
        DwDateTime = fileTimeUtc;
    }

    private System.DateTime ToDateTime()
    {
        if (DwDateTime == 0)
            return System.DateTime.SpecifyKind(System.DateTime.MinValue, DateTimeKind.Utc);
        if (DwDateTime == long.MaxValue)
            return System.DateTime.SpecifyKind(System.DateTime.MaxValue, DateTimeKind.Utc);
        return System.DateTime.FromFileTimeUtc(DwDateTime);
    }

    public static DateTime FromDateTime(System.DateTime dateTime)
    {
        if (dateTime == System.DateTime.MinValue)
            return new DateTime(0);
        if (dateTime == System.DateTime.MaxValue)
            return new DateTime(long.MaxValue);
        return new DateTime(dateTime.ToFileTimeUtc());
    }

    public static implicit operator System.DateTime(DateTime dateTime)
        => dateTime.ToDateTime();

    public static implicit operator DateTime(System.DateTime dateTime)
        => FromDateTime(dateTime);
}
