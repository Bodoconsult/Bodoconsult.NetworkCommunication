//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//namespace Bodoconsult.NetworkCommunication.Interfaces;

///// <summary>
///// An interface for central order generation via factories for each order type
///// One implementation of this interface is intended to be hold in DI container as singleton
///// </summary>
//public interface IMasterOrderFactory
//{
//    /// <summary>
//    /// All registered factories
//    /// </summary>
//    IList<IOrderFactory> RegisteredFactories { get; }

//    /// <summary>
//    /// Get a factory for a certain order type
//    /// </summary>
//    /// <param name="requestedTypeOfOrder">Requested order type</param>
//    /// <returns></returns>
//    IOrderFactory GetFactory(string requestedTypeOfOrder);

//    /// <summary>
//    /// Get an order for a certain order type
//    /// </summary>
//    /// <param name="requestedTypeOfOrder">Requested order type</param>
//    /// <param name="parameterSet">Current parameter set to use for the newly created order</param>
//    /// <returns></returns>
//    IOrder GetOrder(string requestedTypeOfOrder, IParameterSet parameterSet);

//}