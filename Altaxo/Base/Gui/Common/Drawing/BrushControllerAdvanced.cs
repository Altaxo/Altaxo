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
using System.Collections.Generic;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Units;

namespace Altaxo.Gui.Common.Drawing
{
  public interface IBrushViewAdvanced : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(BrushX))]
  [ExpectedTypeOfView(typeof(IBrushViewAdvanced))]
  public class BrushControllerAdvanced : MVCANDControllerEditImmutableDocBase<BrushX, IBrushViewAdvanced>
  {
    private Main.InstancePropertyController _additionalPropertiesController;

    private TextureScalingController _textureScalingController;


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_additionalPropertiesController, () => _additionalPropertiesController = null);
      yield return new ControllerAndSetNullMethod(_textureScalingController, () => _textureScalingController = null);
    }

    #region Bindings

    public BrushX BrushDocument => _doc;

    public BrushType BrushType
    {
      get => _doc.BrushType;
      set
      {
        if (!(BrushType == value))
        {
          _doc = _doc.WithBrushType(value);
          OnPropertyChanged(nameof(BrushType));

          OnPropertyChanged(nameof(ForeColor));
          OnPropertyChanged(nameof(BackColor));
          OnPropertyChanged(nameof(ExchangeColors));
          OnPropertyChanged(nameof(WrapMode));
          OnPropertyChanged(nameof(GradientAngle));
          OnPropertyChanged(nameof(GradientFocus));
          OnPropertyChanged(nameof(GradientColorScale));
          OnPropertyChanged(nameof(TextureOffsetX));
          OnPropertyChanged(nameof(TextureOffsetY));
          OnPropertyChanged(nameof(TextureImage));

          EnableElementsInDependenceOnBrushType();
          OnMadeDirty();
        }
      }
    }

    bool _foreColorEnable;
    public bool ForeColorEnable { get => _foreColorEnable; set { if (!(ForeColorEnable == value)) { _foreColorEnable = value; OnPropertyChanged(nameof(ForeColorEnable)); } } }

    public NamedColor ForeColor
    {
      get => _doc.Color;
      set
      {
        if (!(ForeColor == value))
        {
          _doc = _doc.WithColor(value);
          OnPropertyChanged(nameof(ForeColor));
          OnMadeDirty();
        }
      }
    }

    private bool _restrictBrushColorToPlotColorsOnly;
    public bool ShowPlotColorsOnly { get => _restrictBrushColorToPlotColorsOnly; set { if (!(ShowPlotColorsOnly == value)) { _restrictBrushColorToPlotColorsOnly = value; OnPropertyChanged(nameof(ShowPlotColorsOnly)); } } }


    bool _backColorEnable;
    public bool BackColorEnable { get => _backColorEnable; set { if (!(BackColorEnable == value)) { _backColorEnable = value; OnPropertyChanged(nameof(BackColorEnable)); } } }
    public NamedColor BackColor
    {
      get => _doc.BackColor;
      set
      {
        if (!(BackColor == value))
        {
          _doc = _doc.WithBackColor(value);
          OnPropertyChanged(nameof(BackColor));
          OnMadeDirty();
        }
      }
    }

    bool _exchangeColorsEnable;
    public bool ExchangeColorsEnable { get => _exchangeColorsEnable; set { if (!(ExchangeColorsEnable == value)) { _exchangeColorsEnable = value; OnPropertyChanged(nameof(ExchangeColorsEnable)); } } }

    public bool ExchangeColors
    {
      get => _doc.ExchangeColors;
      set
      {
        if (!(ExchangeColors == value))
        {
          _doc = _doc.WithExchangedColors(value);
          OnPropertyChanged(nameof(ExchangeColors));
          OnPropertyChanged(nameof(ForeColor));
          OnPropertyChanged(nameof(BackColor));
          OnMadeDirty();
        }
      }
    }

    bool _wrapModeEnable;
    public bool WrapModeEnable { get => _wrapModeEnable; set { if (!(WrapModeEnable == value)) { _wrapModeEnable = value; OnPropertyChanged(nameof(WrapModeEnable)); } } }


    public System.Drawing.Drawing2D.WrapMode WrapMode
    {
      get => _doc.WrapMode;
      set
      {
        if (!(WrapMode == value))
        {
          _doc = _doc.WithWrapMode(value);
          OnPropertyChanged(nameof(WrapMode));
          OnMadeDirty();
        }
      }
    }

    public QuantityWithUnitGuiEnvironment GradientAngleEnvironment { get; set; }
    bool _gradientAngleEnable;
    public bool GradientAngleEnable { get => _gradientAngleEnable; set { if (!(GradientAngleEnable == value)) { _gradientAngleEnable = value; OnPropertyChanged(nameof(GradientAngleEnable)); } } }

    public DimensionfulQuantity GradientAngle
    {
      get => new DimensionfulQuantity(_doc.GradientAngle, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(GradientAngleEnvironment.DefaultUnit);
      set
      {
        if (!(GradientAngle == value))
        {
          _doc = _doc.WithGradientAngle(value.AsValueIn(Altaxo.Units.Angle.Degree.Instance));
          OnPropertyChanged(nameof(GradientAngle));
          OnMadeDirty();
        }
      }
    }

    public QuantityWithUnitGuiEnvironment GradientFocusEnvironment { get; set; }
    bool _gradientFocusEnable;
    public bool GradientFocusEnable { get => _gradientFocusEnable; set { if (!(GradientFocusEnable == value)) { _gradientFocusEnable = value; OnPropertyChanged(nameof(GradientFocusEnable)); } } }

    public DimensionfulQuantity GradientFocus
    {
      get => new DimensionfulQuantity(_doc.GradientFocus, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GradientFocusEnvironment.DefaultUnit);
      set
      {
        if (!(GradientFocus == value))
        {
          _doc = _doc.WithGradientFocus(value.AsValueInSIUnits);
          OnPropertyChanged(nameof(GradientFocus));
          OnMadeDirty();
        }
      }
    }

    public QuantityWithUnitGuiEnvironment GradientColorScaleEnvironment { get; set; }
    bool _gradientColorScaleEnable;
    public bool GradientColorScaleEnable { get => _gradientColorScaleEnable; set { if (!(GradientColorScaleEnable == value)) { _gradientColorScaleEnable = value; OnPropertyChanged(nameof(GradientColorScaleEnable)); } } }
    public DimensionfulQuantity GradientColorScale
    {
      get => new DimensionfulQuantity(_doc.GradientColorScale, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GradientColorScaleEnvironment.DefaultUnit);
      set
      {
        if (!(GradientColorScale == value))
        {
          _doc = _doc.WithGradientColorScale(value.AsValueInSIUnits);
          OnPropertyChanged(nameof(GradientColorScale));
          OnMadeDirty();
        }
      }
    }


    public QuantityWithUnitGuiEnvironment TextureOffsetXEnvironment => RelationEnvironment.Instance;
    bool _textureOffsetXEnable;
    public bool TextureOffsetXEnable { get => _textureOffsetXEnable; set { if (!(TextureOffsetXEnable == value)) { _textureOffsetXEnable = value; OnPropertyChanged(nameof(TextureOffsetXEnable)); } } }
    public DimensionfulQuantity TextureOffsetX
    {
      get => new DimensionfulQuantity(_doc.TextureOffsetX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(TextureOffsetXEnvironment.DefaultUnit);
      set
      {
        if (!(TextureOffsetX == value))
        {
          _doc = _doc.WithTextureOffsetX(value.AsValueInSIUnits);
          OnPropertyChanged(nameof(TextureOffsetX));
          OnMadeDirty();
        }
      }
    }



    public QuantityWithUnitGuiEnvironment TextureOffsetYEnvironment => RelationEnvironment.Instance;

    bool _textureOffsetYEnable;
    public bool TextureOffsetYEnable { get => _textureOffsetYEnable; set { if (!(TextureOffsetYEnable == value)) { _textureOffsetYEnable = value; OnPropertyChanged(nameof(TextureOffsetYEnable)); } } }

    public DimensionfulQuantity TextureOffsetY
    {
      get => new DimensionfulQuantity(_doc.TextureOffsetY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(TextureOffsetYEnvironment.DefaultUnit);
      set
      {
        if (!(TextureOffsetY == value))
        {
          _doc = _doc.WithTextureOffsetY(value.AsValueInSIUnits);
          OnPropertyChanged(nameof(TextureOffsetY));
          OnMadeDirty();
        }
      }
    }

    bool _textureScalingEnable;
    public bool TextureScalingEnable { get => _textureScalingEnable; set { if (!(TextureScalingEnable == value)) { _textureScalingEnable = value; OnPropertyChanged(nameof(TextureScalingEnable)); } } }


    public object TextureScalingView => _textureScalingController?.ViewObject;

    bool _textureImageEnable;
    public bool TextureImageEnable { get => _textureImageEnable; set { if (!(TextureImageEnable == value)) { _textureImageEnable = value; OnPropertyChanged(nameof(TextureImageEnable)); } } }

    public ImageProxy TextureImage
    {
      get { return _doc.TextureImage; }
      set
      {
        if (!object.ReferenceEquals(_doc.TextureImage, value))
        {
          _doc = _doc.WithTextureImage(value);
          OnPropertyChanged(nameof(TextureImage));
          _additionalPropertiesController.InitializeDocument(_doc.TextureImage);
          if (_doc.TextureImage is not null)
            _textureScalingController.SourceTextureSize = GetSizeOfImageProxy(_doc.TextureImage);
          OnMadeDirty();
        }
      }
    }

    public Main.InstancePropertyController AdditionalPropertiesController => _additionalPropertiesController;

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _additionalPropertiesController = new Main.InstancePropertyController() { UseDocumentCopy = UseDocument.Directly };
        _additionalPropertiesController.MadeDirty += EhTextureImageChanged;
        _additionalPropertiesController.InitializeDocument(_doc.TextureImage);

        _textureScalingController = new TextureScalingController() { UseDocumentCopy = UseDocument.Directly };
        _textureScalingController.MadeDirty += EhTextureScalingChanged;
        _textureScalingController.InitializeDocument(_doc.TextureScale);
        if (_doc.TextureImage is not null)
          _textureScalingController.SourceTextureSize = GetSizeOfImageProxy(_doc.TextureImage);
      }

      if (_view is not null)
      {
        if (_textureScalingController.ViewObject is null)
        {
          Current.Gui.FindAndAttachControlTo(_textureScalingController);
          OnPropertyChanged(nameof(TextureScalingView));
        }

        EnableElementsInDependenceOnBrushType();
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    protected override void OnMadeDirty()
    {
      base.OnMadeDirty();
      OnPropertyChanged(nameof(BrushDocument));
    }

    #region Other helper functions

    private void EnableElementsInDependenceOnBrushType()
    {
      bool foreColor = false, backColor = false, exchangeColor = false, wrapMode = false,
        gradientFocus = false, gradientColorScale = false, gradientAngle = false,
        textureScale = false, textureImage = false, textureOffsetX = false, textureOffsetY = false;

      switch (_doc.BrushType)
      {
        case BrushType.SolidBrush:
          foreColor = true;
          break;

        case BrushType.LinearGradientBrush:
        case BrushType.SigmaBellShapeLinearGradientBrush:
        case BrushType.TriangularShapeLinearGradientBrush:
          foreColor = true;
          backColor = true;
          exchangeColor = true;
          wrapMode = true;
          gradientAngle = true;
          if (_doc.BrushType != BrushType.LinearGradientBrush)
          {
            gradientFocus = true;
            gradientColorScale = true;
          }
          break;

        case BrushType.PathGradientBrush:
        case BrushType.SigmaBellShapePathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
          foreColor = true;
          backColor = true;
          exchangeColor = true;
          wrapMode = true;
          textureOffsetX = true;
          textureOffsetY = true;
          if (_doc.BrushType != BrushType.PathGradientBrush)
          {
            gradientColorScale = true;
          }
          break;

        case BrushType.HatchBrush:
        case BrushType.SyntheticTextureBrush:
          foreColor = true;
          backColor = true;
          exchangeColor = true;
          wrapMode = true;
          gradientAngle = true;
          textureScale = true;
          textureImage = true;
          textureOffsetX = true;
          textureOffsetY = true;
          break;

        case BrushType.TextureBrush:
          wrapMode = true;
          gradientAngle = true;
          textureScale = true;
          textureImage = true;
          textureOffsetX = true;
          textureOffsetY = true;
          break;
      }
      ForeColorEnable = foreColor;
      BackColorEnable = backColor;
      ExchangeColorsEnable = exchangeColor;
      WrapModeEnable = wrapMode;
      GradientFocusEnable = gradientFocus;
      GradientColorScaleEnable = gradientColorScale;
      GradientAngleEnable = gradientAngle;
      TextureScalingEnable = textureScale;
      TextureOffsetXEnable = textureOffsetX;
      TextureOffsetYEnable = textureOffsetY;
      TextureImageEnable = textureImage;
      //_view.AdditionalPropertiesView
    }

    private VectorD2D GetSizeOfImageProxy(ImageProxy proxy)
    {
      return proxy.Size;
    }

    #endregion Other helper functions

    #region Event handlers





    private void EhTextureImageChanged(IMVCANController ctrl)
    {
      _doc = _doc.WithTextureImage((ImageProxy)_additionalPropertiesController.ProvisionalModelObject);
      OnMadeDirty();
    }

    private void EhTextureScalingChanged(IMVCANController ctrl)
    {
      _doc = _doc.WithTextureScale((TextureScaling)_textureScalingController.ProvisionalModelObject);
      OnMadeDirty();
    }



    #endregion Event handlers
  }
}
