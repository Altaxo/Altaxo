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
using System.Collections.Immutable;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Contains options for master curve creation.
  /// </summary>
  public record MasterCurveCreationOptions : MasterCurveCreationOptionsBase
  {
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
    public MasterCurveTableOutputOptions TableOutputOptions { get; init; } = new MasterCurveTableOutputOptions();

    public MasterCurveCreationOptions With(MasterCurveImprovementOptions improvementOptions)
    {
      return this with
      {
        ShiftOrder = improvementOptions.ShiftOrder,
        OptimizationMethod = improvementOptions.OptimizationMethod,
        _numberOfIterations = improvementOptions.NumberOfIterations, // in order to circumvent the >0 criterion
        RequiredRelativeOverlap = improvementOptions.RequiredRelativeOverlap,
        MasterCurveGroupOptionsChoice = improvementOptions.MasterCurveGroupOptionsChoice,
        GroupOptions = improvementOptions.GroupOptions,
        MasterCurveImprovementOptions = null,
      };
    }

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

        info.AddValue("ShiftOrder", s.ShiftOrder);
        info.AddEnum("OptimizationMethod", s.OptimizationMethod);
        info.AddValue("NumberOfIterations", s.NumberOfIterations);
        info.AddValue("RequiredRelativeOverlap", s.RequiredRelativeOverlap);
        info.AddEnum("GroupOptionsChoice", s.MasterCurveGroupOptionsChoice);
        info.AddArray("GroupOptions", s.GroupOptions, s.GroupOptions.Count);
        info.AddValueOrNull("ImprovementOptions", s.MasterCurveImprovementOptions);
        info.AddValue("Property1Name", s.Property1Name);
        info.AddNullableEnum("Property1TemperatureRepresentation", s.Property1TemperatureRepresentation);
        info.AddValue("Property2Name", s.Property2Name);
        info.AddValue("ReferenceValue", s.ReferenceValue);
        info.AddValue("UseExactReferenceValue", s.UseExactReferenceValue);
        info.AddValue("ReferenceColumnIndex", s.IndexOfReferenceColumnInColumnGroup);
        info.AddValue("TableOutputOptions", s.TableOutputOptions);

      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var ShiftOrder = info.GetValue<ShiftOrder.IShiftOrder>("ShiftOrder", null);
        var OptimizationMethod = info.GetEnum<OptimizationMethod>("OptimizationMethod");
        var NumberOfIterations = info.GetInt32("NumberOfIterations");
        var RequiredRelativeOverlap = info.GetDouble("RequiredRelativeOverlap");
        var GroupOptionsChoice = info.GetEnum<MasterCurveGroupOptionsChoice>("GroupOptionsChoice");
        var GroupOptions = info.GetArrayOfValues<MasterCurveGroupOptions>("GroupOptions", null);
        var ImprovementOptions = info.GetValueOrNull<MasterCurveImprovementOptions>("ImprovementOptions", null);
        var Property1Name = info.GetString("Property1Name");
        var Property1TemperatureRepresentation = info.GetNullableEnum<TemperatureRepresentation>("Property1TemperatureRepresentation");
        var Property2Name = info.GetString("Property2Name");
        var ReferenceValue = info.GetNullableDouble("ReferenceValue");
        var UseExactReferenceValue = info.GetBoolean("UseExactReferenceValue");
        var ReferenceColumnIndex = info.GetInt32("ReferenceColumnIndex");
        var TableOutputOptions = info.GetValue<MasterCurveTableOutputOptions>("TableOutputOptions", null);

        return new MasterCurveCreationOptions()
        {
          ShiftOrder = ShiftOrder,
          OptimizationMethod = OptimizationMethod,
          NumberOfIterations = NumberOfIterations,
          RequiredRelativeOverlap = RequiredRelativeOverlap,
          MasterCurveGroupOptionsChoice = GroupOptionsChoice,
          GroupOptions = GroupOptions.ToImmutableList(),
          MasterCurveImprovementOptions = ImprovementOptions,
          Property1Name = Property1Name,
          Property1TemperatureRepresentation = Property1TemperatureRepresentation,
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
