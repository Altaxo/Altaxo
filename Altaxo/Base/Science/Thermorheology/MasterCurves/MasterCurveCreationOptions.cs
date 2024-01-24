#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Collections.Immutable;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Contains options for master curve creation.
  /// </summary>
  public record MasterCurveCreationOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets the index of that curve where the fit starts (the curve whose x-values are fixed). If this value is null, the value would be determined automatically. If this is not possible,
    /// the value will be assumed to be zero.
    /// </summary>
    public int? IndexOfPivotCurve { get; init; }

    /// <summary>
    /// Designates the order with which the curves are shifted to the master curve.
    /// </summary>
    public ShiftOrder ShiftOrder { get; init; } = ShiftOrder.PivotToLastAlternating;




    /// <summary>
    /// Determines the method to best fit the data into the master curve.
    /// </summary>
    public OptimizationMethod OptimizationMethod { get; init; }

    protected int _numberOfIterations = 20;

    /// <summary>
    /// Gets or sets the number of iterations. Must be greater than or equal to 1.
    /// This number determines how many rounds the master curve is fitted. Increasing this value will in most cases
    /// increase the quality of the fit.
    /// </summary>
    /// <value>
    /// The number of iterations for master curve creation.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">value - Must be a number >= 1</exception>
    public int NumberOfIterations
    {
      get { return _numberOfIterations; }
      init
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be a number >= 1");

        _numberOfIterations = value;
      }
    }

    protected double _requiredRelativeOverlap = 0;
    /// <summary>
    /// Gets/sets the required relative overlap. The default value is 0, which means that a curve part only needs to touch the rest of the master curve.
    /// Setting this to a value, for instance to 0.1, means that a curve part needs an overlapping of 10% (of its x-range) with the rest of the master curve.
    /// This value can also be set to negative values. For instance, setting it to -1 means that a curve part could be in 100% distance (of its x-range) to the rest of the master curve.
    /// </summary>
    /// <value>
    /// The required overlap.
    /// </value>
    public double RequiredRelativeOverlap
    {
      get => _requiredRelativeOverlap;
      init
      {
        if (double.IsNaN(value))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be a valid number");

        _requiredRelativeOverlap = value;
      }
    }

    public MasterCurveGroupOptionsChoice MasterCurveGroupOptionsChoice { get; init; } = MasterCurveGroupOptionsChoice.SameForAllGroups;

    /// <summary>
    /// Get the options for each group. If there is only one <see cref="MasterCurveGroupOptionsWithScalarInterpolation"/>, and multiple groups, the options are applied
    /// to each of the groups. Otherwise, the number of group options must match the number of groups. If there is one <see cref="MasterCurveGroupOptionsWithComplexInterpolation"/>,
    /// two groups are needed.
    /// </summary>
    public ImmutableList<MasterCurveGroupOptions> GroupOptions { get; init; } = new MasterCurveGroupOptions[] { new MasterCurveGroupOptionsWithScalarInterpolation() }.ToImmutableList();

    /// <summary>
    /// If not null, a second stage of fitting can be appended when creating the master curve.
    /// In the default stage, usually a robust interpolation method is chosen. Then, in the 2nd stage, a less robust interpolation method can be used, because the
    /// master curve is already created, and only fine adjustments need to be done.
    /// </summary>
    public MasterCurveImprovementOptions? MasterCurveImprovementOptions { get; init; }

    /// <summary>
    /// Name of the first column property. In most cases, this is something like 'Temperature'.
    /// If unused, set this to <see cref="string.Empty"/>
    /// </summary>
    public string Property1Name { get; init; } = string.Empty;

    /// <summary>
    /// If not null, <see cref="Property1Name"/> represents a temperature, and the value here represents the unit of the temperature.
    /// </summary>
    public TemperatureRepresentation? Property1TemperatureRepresentation { get; init; }

    /// <summary>
    /// Name of the second column property. In most cases, this is something like 'Temperature'.
    /// If unused, set this to <see cref="string.Empty"/>
    /// </summary>
    public string Property2Name { get; init; } = string.Empty;

    /// <summary>
    /// If set, the resulting shift values are transformed in such a manner that the resulting shift at the <see cref="Property1Name"/> is 1.
    /// The behavior depends also on the value of <see cref="UseExactReferenceValue"/>. If this property is false, the curve that is nearest to the reference
    /// value is used as reference curve. If <see cref="UseExactReferenceValue"/> is true, the shift values are interpolated, and the exact reference value is used.
    /// If the value is not set, then the curve with the index <see cref="IndexOfReferenceColumnInColumnGroup"/> is used as reference curve.
    /// </summary>
    public double? ReferenceValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether to use the exact reference value. See <see cref="ReferenceValue"/> for explanations.
    /// </summary>
    public bool UseExactReferenceValue { get; init; }

    /// <summary>Index of the reference curve. This value is only used if <see cref="ReferenceValue"/> is null; otherwise the value of
    /// the reference column will be determined from the value of <see cref="ReferenceValue"/> and the <see cref="Property1Name"/> of the curves.</summary>
    public int IndexOfReferenceColumnInColumnGroup { get; init; }

    /// <summary>
    /// Gets the output options for writing the resulting data into the master curve table.
    /// </summary>
    public MasterCurveTableOutputOptions TableOutputOptions { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2024-01-24 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MasterCurveCreationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MasterCurveCreationOptions)obj;

        info.AddValue("IndexOfPivotCurve", s.IndexOfPivotCurve);
        info.AddEnum("ShiftOrder", s.ShiftOrder);
        info.AddEnum("OptimizationMethod", s.OptimizationMethod);
        info.AddValue("NumberOfIterations", s.NumberOfIterations);
        info.AddValue("RequiredRelativeOverlap", s.RequiredRelativeOverlap);
        info.AddEnum("GroupOptionsChoice", s.MasterCurveGroupOptionsChoice);
        info.AddArray("GroupOptions", s.GroupOptions, s.GroupOptions.Count);
        info.AddValueOrNull("ImprovmentOptions", s.MasterCurveImprovementOptions);
        info.AddValue("Property1Name", s.Property1Name);
        info.AddValue("Property2Name", s.Property2Name);
        info.AddValue("ReferenceValue", s.ReferenceValue);
        info.AddValue("UseExactReferenceValue", s.UseExactReferenceValue);
        info.AddValue("ReferenceColumnIndex", s.IndexOfReferenceColumnInColumnGroup);
        info.AddValue("TableOutputOptions", s.TableOutputOptions);

      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var IndexOfPivotCurve = info.GetNullableInt32("IndexOfPivotCurve");
        var ShiftOrder = info.GetEnum<ShiftOrder>("ShiftOrder");
        var OptimizationMethod = info.GetEnum<OptimizationMethod>("OptimizationMethod");
        var NumberOfIterations = info.GetInt32("NumberOfIterations");
        var RequiredRelativeOverlap = info.GetDouble("RequiredRelativeOverlap");
        var GroupOptionsChoice = info.GetEnum<MasterCurveGroupOptionsChoice>("GroupOptionsChoice");
        var GroupOptions = info.GetArrayOfValues<MasterCurveGroupOptions>("GroupOptions", null);
        var ImprovementOptions = info.GetValueOrNull<MasterCurveImprovementOptions>("ImprovementOptions", null);
        var Property1Name = info.GetString("Property1Name");
        var Property2Name = info.GetString("Property2Name");
        var ReferenceValue = info.GetNullableDouble("ReferenceValue");
        var UseExactReferenceValue = info.GetBoolean("UseExactReferenceValue");
        var ReferenceColumnIndex = info.GetInt32("ReferenceColumnIndex");
        var TableOutputOptions = info.GetValue<MasterCurveTableOutputOptions>("TableOutputOptions", null);

        return new MasterCurveCreationOptions()
        {
          IndexOfPivotCurve = IndexOfPivotCurve,
          ShiftOrder = ShiftOrder,
          OptimizationMethod = OptimizationMethod,
          NumberOfIterations = NumberOfIterations,
          RequiredRelativeOverlap = RequiredRelativeOverlap,
          MasterCurveGroupOptionsChoice = GroupOptionsChoice,
          GroupOptions = GroupOptions.ToImmutableList(),
          MasterCurveImprovementOptions = ImprovementOptions,
          Property1Name = Property1Name,
          Property2Name = Property2Name,
          ReferenceValue = ReferenceValue,
          UseExactReferenceValue = UseExactReferenceValue,
          IndexOfReferenceColumnInColumnGroup = ReferenceColumnIndex,
          TableOutputOptions = TableOutputOptions,
        };
      }
    }

    #endregion



  }
}
