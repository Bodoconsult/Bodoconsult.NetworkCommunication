// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.NetworkCommands;

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