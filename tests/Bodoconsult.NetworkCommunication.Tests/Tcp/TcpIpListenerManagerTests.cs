// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp
{
    [TestFixture]
    internal class TcpIpListenerManagerTests
    {
        private const int Port = 10000;

        [Test]
        public void Ctor_ValidSetup_PropsSetCorrectly()
        {
            // Arrange 

            // Act  
            var lm = new TcpIpListenerManager();

            // Assert
            Assert.That(lm.CurrentConsumers.Count, Is.EqualTo(0));
            Assert.That(lm.CurrentSockets.Count, Is.EqualTo(0));
        }


        [Test]
        public void RegisterListener_NotRegisteredPort_ListenerRegistered()
        {
            // Arrange 
            var lm = new TcpIpListenerManager();

            ClientConnectionAcceptedDelegate acceptDelegate = AcceptDelegate1;

            // Act  
            var listener = lm.RegisterListener(Port, acceptDelegate);

            // Assert
            Assert.That(lm.CurrentConsumers.Count, Is.EqualTo(1));
            Assert.That(lm.CurrentSockets.Count, Is.EqualTo(1));

            var consumers = lm.CurrentConsumers[0].Value;

            Assert.That(consumers.Count, Is.EqualTo(1));

            Assert.That(lm.CurrentConsumers.Exists(x=> x.Key == listener), Is.True);
            Assert.That(lm.CurrentSockets.Exists(x => x.Key == Port), Is.True);
            Assert.That(lm.CurrentSockets.Exists(x => x.Value == listener), Is.True);

            lm.Dispose();
        }

        [Test]
        public void RegisterListener_RegisteredPort_OnlyConsumerRegistered()
        {
            // Arrange 
            var lm = new TcpIpListenerManager();

            var listener1 = lm.RegisterListener(Port, AcceptDelegate1);

            // Act  
            var listener2 = lm.RegisterListener(Port, AcceptDelegate2);

            // Assert
            Assert.That(lm.CurrentConsumers.Count, Is.EqualTo(1));
            Assert.That(lm.CurrentSockets.Count, Is.EqualTo(1));

            var consumers = lm.CurrentConsumers[0].Value;

            Assert.That(consumers.Count, Is.EqualTo(2));

            Assert.That(listener1, Is.SameAs(listener2));

            Assert.That(lm.CurrentConsumers.Exists(x => x.Key == listener1), Is.True);
            Assert.That(lm.CurrentSockets.Exists(x => x.Key == Port), Is.True);
            Assert.That(lm.CurrentSockets.Exists(x => x.Value == listener1), Is.True);

            lm.Dispose();
        }


        [Test]
        public void RegisterListener_RegisteredPortSameDelegate_OnlyConsumerRegistered()
        {
            // Arrange 
            var lm = new TcpIpListenerManager();

            ClientConnectionAcceptedDelegate acceptDelegate = AcceptDelegate1;

            var listener1 = lm.RegisterListener(Port, acceptDelegate);

            // Act  
            var listener2 = lm.RegisterListener(Port, acceptDelegate);

            // Assert
            Assert.That(lm.CurrentConsumers.Count, Is.EqualTo(1));
            Assert.That(lm.CurrentSockets.Count, Is.EqualTo(1));

            var consumers = lm.CurrentConsumers[0].Value;

            Assert.That(consumers.Count, Is.EqualTo(1));

            Assert.That(listener1, Is.SameAs(listener2));

            Assert.That(lm.CurrentConsumers.Exists(x => x.Key == listener1), Is.True);
            Assert.That(lm.CurrentSockets.Exists(x => x.Key == Port), Is.True);
            Assert.That(lm.CurrentSockets.Exists(x => x.Value == listener1), Is.True);

            lm.Dispose();
        }

        [Test]
        public void UnRegisterListener_RegisteredPort_ListenerUnregistered()
        {
            // Arrange 
            var lm = new TcpIpListenerManager();

            ClientConnectionAcceptedDelegate acceptDelegate = AcceptDelegate1;

            var listener = lm.RegisterListener(Port, acceptDelegate);

            // Act  
            lm.UnregisterListener(Port, acceptDelegate);

            // Assert
            Assert.That(lm.CurrentConsumers.Count, Is.EqualTo(0));
            Assert.That(lm.CurrentSockets.Count, Is.EqualTo(0));

            lm.Dispose();
        }

        private bool AcceptDelegate1(Socket clientSocket)
        {
            return true;
        }

        private bool AcceptDelegate2(Socket clientSocket)
        {
            return true;
        }
    }
}
