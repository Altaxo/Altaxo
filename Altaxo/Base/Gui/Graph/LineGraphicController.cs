#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using Altaxo.Main.GUI;
using Altaxo.Graph.G2D;
using Altaxo.Graph.G2D.Shapes;

namespace Altaxo.Gui.Graph
{
  public interface ILineGraphicView
  {
    PenHolder DocPen { get; set; }
    System.Drawing.PointF DocPosition { get; set; }
    System.Drawing.SizeF DocSize { get; set; }
    float DocRotation { get; set; }
  }
  public interface ILineGraphicViewEventSink
  {
  }

  [UserControllerForObject(typeof(LineGraphic),101)]
  public class LineGraphicController : ILineGraphicViewEventSink, IMVCAController
  {
    ILineGraphicView _view;
    LineGraphic _doc;
    LineGraphic _tempdoc;

    #region IMVCController Members

    public LineGraphicController(LineGraphic doc)
    {
      _doc = doc;
      _tempdoc = (LineGraphic)doc.Clone();
      Initialize(true);
    }

    void Initialize(bool bInit)
    {
      if (_view != null)
      {
        _view.DocPen = _tempdoc.Pen;
        _view.DocPosition = _tempdoc.Position;
        _view.DocSize = _tempdoc.Size;
        _view.DocRotation = _tempdoc.Rotation;
      }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
   //     if (_view != null)
   //       _view.Controller = null;

        _view = value as ILineGraphicView;

        Initialize(false);

  //      if (_view != null)
    //      _view.Controller = this;
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc.Pen = _view.DocPen;
      _doc.Position = _view.DocPosition;
      _doc.Size = _view.DocSize;
      _doc.Rotation = _view.DocRotation;
      return true;
    }

    #endregion
  }
}
