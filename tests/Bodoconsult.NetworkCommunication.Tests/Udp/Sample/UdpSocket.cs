// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Sample;

public class UdpSocket: IDisposable
{
    private readonly UdpClient _socket;
    private IPEndPoint _epFrom;
    private readonly string _name;
    private readonly bool _isServer;

    public UdpSocket(string name, string ipAddress, int port, bool isServer)
    {
        _name = name;
        _isServer = isServer;

        if (_isServer)
        {
            _socket = new UdpClient(port);
            _epFrom = new IPEndPoint(IPAddress.Any, port);
        }
        else
        {
            _socket = new UdpClient();
            _epFrom = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            _socket.Connect(_epFrom);
        }
    }

    public void Send(string text)
    {
        int result;
        var data = Encoding.ASCII.GetBytes(text);

        if (_isServer)
        {
            
            result = _socket.Send([1], 1, _epFrom);
            Debug.Print($"{_name}: sent {result} bytes");
            return;
        }

        result = _socket.Send(data, data.Length);
        Debug.Print($"{_name}: sent {result} bytes");
    }

    public byte[] Receive()
    {
        byte[] data;

        if (_isServer)
        {
            data = _socket.Receive(ref _epFrom);
        }
        else
        {
            data = _socket.Receive(ref _epFrom);
        }

        Debug.Print($"{_name}: receive data from {_epFrom}: {data.Length} bytes");
        return data;
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}