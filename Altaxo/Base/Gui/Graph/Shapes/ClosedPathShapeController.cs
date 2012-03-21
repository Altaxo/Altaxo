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

namespace Altaxo.Gui.Graph.Shapes
{
  public interface IClosedPathShapeView
  {
    PenX DocPen { get; set; }
    BrushX DocBrush { get; set; }
    PointD2D DocPosition { get; set; }
    PointD2D DocSize { get; set; }
    double DocRotation { get; set; }
    double DocShear { get; set; }
    double DocScaleX { get; set; }
    double DocScaleY { get; set; }
  }
 

  [UserControllerForObject(typeof(ClosedPathShapeBase))]
  [ExpectedTypeOfView(typeof(IClosedPathShapeView))]
  public class ClosedPathShapeController : MVCANControllerBase<ClosedPathShapeBase,IClosedPathShapeView>
  {
		protected override void  Initialize(bool initData)
{
      if (_view != null)
      {
        _view.DocPen = _doc.Pen;
        _view.DocBrush = _doc.Brush;
        _view.DocPosition = _doc.Position;
        _view.DocSize = _doc.Size;
        _view.DocRotation = _doc.Rotation;
        _view.DocShear = _doc.Shear;
        _view.DocScaleX = _doc.ScaleX;
        _view.DocScaleY = _doc.ScaleY;
      }
    }

  




    public override bool Apply()
    {
      _doc.Pen = _view.DocPen;
      _doc.Brush = _view.DocBrush;
      _doc.Position = _view.DocPosition;
      _doc.Size = _view.DocSize;
      _doc.Rotation = _view.DocRotation;
      _doc.Shear = _view.DocShear;
			_doc.Scale = new PointD2D(_view.DocScaleX, _view.DocScaleY);

			if (_useDocumentCopy)
				_originalDoc.CopyFrom(_doc);

      return true;
    }

  }
}
