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

namespace Altaxo.Science.Spectroscopy.PeakEnhancement
{
  /// <summary>
  /// Enhances peaks by applying SNIP baseline subtraction, using a very small half-width for the SNIP algorithm. See also <see cref="BaselineEstimation.SNIP_Linear"/>.
  /// </summary>
  public record PeakEnhancementSNIP : IPeakEnhancement
  {
    private double _halfWidth = 1;
    private bool _isHalfWidthInXUnits;
    private int _numberOfApplications = 1;

    /// <summary>
    /// Half of the width of the averaging window. This value should be set to
    /// a very small value here, less than a half of the half width of the peaks.
    /// A value of 1 point (which is the default) will do in most cases.
    /// </summary>
    public double HalfWidth
    {
      get { return _halfWidth; }
      init
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException("Value must be >=0", nameof(HalfWidth));
        _halfWidth = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="HalfWidth"/> is in units of points or in x-units.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the unit is points; otherwise, if the unit is that of the x-axis, <c>false</c>.
    /// </value>
    public bool IsHalfWidthInXUnits
    {
      get
      {
        return _isHalfWidthInXUnits;
      }
      init
      {
        _isHalfWidthInXUnits = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of applications, i.e. the number of times this algorithm is applied to the spectrum.
    /// </summary>
    /// <value>
    /// The number of applications.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">NumberOfApplications - Must be a value >= 1</exception>
    public int NumberOfApplications
    {
      get => _numberOfApplications;
      set
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException(nameof(NumberOfApplications), "Must be a value >= 1");

        _numberOfApplications = value;
      }
    }


    #region Serialization

    /// <summary>
    /// 2023-01-27 V0 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakEnhancementSNIP), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakEnhancementSNIP)obj;
        info.AddValue("HalfWidth", s.HalfWidth);
        info.AddValue("IsHalfWidthInXUnits", s.IsHalfWidthInXUnits);
        info.AddValue("NumberOfApplications", s.NumberOfApplications);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var halfWidth = info.GetDouble("HalfWidth");
        var isHalfWidthInXUnits = info.GetBoolean("IsHalfWidthInXUnits");
        var numberOfApplications = info.GetInt32("NumberOfApplications");

        return o is null ? new PeakEnhancementSNIP
        {
          HalfWidth = halfWidth,
          IsHalfWidthInXUnits = isHalfWidthInXUnits,
          NumberOfApplications = numberOfApplications
        } :
          ((PeakEnhancementSNIP)o) with
          {
            HalfWidth = halfWidth,
            IsHalfWidthInXUnits = isHalfWidthInXUnits,
            NumberOfApplications = numberOfApplications
          };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var snip = new BaselineEstimation.SNIP_Linear { HalfWidth = HalfWidth, IsHalfWidthInXUnits = IsHalfWidthInXUnits };

      for (int i = 0; i < NumberOfApplications; ++i)
      {
        (x, y, regions) = snip.Execute(x, y, regions);
      }

      return (x, y, regions);
    }
  }
}
