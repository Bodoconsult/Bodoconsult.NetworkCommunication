// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Default implemenation of <see cref="IOrderIdGenerator"/> creating the ID as current tick number
/// </summary>
public class DefaultOrderIdGenerator : IOrderIdGenerator
{
    private readonly Lock _tickLock = new();
    private readonly IAppDateService _appDateService;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appDateService">Current app date service</param>
    public DefaultOrderIdGenerator(IAppDateService appDateService)
    {
        _appDateService = appDateService;
    }


    /// <summary>
    /// Get the next order ID
    /// </summary>
    /// <returns>Order ID</returns>
    public long NextId()
    {
        lock (_tickLock)
        {
            return _appDateService.GetCurrentTicks();
        }
    }
}