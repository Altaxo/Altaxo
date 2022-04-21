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
using System;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Gui.Graph.Graph3D.Shapes;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Graph3D
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
    private VectorD3D _parentSize;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% Parent X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Parent Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerZSizeUnit = new ChangeableRelativePercentUnit("% Parent Z-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

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

    private QuantityWithUnitGuiEnvironment _zSizeEnvironment = SizeEnvironment.Instance;

    public QuantityWithUnitGuiEnvironment ZSizeEnvironment
    {
      get => _zSizeEnvironment;
      set
      {
        if (!(_zSizeEnvironment == value))
        {
          _zSizeEnvironment = value;
          OnPropertyChanged(nameof(ZSizeEnvironment));
        }
      }
    }


    private DimensionfulQuantity _zSize;

    public DimensionfulQuantity ZSize
    {
      get => _zSize;
      set
      {
        if (!(_zSize == value))
        {
          _zSize = value;
          OnPropertyChanged(nameof(ZSize));
          SizeZChanged?.Invoke(SizeZ);
        }
      }
    }

    public event Action<RADouble> SizeZChanged;

    public RADouble SizeZ
    {
      get
      {
        RADouble result;
        var zSize = ZSize;

        if (object.ReferenceEquals(zSize.Unit, _percentLayerXSizeUnit))
          result = RADouble.NewRel(zSize.Value / 100);
        else
          result = RADouble.NewAbs(zSize.AsValueIn(AUL.Point.Instance));

        return result;
      }
      set
      {
        ZSize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerZSizeUnit);
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



    private QuantityWithUnitGuiEnvironment _zPositionEnvironment = PositionEnvironment.Instance;

    public QuantityWithUnitGuiEnvironment ZPositionEnvironment
    {
      get => _zPositionEnvironment;
      set
      {
        if (!(_zPositionEnvironment == value))
        {
          _zPositionEnvironment = value;
          OnPropertyChanged(nameof(ZPositionEnvironment));
        }
      }
    }

    private DimensionfulQuantity _zPosition;

    public DimensionfulQuantity ZPosition
    {
      get => _zPosition;
      set
      {
        if (!(_zPosition == value))
        {
          _zPosition = value;
          OnPropertyChanged(nameof(ZPosition));
        }
      }
    }




    #endregion Position

    #region Rotation

    public QuantityWithUnitGuiEnvironment RotationEnvironment => AngleEnvironment.Instance;


    private DimensionfulQuantity _xRotation;

    public DimensionfulQuantity XRotation
    {
      get => _xRotation;
      set
      {
        if (!(_xRotation == value))
        {
          _xRotation = value;
          OnPropertyChanged(nameof(XRotation));
        }
      }
    }

    private DimensionfulQuantity _yRotation;

    public DimensionfulQuantity YRotation
    {
      get => _yRotation;
      set
      {
        if (!(_yRotation == value))
        {
          _yRotation = value;
          OnPropertyChanged(nameof(YRotation));
        }
      }
    }

    private DimensionfulQuantity _zRotation;

    public DimensionfulQuantity ZRotation
    {
      get => _zRotation;
      set
      {
        if (!(_zRotation == value))
        {
          _zRotation = value;
          OnPropertyChanged(nameof(ZRotation));
        }
      }
    }

    #endregion

    #region Shear

    public QuantityWithUnitGuiEnvironment ShearEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _xShear;

    public DimensionfulQuantity XShear
    {
      get => _xShear;
      set
      {
        if (!(_xShear == value))
        {
          _xShear = value;
          OnPropertyChanged(nameof(XShear));
        }
      }
    }

    private DimensionfulQuantity _yShear;
    public DimensionfulQuantity YShear
    {
      get => _yShear;
      set
      {
        if (!(_yShear == value))
        {
          _yShear = value;
          OnPropertyChanged(nameof(YShear));
        }
      }
    }

    private DimensionfulQuantity _zShear;

    public DimensionfulQuantity ZShear
    {
      get => _zShear;
      set
      {
        if (!(_zShear == value))
        {
          _zShear = value;
          OnPropertyChanged(nameof(ZShear));
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


    private DimensionfulQuantity _zScale;

    public DimensionfulQuantity ZScale
    {
      get => _zScale;
      set
      {
        if (!(_zScale == value))
        {
          _zScale = value;
          OnPropertyChanged(nameof(ZScale));
          ScaleZChanged?.Invoke(_zScale.AsValueInSIUnits);
        }
      }
    }

    public double ScaleZ
    {
      get
      {
        return ZScale.AsValueInSIUnits;
      }
      set
      {
        ZScale = new DimensionfulQuantity(value, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
      }
    }

    public event Action<double> ScaleXChanged;
    public event Action<double> ScaleYChanged;
    public event Action<double> ScaleZChanged;

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
        _percentLayerZSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.Z, AUL.Point.Instance);

        XSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
        YSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });
        ZSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerZSizeUnit });

        XPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
        YPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });
        ZPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerZSizeUnit });


        // Size
        AreSizeElementsVisible = !_doc.IsAutoSized;
        AreSizeElementsEnabled = true;

        if (!_doc.IsAutoSized)
        {
          XSize = _doc.SizeX.IsAbsolute ? new DimensionfulQuantity(_doc.SizeX.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.SizeX.Value * 100, _percentLayerXSizeUnit);
          YSize = _doc.SizeY.IsAbsolute ? new DimensionfulQuantity(_doc.SizeY.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.SizeY.Value * 100, _percentLayerYSizeUnit);
          ZSize = _doc.SizeZ.IsAbsolute ? new DimensionfulQuantity(_doc.SizeZ.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.SizeZ.Value * 100, _percentLayerZSizeUnit);
        }

        // Position
        XPosition = _doc.PositionX.IsAbsolute ? new DimensionfulQuantity(_doc.PositionX.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PositionX.Value * 100, _percentLayerXSizeUnit);
        YPosition = _doc.PositionY.IsAbsolute ? new DimensionfulQuantity(_doc.PositionY.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PositionY.Value * 100, _percentLayerYSizeUnit);
        ZPosition = _doc.PositionZ.IsAbsolute ? new DimensionfulQuantity(_doc.PositionZ.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PositionZ.Value * 100, _percentLayerZSizeUnit);

        // Rotation
        XRotation = new DimensionfulQuantity(_doc.RotationX, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        YRotation = new DimensionfulQuantity(_doc.RotationY, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        ZRotation = new DimensionfulQuantity(_doc.RotationZ, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);

        // Shear
        XShear = new DimensionfulQuantity(_doc.ShearX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);
        YShear = new DimensionfulQuantity(_doc.ShearY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);
        ZShear = new DimensionfulQuantity(_doc.ShearZ, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);

        // Scale
        XScale = new DimensionfulQuantity(_doc.ScaleX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
        YScale = new DimensionfulQuantity(_doc.ScaleY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
        ZScale = new DimensionfulQuantity(_doc.ScaleZ, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);

        _localAnchoringController = new AnchoringController();
        _localAnchoringController.InitializeDocument(new AnchoringModel { Title = "Local anchor", ReferenceSize = _doc.AbsoluteSize, PivotX = _doc.LocalAnchorX, PivotY = _doc.LocalAnchorY, PivotZ = _doc.LocalAnchorZ });

        _parentAnchoringController = new AnchoringController();
        _parentAnchoringController.InitializeDocument(new AnchoringModel { Title = "Parent anchor", ReferenceSize = _doc.ParentSize, PivotX = _doc.ParentAnchorX, PivotY = _doc.ParentAnchorY, PivotZ = _doc.ParentAnchorZ });
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc.RotationX = XRotation.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
        _doc.RotationY = YRotation.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
        _doc.RotationZ = ZRotation.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
        _doc.ShearX = XShear.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
        _doc.ShearY = YShear.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
        _doc.ShearZ = ZShear.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
        _doc.ScaleX = XScale.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
        _doc.ScaleY = YScale.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
        _doc.ScaleZ = ZScale.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);

        if (!_doc.IsAutoSized)
        {
          var xSize = XSize;
          var ySize = YSize;
          var zSize = ZSize;

          if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
            _doc.SizeX = RADouble.NewRel(xSize.Value / 100);
          else
            _doc.SizeX = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));

          if (object.ReferenceEquals(ySize.Unit, _percentLayerYSizeUnit))
            _doc.SizeY = RADouble.NewRel(ySize.Value / 100);
          else
            _doc.SizeY = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));

          if (object.ReferenceEquals(zSize.Unit, _percentLayerZSizeUnit))
            _doc.SizeZ = RADouble.NewRel(zSize.Value / 100);
          else
            _doc.SizeZ = RADouble.NewAbs(zSize.AsValueIn(AUL.Point.Instance));
        }

        var xPos = XPosition;
        var yPos = YPosition;
        var zPos = ZPosition;

        if (object.ReferenceEquals(xPos.Unit, _percentLayerXSizeUnit))
          _doc.PositionX = RADouble.NewRel(xPos.Value / 100);
        else
          _doc.PositionX = RADouble.NewAbs(xPos.AsValueIn(AUL.Point.Instance));

        if (object.ReferenceEquals(yPos.Unit, _percentLayerYSizeUnit))
          _doc.PositionY = RADouble.NewRel(yPos.Value / 100);
        else
          _doc.PositionY = RADouble.NewAbs(yPos.AsValueIn(AUL.Point.Instance));

        if (object.ReferenceEquals(zPos.Unit, _percentLayerZSizeUnit))
          _doc.PositionZ = RADouble.NewRel(zPos.Value / 100);
        else
          _doc.PositionZ = RADouble.NewAbs(zPos.AsValueIn(AUL.Point.Instance));

        if (false == _localAnchoringController.Apply(disposeController))
          return ApplyEnd(false, disposeController);
        if (false == _parentAnchoringController.Apply(disposeController))
          return ApplyEnd(false, disposeController);

        var localAnchor = (AnchoringModel)_localAnchoringController.ModelObject;
        var parentAnchor = (AnchoringModel)_parentAnchoringController.ModelObject;

        _doc.LocalAnchorX = localAnchor.PivotX;
        _doc.LocalAnchorY = localAnchor.PivotY;
        _doc.LocalAnchorZ = localAnchor.PivotZ;

        _doc.ParentAnchorX = parentAnchor.PivotX;
        _doc.ParentAnchorY = parentAnchor.PivotY;
        _doc.ParentAnchorZ = parentAnchor.PivotZ;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message, "Error applying ItemLocationDirect");
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
