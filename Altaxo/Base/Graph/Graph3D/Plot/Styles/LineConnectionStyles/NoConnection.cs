#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Graph.Graph3D.Plot.Styles.LineConnectionStyles
{
  /// <summary>
  /// Represents a line-connection style that draws no connecting line.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class NoConnection : LineConnectionStyleBase
  {
    /// <summary>
    /// Gets the singleton instance of the <see cref="NoConnection"/> style.
    /// </summary>
    public static NoConnection Instance { get; private set; } = new NoConnection();

    /// <summary>
    /// Initializes a new instance of the <see cref="NoConnection"/> class.
    /// </summary>
    private NoConnection()
    {
    }

    #region Serialization

    /// <summary>
    /// 2016-05-09 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="NoConnection"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NoConnection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Instance;
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public override void Paint(
      IGraphicsContext3D g,
      Processed3DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      PenX3D pen,
      Func<int, double>? symbolGap,
      int skipFrequency,
      bool connectCircular)
    {
    }
  }
}
