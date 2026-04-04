// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.NetworkCommands;

/// <summary>
/// Builder for Telnet style commands like "log,chstat". Comma is used as separator. First token is the command itself, other tokens (if available) are parameters. Command string and parameters names and values may not contain spaces
/// </summary>
public class TncpCommandBuilder : INetworkCommandBuilder
{
    /// <summary>
    /// Build a <see cref="NetworkCommand"/> instance from a command string and a list of parameters
    /// </summary>
    /// <param name="command">Command string</param>
    /// <param name="parameters">Dictionary with parameter anem and parameter value</param>
    /// <returns><see cref="NetworkCommand"/> instance with the parsed command string</returns>
    public string BuildIt(string command, List<NetworkCommandParameter>? parameters)
    {
        if (string.IsNullOrEmpty(command))
        {
            throw new ArgumentException("No command string delivered");
        }

        if (parameters == null || parameters.Count == 0)
        {
            return command;
        }

        var result = new StringBuilder();
        result.Append(command);

        foreach (var parameter in parameters)
        {
            result.Append($",{parameter.Value.Trim()}");
        }

        return result.ToString();
    }
}