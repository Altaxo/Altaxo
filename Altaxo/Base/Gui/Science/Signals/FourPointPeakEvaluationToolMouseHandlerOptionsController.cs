#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Drawing;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Science.Signals;

namespace Altaxo.Gui.Science.Signals
{
  /// <summary>
  /// View interface for four-point peak evaluation tool options.
  /// </summary>
  public interface IFourPointPeakEvaluationToolMouseHandlerOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="FourPointPeakEvaluationToolMouseHandlerOptions"/>.
  /// </summary>
  [UserControllerForObject(typeof(FourPointPeakEvaluationToolMouseHandlerOptions))]
  [ExpectedTypeOfView(typeof(IFourPointPeakEvaluationToolMouseHandlerOptionsView))]
  public class FourPointPeakEvaluationToolMouseHandlerOptionsController : MVCANControllerEditImmutableDocBase<FourPointPeakEvaluationToolMouseHandlerOptions, IFourPointPeakEvaluationToolMouseHandlerOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(PenController, () => PenController = null!);
      yield return new ControllerAndSetNullMethod(BrushController, () => BrushController = null!);
    }

    #region Bindings

    private bool _showOptionsWhenToolIsActivated;

    /// <summary>
    /// Gets or sets a value indicating whether options are shown when the tool is activated.
    /// </summary>
    public bool ShowOptionsWhenToolIsActivated
    {
      get => _showOptionsWhenToolIsActivated;
      set
      {
        if (!(_showOptionsWhenToolIsActivated == value))
        {
          _showOptionsWhenToolIsActivated = value;
          OnPropertyChanged(nameof(ShowOptionsWhenToolIsActivated));
        }
      }
    }

    private ColorTypeThicknessPenController _penController;

    /// <summary>
    /// Gets or sets the controller for the line pen.
    /// </summary>
    public ColorTypeThicknessPenController PenController
    {
      get => _penController;
      set
      {
        if (!(_penController == value))
        {
          _penController?.Dispose();
          _penController = value;
          OnPropertyChanged(nameof(PenController));
        }
      }
    }

    private BrushControllerAdvanced _brushController;

    /// <summary>
    /// Gets or sets the controller for the area brush.
    /// </summary>
    public BrushControllerAdvanced BrushController
    {
      get => _brushController;
      set
      {
        if (!(_brushController == value))
        {
          _brushController?.Dispose();
          _brushController = value;
          OnPropertyChanged(nameof(BrushController));
        }
      }
    }

    #endregion


    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ShowOptionsWhenToolIsActivated = _doc.ShowOptionsWhenToolIsActivated;
        PenController = new ColorTypeThicknessPenController(_doc.LinePen);
        BrushController = new BrushControllerAdvanced();
        BrushController.InitializeDocument(_doc.AreaBrush);
      }
    }
    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = new FourPointPeakEvaluationToolMouseHandlerOptions()
      {
        ShowOptionsWhenToolIsActivated = ShowOptionsWhenToolIsActivated,
        LinePen = PenController.Pen,
        AreaBrush = (BrushX)BrushController.ModelObject,
      };

      return ApplyEnd(true, disposeController);

    }

  }
}
