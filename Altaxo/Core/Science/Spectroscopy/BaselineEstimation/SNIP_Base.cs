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

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  public abstract record SNIP_Base : IBaselineEstimation
  {
    protected double _halfWidth = 15;
    protected bool _isHalfWidthInXUnits;
    protected int _numberOfRegularStages = 40;

    /// <summary>
    /// Half of the width of the averaging window. This value should be set to
    /// roughly the FWHM (full width half maximum) of the broadest peak in the spectrum.
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
    /// Gets or sets the number of regular iterations. Default is 40.
    /// </summary>
    /// <value>
    /// The number of regular iterations.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">Number of iterations must be at least one. - NumberOfRegularStages</exception>
    public int NumberOfRegularIterations
    {
      get { return _numberOfRegularStages; }
      init
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException("Number of iterations must be at least one.", nameof(NumberOfRegularIterations));
        _numberOfRegularStages = value;
      }
    }

    public abstract double[] Execute(double[] xArray, double[] yArray);
  }
}
