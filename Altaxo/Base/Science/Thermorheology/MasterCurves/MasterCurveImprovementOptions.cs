#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
  /// Contains options for improving the master after it was created.
  /// </summary>
  public record MasterCurveImprovementOptions : Main.IImmutable
  {
    /// <summary>
    /// Designates the order with which the curves are shifted to the master curve.
    /// </summary>
    public ShiftOrder.IShiftOrder ShiftOrder { get; init; } = new ShiftOrder.PivotToLastAlternating();

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

    public MasterCurveGroupOptionsChoice MasterCurveGroupOptionsChoice { get; init; } = MasterCurveGroupOptionsChoice.SameForAllGroups;

    /// <summary>
    /// Get the options for each group. If there is only one <see cref="MasterCurveGroupOptionsWithScalarInterpolation"/>, and multiple groups, the options are applied
    /// to each of the groups. Otherwise, the number of group options must match the number of groups. If there is one <see cref="MasterCurveGroupOptionsWithComplexInterpolation"/>,
    /// two groups are needed.
    /// </summary>
    public ImmutableList<MasterCurveGroupOptions> GroupOptions { get; init; } = new MasterCurveGroupOptions[] { new MasterCurveGroupOptionsWithScalarInterpolation() }.ToImmutableList();


    #region Serialization

    /// <summary>
    /// V0: 2024-01-24 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MasterCurveImprovementOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MasterCurveImprovementOptions)obj;

        info.AddValue("ShiftOrder", s.ShiftOrder);
        info.AddEnum("OptimizationMethod", s.OptimizationMethod);
        info.AddValue("NumberOfIterations", s.NumberOfIterations);
        info.AddEnum("GroupOptionsChoice", s.MasterCurveGroupOptionsChoice);
        info.AddArray("GroupOptions", s.GroupOptions, s.GroupOptions.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var ShiftOrder = info.GetValue<ShiftOrder.IShiftOrder>("ShiftOrder", null);
        var OptimizationMethod = info.GetEnum<OptimizationMethod>("OptimizationMethod");
        var NumberOfIterations = info.GetInt32("NumberOfIterations");
        var GroupOptionsChoice = info.GetEnum<MasterCurveGroupOptionsChoice>("GroupOptionsChoice");
        var GroupOptions = info.GetArrayOfValues<MasterCurveGroupOptions>("GroupOptions", null);

        return new MasterCurveImprovementOptions()
        {
          ShiftOrder = ShiftOrder,
          OptimizationMethod = OptimizationMethod,
          NumberOfIterations = NumberOfIterations,
          MasterCurveGroupOptionsChoice = GroupOptionsChoice,
          GroupOptions = GroupOptions.ToImmutableList(),
        };
      }
    }

    #endregion



  }
}
