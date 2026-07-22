// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.NetworkCommunication.ClientNotifications.Notifications;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.App;
using IpBackend.Bll.ClientNotifications;

namespace IpCommunicationSampleTests.Backend.ClientNotifications;

[TestFixture]
internal class ClientBtcpNetworkingClientMessagingServiceTests
{
    //private readonly IAppLoggerProxy _appLogger = TestDataHelper.Logger;

    //[OneTimeTearDown]
    //public void Cleanup()
    //{
    //    _appLogger.Dispose();
    //}


    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var conv = new ClientBtcpNetworkingClientMessagingService(Globals.Instance);

        // Assert
        Assert.That(conv.ConversionRules.Count, Is.Not.Zero);
    }

    [Test]
    public void MapToBusinessTransactionRequestData_StateChangedEventFired_ReturnsRequestData()
    {
        // Arrange 
        var appGlobals = Globals.Instance;
        var conv = new ClientBtcpNetworkingClientMessagingService(appGlobals);

        const int deviceStateId = 2;
        const int businessStateId = 3;
        const int businessSubstateId = 4;
        const string deviceStateName = "Blubb";
        const string businessStateName = "Blubb";
        const string businessSubstateName = "Blebb";

        var request = new StateMachineStateNotification
        {
            DeviceStateId = deviceStateId,
            DeviceStateName = deviceStateName,
            BusinessStateId = businessStateId,
            BusinessStateName = businessStateName,
            BusinessSubstateId = businessSubstateId,
            BusinessSubstateName = businessSubstateName
        };

        var expectedPayload =
            Encoding.UTF8.GetBytes(
                $"{deviceStateId}\u0005{deviceStateName}\u0005{businessStateId}\u0005{businessStateName}\u0005{businessSubstateId}\u0005{businessSubstateName}\u0005{appGlobals.AppStartParameter.AppName} {appGlobals.AppStartParameter.AppVersion}");

        // Act  
        var result = conv.Convert(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(result);

            Assert.That(result.GetType(), Is.EqualTo(typeof(BtcpRequestOutboundDataMessage)));

            var msg = (BtcpRequestOutboundDataMessage)result;

            Assert.That(msg.DataBlock, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(msg.DataBlock);

            Assert.That(msg.DataBlock.Data.Length, Is.EqualTo(expectedPayload.Length + 1));
        }
    }
}