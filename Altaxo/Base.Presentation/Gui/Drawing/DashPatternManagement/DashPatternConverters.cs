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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Drawing;
using Altaxo.Drawing.DashPatternManagement;
using Altaxo.Drawing.DashPatterns;

namespace Altaxo.Gui.Drawing.DashPatternManagement
{
  public class DashPatternToItemNameConverter : IValueConverter
  {
    private System.Windows.Controls.ComboBox _comboBox;
    private object _originalToolTip;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashPatternToItemNameConverter"/> class.
    /// The thus created instance is appropriate to convert IDashPattern to string, but not vice versa.
    /// </summary>
    public DashPatternToItemNameConverter()
    {
    }

    public DashPatternToItemNameConverter(System.Windows.Controls.ComboBox comboBox)
    {
      _comboBox = comboBox;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string result;
      if (value is null)
      {
        result = string.Empty;
      }
      else if (value is Altaxo.Drawing.DashPatterns.Custom)
      {
        var stb = new StringBuilder();
        var custom = ((Altaxo.Drawing.DashPatterns.Custom)value);
        for (int i = 0; i < custom.Count - 1; ++i)
          stb.AppendFormat("{0}; ", custom[i]);
        stb.AppendFormat("{0}", custom[custom.Count - 1]);
        result = stb.ToString();
      }
      else if (value is IDashPattern)
      {
        result = value.GetType().Name;
      }
      else if (value is string)
      {
        result = "DasistStuss: " + (string)value;
      }
      else
      {
        throw new ArgumentException("Unexpected class to convert: " + value.GetType().ToString(), nameof(value));
      }
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string text = (string)value;
      var result = ConvertFromText(text, out var error);

      if (error is null)
        return result; // Ok conversion to a custom Dash pattern was possible
      else
        return Binding.DoNothing; // For all other cases: do nothing, since we can not deduce from the name only to which dash pattern list the item belongs.
    }

    /// <summary>
    /// Converts text, e.g. '2;2', to a custom dash pattern style. The Gui culture is used for the number format; valid separator chars are space, tab and semicolon.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="error">The error.</param>
    /// <returns></returns>
    private static IDashPattern ConvertFromText(string text, out string error)
    {
      error = null;
      text = text.Trim();
      var parts = text.Split(new char[] { ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
      var valList = new List<double>();
      foreach (var part in parts)
      {
        var parttrimmed = part.Trim();
        if (string.IsNullOrEmpty(parttrimmed))
          continue;

        if (!Altaxo.Serialization.GUIConversion.IsDouble(parttrimmed, out var val))
          error = "Provided string can not be converted to a numeric value";
        else if (!(val > 0 && val < double.MaxValue))
          error = "One of the provided values is not a valid positive number";
        else
          valList.Add(val);
      }

      if (valList.Count < 1 && error is null) // only use this error, if there is no other error;
        error = "At least one number is neccessary";

      return error is not null ? null : new Altaxo.Drawing.DashPatterns.Custom(valList);
    }

    public string EhValidateText(object obj, System.Globalization.CultureInfo info)
    {
      string text = (string)obj;
      var result = ConvertFromText(text, out var error);

      if (_comboBox is not null)
      {
        if (error is not null)
        {
          _originalToolTip = _comboBox.ToolTip;
          _comboBox.ToolTip = error;
        }
        else
        {
          _comboBox.ToolTip = _originalToolTip;
          _originalToolTip = null;
        }
      }

      return error;
    }
  }

  public class DashPatternToImageSourceConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      const double height = 1;
      const double width = 2;
      const double lineWidth = height / 5;

      DashStyle dashStyle = null;

      if (value is IDashPattern val)
      {
        val = (IDashPattern)value;

        if (val is Solid)
          dashStyle = DashStyles.Solid;
        else if (val is Dash)
          dashStyle = DashStyles.Dash;
        else if (val is Dot)
          dashStyle = DashStyles.Dot;
        else if (val is DashDot)
          dashStyle = DashStyles.DashDot;
        else if (val is DashDotDot)
          dashStyle = DashStyles.DashDotDot;
        else
          dashStyle = new DashStyle(val, 0);
      }
      else if (value is System.Type ty)
      {
        if (ty == typeof(Solid))
          dashStyle = DashStyles.Solid;
        else if (ty == typeof(Dash))
          dashStyle = DashStyles.Dash;
        else if (ty == typeof(Dot))
          dashStyle = DashStyles.Dot;
        else if (ty == typeof(DashDot))
          dashStyle = DashStyles.DashDot;
        else if (ty == typeof(DashDotDot))
          dashStyle = DashStyles.DashDotDot;
      }

      if (dashStyle is null)
        return null;

      // draws a transparent outline to fix the borders
      var drawingGroup = new DrawingGroup();

      var geometryDrawing = new GeometryDrawing
      {
        Geometry = new RectangleGeometry(new Rect(0, 0, width, height)),
        Pen = new Pen(Brushes.Transparent, 0)
      };
      drawingGroup.Children.Add(geometryDrawing);

      geometryDrawing = new GeometryDrawing() { Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2)) };
      geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { DashStyle = dashStyle };
      drawingGroup.Children.Add(geometryDrawing);

      var geometryImage = new DrawingImage(drawingGroup);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class DashPatternToListNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var listName = DashPatternListManager.Instance.GetParentList(value as IDashPattern)?.Name;
      if (listName is not null)
      {
        var entry = DashPatternListManager.Instance.GetEntryValue(listName);
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
