// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Extension methods for Memory&lt;byte&gt; instances
/// </summary>
public static class MemoryExtensions
{
    /// <summary>
    /// Is the instance content the same as the one of the checkInstance
    /// </summary>
    /// <param name="instance">Current instance</param>
    /// <param name="checkInstance">Instance to compare with</param>
    /// <returns>True if both instances have the same byte content else false</returns>
    public static bool IsEqualTo(this Memory<byte> instance, Memory<byte> checkInstance)
    {
        if (instance.Length != checkInstance.Length)
        {
            return false;
        }

        for (var i = 0; i < instance.Length; i++)
        {
            if (instance.Slice(i, 1).Span[0] != checkInstance.Slice(i, 1).Span[0])
            {
                return false;
            }
        }

        return true;
    }

}