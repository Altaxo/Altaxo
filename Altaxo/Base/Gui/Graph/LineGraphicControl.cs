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

namespace Altaxo.Gui.Graph
{
  [UserControlForController(typeof(ILineGraphicViewEventSink))]
  public partial class LineGraphicControl : UserControl, ILineGraphicView
  {
    public LineGraphicControl()
    {
      InitializeComponent();
    }

    #region ILineGraphicView Members

    public PenX DocPen
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

    public PointF DocPosition
    {
      get
      {
        return _positionSizeGlue.Position;
      }
      set
      {
        _positionSizeGlue.Position = value;
      }
    }

    public SizeF DocSize
    {
      get
      {
        return _positionSizeGlue.Size;
      }
      set
      {
        _positionSizeGlue.Size = value;
      }
    }
    public float DocRotation
    {
      get
      {
        return (float)_positionSizeGlue.Rotation;
      }
      set
      {
        _positionSizeGlue.Rotation = value;
      }
    }

    #endregion
  }
}
