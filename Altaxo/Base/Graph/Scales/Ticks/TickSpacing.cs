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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  /// <summary>
  /// Base class responsible for the spacing of ticks (major and minor) along a scale.
  /// </summary>
  public abstract class TickSpacing
    :
    Main.SuspendableDocumentNodeWithEventArgs, Main.ICopyFrom
  {
    /// <summary>
    /// The next update sequence number.
    /// </summary>
    protected static int _nextUpdateSequenceNumber;

    /// <summary>
    /// The current update sequence number.
    /// </summary>
    protected int _updateSequenceNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="TickSpacing"/> class.
    /// </summary>
    protected TickSpacing()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TickSpacing"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public TickSpacing(TickSpacing from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Creates a copy of this tick-spacing instance.
    /// </summary>
    /// <returns>A cloned tick-spacing instance.</returns>
    public abstract object Clone();

    /// <inheritdoc />
    public abstract bool CopyFrom(object obj);

    /// <summary>
    /// Gets the update sequence number of this instance.
    /// </summary>
    public int UpdateSequenceNumber { get { return _updateSequenceNumber; } }

    /// <summary>
    /// Decides giving a raw org and end value, whether or not the scale boundaries should be extended to
    /// have more 'nice' values. If the boundaries should be changed, the function return true, and the
    /// org and end argument contain the proposed new scale boundaries.
    /// </summary>
    /// <param name="org">Raw scale org.</param>
    /// <param name="end">Raw scale end.</param>
    /// <param name="isOrgExtendable">True when the org is allowed to be extended.</param>
    /// <param name="isEndExtendable">True when the scale end can be extended.</param>
    /// <returns>True when org or end are changed. False otherwise.</returns>
    public abstract bool PreProcessScaleBoundaries(ref AltaxoVariant org, ref AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable);

    /// <summary>
    /// Calculates the ticks based on the org and end of the scale. Org and End now are given and can not be changed anymore.
    /// </summary>
    /// <param name="org">Scale origin.</param>
    /// <param name="end">Scale end.</param>
    /// <param name="scale">The scale.</param>
    public abstract void FinalProcessScaleBoundaries(AltaxoVariant org, AltaxoVariant end, Scale scale);

    /// <summary>
    /// This will return the the major ticks as <see cref="AltaxoVariant" />.
    /// </summary>
    /// <returns>The array with major tick values.</returns>
    public abstract AltaxoVariant[] GetMajorTicksAsVariant();

    /// <summary>
    /// This will return the minor ticks as array of <see cref="AltaxoVariant" />.
    /// </summary>
    /// <returns>The array with minor tick values.</returns>
    public abstract AltaxoVariant[] GetMinorTicksAsVariant();

    /// <summary>
    /// Gets the major ticks in normalized scale coordinates.
    /// </summary>
    /// <param name="scale">The scale used to convert physical values to normalized values.</param>
    /// <returns>The major ticks in normalized coordinates.</returns>
    public virtual double[] GetMajorTicksNormal(Scale scale)
    {
      AltaxoVariant[] vars = GetMajorTicksAsVariant();
      double[] result = new double[vars.Length];
      for (int i = 0; i < vars.Length; i++)
        result[i] = scale.PhysicalVariantToNormal(vars[i]);

      return result;
    }

    /// <summary>
    /// Gets the minor ticks in normalized scale coordinates.
    /// </summary>
    /// <param name="scale">The scale used to convert physical values to normalized values.</param>
    /// <returns>The minor ticks in normalized coordinates.</returns>
    public virtual double[] GetMinorTicksNormal(Scale scale)
    {
      AltaxoVariant[] vars = GetMinorTicksAsVariant();
      double[] result = new double[vars.Length];
      for (int i = 0; i < vars.Length; i++)
        result[i] = scale.PhysicalVariantToNormal(vars[i]);

      return result;
    }

    /// <inheritdoc />
    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      _updateSequenceNumber = _nextUpdateSequenceNumber++;
      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    /// <inheritdoc />
    protected override void EhSelfChanged(EventArgs e)
    {
      _updateSequenceNumber = _nextUpdateSequenceNumber++;
      base.EhSelfChanged(e);
    }
  }
}
