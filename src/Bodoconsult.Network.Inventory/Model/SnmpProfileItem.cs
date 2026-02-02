using System;
using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Contains limts and othe rinformation for certain OIDs of a SNMP profile
/// </summary>
public class SnmpProfileItem
{
    private string _limitLowValues;
    private string _limitMediumValues;
    private string _limitHighValues;
    private string _limitExistentialValues;

    /// <summary>
    /// default ctor
    /// </summary>
    public SnmpProfileItem()
    {
        ValueNotFoundReaction = CheckSeverityLevelValueNotFoundReaction.ReactWithLowLevel;
    }

    /// <summary>
    /// Effective values for checking request data
    /// </summary>
    [JsonIgnore]
    public Dictionary<int, WarningSeverityLevel> EffectiveValues = new Dictionary<int, WarningSeverityLevel>();


    public CheckSeverityLevelValueNotFoundReaction ValueNotFoundReaction { get; set; }


    /// <summary>
    /// OID
    /// </summary>
    public string Oid { get; set; }


    /// <summary>
    /// Values seperated by semicolon for the limit of severity LOW. Ranges to be described with -, like 5-10
    /// </summary>
    /// <example>Sample: 1-4;5;6;7-15;16</example>
    public string LimitLowValues
    {
        get { return _limitLowValues; }
        set
        {
            _limitLowValues = value;
            GetEffectiveValues();
        }
    }

    /// <summary>
    /// Values seperated by semicolon for the limit of severity MEDIUM. Ranges to be described with -, like 5-10
    /// </summary>
    /// <example>Sample: 1-4;5;6;7-15;16</example>
    public string LimitMediumValues
    {
        get { return _limitMediumValues; }
        set
        {
            _limitMediumValues = value;
            GetEffectiveValues();
        }
    }


    /// <summary>
    /// Values seperated by semicolon for the limit of severity MEDIUM. Ranges to be described with -, like 5-10
    /// </summary>
    /// <example>Sample: 1-4;5;6;7-15;16</example>
    public string LimitHighValues
    {
        get { return _limitHighValues; }
        set
        {
            _limitHighValues = value;
            GetEffectiveValues();
        }
    }


    /// <summary>
    /// Values seperated by semicolon for the limit of severity MEDIUM. Ranges to be described with -, like 5-10
    /// </summary>
    /// <example>Sample: 1-4;5;6;7-15;16</example>
    public string LimitExistentialValues
    {
        get { return _limitExistentialValues; }
        set
        {
            _limitExistentialValues = value;
            GetEffectiveValues();
        }
    }

    /// <summary>
    /// Path to the value in the network item XML file where values of the current OID is save in
    /// </summary>
    public string XmlPath { get; set; }


    /// <summary>
    /// Calculate the effective values
    /// </summary>
    private void GetEffectiveValues()
    {
        EffectiveValues.Clear();

        GetEffectiveValuesLevel(_limitLowValues, WarningSeverityLevel.Low);

        GetEffectiveValuesLevel(_limitMediumValues, WarningSeverityLevel.Medium);

        GetEffectiveValuesLevel(_limitHighValues, WarningSeverityLevel.High);

        GetEffectiveValuesLevel(_limitExistentialValues, WarningSeverityLevel.Existential);
    }


    private void GetEffectiveValuesLevel(string values, WarningSeverityLevel level)
    {
        if (string.IsNullOrEmpty(values)) return;

        var array = values.Split(';');

        foreach (var value in array)
        {
            if (value.Contains("-"))
            {
                var rangeValues = value.Split('-');
                for(var i = Convert.ToInt32(rangeValues[0]); i<Convert.ToInt32(rangeValues[1]);i++)
                {
                    EffectiveValues.Add(i, level);
                }
            }
            else
            {
                EffectiveValues.Add(Convert.ToInt32(value), level);
            }
        }
    }


    /// <summary>
    /// Check if a current value exists and return the related <see cref="WarningSeverityLevel"/>.
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Level of severity the value falling in</returns>
    public WarningSeverityLevel CheckSeverityLevel(int value)
    {
        var isValue = EffectiveValues.ContainsKey(value);

        if (isValue)
        {
            var data = EffectiveValues[value];
            return data;
        }

        if (ValueNotFoundReaction ==CheckSeverityLevelValueNotFoundReaction.ReactWithLowLevel)
        {
            return WarningSeverityLevel.Low;
        }


        return WarningSeverityLevel.None;
    }

    public string Description { get; set; }
}


public enum CheckSeverityLevelValueNotFoundReaction
{
    ReactWithLowLevel,
    ReactWithAllOk
}