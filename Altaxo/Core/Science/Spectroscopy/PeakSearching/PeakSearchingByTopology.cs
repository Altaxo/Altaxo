#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Executes area normalization : y' = (y-min)/(mean), in which min and mean are the minimal and the mean values of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record PeakSearchingByTopology : IPeakSearching
  {
    private double? _minimalProminence = 0.01;

    public double? MinimalProminence
    {
      get => _minimalProminence;
      init
      {
        if (value.HasValue && !(value.Value >= 0))
          throw new ArgumentOutOfRangeException("Value must be >=0", nameof(MinimalProminence));

        _minimalProminence = value;
      }
    }


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingByTopology), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingByTopology)obj;
        info.AddValue("MinimalProminence", s._minimalProminence);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalProminence = info.GetNullableDouble("MinimalProminence");
        return new PeakSearchingByTopology()
        {
          MinimalProminence = minimalProminence,
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public IPeakSearchingResult Execute(double[] x)
    {
      var pf = new PeakFinder();

      pf.SetProminence(_minimalProminence ?? 0.0);
      pf.SetRelativeHeight(0.5);
      pf.SetWidth(0.0);
      pf.SetHeight(0.0);
      pf.Execute(x);

      var arr = new PeakDescription[pf.PeakPositions.Length];

      for (int i = 0; i < pf.PeakPositions.Length; i++)
      {
        arr[i] = new PeakDescription()
        {
          PositionIndex = pf.PeakPositions[i],
          Prominence = pf.Prominences![i],
          Height = pf.PeakHeights![i],
          Width = pf.Widths![i],
          RelativeHeightOfWidthDetermination = 0.5,
          AbsoluteHeightOfWidthDetermination = pf.WidthHeights![i],
        };
      }

      return new Result()
      {
        PeakDescriptions = arr,
      };
    }

    #region Result

    class Result : IPeakSearchingResult
    {
      IReadOnlyList<PeakDescription> _description = new PeakDescription[0];

      public IReadOnlyList<PeakDescription> PeakDescriptions
      {
        get => _description;
        init => _description = value ?? throw new ArgumentNullException();
      }
    }
    #endregion
  }
}
