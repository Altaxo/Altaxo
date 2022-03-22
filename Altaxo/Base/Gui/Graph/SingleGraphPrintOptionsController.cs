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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
  using Altaxo.Collections;
  using Altaxo.Graph;
  using Altaxo.Graph.Gdi;
  using Altaxo.Gui.Common;
  using Altaxo.Units;

  public interface ISingleGraphPrintOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(ISingleGraphPrintOptionsView))]
  [UserControllerForObject(typeof(SingleGraphPrintOptions))]
  public class SingleGraphPrintOptionsController : MVCANControllerEditImmutableDocBase<SingleGraphPrintOptions, ISingleGraphPrintOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<SingleGraphPrintLocation> _printLocation;

    public ItemsController<SingleGraphPrintLocation> PrintLocation
    {
      get => _printLocation;
      set
      {
        if (!(_printLocation == value))
        {
          _printLocation?.Dispose();
          _printLocation = value;
          OnPropertyChanged(nameof(PrintLocation));
        }
      }
    }

    private bool _fitGraphToPrintIfLarger;

    public bool FitGraphToPrintIfLarger
    {
      get => _fitGraphToPrintIfLarger;
      set
      {
        if (!(_fitGraphToPrintIfLarger == value))
        {
          _fitGraphToPrintIfLarger = value;
          OnPropertyChanged(nameof(FitGraphToPrintIfLarger));
        }
      }
    }

    private bool _fitGraphToPrintIfSmaller;

    public bool FitGraphToPrintIfSmaller
    {
      get => _fitGraphToPrintIfSmaller;
      set
      {
        if (!(_fitGraphToPrintIfSmaller == value))
        {
          _fitGraphToPrintIfSmaller = value;
          OnPropertyChanged(nameof(FitGraphToPrintIfSmaller));
        }
      }
    }

    private bool _printCropMarks;

    public bool PrintCropMarks
    {
      get => _printCropMarks;
      set
      {
        if (!(_printCropMarks == value))
        {
          _printCropMarks = value;
          OnPropertyChanged(nameof(PrintCropMarks));
        }
      }
    }

    private bool _rotatePageAutomatically;

    public bool RotatePageAutomatically
    {
      get => _rotatePageAutomatically;
      set
      {
        if (!(_rotatePageAutomatically == value))
        {
          _rotatePageAutomatically = value;
          OnPropertyChanged(nameof(RotatePageAutomatically));
        }
      }
    }

    private bool _tilePages;

    public bool TilePages
    {
      get => _tilePages;
      set
      {
        if (!(_tilePages == value))
        {
          _tilePages = value;
          OnPropertyChanged(nameof(TilePages));
        }
      }
    }

    private bool _useFixedZoomFactor;

    public bool UseFixedZoomFactor
    {
      get => _useFixedZoomFactor;
      set
      {
        if (!(_useFixedZoomFactor == value))
        {
          _useFixedZoomFactor = value;
          OnPropertyChanged(nameof(UseFixedZoomFactor));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment ZoomFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _zoomFactor;

    public DimensionfulQuantity ZoomFactor
    {
      get => _zoomFactor;
      set
      {
        if (!(_zoomFactor == value))
        {
          _zoomFactor = value;
          OnPropertyChanged(nameof(ZoomFactor));
        }
      }
    }


#endregion

    protected override void Initialize(bool initData)
    {
      if (initData)
      {
        PrintLocation = new ItemsController<SingleGraphPrintLocation>(new SelectableListNodeList(_doc.PrintLocation));
      
        FitGraphToPrintIfLarger =_doc.FitGraphToPrintIfLarger;
        FitGraphToPrintIfSmaller =_doc.FitGraphToPrintIfSmaller;
        PrintCropMarks =_doc.PrintCropMarks;
        RotatePageAutomatically =_doc.RotatePageAutomatically;
        TilePages=_doc.TilePages;
        UseFixedZoomFactor=_doc.UseFixedZoomFactor;
        ZoomFactor = new DimensionfulQuantity(_doc.ZoomFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ZoomFactorEnvironment.DefaultUnit);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.PrintLocation = PrintLocation.SelectedValue;
      _doc.FitGraphToPrintIfLarger = FitGraphToPrintIfLarger;
      _doc.FitGraphToPrintIfSmaller = FitGraphToPrintIfSmaller;
      _doc.PrintCropMarks = PrintCropMarks;
      _doc.RotatePageAutomatically = RotatePageAutomatically;
      _doc.TilePages = TilePages;
      _doc.UseFixedZoomFactor = UseFixedZoomFactor;
      _doc.ZoomFactor = ZoomFactor.AsValueInSIUnits;

      return ApplyEnd(true, disposeController);
    }
  }
}
