// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Sample;

public class UdpSocket3 : IDisposable
{
    private readonly UdpClient _socket;
    private IPEndPoint EpFrom { get; set; }
    private readonly string _name;
    private readonly bool _isServer;

    public UdpSocket3(string name, string ipAddress, int port, bool isServer)
    {
        _name = name;
        _isServer = isServer;

        if (_isServer)
        {
            _socket = new UdpClient();
            EpFrom = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Debug.Print($"{_name}: server sending to {EpFrom}");
        }
        else
        {
            _socket = new UdpClient(port);
            EpFrom = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Debug.Print($"{_name}: client receiving from {EpFrom}");
            //_socket.Connect(EpFrom);
        }
    }

    public void Send(string text)
    {
        var data = Encoding.ASCII.GetBytes(text);

        if (_isServer)
        {
            var x = _socket.Send([1], 1, EpFrom);
            Debug.Print($"{_name}: sent to {EpFrom}: {x} bytes");
            return;
        }

        var y = _socket.Send(data, data.Length);
        Debug.Print($"{_name}: sent to {EpFrom}: {y} bytes");
    }

    public byte[] Receive()
    {
        byte[] data;

        var ep = EpFrom;

        if (_isServer)
        {
            data = _socket.Receive(ref ep);
        }
        else
        {
            data = _socket.Receive(ref ep);
        }

        Debug.Print($"{_name}: receive data from {ep}: {data.Length} bytes");
        return data;
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}