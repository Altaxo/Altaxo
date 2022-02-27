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
using Altaxo.Drawing;
using Altaxo.Units;

namespace Altaxo.Gui.Common.Drawing
{

  public interface IPenAllPropertiesView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IPenAllPropertiesView))]
  public class PenAllPropertiesController : MVCANDControllerEditImmutableDocBase<PenX, IPenAllPropertiesView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_startCap, () => _startCap = null);
      yield return new ControllerAndSetNullMethod(_endCap, () => _endCap = null);
    }

    public PenAllPropertiesController() { }
    public PenAllPropertiesController(PenX pen)
    {
      _doc = _originalDoc = pen ?? throw new ArgumentNullException(nameof(pen));
      Initialize(true);
    }

    #region Binding

    private bool _showPlotColorsOnly;
    public bool ShowPlotColorsOnly { get => _showPlotColorsOnly; set { if (!(ShowPlotColorsOnly == value)) { _showPlotColorsOnly = value; OnPropertyChanged(nameof(ShowPlotColorsOnly)); } } }

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

    public QuantityWithUnitGuiEnvironment LineThicknessEnvironment { get; set; } = Altaxo.Gui.LineThicknessEnvironment.Instance;
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

    StartEndCapController _startCap = new() { IsForEndCap = false };
    public StartEndCapController StartCap => _startCap;

    StartEndCapController _endCap = new() { IsForEndCap = true };
    public StartEndCapController EndCap => _endCap;


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

    public QuantityWithUnitGuiEnvironment MiterLimitEnvironment { get; set; } = Altaxo.Gui.RelationEnvironment.Instance;
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

    public PenX Pen => _doc;

    #endregion

    protected override void Initialize(bool initData)
    {
      if (initData)
      {
        StartCap.InitializeDocument(_doc.StartCap);
        StartCap.MadeDirty -= EhStartCapChanged;
        StartCap.MadeDirty += EhStartCapChanged;

        EndCap.InitializeDocument(_doc.EndCap);
        EndCap.MadeDirty -= EhEndCapChanged;
        EndCap.MadeDirty += EhEndCapChanged;
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

    #region IApplyController Members

    protected override void OnMadeDirty()
    {
      OnPropertyChanged(nameof(Pen));
      base.OnMadeDirty();
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    #endregion IApplyController Members
  }
}
