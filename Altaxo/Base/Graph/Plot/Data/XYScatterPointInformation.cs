#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion
using System;
using System.Drawing;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Provides information about a single scatter point, like coordinates, row index, index into plot and so on.
  /// </summary>
  [Serializable]
  public class XYScatterPointInformation
  {
    PointF _layerCoordinates;
    int _rowIndex;
    int _plotIndex;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="layerCoordinates"></param>
    /// <param name="rowIndex"></param>
    /// <param name="plotIndex"></param>
    public XYScatterPointInformation(PointF layerCoordinates, int rowIndex, int plotIndex)
    {
      _layerCoordinates = layerCoordinates;
      _rowIndex = rowIndex;
      _plotIndex = plotIndex;
    }

  

    /// <summary>
    /// Layer coordinates of the plot point.
    /// </summary>
    public PointF LayerCoordinates
    {
      get
      {
        return _layerCoordinates;
      }
      set
      {
        _layerCoordinates = value;
      }
    }
   

    /// <summary>
    /// Index into the row (of a DataColumn for instance) that represents this scatter point.
    /// </summary>
    public int RowIndex
    {
      get
      {
        return _rowIndex;
      }
      set
      {
        _rowIndex = value;
      }
    }
   

    /// <summary>
    /// Index of plot point, i.e. the number of points plotted before the point. Since it is possible that
    /// some points are invalid, this number can be smaller than <see cref="RowIndex" />.
    /// </summary>
    public int PlotIndex
    {
      get
      {
        return _plotIndex;
      }
      set
      {
        _plotIndex = value;
      }
    }
  }
}
