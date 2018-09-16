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
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;
using sd = System.Drawing;

namespace Altaxo.Gui.Graph
{
  public class GdiFontGlue
  {
    private double _fontSize = 12;
    private string _fontFamilyName = GdiFontManager.GenericSansSerifFontFamilyName;
    private FontXStyle _fontStyle = FontXStyle.Regular;

    public double FontSize
    {
      get { return _fontSize; }
      set
      {
        var oldValue = _fontSize;
        _fontSize = value;
        if (_guiFontSize != null && oldValue != value)
          _guiFontSize.SelectedQuantityAsValueInPoints = value;
      }
    }

    public string FontFamilyName
    {
      get { return _fontFamilyName; }
      set
      {
        var oldValue = _fontFamilyName;
        _fontFamilyName = value;
        if (null != _guiFontFamily && oldValue != value)
          _guiFontFamily.SelectedFontFamilyName = value;
      }
    }

    public FontXStyle FontStyle
    {
      get { return _fontStyle; }
      set
      {
        var oldValue = _fontStyle;
        _fontStyle = value;
        if (null != _guiFontStyle && oldValue != value)
          _guiFontStyle.SelectedFontStyle = value;
      }
    }

    public event EventHandler SelectedFontChanged;

    public FontX SelectedFont
    {
      get
      {
        return GdiFontManager.GetFontX(_fontFamilyName, _fontSize, _fontStyle);
      }
      set
      {
        FontSize = value.Size;
        FontStyle = value.Style;
        FontFamilyName = GdiFontManager.GetValidFontFamilyName(value);
      }
    }

    private FontSizeComboBox _guiFontSize;

    public FontSizeComboBox GuiFontSize
    {
      get { return _guiFontSize; }
      set
      {
        if (null != _guiFontSize)
          _guiFontSize.SelectedQuantityChanged -= _guiFontStyle_SelectedFontSizeChanged;

        _guiFontSize = value;
        _guiFontSize.SelectedQuantityAsValueInPoints = _fontSize;

        if (null != _guiFontSize)
          _guiFontSize.SelectedQuantityChanged += _guiFontStyle_SelectedFontSizeChanged;
      }
    }

    private FontFamilyComboBox _guiFontFamily;

    public FontFamilyComboBox GuiFontFamily
    {
      get { return _guiFontFamily; }
      set
      {
        if (null != _guiFontFamily)
          _guiFontFamily.SelectedFontFamilyNameChanged -= _guiFontStyle_SelectedFontFamilyNameChanged;

        _guiFontFamily = value;
        _guiFontFamily.SelectedFontFamilyName = _fontFamilyName;

        if (null != _guiFontFamily)
          _guiFontFamily.SelectedFontFamilyNameChanged += _guiFontStyle_SelectedFontFamilyNameChanged;
      }
    }

    private FontStyleComboBox _guiFontStyle;

    public FontStyleComboBox GuiFontStyle
    {
      get { return _guiFontStyle; }
      set
      {
        if (null != _guiFontStyle)
          _guiFontStyle.SelectedFontStyleChanged -= _guiFontStyle_SelectedFontStyleChanged;

        _guiFontStyle = value;
        _guiFontStyle.SelectedFontStyle = _fontStyle;

        if (null != _guiFontStyle)
          _guiFontStyle.SelectedFontStyleChanged += _guiFontStyle_SelectedFontStyleChanged;
      }
    }

    private void _guiFontStyle_SelectedFontSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var oldFontSize = _fontSize;
      _fontSize = _guiFontSize.SelectedQuantityAsValueInPoints;

      if (oldFontSize != _fontSize)
        OnFontChanged();
    }

    private void _guiFontStyle_SelectedFontStyleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var oldFontStyle = _fontStyle;
      _fontStyle = _guiFontStyle.SelectedFontStyle;
      if (oldFontStyle != _fontStyle)
        OnFontChanged();
    }

    private void _guiFontStyle_SelectedFontFamilyNameChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var oldFontFamily = _fontFamilyName;
      _fontFamilyName = _guiFontFamily.SelectedFontFamilyName;
      if (oldFontFamily != _fontFamilyName)
        OnFontChanged();
    }

    protected virtual void OnFontChanged()
    {
      if (null != SelectedFontChanged)
        SelectedFontChanged(this, EventArgs.Empty);
    }
  }
}
