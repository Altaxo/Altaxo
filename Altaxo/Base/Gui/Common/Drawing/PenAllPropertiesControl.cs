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
  public partial class PenAllPropertiesControl : UserControl, IMVCAController
  {
    

    public PenAllPropertiesControl()
    {
      InitializeComponent();
    }

    public PenX Pen
    {
      get 
      {
        return _penGlue.Pen; 
      }
      set
      {
        _penGlue.Pen = value;
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
      get { return _penGlue.Pen; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      return true;
    }

    #endregion

    private void EhPenChanged(object sender, EventArgs e)
    {
      _LineDesignPanel.Invalidate();
    }

    private void EhPenPreview_Paint(object sender, PaintEventArgs e)
    {
      Graphics grfx = e.Graphics;
      Rectangle fullRect = _LineDesignPanel.ClientRectangle;

      PointF p1 = new PointF(fullRect.Left + fullRect.Width / 4.0f, fullRect.Top + fullRect.Height / 2.0f);
      PointF p2 = new PointF(fullRect.Right - fullRect.Width / 4.0f, fullRect.Top + fullRect.Height / 2.0f);

      if(_penGlue!=null)
        grfx.DrawLine(_penGlue.Pen, p1, p2);
 
    }
  }
}
