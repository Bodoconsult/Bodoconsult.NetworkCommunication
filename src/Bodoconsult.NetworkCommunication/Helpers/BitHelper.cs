// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Helpers;

/// <summary>
/// Helper class for handling number to byte array conversion. Resulting byte arrays or input arrays are always big endian style.
/// </summary>
public static class BitHelper
{
    /// <summary>
    /// Is the OS a little endian OS (like Windows)
    /// </summary>
    private static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

    /// <summary>
    /// Convert big endian byte array to long value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static long ToInt64FromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToInt64(rawBytes, 0);
    }

    /// <summary>
    /// Convert big endian byte array to ulong value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static ulong ToUInt64FromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToUInt64(rawBytes, 0);
    }

    /// <summary>
    /// Convert big endian byte array to long value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static int ToInt32FromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToInt32(rawBytes, 0);
    }

    /// <summary>
    /// Convert big endian byte array to uint value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static uint ToUInt32FromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToUInt32(rawBytes, 0);
    }

    /// <summary>
    /// Convert big endian byte array to ushort value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static ushort ToUInt16FromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToUInt16(rawBytes, 0);
    }

    /// <summary>
    /// Convert big endian byte array to short value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static short ToInt16FromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToInt16(rawBytes, 0);
    }

    /// <summary>
    /// Convert the least significant bytes big endian byte array to long value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static short ToInt16FromBigEndianLeastSignificantBytes(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToInt16(rawBytes, 0);
    }


    /// <summary>
    /// Convert big endian byte array to double value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static double ToDoubleFromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToDouble(rawBytes, 0);
    }

    /// <summary>
    /// Convert big endian byte array to single (float) value
    /// </summary>
    /// <param name="rawBytes">Big endian byte array representing the number</param>
    /// <returns>Number</returns>
    public static float ToSingleFromBigEndian(byte[] rawBytes)
    {
        if (IsLittleEndian)
        {
            Array.Reverse(rawBytes);
        }

        return BitConverter.ToSingle(rawBytes, 0);
    }

    /// <summary>
    /// Convert long value to big endian byte array
    /// </summary>
    /// <param name="number">Number</param>
    /// <returns>Big endian byte array representing the number</returns>
    public static byte[] FromInt64ToBigEndian(long number)
    {
        var intBytes = BitConverter.GetBytes(number);
        if (IsLittleEndian)
        {
            Array.Reverse(intBytes);
        }
        return intBytes;
    }

    /// <summary>
    /// Convert integer value to big endian byte array
    /// </summary>
    /// <param name="number">Number</param>
    /// <returns>Big endian byte array representing the number</returns>
    public static byte[] FromInt32ToBigEndian(int number)
    {
        var intBytes = BitConverter.GetBytes(number);
        if (IsLittleEndian)
        {
            Array.Reverse(intBytes);
        }
        return intBytes;
    }

    /// <summary>
    /// Convert short value to big endian byte array
    /// </summary>
    /// <param name="number">Number</param>
    /// <returns>Big endian byte array representing the number</returns>
    public static byte[] FromInt16ToBigEndian(short number)
    {
        var intBytes = BitConverter.GetBytes(number);
        if (IsLittleEndian)
        {
            Array.Reverse(intBytes);
        }
        return intBytes;
    }

    /// <summary>
    /// Convert double value to big endian byte array
    /// </summary>
    /// <param name="number">Number</param>
    /// <returns>Big endian byte array representing the number</returns>
    public static byte[] FromDoubleToBigEndian(double number)
    {
        var intBytes = BitConverter.GetBytes(number);
        if (IsLittleEndian)
        {
            Array.Reverse(intBytes);
        }
        return intBytes;
    }

    /// <summary>
    /// Convert single (float) value to big endian byte array
    /// </summary>
    /// <param name="number">Number</param>
    /// <returns>Big endian byte array representing the number</returns>
    public static byte[] FromSingleToBigEndian(float number)
    {
        var intBytes = BitConverter.GetBytes(number);
        if (IsLittleEndian)
        {
            Array.Reverse(intBytes);
        }
        return intBytes;
    }

    /// <summary>
    /// Get the high significant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the high significant bits</returns>
    public static int HighSignificantBits(ulong value)
    {
        var sb = 0;

        if ((value & 0xFFFFFFFF) == 0xFFFFFFF)
        {
            sb += 32;
        }
        else
        {
            value >>= 32;
        }

        if ((value & 0xFFFF0000) == 0xFFFF0000)
        {
            sb += 16;
        }
        else
        {
            value >>= 16;
        }

        if ((value & 0xFF00) == 0xFF00)
        {
            sb += 8;
        }
        else
        {
            value >>= 8;
        }

        if ((value & 0xF0) == 0xF0)
        {
            sb += 4;
        }
        else
        {
            value >>= 4;
        }

        if ((value & 0xC) == 0xC)
        {
            sb += 2;
        }
        else
        {
            value >>= 2;
        }

        if ((value & 2) == 2)
        {
            sb += 1;
        }
        else
        {
            value >>= 1;
        }

        if ((value & 1) == 1)
        {
            sb += 1;
        }

        return sb;
    }

    /// <summary>
    /// Get the high significant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the high significant bits</returns>
    public static int HighSignificantBits(uint value)
    {
        var sb = 0;

        if ((value & 0xFFFF0000) == 0xFFFF0000)
        {
            sb = 16;
        }
        else
        {
            value >>= 16;
        }

        if ((value & 0xFF00) == 0xFF00)
        {
            sb += 8;
        }
        else
        {
            value >>= 8;
        }

        if ((value & 0xF0) == 0xF0)
        {
            sb += 4;
        }
        else
        {
            value >>= 4;
        }

        if ((value & 0xC) == 0xC)
        {
            sb += 2;
        }
        else
        {
            value >>= 2;
        }

        if ((value & 2) == 2)
        {
            sb += 1;
        }
        else
        {
            value >>= 1;
        }

        if ((value & 1) == 1)
        {
            sb += 1;
        }

        return sb;
    }

    /// <summary>
    /// Get the high insignificant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the high insignificant bits</returns>
    public static int HighInsignificantBits(ulong value) => HighSignificantBits(~value);

    /// <summary>
    /// Get the high insignificant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the high insignificant bits</returns>
    public static int HighInsignificantBits(uint value) => HighSignificantBits(~value);

    /// <summary>
    /// Get the low insignificant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the low insignificant bits</returns>
    public static int LowInsignificantBits(ulong value)
    {
        var sb = 0;

        if ((value & 0xFFFFFFFF) == 0)
        {
            sb = 32;
            value >>= 32;
        }

        if ((value & 0xFFFF) == 0)
        {
            sb += 16;
            value >>= 16;
        }

        if ((value & 0xFF) == 0)
        {
            sb += 8;
            value >>= 8;
        }

        if ((value & 0x0F) == 0)
        {
            sb += 4;
            value >>= 4;
        }

        if ((value & 0x03) == 0)
        {
            sb += 2;
            value >>= 2;
        }

        if ((value & 1) == 0)
            sb += 1;

        return sb;
    }

    /// <summary>
    /// Get the low insignificant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the low insignificant bits</returns>
    public static int LowInsignificantBits(uint value)
    {
        var sb = 0;

        if ((value & 0xFFFF) == 0)
        {
            sb = 16;
            value >>= 16;
        }

        if ((value & 0xFF) == 0)
        {
            sb += 8;
            value >>= 8;
        }

        if ((value & 0x0F) == 0)
        {
            sb += 4;
            value >>= 4;
        }

        if ((value & 0x03) == 0)
        {
            sb += 2;
            value >>= 2;
        }

        if ((value & 1) == 0)
            sb += 1;

        return sb;
    }

    /// <summary>
    /// Get the low significant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the low significant bits</returns>
    public static int LowSignificantBits(uint value) => LowInsignificantBits(~value);

    /// <summary>
    /// Get the low significant bits
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value representing the low significant bits</returns>
    public static int LowSignificantBits(ulong value) => LowInsignificantBits(~value);
}