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
using System.Text;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Gui.Graph
{
  public interface ILineGraphicView
  {
    PenX DocPen { get; set; }
    PointD2D DocPosition { get; set; }
    PointD2D DocSize { get; set; }
    double DocRotation { get; set; }
  }
  public interface ILineGraphicViewEventSink
  {
  }

  [UserControllerForObject(typeof(OpenPathShapeBase),101)]
  [ExpectedTypeOfView(typeof(ILineGraphicView))]
  public class LineGraphicController : ILineGraphicViewEventSink, IMVCAController
  {
    ILineGraphicView _view;
    OpenPathShapeBase _doc;
    OpenPathShapeBase _tempdoc;

    #region IMVCController Members

    public LineGraphicController(OpenPathShapeBase doc)
    {
      _doc = doc;
      _tempdoc = (OpenPathShapeBase)doc.Clone();
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
