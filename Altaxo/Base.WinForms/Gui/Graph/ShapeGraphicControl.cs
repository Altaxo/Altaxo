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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph
{
  [UserControlForController(typeof(IShapeGraphicViewEventSink))]
  public partial class ShapeGraphicControl : UserControl, IShapeGraphicView
  {
    public ShapeGraphicControl()
    {
      InitializeComponent();
    }

    #region IShapeGraphicView Members

    public PenX DocPen
    {
      get
      {
        return _ctrlPenColorTypeThickness.DocPen;
      }
      set
      {
        _ctrlPenColorTypeThickness.DocPen = value;
      }
    }

    public bool IsFilled
    {
      get
      {
        return _chkFillShapeEnable.Checked;
      }
      set
      {
        _chkFillShapeEnable.Checked = value;
        _cbFillBrush.Enabled = value;
      }
    }


    public BrushX DocBrush
    {
      get
      {
        return _cbFillBrush.Brush;
      }
      set
      {
        _cbFillBrush.Brush = value;
      }
    }

    public PointD2D DocPosition
    {
      get
      {
        return _ctrlPosSize.PositionSizeGlue.Position;
      }
      set
      {
        _ctrlPosSize.PositionSizeGlue.Position = value;
      }
    }

    public PointD2D DocSize
    {
      get
      {
        return _ctrlPosSize.PositionSizeGlue.Size;
      }
      set
      {
        _ctrlPosSize.PositionSizeGlue.Size = value;
      }
    }
    public double DocRotation
    {
      get
      {
        return _ctrlPosSize.PositionSizeGlue.Rotation;
      }
      set
      {
        _ctrlPosSize.PositionSizeGlue.Rotation = value;
      }
    }

    public double DocShear
    {
      get
      {
        return _ctrlPosSize.PositionSizeGlue.Shear;
      }
      set
      {
        _ctrlPosSize.PositionSizeGlue.Shear = value;
      }
    }
    public double DocScaleX
    {
      get
      {
        return _ctrlPosSize.PositionSizeGlue.ScaleX;
      }
      set
      {
        _ctrlPosSize.PositionSizeGlue.ScaleX = value;
      }
    }
    public double DocScaleY
    {
      get
      {
        return _ctrlPosSize.PositionSizeGlue.ScaleY;
      }
      set
      {
        _ctrlPosSize.PositionSizeGlue.ScaleY = value;
      }
    }

    #endregion

    private void EhIsShapedFilled_CheckChanged(object sender, EventArgs e)
    {
      bool isFilled = _chkFillShapeEnable.Checked;
      _cbFillBrush.Enabled = isFilled;
    }
  }
}
