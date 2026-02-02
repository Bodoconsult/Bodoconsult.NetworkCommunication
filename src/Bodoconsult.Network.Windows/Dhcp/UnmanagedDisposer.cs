// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp;

internal readonly struct UnmanagedDisposer<T> : IDisposable
{
#pragma warning disable IDE0032 // Use auto property
    private readonly IntPtr _pointer;
#pragma warning restore IDE0032 // Use auto property

    public IntPtr Pointer => _pointer;

    public UnmanagedDisposer(T structure)
    {
        var size = Marshal.SizeOf(structure);
        _pointer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structure, _pointer, false);
        }
        catch (Exception)
        {
            Marshal.FreeHGlobal(_pointer);
            throw;
        }
    }

    public void Dispose()
    {
        if (_pointer != IntPtr.Zero)
        {
            Marshal.DestroyStructure(_pointer, typeof(T));
            Marshal.FreeHGlobal(_pointer);
        }
    }

    public static implicit operator IntPtr(UnmanagedDisposer<T> disposer)
    {
        return disposer._pointer;
    }
}