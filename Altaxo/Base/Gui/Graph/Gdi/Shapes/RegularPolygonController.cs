#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface IRegularPolygonView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(RegularPolygon), 110)]
  [ExpectedTypeOfView(typeof(IRegularPolygonView))]
  public class RegularPolygonController : MVCANControllerEditOriginalDocBase<RegularPolygon, IRegularPolygonView>
  {


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_shapeCtrl, () => ShapeCtrl = null);
    }

    #region Bindings

    private ClosedPathShapeController _shapeCtrl;

    public ClosedPathShapeController ShapeCtrl
    {
      get => _shapeCtrl;
      set
      {
        if (!(_shapeCtrl == value))
        {
          _shapeCtrl?.Dispose();
          _shapeCtrl = value;
          OnPropertyChanged(nameof(ShapeCtrl));
        }
      }
    }


    private int _vertices;

    public int Vertices
    {
      get => _vertices;
      set
      {
        if (!(_vertices == value))
        {
          _vertices = value;
          OnPropertyChanged(nameof(Vertices));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment CornerRadiusEnvironment => SizeEnvironment.Instance;
    private DimensionfulQuantity _cornerRadius;

    public DimensionfulQuantity CornerRadius
    {
      get => _cornerRadius;
      set
      {
        if (!(_cornerRadius == value))
        {
          _cornerRadius = value;
          OnPropertyChanged(nameof(CornerRadius));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var shapeCtrl = new ClosedPathShapeController() { UseDocumentCopy = UseDocument.Directly };
        shapeCtrl.InitializeDocument(_doc);
        Current.Gui.FindAndAttachControlTo(shapeCtrl);
        ShapeCtrl = shapeCtrl;

        Vertices = _doc.NumberOfVertices;
        CornerRadius = new DimensionfulQuantity(_doc.CornerRadius, Altaxo.Units.Length.Point.Instance).AsQuantityIn(CornerRadiusEnvironment.DefaultUnit);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_shapeCtrl.Apply(disposeController))
        return false;

      _doc.CornerRadius = CornerRadius.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.NumberOfVertices = Vertices;

      return ApplyEnd(true, disposeController);
    }
  }
}
