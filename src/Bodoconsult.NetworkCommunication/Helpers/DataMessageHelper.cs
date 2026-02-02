// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using System.Buffers;
using System.Text;

namespace Bodoconsult.NetworkCommunication.Helpers;

/// <summary>
/// Helper class for data messaging
/// </summary>
public static class DataMessageHelper
{

    /// <summary>
    /// Create a string from a byte array
    /// </summary>
    /// <param name="ba">Byte array</param>
    /// <param name="prefix">Prefix to add to the string</param>
    /// <returns>String with prefix and attached byte array</returns>
    public static string ByteArrayToString(Memory<byte> ba, string prefix = null)
    {
        var value = new StringBuilder();
        value.Append(prefix);

        foreach (var b in ba.Span)
        {
            value.Append($"[{b:X2}]");
        }
        return value.ToString();
    }

    /// <summary>
    /// Get a string from a byte array
    /// </summary>
    /// <param name="data">Byte array</param>
    /// <returns>Byte array as string</returns>
    public static string GetStringFromArray(byte[] data)
    {
        var value = new StringBuilder();
        foreach (var b in data)
        {
            if (b <= 33 || b >= 127)
            {
                value.Append($"[{b:X2}]");
            }
            else
            {
                value.Append(Convert.ToChar(b));
            }
        }
        return value.ToString();
    }


    /// <summary>
    /// Get a string from a byte array
    /// </summary>
    /// <param name="data">Byte array</param>
    /// <returns>Byte array as string</returns>
    public static string GetStringFromArray(ReadOnlyMemory<byte> data)
    {
        var value = new StringBuilder();
        byte b;

        for (var i = 0; i < data.Length; i++)
        {
            b = data.Slice(i, 1).Span[0];
            if (b <= 33 || b >= 127)
            {
                value.Append($"[{b:X2}]");
            }
            else
            {
                value.Append(Convert.ToChar(b));
            }
        }
        return value.ToString();
    }

    ///// <summary>
    ///// Get a string from a byte array
    ///// </summary>
    ///// <param name="data">Byte array</param>
    ///// <returns>Byte array as string</returns>
    //public static string GetStringFromArray(ReadOnlySequence<byte> data)
    //{
    //    var value = new StringBuilder();
    //    byte b;

    //    for (var i = 0; i < data.Length; i++)
    //    {
    //        b = data.Slice(i, 1).FirstSpan[0];
    //        if (b <= 33 || b >= 127)
    //        {
    //            value.Append($"[{b:X2}]");
    //        }
    //        else
    //        {
    //            value.Append(Convert.ToChar(b));
    //        }
    //    }
    //    return value.ToString();
    //}

    /// <summary>
    /// Get a string from a byte array
    /// </summary>
    /// <param name="data">Byte array</param>
    /// <returns>Byte array as string</returns>
    public static string GetStringFromArray(Memory<byte> data)
    {
        var value = new StringBuilder();
        byte b;

        for (var i = 0; i < data.Length; i++)
        {
            b = data.Slice(i, 1).Span[0];
            if (b <= 33 || b >= 127)
            {
                value.Append($"[{b:X2}]");
            }
            else
            {
                value.Append(Convert.ToChar(b));
            }
        }
        return value.ToString();
    }

    /// <summary>
    /// Get a string from a byte array in C# style (for copying it to unit tests)
    /// </summary>
    /// <param name="data">Byte array</param>
    /// <returns>Byte array as string</returns>
    public static string GetStringFromArrayCsharpStyle(byte[] data)
    {

        var result = new StringBuilder();

        result.AppendLine(GetStringFromArray(data));

        result.Append("{ ");

        foreach (var b in data)
        {
            result.Append($"0x{b:x}, ");
        }

        var s = result.ToString();

        return s.EndsWith(", ", StringComparison.OrdinalIgnoreCase) ?
            $"{s[..^2]} }}" :
            $"{s} }}";
    }

    /// <summary>
    /// Get a string from a byte array in C# style (for copying it to unit tests)
    /// </summary>
    /// <param name="data">Byte array as read only memory</param>
    /// <returns>Byte array as string</returns>
    public static string GetStringFromArrayCsharpStyle(ReadOnlyMemory<byte> data)
    {

        var result = new StringBuilder();

        result.Append(GetStringFromArray(data));

        result.Append("  { ");

        for (var i = 0; i < data.Length; i++)
        {
            result.Append($"0x{data.Slice(i, 1).Span[0]:x}, ");
        }

        var s = result.ToString();

        return s.EndsWith(", ", StringComparison.OrdinalIgnoreCase) ?
            $"{s[..^2]} }}" :
            $"{s} }}";
    }

    /// <summary>
    /// Get a string from a byte array in C# style (for copying it to unit tests)
    /// </summary>
    /// <param name="data">Byte array as readonly span</param>
    /// <returns>Byte array as string</returns>
    public static string GetStringFromArrayCsharpStyle(ref ReadOnlySequence<byte> data)
    {

        var result = new StringBuilder();

        result.Append("{ ");

        for (var i = 0; i < data.Length; i++)
        {
            var b = data.Slice(i, 1).FirstSpan[0];
            result.Append($"0x{b:x}, ");
        }

        var s = result.ToString();

        return s.EndsWith(", ", StringComparison.OrdinalIgnoreCase) ?
            $"{s[..^2]} }}" :
            $"{s} }}";

    }

    /// <summary>
    /// Check if command has 0x0 bytes at the end and remove them if case of
    /// </summary>
    /// <param name="command">Comman to check</param>
    /// <returns>Cleaned command</returns>
    public static ReadOnlySequence<byte> CheckCommandForNullAtTheEnd(in ReadOnlySequence<byte> command)
    {

        if (command.Length == 0)
        {
            return command;
        }

        if (command.Length == 2)
        {
            return command;
        }

        if (command.Slice(command.Length - 1, 1).First.Span[0] != 0)
        {
            return command;
        }

        for (var i = command.Length - 1; i >= 0; i--)
        {
            if (command.Slice(i, 1).First.Span[0] != 0)
            {
                return command.Slice(0, i);
            }
        }

        return command;
    }
}