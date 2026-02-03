// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Buffers;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Extension methods for ReadOnlySequence&lt;byte&gt; instances
/// </summary>
public static class ReadOnlySequenceExtensions 
{
    /// <summary>
    /// Is the instance content the same as the one of the checkInstance
    /// </summary>
    /// <param name="instance">Current instance</param>
    /// <param name="checkInstance">Instance to compare with</param>
    /// <returns>True if both instances have the same byte content else false</returns>
    public static bool IsEqualTo(this ReadOnlySequence<byte> instance, ReadOnlySequence<byte> checkInstance)
    {
        if (instance.Length != checkInstance.Length)
        {
            return false;
        }

        for (var i = 0; i < instance.Length; i++)
        {
            if (instance.Slice(i, 1).FirstSpan[0] != checkInstance.Slice(i, 1).FirstSpan[0])
            {
                return false;
            }
        }

        return true;
    }

}