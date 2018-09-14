#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  public abstract class NumericTickSpacing : TickSpacing
  {
    public NumericTickSpacing()
    {
    }

    public NumericTickSpacing(NumericTickSpacing from)
      : base(from) // everything is done here, since CopyFrom is virtual!
    {
    }

    /// <summary>
    /// Returns the physical values
    /// at which major ticks should occur
    /// </summary>
    /// <returns>physical values for the major ticks</returns>
    public virtual double[] GetMajorTicks()
    {
      return new double[] { }; // return a empty array per default
    }

    /// <summary>
    /// GetMinorTicks returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public virtual double[] GetMinorTicks()
    {
      return new double[] { }; // return a empty array per default
    }

    public override double[] GetMajorTicksNormal(Scale scale)
    {
      double[] ticks = GetMajorTicks();
      for (int i = 0; i < ticks.Length; i++)
      {
        ticks[i] = scale.PhysicalVariantToNormal(ticks[i]);
      }
      return ticks;
    }

    public override double[] GetMinorTicksNormal(Scale scale)
    {
      double[] ticks = GetMinorTicks();
      for (int i = 0; i < ticks.Length; i++)
      {
        ticks[i] = scale.PhysicalVariantToNormal(ticks[i]);
      }
      return ticks;
    }

    public override AltaxoVariant[] GetMajorTicksAsVariant()
    {
      double[] ticks = GetMajorTicks();
      var vticks = new AltaxoVariant[ticks.Length];
      for (int i = 0; i < ticks.Length; ++i)
        vticks[i] = ticks[i];
      return vticks;
    }

    public override AltaxoVariant[] GetMinorTicksAsVariant()
    {
      double[] ticks = GetMinorTicks();
      var vticks = new AltaxoVariant[ticks.Length];
      for (int i = 0; i < ticks.Length; ++i)
        vticks[i] = ticks[i];
      return vticks;
    }
  }
}
