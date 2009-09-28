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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
  public partial class BarGraphPlotStyleControl : UserControl, IBarGraphPlotStyleView
  {
    public BarGraphPlotStyleControl()
    {
      InitializeComponent();
    }

    #region IBarGraphPlotStyleView Members

    public bool IndependentColor
    {
      get
      {
        return _chkIndependentColor.Checked;
      }
      set
      {
        _chkIndependentColor.Checked = value;
      }
    }

    public Altaxo.Graph.Gdi.BrushX FillBrush
    {
      get
      {
        return this._cbFillBrush.Brush;
      }
      set
      {
        _cbFillBrush.Brush = value;
      }
    }

    public Altaxo.Graph.Gdi.PenX FillPen
    {
      get
      {
        if (_chkFrameBar.Checked)
          return _framePenGlue.Pen;
        else
          return null;
      }
      set
      {
        _chkFrameBar.Checked = (value != null);
        _cbPenColor.Enabled = (value != null);
        if (value != null)
          _framePenGlue.Pen = value;
        else
          _framePenGlue.Pen = new Altaxo.Graph.Gdi.PenX(Color.Black);
      }
    }

    public string InnerGap
    {
      get
      {
        return _edInnerGap.Text;
      }
      set
      {
        _edInnerGap.Text = value;
      }
    }

    public string OuterGap
    {
      get
      {
        return _edOuterGap.Text;
      }
      set
      {
        _edOuterGap.Text = value;
      }
    }

    public bool UsePhysicalBaseValue
    {
      get
      {
        return false;
      }
      set
      {
      }
    }


    public string BaseValue
    {
      get
      {
        return _edBaseValue.Text;
      }
      set
      {
        _edBaseValue.Text = value;
      }
    }

    public bool StartAtPreviousItem
    {
      get
      {
        return _chkUsePreviousItem.Checked;
      }
      set
      {
        _chkUsePreviousItem.Checked = value;
        _edYGap.Enabled = value;
      }
    }

    public string YGap
    {
      get
      {
        return _edYGap.Text;
      }
      set
      {
        _edYGap.Text = value;
      }
    }

    #endregion

    private void _chkFrameBar_CheckedChanged(object sender, EventArgs e)
    {
      _cbPenColor.Enabled = _chkFrameBar.Checked;
    }

    private void _chkUsePreviousItem_CheckedChanged(object sender, EventArgs e)
    {
      _edYGap.Enabled = _chkUsePreviousItem.Checked;
    }
  }
}
