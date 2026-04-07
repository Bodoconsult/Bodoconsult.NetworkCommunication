// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DigitalTwins
{
    /// <summary>
    /// Definition for sync bytes and sync byte blocks
    /// </summary>
    public struct SyncByteDefinition
    {
        /// <summary>
        /// First byte of the sync byte
        /// </summary>
        public byte SyncByte { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; set; }
    }

    public class SfxpDigitalTwinMessageFactory : IDigitalTwinMessageFactory
    {
        private long _messageId = 0;
        private int _lastChunkId;
        private int _syncByteCounter;
        private int _lastSampleCounter;

        /// <summary>
        ///  The regular sync byte definition
        /// </summary>
        public SyncByteDefinition RegularSyncByte { get; set; } = new()
        {
            Length = 1,
            SyncByte = 0x0
        };

        /// <summary>
        /// The rsync byte block definition for the samplecounters
        /// </summary>
        public SyncByteDefinition SampleCounterSyncByteBlock { get; set; } = new()
        {
            Length = 12,
            SyncByte = 0x9
        };

        /// <summary>
        /// The interval for sending sample counter instead of regular sync byte
        /// </summary>
        public double SendSampleCounterInterval { get; set; } = 5.0;

        /// <summary>
        /// Maximum message length to be sent
        /// </summary>
        public int MaximumMessageLength { get; set; } = 16384;

        /// <summary>
        /// The length of a data chunk
        /// </summary>
        public int DataChunkLength { get; set; } = 8;

        /// <summary>
        /// The number of data chunks sent before the enxt sync byte is sent
        /// </summary>
        public int NumberOfChunksBeforeSyncByteIsSent { get; set; } = 253;

        /// <summary>
        /// The numebr of messages created
        /// </summary>
        public int NumberOfMessagesCreated { get; set; } = 10;


        public List<ReadOnlySequence<byte>> GenerateMessages()
        {
            var result = new List<ReadOnlySequence<byte>>();

            for (var i = 0; i < NumberOfMessagesCreated; i++)
            {
                CreateMessage(i, result);
            }

            return result;
        }

        private void CreateMessage(int i, List<ReadOnlySequence<byte>> result)
        {
            var data = new List<byte>();

            // MessageId (as big endian)
            var islittleEndian = BitConverter.IsLittleEndian;
            var intBytes = BitConverter.GetBytes(_messageId);
            if (!islittleEndian)
            {
                Array.Reverse(intBytes);
            }

            data.AddRange(intBytes);

            while (true)
            {
                // Add sync byte
                if (_lastChunkId == 0)
                {
                    SyncByteDefinition sd;
                    byte[] syncBytes;

                    var test = _lastSampleCounter % SendSampleCounterInterval;
                    if (_syncByteCounter >= SendSampleCounterInterval && (_syncByteCounter % SendSampleCounterInterval < 0.0001))
                    {
                        Debug.Print($"1dS: {_syncByteCounter} // {_lastSampleCounter}");
                        // Send sample counter
                        sd = RegularSyncByte;
                        syncBytes = [sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1];
                        _lastSampleCounter = _syncByteCounter;
                    }
                    else
                    {
                        // repeat the sample counter
                        if (_syncByteCounter > SendSampleCounterInterval && _lastSampleCounter + 1 == _syncByteCounter)
                        {
                            Debug.Print($"2dS: {_syncByteCounter} // {_lastSampleCounter}");
                            sd = RegularSyncByte;
                            syncBytes = [sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1];
                        }
                        else
                        {
                            Debug.Print($"N: {_syncByteCounter} // {_lastSampleCounter}");
                            // Normal sync byte
                            sd = SampleCounterSyncByteBlock;
                            syncBytes = [sd.SyncByte];
                        }

                    }

                    if (data.Count > MaximumMessageLength - sd.Length)
                    {
                        break;
                    }

                    data.AddRange(syncBytes);

                    _syncByteCounter++;
                }

                if (data.Count == MaximumMessageLength - 1)
                {
                    break;
                }

                // Create chunks
                for (var j = _lastChunkId; j < NumberOfChunksBeforeSyncByteIsSent; j++)
                {
                    // Add chunk if possible
                    var chunk = GenerateChunk(j);

                    // Resulting message is too long: leave here but remember chunk ID
                    if (data.Count + chunk.Count >= MaximumMessageLength)
                    {
                        _lastChunkId = j;
                        break;
                    }

                    data.AddRange(chunk);
                }

                _lastChunkId = 0;
            }

            _messageId++;

            result.Add(new ReadOnlySequence<byte>(data.ToArray()));
        }

        private List<byte> GenerateChunk(int j)
        {
            var result = new List<byte>();
            j += 10;

            for (var i = 0; i < DataChunkLength; i++)
            {
                result.Add((byte)j);
            }

            return result;
        }
    }
}
