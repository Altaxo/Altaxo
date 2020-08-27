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

#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface IRegularPolygonView
  {
    IClosedPathShapeView ShapeGraphicView { get; }

    int Vertices { get; set; }

    double CornerRadiusPt { get; set; }
  }

  [UserControllerForObject(typeof(RegularPolygon), 110)]
  [ExpectedTypeOfView(typeof(IRegularPolygonView))]
  public class RegularPolygonController : MVCANControllerEditOriginalDocBase<RegularPolygon, IRegularPolygonView>
  {
    private ClosedPathShapeController _shapeCtrl;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_shapeCtrl, () => _shapeCtrl = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _shapeCtrl = new ClosedPathShapeController() { UseDocumentCopy = UseDocument.Directly };
        _shapeCtrl.InitializeDocument(_doc);
      }
      if (null != _view)
      {
        if (null == _shapeCtrl.ViewObject)
          _shapeCtrl.ViewObject = _view.ShapeGraphicView;

        _view.Vertices = _doc.NumberOfVertices;
        _view.CornerRadiusPt = _doc.CornerRadius;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_shapeCtrl.Apply(disposeController))
        return false;

      _doc.CornerRadius = _view.CornerRadiusPt;
      _doc.NumberOfVertices = _view.Vertices;

      return ApplyEnd(true, disposeController);
    }
  }
}
