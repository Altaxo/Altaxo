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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Interaction logic for DashStyleComboBox.xaml
  /// </summary>
  public partial class DashStyleComboBox : EditableImageComboBox
  {
    #region Converter

    private class CC : IValueConverter
    {
      private ComboBox _cb;
      private object _originalToolTip;

      public CC(ComboBox c)
      {
        _cb = c;
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        var val = (DashStyleEx)value;
        if (val.IsKnownStyle)
          return val.ToString();
        else
        {
          var stb = new StringBuilder();
          var arr = val.CustomStyle;
          for (int i = 0; i < arr.Length - 1; ++i)
            stb.AppendFormat("{0}; ", arr[i]);
          stb.AppendFormat("{0}", arr[arr.Length - 1]);
          return stb.ToString();
        }
      }

      private static DashStyleEx ConvertFromText(string text, out string error)
      {
        error = null;
        text = text.Trim();
        if (_knownStylesDict.ContainsKey(text))
          return _knownStylesDict[text];

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

        return new DashStyleEx(valList.ToArray());
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        string text = (string)value;
        var result = ConvertFromText(text, out var error);

        if (error is null)
          return result;
        else
          return Binding.DoNothing;
      }

      public string EhValidateText(object obj, System.Globalization.CultureInfo info)
      {
        string text = (string)obj;
        var result = ConvertFromText(text, out var error);

        if (error is not null)
        {
          _originalToolTip = _cb.ToolTip;
          _cb.ToolTip = error;
        }
        else
        {
          _cb.ToolTip = _originalToolTip;
          _originalToolTip = null;
        }

        return error;
      }
    }

    #endregion Converter

    private static Dictionary<DashStyleEx, ImageSource> _cachedImages = new Dictionary<DashStyleEx, ImageSource>();
    private CC _valueConverter;
    private static Dictionary<string, DashStyleEx> _knownStylesDict = new Dictionary<string, DashStyleEx>();
    private static DashStyleEx[] _knownStylesList;

    static DashStyleComboBox()
    {
      _knownStylesList = new DashStyleEx[] { DashStyleEx.Solid, DashStyleEx.Dash, DashStyleEx.Dot, DashStyleEx.DashDot, DashStyleEx.DashDotDot };

      foreach (var e in _knownStylesList)
        _knownStylesDict.Add(e.ToString(), e);
    }

    public DashStyleComboBox()
    {
      InitializeComponent();

      foreach (var e in _knownStylesList)
        Items.Add(new ImageComboBoxItem(this, e));

      var _valueBinding = new Binding
      {
        Source = this,
        Path = new PropertyPath(_nameOfValueProp)
      };
      _valueConverter = new CC(this);
      _valueBinding.Converter = _valueConverter;
      _valueBinding.ValidationRules.Add(new ValidationWithErrorString(_valueConverter.EhValidateText));
      SetBinding(ComboBox.TextProperty, _valueBinding);

      _img.Source = GetImage(SelectedDashStyle);
    }

    #region Dependency property

    private const string _nameOfValueProp = "SelectedDashStyle";

    public DashStyleEx SelectedDashStyle
    {
      get { return (DashStyleEx)GetValue(SelectedDashStyleProperty); }
      set { SetValue(SelectedDashStyleProperty, value); }
    }

    public static readonly DependencyProperty SelectedDashStyleProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(DashStyleEx), typeof(DashStyleComboBox),
        new FrameworkPropertyMetadata(DashStyleEx.Solid, OnSelectedDashStyleChanged));

    private static void OnSelectedDashStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((DashStyleComboBox)obj).EhSelectedDashStyleChanged(obj, args);
    }

    #endregion Dependency property

    protected virtual void EhSelectedDashStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (_img is not null)
      {
        var val = (DashStyleEx)args.NewValue;
        _img.Source = GetImage(val);
      }
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (DashStyleEx)item;
      if (!_cachedImages.TryGetValue(val, out var result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public override string GetItemText(object item)
    {
      return (string)_valueConverter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
    }

    public static ImageSource GetImage(DashStyleEx val)
    {
      const double height = 1;
      const double width = 2;
      const double lineWidth = height / 5;

      DashStyle dashStyle = DashStyles.Solid;

      if (val.IsKnownStyle)
      {
        if (val == DashStyleEx.Solid)
          dashStyle = DashStyles.Solid;
        else if (val == DashStyleEx.Dash)
          dashStyle = DashStyles.Dash;
        else if (val == DashStyleEx.Dot)
          dashStyle = DashStyles.Dot;
        else if (val == DashStyleEx.DashDot)
          dashStyle = DashStyles.DashDot;
        else if (val == DashStyleEx.DashDotDot)
          dashStyle = DashStyles.DashDotDot;
        else if (val == DashStyleEx.LongDash)
          dashStyle = new DashStyle(new double[] { 5, 2 }, 0);
      }
      else if (val.IsCustomStyle)
      {
        var list = new List<double>();
        foreach (var e in val.CustomStyle)
          list.Add(e);
        dashStyle = new DashStyle(list, 0);
      }

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
  }
}
