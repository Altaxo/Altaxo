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

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using Altaxo.Drawing;
using Altaxo.Units;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Defines the view contract for editing all pen properties.
  /// </summary>
  public interface IPenAllPropertiesView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for comprehensive <see cref="PenX"/> editing.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPenAllPropertiesView))]
  public class PenAllPropertiesController : MVCANDControllerEditImmutableDocBase<PenX, IPenAllPropertiesView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_startCap, () => StartCap = null);
      yield return new ControllerAndSetNullMethod(_endCap, () => EndCap = null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PenAllPropertiesController"/> class.
    /// </summary>
    public PenAllPropertiesController()
    {
        CmdShowCustomPen = new RelayCommand(EhShowCustomPen);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PenAllPropertiesController"/> class.
    /// </summary>
    /// <param name="pen">The pen to edit.</param>
    public PenAllPropertiesController(PenX pen) : this()
    {
      _doc = _originalDoc = pen ?? throw new ArgumentNullException(nameof(pen));
      Initialize(true);
    }

    #region Binding

    /// <summary>
    /// Gets the command for opening the pen editor dialog.
    /// </summary>
    public ICommand CmdShowCustomPen { get; }

    private bool _showPlotColorsOnly;

    /// <summary>
    /// Gets or sets a value indicating whether only plot colors are shown.
    /// </summary>
    public bool ShowPlotColorsOnly
    {
      get => _showPlotColorsOnly;
      set
      {
        if (!(_showPlotColorsOnly == value))
        {
          _showPlotColorsOnly = value;
          OnPropertyChanged(nameof(ShowPlotColorsOnly));
        }
      }
    }


    /// <summary>
    /// Gets or sets the pen brush.
    /// </summary>
    public BrushX Brush
    {
      get => _doc.Brush;
      set
      {
        if (!object.ReferenceEquals(Brush, value))
        {
          _doc = _doc.WithBrush(value);
          OnPropertyChanged(nameof(Brush));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the environment used for line thickness.
    /// </summary>
    public QuantityWithUnitGuiEnvironment LineThicknessEnvironment { get; set; } = Altaxo.Gui.LineThicknessEnvironment.Instance;
    /// <summary>
    /// Gets or sets the line thickness.
    /// </summary>
    public DimensionfulQuantity LineThickness
    {
      get => new DimensionfulQuantity(_doc.Width, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineThicknessEnvironment.DefaultUnit);
      set
      {
        if (!(LineThickness == value))
        {
          _doc = _doc.WithWidth(value.AsValueIn(Altaxo.Units.Length.Point.Instance));
          OnPropertyChanged(nameof(LineThickness));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the dash pattern.
    /// </summary>
    public IDashPattern DashPattern
    {
      get => _doc.DashPattern;
      set
      {
        if (!object.ReferenceEquals(DashPattern, value))
        {
          _doc = _doc.WithDashPattern(value);
          OnPropertyChanged(nameof(DashPattern));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the dash cap.
    /// </summary>
    public DashCap DashCap
    {
      get => _doc.DashCap;
      set
      {
        if (!object.Equals(DashCap, value))
        {
          _doc = _doc.WithDashCap(value);
          OnPropertyChanged(nameof(DashCap));
          OnMadeDirty();
        }
      }
    }



    StartEndCapController _startCap;

    /// <summary>
    /// Gets or sets the controller for the start cap.
    /// </summary>
    public StartEndCapController StartCap
    {
      get => _startCap;
      set
      {
        if (!(_startCap == value))
        {
          if (_startCap is { } oldC)
            oldC.MadeDirty -= EhStartCapChanged;

          _startCap?.Dispose();
          _startCap = value;

          if (_startCap is { } newC)
            newC.MadeDirty += EhStartCapChanged;

          OnPropertyChanged(nameof(StartCap));
        }
      }
    }


    StartEndCapController _endCap;

    /// <summary>
    /// Gets or sets the controller for the end cap.
    /// </summary>
    public StartEndCapController EndCap
    {
      get => _endCap;
      set
      {
        if (!(_endCap == value))
        {
          if (_endCap is { } oldC)
            oldC.MadeDirty -= EhEndCapChanged;

          _endCap?.Dispose();
          _endCap = value;

          if (_endCap is { } newC)
            newC.MadeDirty += EhEndCapChanged;

          OnPropertyChanged(nameof(EndCap));
        }
      }
    }


    /// <summary>
    /// Gets or sets the line join style.
    /// </summary>
    public LineJoin LineJoin
    {
      get => _doc.LineJoin;
      set
      {
        if (!object.Equals(LineJoin, value))
        {
          _doc = _doc.WithLineJoin(value);
          OnPropertyChanged(nameof(LineJoin));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the environment used for the miter limit.
    /// </summary>
    public QuantityWithUnitGuiEnvironment MiterLimitEnvironment { get; set; } = Altaxo.Gui.RelationEnvironment.Instance;
    /// <summary>
    /// Gets or sets the miter limit.
    /// </summary>
    public DimensionfulQuantity MiterLimit
    {
      get => new DimensionfulQuantity(_doc.MiterLimit, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MiterLimitEnvironment.DefaultUnit);
      set
      {
        if (!(MiterLimit == value))
        {
          _doc = _doc.WithMiterLimit(value.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance));
          OnPropertyChanged(nameof(MiterLimit));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets the edited pen.
    /// </summary>
    public PenX Pen => _doc;

    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      if (initData)
      {
        StartCap = new StartEndCapController() { IsForEndCap = false };
        StartCap.InitializeDocument(_doc.StartCap);

        EndCap = new StartEndCapController() { IsForEndCap = true };
        EndCap.InitializeDocument(_doc.EndCap);
      }
    }

  

    private void EhStartCapChanged(IMVCANDController obj)
    {
      _doc = _doc.WithStartCap((ILineCap)obj.ProvisionalModelObject);
      OnPropertyChanged(nameof(StartCap));
      OnMadeDirty();
    }

    private void EhEndCapChanged(IMVCANDController obj)
    {
      _doc = _doc.WithEndCap((ILineCap)obj.ProvisionalModelObject);
      OnPropertyChanged(nameof(EndCap));
      OnMadeDirty();
    }


    /// <inheritdoc/>
    protected override void OnMadeDirty()
    {
      OnPropertyChanged(nameof(Pen));
      base.OnMadeDirty();
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }


    private void EhShowCustomPen()
    {
      var ctrler = new PenAllPropertiesController(_doc)
      {
        ShowPlotColorsOnly = _showPlotColorsOnly,
      };

      if (Current.Gui.ShowDialog(ctrler, "Edit pen properties"))
      {
        _doc = (PenX)ctrler.ModelObject;
        OnPropertyChanged(nameof(Brush));
        OnPropertyChanged(nameof(LineThickness));
        OnPropertyChanged(nameof(DashPattern));
        OnPropertyChanged(nameof(DashCap));
        StartCap.InitializeDocument(_doc.StartCap);
        EndCap.InitializeDocument(_doc.EndCap);
        OnPropertyChanged(nameof(LineJoin));
        OnPropertyChanged(nameof(MiterLimit));
        OnMadeDirty();
      }
    }
  }
}
