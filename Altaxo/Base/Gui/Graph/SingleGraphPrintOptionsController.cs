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

  /// <summary>
  /// View contract for editing single-graph print options.
  /// </summary>
  public interface ISingleGraphPrintOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing <see cref="SingleGraphPrintOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(ISingleGraphPrintOptionsView))]
  [UserControllerForObject(typeof(SingleGraphPrintOptions))]
  public class SingleGraphPrintOptionsController : MVCANControllerEditImmutableDocBase<SingleGraphPrintOptions, ISingleGraphPrintOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<SingleGraphPrintLocation> _printLocation;

    /// <summary>
    /// Gets or sets the print location.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether graphs larger than the page are fitted.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether graphs smaller than the page are fitted.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether crop marks are printed.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the page is rotated automatically.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether pages are tiled.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether a fixed zoom factor is used.
    /// </summary>
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

    /// <summary>
    /// Gets the quantity environment for the zoom factor.
    /// </summary>
    public QuantityWithUnitGuiEnvironment ZoomFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _zoomFactor;

    /// <summary>
    /// Gets or sets the zoom factor.
    /// </summary>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
