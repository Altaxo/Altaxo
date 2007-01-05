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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using System.Drawing;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public class FontControlsGlue : Component
  {
   

    public FontControlsGlue()
    {
      
    }

    #region Font

    public GraphicsUnit _fontUnit = GraphicsUnit.World;
    /// <summary>
    /// The unit used to create the font.
    /// </summary>
    public GraphicsUnit FontUnit
    {
      get
      {
        return _fontUnit;
      }
      set
      {
        GraphicsUnit oldValue = _fontUnit;
        _fontUnit = value;
        if (value != oldValue)
        {
          if (_font != null)
            this.Font = new Font(_font.FontFamily, _font.Size, _font.Style, _fontUnit);
        }
      }
    }

    Font _font;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Font Font
    {
      get
      {
        return _font;
      }
      set
      {
        _font = value;
        _fontUnit = value.Unit;
       
        CbFont = _cbFont;
        CbFontSize = _cbFontSize;
      }
    }

    public event EventHandler FontChanged;
    protected virtual void OnFontChanged()
    {
      if (FontChanged != null)
        FontChanged(this, EventArgs.Empty);
    }

    #endregion

  

    #region Font

    FontComboBox _cbFont;
    public FontComboBox CbFont
    {
      get { return _cbFont; }
      set
      {
        if (_cbFont != null)
          _cbFont.SelectionChangeCommitted -= EhFont_SelectionChangeCommitted;

        _cbFont = value;
        if (_font != null && _cbFont != null)
          _cbFont.FontDocument = _font;

        if (_cbFont != null)
          _cbFont.SelectionChangeCommitted += EhFont_SelectionChangeCommitted;
      }
    }

    void EhFont_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_font != null)
      {
        Font temp = _cbFont.FontDocument;
        _font = new Font(temp.FontFamily, _font.Size, temp.Style);
        OnFontChanged();
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
          _cbFontSize.SelectionChangeCommitted -= EhFontSize_SelectionChangeCommitted;
          _cbFontSize.TextUpdate -= EhFontSize_TextChanged;
        }

        _cbFontSize = value;
        if (_font != null && _cbFontSize != null)
          _cbFontSize.FontSize = _font.Size;

        if (_cbFontSize != null)
        {
          _cbFontSize.SelectionChangeCommitted += EhFontSize_SelectionChangeCommitted;
          _cbFontSize.TextUpdate += EhFontSize_TextChanged;
        }
      }
    }

    void EhFontSize_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_font != null)
      {
        _font = new Font(_font.FontFamily,_cbFontSize.FontSize,_font.Style);
        OnFontChanged();
      }
    }
    void EhFontSize_TextChanged(object sender, EventArgs e)
    {
      if (_font != null)
      {
        _font = new Font(_font.FontFamily, _cbFontSize.FontSize, _font.Style);
        OnFontChanged();
      }
    }

    #endregion

  

  
  }
}
