// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Helper class for validation of entities
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates an object (entity) using DataAnnotations
    /// </summary>
    /// <param name="data">Object to validate</param>
    /// <returns>(Empty) list of validation results. Empty list means there were no validation errors, the object is valid.</returns>
    public static ICollection<ValidationResult> ValidateObject(object data)
    {
        var vc = new ValidationContext(data);

        ICollection<ValidationResult> results = new List<ValidationResult>(); // Will contain the results of the validation
        var isValid = Validator.TryValidateObject(data, vc, results, true); // Validates the object and its properties using the previously created context.

        if (isValid)
        {
            return results;
        }

        foreach (var result in results)
        {
            var members = new StringBuilder();
            foreach (var member in result.MemberNames)
            {
                members.AppendLine($"{member};");
            }

            var m = members.ToString();
            if (m.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
            {
                m = m.Substring(0, members.Length - 1);
            }

            Debug.Print($"{m}: {result.ErrorMessage}");
        }

        return results;
    }

    /// <summary>
    /// Format validations error result as message string
    /// </summary>
    /// <param name="validationResult"></param>
    /// <returns>Message string</returns>
    public static string? FormatValidationErrors(ICollection<ValidationResult>? validationResult)
    {
        if (validationResult == null)
        {
            return null;
        }

        var sb = new StringBuilder();

        sb.AppendLine("Validation error:");

        foreach (var result in validationResult)
        {
            sb.AppendLine($"{result.ErrorMessage}");
        }

        return sb.ToString();
    }
}