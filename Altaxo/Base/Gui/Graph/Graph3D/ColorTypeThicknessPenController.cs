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
using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Drawing.D3D;
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph3D
{

  /// <summary>
  /// View contract for editing color and thickness pen properties.
  /// </summary>
  public interface IColorTypeThicknessPenView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing simplified <see cref="PenX3D"/> properties.
  /// </summary>
  public class ColorTypeThicknessPenController : MVCANDControllerEditImmutableDocBase<PenX3D, object>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorTypeThicknessPenController"/> class.
    /// </summary>
    public ColorTypeThicknessPenController()
    {
      CmdShowCustomPen = new RelayCommand(EhShowCustomPen);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorTypeThicknessPenController"/> class for the specified pen.
    /// </summary>
    public ColorTypeThicknessPenController(PenX3D pen) : this()
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
    public PenX3D Pen
    {
      get => _doc;
      set
      {
        _doc = value;
        Initialize(true);
        OnPropertyChanged(nameof(Pen));
        OnPropertyChanged(nameof(Material));
        OnPropertyChanged(nameof(DashPattern));
        OnPropertyChanged(nameof(LineThickness));
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
    /// Gets or sets the pen material.
    /// </summary>
    public IMaterial Material
    {
      get => _doc.Material;
      set
      {
        if (!object.ReferenceEquals(Material, value))
        {
          _doc = _doc.WithMaterial(value);
          OnPropertyChanged(nameof(Material));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the line thickness.
    /// </summary>
    public double LineThickness
    {
      get => _doc.Thickness1;
      set
      {
        if (!(LineThickness == value))
        {
          _doc = _doc.WithUniformThickness(value);
          OnPropertyChanged(nameof(LineThickness));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the dash pattern.
    /// </summary>
    public Altaxo.Drawing.IDashPattern DashPattern
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
    /// Gets the command that opens the full pen editor.
    /// </summary>
    public ICommand CmdShowCustomPen { get; }

    #endregion

    /// <inheritdoc/>
    protected override void OnMadeDirty()
    {
      base.OnMadeDirty();
      OnPropertyChanged(nameof(Pen));
    }

    /// <summary>
    /// Opens the full pen editor dialog.
    /// </summary>
    public void EhShowCustomPen()
    {
      var ctrler = new PenAllPropertiesController(_doc)
      {
        ShowPlotColorsOnly = _showPlotColorsOnly,
      };


      if (Current.Gui.ShowDialog(ctrler, "Edit pen properties"))
      {
        _doc = (PenX3D)ctrler.ModelObject;
        OnPropertyChanged(nameof(Material));
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

