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
  public record MasterCurveImprovementOptions : MasterCurveCreationOptionsBase
  {
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

    /// <summary>
    /// Gets or sets the number of iterations. Must be greater than or equal to 0 (0 means that the curve is interpolated with the provided function, but nothing more).
    /// This number determines how many rounds the master curve is fitted. Increasing this value will in most cases
    /// increase the quality of the fit.
    /// </summary>
    /// <value>
    /// The number of iterations for master curve creation.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">value - Must be a number >= 1</exception>
    public override int NumberOfIterations
    {
      get { return _numberOfIterations; }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be a number >= 0");

        _numberOfIterations = value;
      }
    }
  }
}
