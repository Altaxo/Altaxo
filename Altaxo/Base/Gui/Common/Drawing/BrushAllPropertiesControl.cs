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

using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public partial class BrushAllPropertiesControl : UserControl, IBrushViewAdvanced,  IMVCAController
  {
    public BrushAllPropertiesControl()
    {
      InitializeComponent();
    }

    public BrushX Brush
    {
      get
      {
        return _brushGlue.Brush;
      }
      set
      {
        _brushGlue.Brush = value;
      }
    }


    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return this;
      }
      set
      {
        throw new Exception("The method or operation is not implemented.");
      }
    }

    public object ModelObject
    {
      get { return _brushGlue.Brush; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      return true;
    }

    #endregion

    private void EhBrushChanged(object sender, EventArgs e)
    {
      _brushPreviewPanel.Invalidate();
    }

    private void EhBrushPreview_Paint(object sender, PaintEventArgs e)
    {
      Graphics grfx = e.Graphics;
      Rectangle fullRect = this._brushPreviewPanel.ClientRectangle;
      fullRect.Inflate(-fullRect.Width / 16, -fullRect.Height / 16);

      if (_brushGlue != null)
      {
        grfx.FillRectangle(Brushes.White, fullRect);
        Rectangle r2 = fullRect;
        r2.Inflate(-r2.Width / 4, -r2.Height / 4);
        grfx.FillRectangle(Brushes.Black, r2);

        _brushGlue.Brush.Rectangle = fullRect;
        grfx.FillRectangle(_brushGlue.Brush, fullRect);
      }

    }
  }
}
