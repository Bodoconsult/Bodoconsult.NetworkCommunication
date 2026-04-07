//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

///// <summary>
///// Helper class for resource management
///// </summary>
//public static class ResourceHelper
//{
//    /// <summary>
//    /// Get a byte array from an embedded resource file in the calling assembly
//    /// </summary>
//    /// <param name="resourceName">resource name = file name</param>
//    /// <returns></returns>
//    public static byte[] GetByteResource(string resourceName)
//    {
//        return GetByteResource(Assembly.GetCallingAssembly(), resourceName);
//    }

//    /// <summary>
//    /// Get a byte array from an embedded resource file from an assembly
//    /// </summary>
//    /// <param name="assembly">Assembly to load the resource from</param>
//    /// <param name="resourceName">resource name = file name</param>
//    /// <returns></returns>
//    public static byte[] GetByteResource(Assembly assembly, string resourceName)
//    {
//        var str = assembly.GetManifestResourceStream(resourceName);

//        if (str == null) return [];

//        var data = new byte[str.Length];

//        str.ReadExactly(data, 0, (int)str.Length);

//        return data;
//    }

//    /// <summary>
//    /// Get a text from an embedded resource file
//    /// </summary>
//    /// <param name="resourceName">resource name = file name</param>
//    /// <returns></returns>
//    public static string? GetTextResource(string resourceName)
//    {
//        var ass = Assembly.GetCallingAssembly();
//        return GetTextResource(ass, resourceName);
//    }


//    /// <summary>
//    /// Get a text from an embedded resource file
//    /// </summary>
//    /// <param name="assembly">Assembly to load the resource from</param>
//    /// <param name="resourceName">resource name = file name</param>
//    /// <returns></returns>
//    public static string? GetTextResource(Assembly assembly, string resourceName)
//    {
//        var str = assembly.GetManifestResourceStream(resourceName);

//        if (str == null)
//        {
//            return null;
//        }

//        using var file = new StreamReader(str);
//        var s = file.ReadToEnd();

//        return s;
//    }
//}