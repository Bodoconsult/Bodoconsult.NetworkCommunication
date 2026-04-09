// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DigitalTwins
{
    public class SfxpDigitalTwinMessageFactory : IDigitalTwinMessageFactory
    {
        private long _messageId;
        private int _lastChunkId;
        private int _syncByteCounter;
        private int _lastSampleCounter;

        private readonly bool _islittleEndian = BitConverter.IsLittleEndian;

        /// <summary>
        ///  The regular sync byte definition
        /// </summary>
        public SyncByteDefinition RegularSyncByte { get; set; } = new()
        {
            Length = 1,
            SyncByte = 0x0
        };

        /// <summary>
        /// The sync byte block definition for the samplecounters
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
        /// The number of messages created
        /// </summary>
        public int NumberOfMessagesCreated { get; set; } = 10;


        /// <summary>
        /// Generate a set of messages as defined with <see cref="NumberOfMessagesCreated"/>
        /// </summary>
        /// <returns>List with messages to send</returns>
        public List<Memory<byte>> GenerateMessages()
        {
            var result = new List<Memory<byte>>();

            for (var i = 0; i < NumberOfMessagesCreated; i++)
            {
                result.Add(CreateMessage());
            }

            return result;
        }

        /// <summary>
        /// Generate the next message in endless mode
        /// </summary>
        /// <returns>Message to send</returns>
        public Memory<byte> GenerateNextMessage()
        {
            return CreateMessage();
        }

        private Memory<byte> CreateMessage()
        {
            var data = new List<byte>();

            // MessageId (as big endian)
            var intBytes = GetMessageIdAsBytes();

            data.AddRange(intBytes);

            while (true)
            {
                // Add sync byte
                if (_lastChunkId == 0)
                {
                    if (AddSyncByte(data))
                    {
                        break;
                    }
                }

                if (data.Count == MaximumMessageLength - 1)
                {
                    break;
                }

                if (CreateChunks(data))
                {
                    break;
                }
            }

            SetNextMessageId();

            return data.ToArray().AsMemory();
        }

        private bool CreateChunks(List<byte> data)
        {
            // Create chunks
            for (var j = _lastChunkId; j < NumberOfChunksBeforeSyncByteIsSent; j++)
            {
                // Add chunk if possible
                var chunk = GenerateChunk(j);

                // Resulting message is too long: leave here but remember chunk ID
                if (data.Count + chunk.Count >= MaximumMessageLength)
                {
                    _lastChunkId = j;
                    return true;
                }

                data.AddRange(chunk);
            }

            _lastChunkId = 0;
            return false;
        }

        /// <summary>
        /// Add the sync bytes
        /// </summary>
        /// <param name="data">Message</param>
        /// <returns>True if no sync byte could be added because of the length of the message else false</returns>
        private bool AddSyncByte(List<byte> data)
        {
            SyncByteDefinition sd;
            byte[] syncBytes;

            //var test = _lastSampleCounter % SendSampleCounterInterval;
            if (_syncByteCounter >= SendSampleCounterInterval && _syncByteCounter % SendSampleCounterInterval < 0.0001)
            {
                //Debug.Print($"1dS: {_syncByteCounter} // {_lastSampleCounter}");
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
                    //Debug.Print($"2dS: {_syncByteCounter} // {_lastSampleCounter}");
                    sd = RegularSyncByte;
                    syncBytes = [sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1, sd.SyncByte, 0x1, 0x1];
                }
                else
                {
                    //Debug.Print($"N: {_syncByteCounter} // {_lastSampleCounter}");
                    // Normal sync byte
                    sd = SampleCounterSyncByteBlock;
                    syncBytes = [sd.SyncByte];
                }
            }

            if (data.Count > MaximumMessageLength - sd.Length)
            {
                return true;
            }

            data.AddRange(syncBytes);

            _syncByteCounter++;
            return false;
        }

        /// <summary>
        /// Set the next message ID. If long.MaxValue is reached restart with 0
        /// </summary>
        private void SetNextMessageId()
        {
            if (_messageId == long.MaxValue)
            {
                _messageId = 0;
            }
            else
            {
                _messageId++;
            }
        }

        /// <summary>
        /// MessageId (as big endian)
        /// </summary>
        /// <returns></returns>
        private byte[] GetMessageIdAsBytes()
        {
            var intBytes = BitConverter.GetBytes(_messageId);
            if (!_islittleEndian)
            {
                Array.Reverse(intBytes);
            }
            return intBytes;
        }

        /// <summary>
        /// Generate a data chunk
        /// </summary>
        /// <param name="index">Index of the data chunk</param>
        /// <returns></returns>
        private List<byte> GenerateChunk(int index)
        {
            var result = new List<byte>(); index += 10;    // Add 10 to avoid issue with 0x9 sync byte

            for (var i = 0; i < DataChunkLength; i++)
            {
                result.Add((byte)index);
            }

            return result;
        }
    }
}
