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
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Graph.Gdi.Shapes;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Gdi
{
  public interface IItemLocationDirectView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IItemLocationDirectView))]
  [UserControllerForObject(typeof(ItemLocationDirect))]
  public class ItemLocationDirectController : MVCANControllerEditOriginalDocBase<ItemLocationDirect, IItemLocationDirectView>
  {
    private PointD2D _parentSize;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% Parent X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Parent Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

    AnchoringController _localAnchoringController;
    AnchoringController _parentAnchoringController;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_localAnchoringController, () => _localAnchoringController = null);
      yield return new ControllerAndSetNullMethod(_parentAnchoringController, () => _parentAnchoringController = null);
    }

    #region Bindings

    #region Size

    private bool _areSizeElementsEnabled = true;

    public bool AreSizeElementsEnabled
    {
      get => _areSizeElementsEnabled;
      set
      {
        if (!(_areSizeElementsEnabled == value))
        {
          _areSizeElementsEnabled = value;
          OnPropertyChanged(nameof(AreSizeElementsEnabled));
        }
      }
    }


    private bool _areSizeElementsVisible = true;

    public bool AreSizeElementsVisible
    {
      get => _areSizeElementsVisible;
      set
      {
        if (!(_areSizeElementsVisible == value))
        {
          _areSizeElementsVisible = value;
          OnPropertyChanged(nameof(AreSizeElementsVisible));
        }
      }
    }


    private QuantityWithUnitGuiEnvironment _xSizeEnvironment = SizeEnvironment.Instance;

    public QuantityWithUnitGuiEnvironment XSizeEnvironment
    {
      get => _xSizeEnvironment;
      set
      {
        if (!(_xSizeEnvironment == value))
        {
          _xSizeEnvironment = value;
          OnPropertyChanged(nameof(XSizeEnvironment));
        }
      }
    }

    private DimensionfulQuantity _xSize;

    public DimensionfulQuantity XSize
    {
      get => _xSize;
      set
      {
        if (!(_xSize == value))
        {
          _xSize = value;
          OnPropertyChanged(nameof(XSize));
          SizeXChanged?.Invoke(SizeX);
        }
      }
    }

    public event Action<RADouble> SizeXChanged;

    public RADouble SizeX
    {
      get
      {
        RADouble result;
        var xSize = XSize;

        if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
          result = RADouble.NewRel(xSize.Value / 100);
        else
          result = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));

        return result;
      }
      set
      {
        XSize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerXSizeUnit);
      }
    }

    private QuantityWithUnitGuiEnvironment _ySizeEnvironment = SizeEnvironment.Instance;

    public QuantityWithUnitGuiEnvironment YSizeEnvironment
    {
      get => _ySizeEnvironment;
      set
      {
        if (!(_ySizeEnvironment == value))
        {
          _ySizeEnvironment = value;
          OnPropertyChanged(nameof(YSizeEnvironment));
        }
      }
    }

    private DimensionfulQuantity _ySize;

    public DimensionfulQuantity YSize
    {
      get => _ySize;
      set
      {
        if (!(_ySize == value))
        {
          _ySize = value;
          OnPropertyChanged(nameof(YSize));
          SizeYChanged?.Invoke(SizeY);
        }
      }
    }

    public event Action<RADouble> SizeYChanged;

    public RADouble SizeY
    {
      get
      {
        RADouble result;
        var ySize = YSize;

        if (object.ReferenceEquals(ySize.Unit, _percentLayerXSizeUnit))
          result = RADouble.NewRel(ySize.Value / 100);
        else
          result = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));

        return result;
      }
      set
      {
        YSize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerYSizeUnit);
      }
    }

    #endregion

    #region Position

    private bool _arePositionElementsEnabled = true;

    public bool ArePositionElementsEnabled
    {
      get => _arePositionElementsEnabled;
      set
      {
        if (!(_arePositionElementsEnabled == value))
        {
          _arePositionElementsEnabled = value;
          OnPropertyChanged(nameof(ArePositionElementsEnabled));
        }
      }
    }

    private bool _arePositionElementsVisible = true;

    public bool ArePositionElementsVisible
    {
      get => _arePositionElementsVisible;
      set
      {
        if (!(_arePositionElementsVisible == value))
        {
          _arePositionElementsVisible = value;
          OnPropertyChanged(nameof(ArePositionElementsVisible));
        }
      }
    }


    private QuantityWithUnitGuiEnvironment _xPositionEnvironment = PositionEnvironment.Instance;

    public QuantityWithUnitGuiEnvironment XPositionEnvironment
    {
      get => _xPositionEnvironment;
      set
      {
        if (!(_xPositionEnvironment == value))
        {
          _xPositionEnvironment = value;
          OnPropertyChanged(nameof(XPositionEnvironment));
        }
      }
    }

    private DimensionfulQuantity _xPosition;

    public DimensionfulQuantity XPosition
    {
      get => _xPosition;
      set
      {
        if (!(_xPosition == value))
        {
          _xPosition = value;
          OnPropertyChanged(nameof(XPosition));
        }
      }
    }

    private QuantityWithUnitGuiEnvironment _yPositionEnvironment = PositionEnvironment.Instance;

    public QuantityWithUnitGuiEnvironment YPositionEnvironment
    {
      get => _yPositionEnvironment;
      set
      {
        if (!(_yPositionEnvironment == value))
        {
          _yPositionEnvironment = value;
          OnPropertyChanged(nameof(YPositionEnvironment));
        }
      }
    }

    private DimensionfulQuantity _yPosition;

    public DimensionfulQuantity YPosition
    {
      get => _yPosition;
      set
      {
        if (!(_yPosition == value))
        {
          _yPosition = value;
          OnPropertyChanged(nameof(YPosition));
        }
      }
    }




    #endregion Position

    #region Rotation

    public QuantityWithUnitGuiEnvironment RotationEnvironment => AngleEnvironment.Instance;


    private DimensionfulQuantity _rotation;

    public DimensionfulQuantity Rotation
    {
      get => _rotation;
      set
      {
        if (!(_rotation == value))
        {
          _rotation = value;
          OnPropertyChanged(nameof(Rotation));
        }
      }
    }

    #endregion

    #region Shear

    public QuantityWithUnitGuiEnvironment ShearEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _shear;

    public DimensionfulQuantity Shear
    {
      get => _shear;
      set
      {
        if (!(_shear == value))
        {
          _shear = value;
          OnPropertyChanged(nameof(Shear));
        }
      }
    }

    #endregion

    #region Scale


    private bool _areScaleElementsEnabled = true;

    public bool AreScaleElementsEnabled
    {
      get => _areScaleElementsEnabled;
      set
      {
        if (!(_areScaleElementsEnabled == value))
        {
          _areScaleElementsEnabled = value;
          OnPropertyChanged(nameof(AreScaleElementsEnabled));
        }
      }
    }

    private bool _areScaleElementsVisible = true;

    public bool AreScaleElementsVisible
    {
      get => _areScaleElementsVisible;
      set
      {
        if (!(_areScaleElementsVisible == value))
        {
          _areScaleElementsVisible = value;
          OnPropertyChanged(nameof(AreScaleElementsVisible));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment ScaleEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _xScale;

    public DimensionfulQuantity XScale
    {
      get => _xScale;
      set
      {
        if (!(_xScale == value))
        {
          _xScale = value;
          OnPropertyChanged(nameof(XScale));
          ScaleXChanged?.Invoke(_xScale.AsValueInSIUnits);
        }
      }
    }

    public double ScaleX
    {
      get
      {
        return XScale.AsValueInSIUnits;
      }
      set
      {
        XScale = new DimensionfulQuantity(value, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
      }
    }

    private DimensionfulQuantity _yScale;

    public DimensionfulQuantity YScale
    {
      get => _yScale;
      set
      {
        if (!(_yScale == value))
        {
          _yScale = value;
          OnPropertyChanged(nameof(YScale));
          ScaleYChanged?.Invoke(_yScale.AsValueInSIUnits);
        }
      }
    }

    public double ScaleY
    {
      get
      {
        return YScale.AsValueInSIUnits;
      }
      set
      {
        YScale = new DimensionfulQuantity(value, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
      }
    }

    public event Action<double> ScaleXChanged;
    public event Action<double> ScaleYChanged;

    #endregion

    #region Anchor

    protected bool _areAnchorElementsEnabled = true;

    public bool AreAnchorElementsEnabled
    {
      get => _areAnchorElementsEnabled;
      set
      {
        if (!(_areAnchorElementsEnabled == value))
        {
          _areAnchorElementsEnabled = value;
          OnPropertyChanged(nameof(AreAnchorElementsEnabled));
        }
      }
    }


    bool _areAnchorElementsVisible = true;
    public bool AreAnchorElementsVisible
    {
      get => _areAnchorElementsVisible;
      set
      {
        if (!(_areAnchorElementsVisible == value))
        {
          _areAnchorElementsVisible = value;
          OnPropertyChanged(nameof(AreAnchorElementsVisible));
        }
      }
    }


    public AnchoringController LocalAnchoringController => _localAnchoringController;

    public AnchoringController ParentAnchoringController => _parentAnchoringController;


    #endregion



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _parentSize = _doc.ParentSize;
        _percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.X, AUL.Point.Instance);
        _percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.Y, AUL.Point.Instance);

        XSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
        YSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });

        XPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
        YPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });

        // Size
        AreSizeElementsVisible = !_doc.IsAutoSized;
        AreSizeElementsEnabled = true;

        if (!_doc.IsAutoSized)
        {
          XSize = _doc.SizeX.IsAbsolute ? new DimensionfulQuantity(_doc.SizeX.Value, AUL.Point.Instance).AsQuantityIn(SizeEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.SizeX.Value * 100, _percentLayerXSizeUnit);
          YSize = _doc.SizeY.IsAbsolute ? new DimensionfulQuantity(_doc.SizeY.Value, AUL.Point.Instance).AsQuantityIn(SizeEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.SizeY.Value * 100, _percentLayerYSizeUnit);
        }

        // Position
        XPosition = _doc.PositionX.IsAbsolute ? new DimensionfulQuantity(_doc.PositionX.Value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.PositionX.Value * 100, _percentLayerXSizeUnit);
        YPosition = _doc.PositionY.IsAbsolute ? new DimensionfulQuantity(_doc.PositionY.Value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit) : new DimensionfulQuantity(_doc.PositionY.Value * 100, _percentLayerYSizeUnit);

        Rotation = new DimensionfulQuantity(_doc.Rotation, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);

        Shear = new DimensionfulQuantity(_doc.ShearX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);
        XScale = new DimensionfulQuantity(_doc.ScaleX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
        YScale = new DimensionfulQuantity(_doc.ScaleY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);

        _localAnchoringController = new AnchoringController();
        _localAnchoringController.InitializeDocument(new AnchoringModel { Title = "Local anchor", ReferenceSize = _doc.AbsoluteSize, PivotX = _doc.LocalAnchorX, PivotY = _doc.LocalAnchorY });

        _parentAnchoringController = new AnchoringController();
        _parentAnchoringController.InitializeDocument(new AnchoringModel { Title = "Parent anchor", ReferenceSize = _doc.ParentSize, PivotX = _doc.ParentAnchorX, PivotY = _doc.ParentAnchorY });
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc.Rotation = Rotation.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
        _doc.ShearX = Shear.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
        _doc.ScaleX = XScale.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
        _doc.ScaleY = YScale.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);

        if (!_doc.IsAutoSized)
        {
          var xSize = XSize;
          var ySize = YSize;

          if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
            _doc.SizeX = RADouble.NewRel(xSize.Value / 100);
          else
            _doc.SizeX = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));

          if (object.ReferenceEquals(ySize.Unit, _percentLayerYSizeUnit))
            _doc.SizeY = RADouble.NewRel(ySize.Value / 100);
          else
            _doc.SizeY = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));
        }

        var xPos = XPosition;
        var yPos = YPosition;

        if (object.ReferenceEquals(xPos.Unit, _percentLayerXSizeUnit))
          _doc.PositionX = RADouble.NewRel(xPos.Value / 100);
        else
          _doc.PositionX = RADouble.NewAbs(xPos.AsValueIn(AUL.Point.Instance));

        if (object.ReferenceEquals(yPos.Unit, _percentLayerYSizeUnit))
          _doc.PositionY = RADouble.NewRel(yPos.Value / 100);
        else
          _doc.PositionY = RADouble.NewAbs(yPos.AsValueIn(AUL.Point.Instance));

        if (false == _localAnchoringController.Apply(disposeController))
          return ApplyEnd(false, disposeController);
        if (false == _parentAnchoringController.Apply(disposeController))
          return ApplyEnd(false, disposeController);

        var localAnchor = (AnchoringModel)_localAnchoringController.ModelObject;
        var parentAnchor = (AnchoringModel)_parentAnchoringController.ModelObject;

        _doc.LocalAnchorX = localAnchor.PivotX;
        _doc.LocalAnchorY = localAnchor.PivotY;

        _doc.ParentAnchorX = parentAnchor.PivotX;
        _doc.ParentAnchorY = parentAnchor.PivotY;
      }
      catch (Exception)
      {
        return false; // indicate that something failed
      }

      return ApplyEnd(true, disposeController);
    }

    #region Service members


    public void ShowSizeElements(bool isVisible, bool isEnabled)
    {
      AreSizeElementsVisible = isVisible;
      AreSizeElementsEnabled = isEnabled;
    }

    public void ShowScaleElements(bool isVisible, bool isEnabled)
    {
      AreScaleElementsVisible = isVisible;
      AreScaleElementsEnabled = isEnabled;
    }

    public void ShowPositionElements(bool isVisible, bool isEnabled)
    {
      ArePositionElementsVisible = isVisible;
      ArePositionElementsEnabled = isEnabled;
    }

    public void ShowAnchorElements(bool isVisible, bool isEnabled)
    {
      AreAnchorElementsVisible = isVisible;
      AreAnchorElementsEnabled = isEnabled;

    }





    #endregion Service members
  }
}
