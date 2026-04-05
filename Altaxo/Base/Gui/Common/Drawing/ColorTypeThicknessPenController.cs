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
using System.Windows.Input;
using Altaxo.Drawing;
using Altaxo.Units;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Defines the view contract for editing a pen by color, type, and thickness.
  /// </summary>
  public interface IColorTypeThicknessPenView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for ColorTypeWidthPenController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IColorTypeThicknessPenView))]
  public class ColorTypeThicknessPenController : MVCANDControllerEditImmutableDocBase<PenX, object>
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorTypeThicknessPenController"/> class.
    /// </summary>
    public ColorTypeThicknessPenController()
    {
      CmdShowCustomPen = new RelayCommand(EhShowCustomPen);
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="ColorTypeThicknessPenController"/> class.
    /// </summary>
    /// <param name="pen">The pen to edit.</param>
    public ColorTypeThicknessPenController(PenX pen) : this()
    {
      _doc = _originalDoc = pen ?? throw new ArgumentNullException(nameof(pen));
      Initialize(true);
    }

    /// <summary>
    /// Gets or sets the pen (the current document of this controller). This property is intended for outside controllers to set the document when the controller is already created.
    /// </summary>
    /// <value>
    /// The pen.
    /// </value>
    public PenX Pen
    {
      get => _doc;
      set
      {
        _doc = value;
        Initialize(true);
        OnPropertyChanged(nameof(Pen));
        OnPropertyChanged(nameof(Brush));
        OnPropertyChanged(nameof(LineThickness));
        OnPropertyChanged(nameof(DashPattern));
      }
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _showPlotColorsOnly;
    /// <summary>
    /// Gets or sets a value indicating whether only plot colors are shown.
    /// </summary>
    public bool ShowPlotColorsOnly { get => _showPlotColorsOnly; set { if (!(ShowPlotColorsOnly == value)) { _showPlotColorsOnly = value; OnPropertyChanged(nameof(ShowPlotColorsOnly)); } } }

    /// <summary>
    /// Gets or sets the brush used by the pen.
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
    /// Gets the command for opening the full pen editor.
    /// </summary>
    public ICommand CmdShowCustomPen { get; }

    #endregion

    /// <inheritdoc/>
    protected override void OnMadeDirty()
    {
      base.OnMadeDirty();
      OnPropertyChanged(nameof(Pen));
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
        OnPropertyChanged(nameof(DashPattern));
        OnPropertyChanged(nameof(LineThickness));
        OnMadeDirty();
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }
  }
}
