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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using sd = System.Drawing;

namespace Altaxo.Gui.Common.Drawing
{
  public class FontControlsGlue : FrameworkElement
  {
    public FontControlsGlue()
    {

    }

    #region Font

    public sd.GraphicsUnit _fontUnit = sd.GraphicsUnit.World;
    /// <summary>
    /// The unit used to create the font.
    /// </summary>
    public sd.GraphicsUnit FontUnit
    {
      get
      {
        return _fontUnit;
      }
      set
      {
        sd.GraphicsUnit oldValue = _fontUnit;
        _fontUnit = value;
        if (value != oldValue)
        {
          if (_font != null)
            this.SelectedFont = new sd.Font(_font.FontFamily, _font.Size, _font.Style, _fontUnit);
        }
      }
    }

    sd.Font _font;
    public sd.Font SelectedFont
    {
      get
      {
        return _font;
      }
      set
      {
        _font = value;
        _fontUnit = value.Unit;

        if (null != CbFontFamily) CbFontFamily.SelectedFontFamily = _font.FontFamily;
        if (null != _cbFontStyle) CbFontStyle.SelectedFontStyle = _font.Style;
        if (null != CbFontSize) CbFontSize.SelectedQuantityAsValueInPoints = _font.Size;
      }
    }

    public event EventHandler SelectedFontChanged;
    protected virtual void OnSelectedFontChanged()
    {
      if (SelectedFontChanged != null)
        SelectedFontChanged(this, EventArgs.Empty);
    }

    #endregion



    #region Font

    FontFamilyComboBox _cbFontFamily;
    public FontFamilyComboBox CbFontFamily
    {
      get { return _cbFontFamily; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(FontFamilyComboBox.SelectedFontFamilyProperty, typeof(FontFamilyComboBox));
        if (null == dpd)
          throw new InvalidOperationException("DependencePropertyDescriptor is null! Please check the corresponding DependencyProperty");

        if (_cbFontFamily != null)
          dpd.RemoveValueChanged(_cbFontFamily, EhFontFamily_SelectionChangeCommitted);

        _cbFontFamily = value;
        if (_font != null && _cbFontFamily != null)
          _cbFontFamily.SelectedFontFamily = _font.FontFamily;

        if (_cbFontFamily != null)
          dpd.AddValueChanged(_cbFontFamily, EhFontFamily_SelectionChangeCommitted);
      }
    }

    void EhFontFamily_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_font != null)
      {
        _font = new sd.Font(_cbFontFamily.SelectedFontFamily, _font.Size, _font.Style, _fontUnit);
        OnSelectedFontChanged();
      }
    }

    #endregion

    #region Style

    FontStyleComboBox _cbFontStyle;
    public FontStyleComboBox CbFontStyle
    {
      get { return _cbFontStyle; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(FontStyleComboBox.SelectedFontStyleProperty, typeof(FontStyleComboBox));

        if (_cbFontStyle != null)
        {
          dpd.RemoveValueChanged(_cbFontStyle, EhFontStyle_SelectionChangeCommitted);
        }

        _cbFontStyle = value;
        if (_font != null && _cbFontStyle != null)
          _cbFontStyle.SelectedFontStyle = _font.Style;

        if (_cbFontStyle != null)
        {
          dpd.AddValueChanged(_cbFontStyle, EhFontStyle_SelectionChangeCommitted);
        }
      }
    }

    void EhFontStyle_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_font != null)
      {
        _font = new sd.Font(_font.FontFamily, _font.Size, _cbFontStyle.SelectedFontStyle, _fontUnit);
        OnSelectedFontChanged();
      }
    }


    #endregion

    #region Size

    FontSizeComboBox _cbFontSize;
    public FontSizeComboBox CbFontSize
    {
      get { return _cbFontSize; }
      set
      {
        if (_cbFontSize != null)
        {
          _cbFontSize.SelectedQuantityChanged -= EhFontSize_SelectionChangeCommitted;
        }

        _cbFontSize = value;
        if (_font != null && _cbFontSize != null)
          _cbFontSize.SelectedQuantityAsValueInPoints = _font.Size;

        if (_cbFontSize != null)
        {
          _cbFontSize.SelectedQuantityChanged += EhFontSize_SelectionChangeCommitted;
        }
      }
    }

    void EhFontSize_SelectionChangeCommitted(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
      if (_font != null)
      {
        _font = new sd.Font(_font.FontFamily, (float)_cbFontSize.SelectedQuantityAsValueInPoints, _font.Style, _fontUnit);
        OnSelectedFontChanged();
      }
    }


    #endregion




  }
}
