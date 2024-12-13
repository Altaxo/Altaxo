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
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface IImageGraphicView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(ImageGraphic))]
  [ExpectedTypeOfView(typeof(IImageGraphicView))]
  public class ImageGraphicController : MVCANControllerEditOriginalDocBase<ImageGraphic, IImageGraphicView>
  {
    private PointD2D _docScale;
    private ItemLocationDirect _docLocation;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_locationController, () => _locationController = null);
    }

    #region Bindings

    private ItemLocationDirectController _locationController;

    public ItemLocationDirectController LocationController
    {
      get => _locationController;
      set
      {
        if (!(_locationController == value))
        {
          _locationController = value;
          OnPropertyChanged(nameof(LocationController));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment SrcSizeEnvironment = SizeEnvironment.Instance;
    private DimensionfulQuantity _srcSizeX;

    public DimensionfulQuantity SrcSizeX
    {
      get => _srcSizeX;
      set
      {
        if (!(_srcSizeX == value))
        {
          _srcSizeX = value;
          OnPropertyChanged(nameof(SrcSizeX));
        }
      }
    }

    private DimensionfulQuantity _srcSizeY;

    public DimensionfulQuantity SrcSizeY
    {
      get => _srcSizeY;
      set
      {
        if (!(_srcSizeY == value))
        {
          _srcSizeY = value;
          OnPropertyChanged(nameof(SrcSizeY));
        }
      }
    }

    private ItemsController<AspectRatioPreservingMode> _keepAspect;

    public ItemsController<AspectRatioPreservingMode> KeepAspect
    {
      get => _keepAspect;
      set
      {
        if (!(_keepAspect == value))
        {
          _keepAspect = value;
          OnPropertyChanged(nameof(KeepAspect));
        }
      }
    }

    private bool _isSizeCalculationBasedOnSourceSize;

    public bool IsSizeCalculationBasedOnSourceSize
    {
      get => _isSizeCalculationBasedOnSourceSize;
      set
      {
        if (!(_isSizeCalculationBasedOnSourceSize == value))
        {
          _isSizeCalculationBasedOnSourceSize = value;
          OnPropertyChanged(nameof(IsSizeCalculationBasedOnSourceSize));
          OnPropertyChanged(nameof(IsSizeCalculationAbsolute));
          EhScalingModeChanged();
        }
      }
    }
    public bool IsSizeCalculationAbsolute
    {
      get => !IsSizeCalculationBasedOnSourceSize;
      set => IsSizeCalculationBasedOnSourceSize = !value;
    }


    #endregion

    private PointD2D SrcSize
    {
      get
      {
        return new PointD2D(SrcSizeX.AsValueIn(Altaxo.Units.Length.Point.Instance), SrcSizeY.AsValueIn(Altaxo.Units.Length.Point.Instance));
      }
      set
      {
        SrcSizeX = new DimensionfulQuantity(value.X, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SrcSizeEnvironment.DefaultUnit);
        SrcSizeY = new DimensionfulQuantity(value.Y, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SrcSizeEnvironment.DefaultUnit);
      }
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var keepAspect = new SelectableListNodeList()
        {
          new SelectableListNode("No",AspectRatioPreservingMode.None, AspectRatioPreservingMode.None== _doc.AspectRatioPreserving ),
          new SelectableListNode("X priority", AspectRatioPreservingMode.PreserveXPriority, AspectRatioPreservingMode.PreserveXPriority == _doc.AspectRatioPreserving),
          new SelectableListNode("Y priority", AspectRatioPreservingMode.PreserveYPriority, AspectRatioPreservingMode.PreserveYPriority == _doc.AspectRatioPreserving)
        };
        KeepAspect = new ItemsController<AspectRatioPreservingMode>(keepAspect, EhAspectRatioPreservingChanged);


        SrcSize = _doc.GetImageSizePt();


        _docScale = new PointD2D(_doc.Size.X / SrcSize.X, _doc.Size.Y / SrcSize.Y);
        _docLocation = new ItemLocationDirect();
        _docLocation.CopyFrom(_doc.Location);
        _docLocation.Scale = new PointD2D(_doc.Size.X / SrcSize.X, _doc.Size.Y / SrcSize.Y);
        _locationController = new ItemLocationDirectController() { UseDocumentCopy = UseDocument.Directly };
        _locationController.InitializeDocument(new object[] { _docLocation });
        Current.Gui.FindAndAttachControlTo(_locationController);

        _locationController.SizeXChanged += EhLocController_SizeXChanged;
        _locationController.SizeYChanged += EhLocController_SizeYChanged;
        _locationController.ScaleXChanged += EhLocController_ScaleXChanged;
        _locationController.ScaleYChanged += EhLocController_ScaleYChanged;
        _locationController.ShowSizeElements(true, !_doc.IsSizeCalculationBasedOnSourceSize);
        _locationController.ShowScaleElements(true, _doc.IsSizeCalculationBasedOnSourceSize);

        IsSizeCalculationBasedOnSourceSize = _doc.IsSizeCalculationBasedOnSourceSize;

      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_locationController.Apply(disposeController))
        return false;

      _docLocation = (ItemLocationDirect)_locationController.ModelObject;

      if (!object.ReferenceEquals(_doc.Location, _docLocation))
        _doc.Location.CopyFrom(_docLocation);

      // all other properties where already set during the session

      return ApplyEnd(true, disposeController);
    }



    private void EhAspectRatioPreservingChanged(AspectRatioPreservingMode newValue)
    {
      _doc.AspectRatioPreserving = newValue;
      _docScale = new PointD2D(_doc.Size.X / SrcSize.X, _doc.Size.Y / SrcSize.Y);
      //_view.DocScale = _docScale;
      //_view.DocSize = _doc.Size;
    }

    private void EhScalingModeChanged()
    {
      _doc.IsSizeCalculationBasedOnSourceSize = IsSizeCalculationBasedOnSourceSize; // false if Size should be used directly, true if the Scale should be used
      _locationController.ShowSizeElements(true, !_doc.IsSizeCalculationBasedOnSourceSize);
      _locationController.ShowScaleElements(true, _doc.IsSizeCalculationBasedOnSourceSize);
    }

    private void EhLocController_SizeXChanged(RADouble sizeX)
    {
      if (!_doc.IsSizeCalculationBasedOnSourceSize)
      {
        var sizeY = _docLocation.SizeY;
        if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
        {
          _docLocation.SizeY = sizeX * (SrcSize.Y / SrcSize.X);
        }
        _docLocation.SizeX = sizeX;
        _docScale = new PointD2D(_docLocation.AbsoluteSizeX / SrcSize.X, _docLocation.AbsoluteSizeY / SrcSize.Y);
        _locationController.ScaleX = _docScale.X;
        _locationController.ScaleY = _docScale.Y;
        _locationController.SizeY = _docLocation.SizeY;
      }
    }

    private void EhLocController_SizeYChanged(RADouble sizeY)
    {
      if (!_doc.IsSizeCalculationBasedOnSourceSize)
      {
        var sizeX = _docLocation.SizeX;
        if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority)
        {
          _docLocation.SizeX = sizeY * (SrcSize.X / SrcSize.Y);
        }
        _docLocation.SizeY = sizeY;
        _docScale = new PointD2D(_docLocation.AbsoluteSizeX / SrcSize.X, _docLocation.AbsoluteSizeY / SrcSize.Y);
        _locationController.ScaleX = _docScale.X;
        _locationController.ScaleY = _docScale.Y;
        _locationController.SizeX = _docLocation.SizeX;
      }
    }

    private void EhLocController_ScaleXChanged(double scaleX)
    {
      _docLocation.ScaleX = scaleX;
      if (_doc.IsSizeCalculationBasedOnSourceSize)
      {
        if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
        {
          _docLocation.ScaleY = scaleX;
        }
        var size = new PointD2D(SrcSize.X * _docLocation.ScaleX, SrcSize.Y * _docLocation.ScaleY);
        _docLocation.AbsoluteSize = size;

        _locationController.ScaleY = _docLocation.ScaleY;
        _locationController.SizeX = _docLocation.SizeX;
        _locationController.SizeY = _docLocation.SizeY;
      }
    }

    private void EhLocController_ScaleYChanged(double scaleY)
    {
      _docLocation.ScaleY = scaleY;
      if (_doc.IsSizeCalculationBasedOnSourceSize)
      {
        if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
        {
          _docLocation.ScaleX = scaleY;
        }
        var size = new PointD2D(SrcSize.X * _docLocation.ScaleX, SrcSize.Y * _docLocation.ScaleY);
        _docLocation.AbsoluteSize = size;

        _locationController.ScaleX = _docLocation.ScaleX;
        _locationController.SizeX = _docLocation.SizeX;
        _locationController.SizeY = _docLocation.SizeY;
      }
    }
  }
}
