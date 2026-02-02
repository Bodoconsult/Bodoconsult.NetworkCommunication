// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// // Licence MIT
// 

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// A result set of a handshake or data message validation
/// </summary>
public struct DataMessageValidatorResult
{

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="isMessageValid">Is the message valid?</param>
    /// <param name="validationResult">Validation result as clear text information for logging</param>
    public DataMessageValidatorResult(bool isMessageValid, string validationResult)
    {
        IsMessageValid = isMessageValid;
        ValidationResult = validationResult;
    }

    /// <summary>
    /// Is the message valid?
    /// </summary>
    public bool IsMessageValid { get;  }

    /// <summary>
    /// Validation result as clear text information for logging
    /// </summary>
    public string ValidationResult { get; }

}