// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Copyright https://stackoverflow.com/questions/71952022/creating-a-readonlysequence-with-multiple-segments

using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Helper class to join byte array structures to a <see cref="ReadOnlySequence{T}"/>.
/// 
/// </summary>
/// <typeparam name="T">T can be of type <see cref="byte"/> or other types usable with <see cref="ReadOnlySequence{T}"></see></typeparam>
public sealed class ChunkedSequence<T>
{
    private ReadOnlyChunk<T> _first;
    private ReadOnlyChunk<T> _current;

    private bool _changed;

    /// <summary>
    /// Default ctor
    /// </summary>
    public ChunkedSequence()
    {
        _first = _current = null;
        _changed = false;
    }

    /// <summary>
    /// Ctor for initializing with an existing <see cref="ReadOnlySequence{T}"/>
    /// </summary>
    /// <param name="sequence">Existing <see cref="ReadOnlySequence{T}"/> instance</param>

    public ChunkedSequence(ReadOnlySequence<T> sequence) : this()
    {
        Append(sequence);
    }

    /// <summary>
    /// Append a <see cref="ReadOnlySequence{T}" />
    /// </summary>
    /// <param name="sequence">Existing <see cref="ReadOnlySequence{T}"/> instance to append</param>
    public void Append(ReadOnlySequence<T> sequence)
    {
        var pos = sequence.Start;
        while (sequence.TryGet(ref pos, out var mem, true))
        {
            Append(mem);
        }
    }

    /// <summary>
    /// Append a <see cref="ReadOnlyMemory{T}" />
    /// </summary>
    /// <param name="memory">Existing <see cref="ReadOnlyMemory{T}"/> instance to append</param>
    public void Append(ReadOnlyMemory<T> memory)
    {
        if (_current == null)
        {
            _first = _current = new ReadOnlyChunk<T>(memory);
        }
        else
        {
            _current = _current.Append(memory);
        }

        _changed = true;
    }

    /// <summary>
    /// Append a <see cref="Memory{T}" />
    /// </summary>
    /// <param name="memory">Existing <see cref="Memory{T}"/> instance to append</param>
    public void Append(Memory<T> memory)
    {
        if (_current == null)
        {
            _first = _current = new ReadOnlyChunk<T>(memory);
        }
        else
        {
            _current = _current.Append(memory);
        }

        _changed = true;
    }


    /// <summary>
    /// Get the current instance of <see cref="ReadOnlySequence{T}"/> 
    /// </summary>
    /// <returns></returns>
    internal ReadOnlySequence<T> GetSequence()
    {
        var sequence = _changed ? new ReadOnlySequence<T>(_first, 0, _current, _current.Memory.Length) : new ReadOnlySequence<T>();

        return sequence;
    }

    /// <summary>
    /// Operator to access the <see cref="ChunkedSequence{T}"/> instance. Usage: ReadOnlySequence{byte} seq = chunkedSequenceInstance;
    /// </summary>
    /// <param name="sequence">Current <see cref="ChunkedSequence{T}"/> instance</param>
    public static implicit operator ReadOnlySequence<T>(ChunkedSequence<T> sequence)
    {
        return sequence.GetSequence();
    }

    /// <summary>
    /// Helper class to implement <see cref="ReadOnlySequenceSegment{T}"/>
    /// </summary>
    /// <typeparam name="TT"></typeparam>
    private sealed class ReadOnlyChunk<TT> : ReadOnlySequenceSegment<TT>
    {
        public ReadOnlyChunk(ReadOnlyMemory<TT> memory)
        {
            Memory = memory;
        }

        public ReadOnlyChunk<TT> Append(ReadOnlyMemory<TT> memory)
        {
            var nextChunk = new ReadOnlyChunk<TT>(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };

            Next = nextChunk;
            return nextChunk;
        }
    }
}