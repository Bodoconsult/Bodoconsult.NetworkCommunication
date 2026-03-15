// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodoconsult.NetworkCommunication.App.Abstractions.NetworkCommands;

/// <summary>
/// Data structure for sending or receiving commands from the network
/// </summary>
public class NetworkCommand
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="command">Current command</param>
    public NetworkCommand(string command)
    {
        Command = command;
    }

    /// <summary>
    /// Current command
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Current command parameters
    /// </summary>
    public List<NetworkCommandParameter> Parameters { get; } = new();
}

/// <summary>
/// Interface for parsing a command string to a <see cref="NetworkCommand"/> instance
/// </summary>
public interface INetworkCommandParser
{
    /// <summary>
    /// Parse the command string to a <see cref="NetworkCommand"/> instance
    /// </summary>
    /// <param name="commandString">Command string to parse</param>
    /// <returns><see cref="NetworkCommand"/> instance with the parsed command string</returns>
    public NetworkCommand Parse(string commandString);
}

/// <summary>
/// Network command parameter
/// </summary>
public struct NetworkCommandParameter
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Parameter value
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// Interface for building a <see cref="NetworkCommand"/> instance
/// </summary>
public interface INetworkCommandBuilder
{
    /// <summary>
    /// Build a <see cref="NetworkCommand"/> instance from a command string and a list of parameters
    /// </summary>
    /// <param name="command">Command string</param>
    /// <param name="parameters">Dictionary with parameter anem and parameter value</param>
    /// <returns><see cref="NetworkCommand"/> instance with the parsed command string</returns>
    public string BuildIt(string command, List<NetworkCommandParameter>? parameters);
}

/// <summary>
/// Builder for Telnet style commands like "log,chstat". Comma is used as separator. First token is the command itself, other tokens (if available) are parameters. Command string and parameters names and values may not contain spaces
/// </summary>
public class TncpCommandBuilder : INetworkCommandBuilder
{
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

/// <summary>
/// Parser for Telnet style commands like "log,chstat". Comma is used as separator. First token is the command itself, other tokens (if available) are parameters. Command string may not contain spaces
/// </summary>
public class TncpCommandParser : INetworkCommandParser
{
    /// <summary>
    /// Parse the command string to a <see cref="NetworkCommand"/> instance
    /// </summary>
    /// <returns><see cref="NetworkCommand"/> instance with the parsed command string</returns>
    public NetworkCommand Parse(string commandString)
    {
        if (string.IsNullOrEmpty(commandString))
        {
            throw new ArgumentException("No command string delivered");
        }

        var tokens = commandString.Trim().Split(',');

        if (tokens.Length == 0)
        {
            throw new ArgumentException("No valid command string delivered");
        }

        if (tokens[0].Length == 0)
        {
            throw new ArgumentException("Invalid command with first token empty delivered");
        }

        var result = new NetworkCommand(tokens[0]);

        // No parameters
        if (tokens.Length == 1)
        {
            return result;
        }

        // Parameters
        for (var index = 1; index < tokens.Length; index++)
        {
            var token = tokens[index];

            var parameter = new NetworkCommandParameter
            {
                Name = $"Parameter{index - 1}",
                Value = token
            };

            result.Parameters.Add(parameter);
        }

        return result;
    }
}