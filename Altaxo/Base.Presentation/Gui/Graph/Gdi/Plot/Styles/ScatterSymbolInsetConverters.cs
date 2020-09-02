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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Frames;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Converts a type, which must be the type of a non-abstract subclass which implements <see cref="IScatterSymbolInset"/>, to an image.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class ScatterSymbolInsetTypeToImageSourceConverter : IValueConverter
  {
    private ScatterSymbolToImageSourceConverter _innerConverter = new ScatterSymbolToImageSourceConverter();

    public double SymbolSize
    {
      get { return _innerConverter.SymbolSize; }
      set { _innerConverter.SymbolSize = value; }
    }

    private IScatterSymbol _referenceSymbol = new Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame()).WithPlotColorInfluence(PlotColorInfluence.FrameColorFull | PlotColorInfluence.InsetColorFull);

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var insetType = value as Type;

      if (insetType is null)
        return null;

      var inset = (IScatterSymbolInset)Activator.CreateInstance(insetType);

      var symbol = _referenceSymbol.WithInset(inset);

      return _innerConverter.Convert(symbol, targetType, parameter, culture);
    }

    private PathFigure GetPathFigure(List<ClipperLib.IntPoint> polygon, double symbolSize)
    {
      if (polygon is null || polygon.Count <= 2)
        return null;

      return new PathFigure(
        new System.Windows.Point(polygon[0].X * symbolSize * SymbolBase.InverseClipperScalingToSymbolSize1, -polygon[0].Y * symbolSize * SymbolBase.InverseClipperScalingToSymbolSize1),
        polygon.Skip(1).Select(pp => new LineSegment(new System.Windows.Point(pp.X * symbolSize * SymbolBase.InverseClipperScalingToSymbolSize1, -pp.Y * symbolSize * SymbolBase.InverseClipperScalingToSymbolSize1), false)), true);
    }

    private PathGeometry GetPathGeometry(List<List<ClipperLib.IntPoint>> polygon, double symbolSize)
    {
      return new PathGeometry(polygon.Where(p => p is not null && p.Count > 2).Select(p => GetPathFigure(p, symbolSize)));
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class ScatterSymbolInsetToItemNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value?.GetType().Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
