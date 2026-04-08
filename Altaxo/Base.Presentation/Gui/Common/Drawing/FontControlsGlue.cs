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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Drawing
{
  /// <summary>
  /// Synchronizes font-related controls with a <see cref="FontX"/> instance.
  /// </summary>
  public class FontXControlsGlue : FrameworkElement
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FontXControlsGlue"/> class.
    /// </summary>
    public FontXControlsGlue()
    {
    }

    #region Font

    private FontX _fontX;

    /// <summary>
    /// Gets or sets the current font model.
    /// </summary>
    protected virtual FontX FontX
    {
      get
      {
        return _fontX;
      }
      set
      {
        _fontX = value;
      }
    }

    /// <summary>
    /// Gets or sets the selected font.
    /// </summary>
    public FontX SelectedFont
    {
      get
      {
        return FontX;
      }
      set
      {
        FontX = value;

        if (CbFontFamily is not null)
          CbFontFamily.SelectedFontFamilyName = GdiFontManager.GetValidFontFamilyName(FontX);
        if (_cbFontStyle is not null)
          CbFontStyle.SelectedFontStyle = FontX.Style;
        if (CbFontSize is not null)
          CbFontSize.SelectedQuantityAsValueInPoints = FontX.Size;
      }
    }

    /// <summary>
    /// Occurs when <see cref="SelectedFont"/> changes.
    /// </summary>
    public event EventHandler? SelectedFontChanged;

    /// <summary>
    /// Raises the <see cref="SelectedFontChanged"/> event.
    /// </summary>
    protected virtual void OnSelectedFontChanged()
    {
      if (SelectedFontChanged is not null)
        SelectedFontChanged(this, EventArgs.Empty);
    }

    #endregion Font

    #region Font

    private FontFamilyComboBox _cbFontFamily;

    /// <summary>
    /// Gets or sets the font family ComboBox.
    /// </summary>
    public FontFamilyComboBox CbFontFamily
    {
      get { return _cbFontFamily; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(FontFamilyComboBox.SelectedFontFamilyNameProperty, typeof(FontFamilyComboBox));
        if (dpd is null)
          throw new InvalidOperationException("DependencePropertyDescriptor is null! Please check the corresponding DependencyProperty");

        if (_cbFontFamily is not null)
          dpd.RemoveValueChanged(_cbFontFamily, EhFontFamily_SelectionChangeCommitted);

        _cbFontFamily = value;
        if (FontX is not null && _cbFontFamily is not null)
          _cbFontFamily.SelectedFontFamilyName = GdiFontManager.GetValidFontFamilyName(FontX);

        if (_cbFontFamily is not null)
          dpd.AddValueChanged(_cbFontFamily, EhFontFamily_SelectionChangeCommitted);
      }
    }

    private void EhFontFamily_SelectionChangeCommitted(object? sender, EventArgs e)
    {
      if (FontX is not null)
      {
        FontX = FontX.WithFamily(_cbFontFamily.SelectedFontFamilyName);
        OnSelectedFontChanged();
      }
    }

    #endregion Font

    #region Style

    private FontStyleComboBox _cbFontStyle;

    /// <summary>
    /// Gets or sets the font style ComboBox.
    /// </summary>
    public FontStyleComboBox CbFontStyle
    {
      get { return _cbFontStyle; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(FontStyleComboBox.SelectedFontStyleProperty, typeof(FontStyleComboBox));

        if (_cbFontStyle is not null)
        {
          dpd.RemoveValueChanged(_cbFontStyle, EhFontStyle_SelectionChangeCommitted);
        }

        _cbFontStyle = value;
        if (FontX is not null && _cbFontStyle is not null)
          _cbFontStyle.SelectedFontStyle = FontX.Style;

        if (_cbFontStyle is not null)
        {
          dpd.AddValueChanged(_cbFontStyle, EhFontStyle_SelectionChangeCommitted);
        }
      }
    }

    private void EhFontStyle_SelectionChangeCommitted(object? sender, EventArgs e)
    {
      if (FontX is not null)
      {
        FontX = FontX.WithStyle(_cbFontStyle.SelectedFontStyle);
        OnSelectedFontChanged();
      }
    }

    #endregion Style

    #region Size

    private FontSizeComboBox _cbFontSize;

    /// <summary>
    /// Gets or sets the font size ComboBox.
    /// </summary>
    public FontSizeComboBox CbFontSize
    {
      get { return _cbFontSize; }
      set
      {
        if (_cbFontSize is not null)
        {
          _cbFontSize.SelectedQuantityChanged -= EhFontSize_SelectionChangeCommitted;
        }

        _cbFontSize = value;
        if (FontX is not null && _cbFontSize is not null)
          _cbFontSize.SelectedQuantityAsValueInPoints = FontX.Size;

        if (_cbFontSize is not null)
        {
          _cbFontSize.SelectedQuantityChanged += EhFontSize_SelectionChangeCommitted;
        }
      }
    }

    private void EhFontSize_SelectionChangeCommitted(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
      if (FontX is not null)
      {
        FontX = FontX.WithSize(_cbFontSize.SelectedQuantityAsValueInPoints);
        OnSelectedFontChanged();
      }
    }

    #endregion Size
  }

  /// <summary>
  /// Synchronizes font-related controls with a <see cref="Altaxo.Drawing.D3D.FontX3D"/> instance.
  /// </summary>
  public class FontX3DControlsGlue : FontXControlsGlue
  {
    private Altaxo.Drawing.D3D.FontX3D _fontX3D;

    /// <inheritdoc/>
    protected override FontX FontX
    {
      get
      {
        return _fontX3D?.Font;
      }
      set
      {
        _fontX3D = new Altaxo.Drawing.D3D.FontX3D(value, _fontX3D.Depth);
      }
    }

    /// <summary>
    /// Gets or sets the current 3D font model.
    /// </summary>
    protected virtual Altaxo.Drawing.D3D.FontX3D FontX3D
    {
      get
      {
        return _fontX3D;
      }
      set
      {
        _fontX3D = value;
      }
    }

    /// <summary>
    /// Gets or sets the selected 3D font.
    /// </summary>
    public new Altaxo.Drawing.D3D.FontX3D SelectedFont
    {
      get
      {
        return _fontX3D;
      }
      set
      {
        _fontX3D = value;

        if (CbFontFamily is not null)
          CbFontFamily.SelectedFontFamilyName = GdiFontManager.GetValidFontFamilyName(FontX);
        if (CbFontStyle is not null)
          CbFontStyle.SelectedFontStyle = FontX.Style;
        if (CbFontSize is not null)
          CbFontSize.SelectedQuantityAsValueInPoints = FontX.Size;
        if (CbFontDepth is not null)
          CbFontDepth.SelectedQuantityAsValueInPoints = FontX3D.Depth;
      }
    }

    #region Size

    private FontSizeComboBox _cbFontDepth;

    /// <summary>
    /// Gets or sets the font depth ComboBox.
    /// </summary>
    public FontSizeComboBox CbFontDepth
    {
      get { return _cbFontDepth; }
      set
      {
        if (_cbFontDepth is not null)
        {
          _cbFontDepth.SelectedQuantityChanged -= EhFontDepth_SelectionChangeCommitted;
        }

        _cbFontDepth = value;
        if (FontX3D is not null && _cbFontDepth is not null)
          _cbFontDepth.SelectedQuantityAsValueInPoints = FontX3D.Depth;

        if (_cbFontDepth is not null)
        {
          _cbFontDepth.SelectedQuantityChanged += EhFontDepth_SelectionChangeCommitted;
        }
      }
    }

    private void EhFontDepth_SelectionChangeCommitted(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
      if (FontX is not null)
      {
        FontX3D = FontX3D.WithDepth(_cbFontDepth.SelectedQuantityAsValueInPoints);
        OnSelectedFontChanged();
      }
    }

    #endregion Size
  }
}
