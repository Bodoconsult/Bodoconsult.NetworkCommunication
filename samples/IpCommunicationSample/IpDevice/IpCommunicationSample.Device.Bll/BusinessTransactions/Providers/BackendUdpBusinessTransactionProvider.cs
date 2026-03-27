// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpCommunicationSample.Device.Bll.Interfaces;

namespace IpCommunicationSample.Device.Bll.BusinessTransactions.Providers
{
    /// <summary>
    /// Impl of <see cref="IBusinessTransactionProvider"/> for UPD messaging
    /// </summary>
    public class BackendUdpBusinessTransactionProvider : IBusinessTransactionProvider
    {
        public readonly IBackendUdpBusinessLogicAdapter BusinessLogicAdapter;

        /// <summary>
        /// Default ctor
        /// </summary>
        public BackendUdpBusinessTransactionProvider(IBackendUdpBusinessLogicAdapter businessLogicAdapter)
        {
            BusinessLogicAdapter = businessLogicAdapter;

            // Load transaction delegates now
            CreateBusinessTransactionDelegates.Add(IpDeviceBusinessTransactionCodes.StartCommunication, Transaction1_StartCommunication);
            CreateBusinessTransactionDelegates.Add(IpDeviceBusinessTransactionCodes.StopCommunication, Transaction2_StopCommunication);
            CreateBusinessTransactionDelegates.Add(IpDeviceBusinessTransactionCodes.StartStreaming, Transaction3_StartStreaming);
            CreateBusinessTransactionDelegates.Add(IpDeviceBusinessTransactionCodes.StopStreaming, Transaction4_StopStreaming);
            CreateBusinessTransactionDelegates.Add(IpDeviceBusinessTransactionCodes.StartSnapshot, Transaction5_StartSnapshot);
            CreateBusinessTransactionDelegates.Add(IpDeviceBusinessTransactionCodes.StopSnapshot, Transaction6_StopSnapshot);
        }


        /// <summary>
        /// A dictionary containing delegates for creating business transactions.
        /// The key of the dictionary is the int transaction ID
        /// </summary>
        public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();

        /// <summary>
        /// Create transaction 1: start the UDP communication
        /// </summary>
        /// <returns>Business transaction</returns>
        public BusinessTransaction Transaction1_StartCommunication()
        {
            var transaction = new BusinessTransaction
            {
                Id = IpDeviceBusinessTransactionCodes.StartCommunication,
                Name = "Start the UDP communication",
                RunBusinessTransactionDelegate = BusinessLogicAdapter.StartCommunication
            };

            transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

            return transaction;
        }

        /// <summary>
        /// Create transaction 2: stop the UDP communication
        /// </summary>
        /// <returns>Business transaction</returns>
        public BusinessTransaction Transaction2_StopCommunication()
        {
            var transaction = new BusinessTransaction
            {
                Id = IpDeviceBusinessTransactionCodes.StopCommunication,
                Name = "Stop the UDP communication",
                RunBusinessTransactionDelegate = BusinessLogicAdapter.StopCommunication
            };

            transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

            return transaction;
        }

        /// <summary>
        /// Create transaction 3: start streaming
        /// </summary>
        /// <returns>Business transaction</returns>
        public BusinessTransaction Transaction3_StartStreaming()
        {
            var transaction = new BusinessTransaction
            {
                Id = IpDeviceBusinessTransactionCodes.StartStreaming,
                Name = "Start streaming",
                RunBusinessTransactionDelegate = BusinessLogicAdapter.StartStreaming
            };

            transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

            return transaction;
        }
        /// <summary>
        /// Create transaction 4: stop streaming
        /// </summary>
        /// <returns>Business transaction</returns>
        public BusinessTransaction Transaction4_StopStreaming()
        {
            var transaction = new BusinessTransaction
            {
                Id = IpDeviceBusinessTransactionCodes.StopStreaming,
                Name = "Stop streaming",
                RunBusinessTransactionDelegate = BusinessLogicAdapter.StopStreaming
            };

            transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

            return transaction;
        }

        /// <summary>
        /// Create transaction 5: start snapshot
        /// </summary>
        /// <returns>Business transaction</returns>
        public BusinessTransaction Transaction5_StartSnapshot()
        {
            var transaction = new BusinessTransaction
            {
                Id = IpDeviceBusinessTransactionCodes.StartSnapshot,
                Name = "Start snapshot",
                RunBusinessTransactionDelegate = BusinessLogicAdapter.StartSnapshot
            };

            transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

            return transaction;
        }

        /// <summary>
        /// Create transaction 6: stop snapshot
        /// </summary>
        /// <returns>Business transaction</returns>
        public BusinessTransaction Transaction6_StopSnapshot()
        {
            var transaction = new BusinessTransaction
            {
                Id = IpDeviceBusinessTransactionCodes.StopSnapshot,
                Name = "Stop snapshot",
                RunBusinessTransactionDelegate = BusinessLogicAdapter.StopSnapshot
            };

            transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

            return transaction;
        }
    }
}
