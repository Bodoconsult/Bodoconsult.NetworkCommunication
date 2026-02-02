// Copyright (c) Mycronic. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Order execution event args. 
/// </summary>
/// <remarks>Interface is required for mocking with MOQ</remarks>
public interface IOrderExecutionNotification : IClientNotification
{
    /// <summary>
    /// The current order
    /// </summary>
    IOrder Order { get; set; }


    ///// <summary>
    ///// The tower serial number
    ///// </summary>
    //string TowerSn { get; }
        
    ///// <summary>
    ///// The UID of the carrier entity
    ///// </summary>
    //Guid CarrierUid { get; }

    ///// <summary>
    ///// The type of the carrier
    ///// </summary>
    //int CarrierType { get; }

    ///// <summary>
    ///// The UID of the joblist entity
    ///// </summary>
    //Guid JobListUid { get; }

    ///// <summary>
    ///// The UID of the joblist item entity
    ///// </summary>
    //Guid JobListElementUid { get; }

    ///// <summary>
    ///// The carrier name
    ///// </summary>
    //string CarrierName { get; }

    ///// <summary>
    ///// The tower magazine slot UID
    ///// </summary>
    //Guid SlotUid { get; }
        
    ///// <summary>
    ///// The terminal used for the order
    ///// </summary>
    //int Terminal { get; }
                
    ///// <summary>
    ///// The order type
    ///// </summary>
    //int OrderType { get; }
        
    ///// <summary>
    ///// UID of the destination depot
    ///// </summary>
    //Guid DestinationDepotUid { get; }
        
}