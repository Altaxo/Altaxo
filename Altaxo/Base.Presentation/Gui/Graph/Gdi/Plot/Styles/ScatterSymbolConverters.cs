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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Converts an instance of <see cref="IScatterSymbol"/> into an image.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class ScatterSymbolToImageSourceConverter : IValueConverter
  {
    public NamedColor PlotColor { get; set; } = NamedColors.Black;
    public double SymbolSize { get; set; } = 16;

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var symbol = value as IScatterSymbol;

      if (symbol is null)
        return null;
      GetPathGeometries(symbol, SymbolSize, out var fill, out var frame, out var inset);
      GetBrushes(symbol, PlotColor, fill, frame, inset, out var fillBrush, out var frameBrush, out var insetBrush);

      // draws a transparent outline to fix the borders
      var drawingGroup = new DrawingGroup();

      if (fill is not null)
      {
        var geometryDrawing = new GeometryDrawing
        {
          Geometry = fill,
          Brush = fillBrush
        };
        drawingGroup.Children.Add(geometryDrawing);
      }

      if (frame is not null)
      {
        var geometryDrawing = new GeometryDrawing
        {
          Geometry = frame,
          Brush = frameBrush
        };
        drawingGroup.Children.Add(geometryDrawing);
      }

      if (inset is not null)
      {
        var geometryDrawing = new GeometryDrawing
        {
          Geometry = inset,
          Brush = insetBrush
        };
        drawingGroup.Children.Add(geometryDrawing);
      }

      var geometryImage = new DrawingImage(drawingGroup);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
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

    private void GetPathGeometries(IScatterSymbol symbol, double symbolSize, out PathGeometry fill, out PathGeometry frame, out PathGeometry inset)
    {
      symbol.CalculatePolygons(null, out var framePolygon, out var insetPolygon, out var fillPolygon);

      fill = fillPolygon is null ? null : GetPathGeometry(fillPolygon, symbolSize);
      frame = framePolygon is null ? null : GetPathGeometry(framePolygon, symbolSize);
      inset = insetPolygon is null ? null : GetPathGeometry(insetPolygon, symbolSize);
    }

    private void GetBrushes(IScatterSymbol scatterSymbol, NamedColor plotColor, PathGeometry fillPath, PathGeometry framePath, PathGeometry insetPath, out Brush fillBrush, out Brush frameBrush, out Brush insetBrush)
    {
      fillBrush = frameBrush = insetBrush = null;

      var plotColorInfluence = scatterSymbol.PlotColorInfluence;

      if (insetPath is not null)
      {
        var insetColor = scatterSymbol.Inset.Color;
        if (plotColorInfluence.HasFlag(PlotColorInfluence.InsetColorFull))
          insetColor = plotColor;
        else if (plotColorInfluence.HasFlag(PlotColorInfluence.InsetColorPreserveAlpha))
          insetColor = plotColor.NewWithAlphaValue(insetColor.Color.A);

        insetBrush = new SolidColorBrush(GuiHelper.ToWpf(insetColor));
      }

      if (fillPath is not null)
      {
        var fillColor = scatterSymbol.FillColor;
        if (plotColorInfluence.HasFlag(PlotColorInfluence.FillColorFull))
          fillColor = plotColor;
        else if (plotColorInfluence.HasFlag(PlotColorInfluence.FillColorPreserveAlpha))
          fillColor = plotColor.NewWithAlphaValue(fillColor.Color.A);

        fillBrush = new SolidColorBrush(GuiHelper.ToWpf(fillColor));
      }

      if (framePath is not null)
      {
        var frameColor = scatterSymbol.Frame.Color;
        if (plotColorInfluence.HasFlag(PlotColorInfluence.FrameColorFull))
          frameColor = plotColor;
        else if (plotColorInfluence.HasFlag(PlotColorInfluence.FrameColorPreserveAlpha))
          frameColor = plotColor.NewWithAlphaValue(frameColor.Color.A);

        frameBrush = new SolidColorBrush(GuiHelper.ToWpf(frameColor));
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converts a type, which must be the type of a non-abstract subclass which implements <see cref="IScatterSymbol"/>, to an image.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class ScatterSymbolTypeToImageSourceConverter : IValueConverter
  {
    private ScatterSymbolToImageSourceConverter _innerConverter = new ScatterSymbolToImageSourceConverter();

    public double SymbolSize
    {
      get { return _innerConverter.SymbolSize; }
      set { _innerConverter.SymbolSize = value; }
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var symbolType = value as Type;

      if (symbolType is null)
        return null;

      var symbol = (IScatterSymbol)Activator.CreateInstance(symbolType);

      return _innerConverter.Convert(symbol, targetType, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class ScatterSymbolToItemNameConverter : IValueConverter
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

  public class ScatterSymbolToListNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var listName = ScatterSymbolListManager.Instance.GetParentList(value as IScatterSymbol)?.Name;
      if (listName is not null)
      {
        var entry = ScatterSymbolListManager.Instance.GetEntryValue(listName);
        string levelName = Enum.GetName(typeof(Altaxo.Main.ItemDefinitionLevel), entry.Level);
        return levelName + "/" + listName;
      }
      else
      {
        return "<<no parent list>>";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
