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

  public interface IColorTypeThicknessPenView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for ColorTypeWidthPenController.
  /// </summary>
  public class ColorTypeThicknessPenController : MVCANDControllerEditImmutableDocBase<PenX3D, object>
  {
    public ColorTypeThicknessPenController()
    {
      CmdShowCustomPen = new RelayCommand(EhShowCustomPen);
    }

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

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _showPlotColorsOnly;
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

    public ICommand CmdShowCustomPen { get; }

    #endregion

    protected override void OnMadeDirty()
    {
      base.OnMadeDirty();
      OnPropertyChanged(nameof(Pen));
    }

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

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }
  }
}

