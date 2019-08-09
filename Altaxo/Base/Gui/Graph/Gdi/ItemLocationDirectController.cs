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

using System;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Gdi
{
  #region Interfaces

  public interface IItemLocationDirectView
  {
    void InitializeXPosition(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

    void InitializeYPosition(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

    void ShowSizeElements(bool isVisible, bool isEnabled);

    void ShowScaleElements(bool isVisible, bool isEnabled);

    void ShowPositionElements(bool isVisible, bool isEnabled);

    void ShowAnchorElements(bool isVisible, bool isEnabled);

    void InitializeYSize(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

    void InitializeXSize(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

    DimensionfulQuantity XPosition { get; }

    DimensionfulQuantity YPosition { get; }

    DimensionfulQuantity XSize { get; }

    DimensionfulQuantity YSize { get; }

    double Rotation { get; set; }

    double Shear { get; set; }

    double ScaleX { get; set; }

    double ScaleY { get; set; }

    void InitializePivot(RADouble pivotX, RADouble pivotY, PointD2D sizeOfOwnGraphic);

    RADouble PivotX { get; }

    RADouble PivotY { get; }

    void InitializeReference(RADouble referenceX, RADouble referenceY, PointD2D sizeOfParent);

    RADouble ReferenceX { get; }

    RADouble ReferenceY { get; }

    event Action SizeXChanged;

    event Action SizeYChanged;

    event Action ScaleXChanged;

    event Action ScaleYChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IItemLocationDirectView))]
  [UserControllerForObject(typeof(ItemLocationDirect))]
  public class ItemLocationDirectController : MVCANControllerEditOriginalDocBase<ItemLocationDirect, IItemLocationDirectView>
  {
    private PointD2D _parentSize;
    private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _xPositionEnvironment;
    private QuantityWithUnitGuiEnvironment _ySizeEnvironment, _yPositionEnvironment;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% Parent X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Parent Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

    protected bool _showPositionElements_Enabled = true, _showPositionElements_IsVisible = true;
    protected bool _showSizeElements_Enabled = true, _showSizeElements_IsVisible = true;
    protected bool _showScaleElements_Enabled = true, _showScaleElements_IsVisible = true;
    protected bool _showAnchorElements_Enabled = true, _showAnchorElements_IsVisible = true;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _parentSize = _doc.ParentSize;
        _percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.X, AUL.Point.Instance);
        _percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.Y, AUL.Point.Instance);

        _xSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
        _ySizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });

        _xPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
        _yPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });
      }
      if (null != _view)
      {
        _view.ShowSizeElements(!_doc.IsAutoSized, true);

        if (!_doc.IsAutoSized)
        {
          var xSize = _doc.SizeX.IsAbsolute ? new DimensionfulQuantity(_doc.SizeX.Value, AUL.Point.Instance).AsQuantityIn(SizeEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.SizeX.Value * 100, _percentLayerXSizeUnit);
          _view.InitializeXSize(xSize, _xSizeEnvironment);
          var ySize = _doc.SizeY.IsAbsolute ? new DimensionfulQuantity(_doc.SizeY.Value, AUL.Point.Instance).AsQuantityIn(SizeEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.SizeY.Value * 100, _percentLayerYSizeUnit);
          _view.InitializeYSize(ySize, _ySizeEnvironment);
        }

        var xPos = _doc.PositionX.IsAbsolute ? new DimensionfulQuantity(_doc.PositionX.Value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.PositionX.Value * 100, _percentLayerXSizeUnit);
        _view.InitializeXPosition(xPos, _xPositionEnvironment);
        var yPos = _doc.PositionY.IsAbsolute ? new DimensionfulQuantity(_doc.PositionY.Value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.PositionY.Value * 100, _percentLayerYSizeUnit);
        _view.InitializeYPosition(yPos, _yPositionEnvironment);

        _view.Rotation = _doc.Rotation;
        _view.Shear = _doc.ShearX;
        _view.ScaleX = _doc.ScaleX;
        _view.ScaleY = _doc.ScaleY;
        _view.InitializePivot(_doc.LocalAnchorX, _doc.LocalAnchorY, _doc.AbsoluteSize);
        _view.InitializeReference(_doc.ParentAnchorX, _doc.ParentAnchorY, _doc.ParentSize);

        _view.ShowPositionElements(_showPositionElements_IsVisible, _showPositionElements_Enabled);
        _view.ShowSizeElements(_showSizeElements_IsVisible, _showSizeElements_Enabled);
        _view.ShowScaleElements(_showScaleElements_IsVisible, _showSizeElements_Enabled);
        _view.ShowAnchorElements(_showAnchorElements_IsVisible, _showAnchorElements_Enabled);
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc.Rotation = _view.Rotation;
        _doc.ShearX = _view.Shear;
        _doc.ScaleX = _view.ScaleX;
        _doc.ScaleY = _view.ScaleY;

        if (!_doc.IsAutoSized)
        {
          var xSize = _view.XSize;
          var ySize = _view.YSize;

          if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
            _doc.SizeX = RADouble.NewRel(xSize.Value / 100);
          else
            _doc.SizeX = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));

          if (object.ReferenceEquals(ySize.Unit, _percentLayerYSizeUnit))
            _doc.SizeY = RADouble.NewRel(ySize.Value / 100);
          else
            _doc.SizeY = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));
        }

        var xPos = _view.XPosition;
        var yPos = _view.YPosition;

        if (object.ReferenceEquals(xPos.Unit, _percentLayerXSizeUnit))
          _doc.PositionX = RADouble.NewRel(xPos.Value / 100);
        else
          _doc.PositionX = RADouble.NewAbs(xPos.AsValueIn(AUL.Point.Instance));

        if (object.ReferenceEquals(yPos.Unit, _percentLayerYSizeUnit))
          _doc.PositionY = RADouble.NewRel(yPos.Value / 100);
        else
          _doc.PositionY = RADouble.NewAbs(yPos.AsValueIn(AUL.Point.Instance));

        _doc.LocalAnchorX = _view.PivotX;
        _doc.LocalAnchorY = _view.PivotY;

        _doc.ParentAnchorX = _view.ReferenceX;
        _doc.ParentAnchorY = _view.ReferenceY;
      }
      catch (Exception)
      {
        return false; // indicate that something failed
      }

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.SizeXChanged += EhSizeXChanged;
      _view.SizeYChanged += EhSizeYChanged;
      _view.ScaleXChanged += EhScaleXChanged;
      _view.ScaleYChanged += EhScaleYChanged;
    }

    protected override void DetachView()
    {
      _view.SizeXChanged -= EhSizeXChanged;
      _view.SizeYChanged -= EhSizeYChanged;
      _view.ScaleXChanged -= EhScaleXChanged;
      _view.ScaleYChanged -= EhScaleYChanged;
      base.DetachView();
    }

    #region Service members

    public event Action<RADouble> SizeXChanged;

    private void EhSizeXChanged()
    {
      var actn = SizeXChanged;
      if (null != actn)
      {
        RADouble result;
        var xSize = _view.XSize;

        if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
          result = RADouble.NewRel(xSize.Value / 100);
        else
          result = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));
        actn(result);
      }
    }

    public event Action<RADouble> SizeYChanged;

    private void EhSizeYChanged()
    {
      var actn = SizeYChanged;
      if (null != actn)
      {
        RADouble result;
        var ySize = _view.YSize;

        if (object.ReferenceEquals(ySize.Unit, _percentLayerYSizeUnit))
          result = RADouble.NewRel(ySize.Value / 100);
        else
          result = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));
        actn(result);
      }
    }

    public event Action<double> ScaleXChanged;

    private void EhScaleXChanged()
    {
      var actn = ScaleXChanged;
      if (null != actn)
      {
        actn(_view.ScaleX);
      }
    }

    public event Action<double> ScaleYChanged;

    private void EhScaleYChanged()
    {
      var actn = ScaleYChanged;
      if (null != actn)
      {
        actn(_view.ScaleY);
      }
    }

    public void ShowSizeElements(bool isVisible, bool isEnabled)
    {
      _showSizeElements_IsVisible = isVisible;
      _showSizeElements_Enabled = isEnabled;
      if (null != _view)
        _view.ShowSizeElements(isVisible, isEnabled);
    }

    public void ShowScaleElements(bool isVisible, bool isEnabled)
    {
      _showScaleElements_IsVisible = isVisible;
      _showScaleElements_Enabled = isEnabled;
      if (null != _view)
        _view.ShowScaleElements(isVisible, isEnabled);
    }

    public void ShowPositionElements(bool isVisible, bool isEnabled)
    {
      _showPositionElements_IsVisible = isVisible;
      _showPositionElements_Enabled = isEnabled;
      if (null != _view)
        _view.ShowPositionElements(isVisible, isEnabled);
    }

    public void ShowAnchorElements(bool isVisible, bool isEnabled)
    {
      _showAnchorElements_IsVisible = isVisible;
      _showAnchorElements_Enabled = isEnabled;
      if (null != _view)
        _view.ShowAnchorElements(isVisible, isEnabled);
    }

    public RADouble SizeX
    {
      get
      {
        RADouble result;
        var xSize = _view.XSize;

        if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
          result = RADouble.NewRel(xSize.Value / 100);
        else
          result = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));

        return result;
      }
      set
      {
        var xSize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerXSizeUnit);
        _view.InitializeXSize(xSize, _xSizeEnvironment);
      }
    }

    public RADouble SizeY
    {
      get
      {
        RADouble result;
        var ySize = _view.YSize;

        if (object.ReferenceEquals(ySize.Unit, _percentLayerXSizeUnit))
          result = RADouble.NewRel(ySize.Value / 100);
        else
          result = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));

        return result;
      }
      set
      {
        var ySize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerYSizeUnit);
        _view.InitializeYSize(ySize, _ySizeEnvironment);
      }
    }

    public double ScaleX
    {
      get
      {
        return _view.ScaleX;
      }
      set
      {
        _view.ScaleX = value;
      }
    }

    public double ScaleY
    {
      get
      {
        return _view.ScaleY;
      }
      set
      {
        _view.ScaleY = value;
      }
    }

    #endregion Service members
  }
}
