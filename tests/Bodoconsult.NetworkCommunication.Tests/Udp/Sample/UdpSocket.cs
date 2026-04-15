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

    //public void Server(string address)
    //{
    //    //_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
    //    _isServer = true;
            

    //    //_socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
    //    //Receive();
    //}

    //public void Client(string address)
    //{

    //    _isServer = false;
    //}

    public void Send(string text)
    {
        var data = Encoding.ASCII.GetBytes(text);

        if (_isServer)
        {
            _socket.Send([1], 1, _epFrom);
            return;
        }

        _socket.Send(data, data.Length);
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