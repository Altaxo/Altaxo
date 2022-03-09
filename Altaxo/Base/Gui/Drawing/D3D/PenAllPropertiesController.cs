#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Drawing.D3D
{
  using System.Windows.Input;
  using Altaxo.Collections;
  using Altaxo.Drawing.D3D;
  using Altaxo.Gui.Common;
  using Altaxo.Units;

  public interface IPenAllPropertiesView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IPenAllPropertiesView))]
  public class PenAllPropertiesController : MVCANDControllerEditImmutableDocBase<PenX3D, IPenAllPropertiesView>
  {
    public PenAllPropertiesController()
    {
      CmdShowCustomPen = new RelayCommand(EhShowCustomPen);
    }

    public PenAllPropertiesController(PenX3D pen) : this()
    {
      _doc = _originalDoc = pen ?? throw new ArgumentNullException(nameof(pen));
      Initialize(true);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_dashStartCap, () => DashStartCap = null);
      yield return new ControllerAndSetNullMethod(_dashEndCap, () => DashEndCap = null);

      yield return new ControllerAndSetNullMethod(_startCap, () => StartCap = null);
      yield return new ControllerAndSetNullMethod(_endCap, () => EndCap = null);
    }


    #region Binding

    public ICommand CmdShowCustomPen { get; }

    private bool _showPlotColorsOnly;
    public bool ShowPlotColorsOnly
    {
      get => _showPlotColorsOnly;
      set
      {
        if (!(ShowPlotColorsOnly == value))
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

    public double LineThickness1
    {
      get => _doc.Thickness1;
      set
      {
        if (!(LineThickness1 == value))
        {
          _doc = _doc.WithThickness1(value);
          OnPropertyChanged(nameof(LineThickness1));
          OnMadeDirty();
        }
      }
    }

    public double LineThickness2
    {
      get => _doc.Thickness2;
      set
      {
        if (!(LineThickness2 == value))
        {
          _doc = _doc.WithThickness2(value);
          OnPropertyChanged(nameof(LineThickness2));
          OnMadeDirty();
        }
      }
    }

    private ItemsController<Type> _crossSection;

    public ItemsController<Type> CrossSection
    {
      get => _crossSection;
      set
      {
        if (!(_crossSection == value))
        {
          _crossSection = value;
          OnPropertyChanged(nameof(CrossSection));
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



    StartEndCapController _dashStartCap;

    public StartEndCapController DashStartCap
    {
      get => _dashStartCap;
      set
      {
        if (!(_dashStartCap == value))
        {
          if (_dashStartCap is { } oldC)
            oldC.MadeDirty -= EhDashStartCapChanged;

          _dashStartCap?.Dispose();
          _dashStartCap = value;

          if (_dashStartCap is { } newC)
            newC.MadeDirty += EhDashStartCapChanged;
          OnPropertyChanged(nameof(DashStartCap));
        }
      }
    }


    StartEndCapController _dashEndCap;

    public StartEndCapController DashEndCap
    {
      get => _dashEndCap;
      set
      {
        if (!(_dashEndCap == value))
        {
          if (_dashEndCap is { } oldC)
            oldC.MadeDirty -= EhDashEndCapChanged;

          _dashEndCap?.Dispose();
          _dashEndCap = value;

          if (_dashEndCap is { } newC)
            newC.MadeDirty += EhDashEndCapChanged;

          OnPropertyChanged(nameof(DashEndCap));
        }
      }
    }


    StartEndCapController _startCap;

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

    public PenLineJoin LineJoin
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

    public PenX3D Pen

    {
      get => _doc;

      set
      {
        if (!(_doc == value))
        {
          _doc = value;
          Initialize(true);

          OnPropertyChanged(nameof(Material));
          OnPropertyChanged(nameof(LineThickness1));
          OnPropertyChanged(nameof(LineThickness2));
          OnPropertyChanged(nameof(CrossSection));
          OnPropertyChanged(nameof(DashPattern));
          OnPropertyChanged(nameof(DashStartCap));
          OnPropertyChanged(nameof(DashEndCap));
          OnPropertyChanged(nameof(StartCap));
          OnPropertyChanged(nameof(EndCap));
          OnPropertyChanged(nameof(LineJoin));
          OnPropertyChanged(nameof(MiterLimit));
          OnMadeDirty();
        }
      }
    }
    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);
      if (initData)
      {
        DashStartCap = new StartEndCapController() { IsForEndCap = false };
        DashStartCap.InitializeDocument(_doc.DashStartCap);

        DashEndCap = new StartEndCapController() { IsForEndCap = true };
        DashEndCap.InitializeDocument(_doc.DashEndCap);


        StartCap = new StartEndCapController() { IsForEndCap = false };
        StartCap.InitializeDocument(_doc.LineStartCap);

        EndCap = new StartEndCapController() { IsForEndCap = true };
        EndCap.InitializeDocument(_doc.LineEndCap);

        InitializeCrossSection();
      }
    }


    public void InitializeCrossSection()
    {
      var crossSectionChoices = new SelectableListNodeList();
      var selectableTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ICrossSectionOfLine));

      foreach (var t in selectableTypes)
      {
        crossSectionChoices.Add(new SelectableListNode(t.Name, t, t == _doc.CrossSection.GetType()));
      }
      CrossSection = new ItemsController<Type>(crossSectionChoices, EhCrossSection_SelectionChangeCommitted);
    }

    private void EhCrossSection_SelectionChangeCommitted(Type type)
    {
      if (type is not null)
      {
        var crossSection = (ICrossSectionOfLine)Activator.CreateInstance(type);
        crossSection = crossSection.WithSize(_doc.Thickness1, _doc.Thickness2);
        var oldPen = _doc;
        _doc = _doc.WithCrossSection(crossSection);
        if (!object.ReferenceEquals(_doc, oldPen))
        {
          OnMadeDirty();
        }
      }
    }

    private void EhDashStartCapChanged(IMVCANDController obj)
    {
      _doc = _doc.WithDashStartCap((ILineCap)obj.ProvisionalModelObject);
      OnPropertyChanged(nameof(DashStartCap));
      OnMadeDirty();
    }

    private void EhDashEndCapChanged(IMVCANDController obj)
    {
      _doc = _doc.WithDashEndCap((ILineCap)obj.ProvisionalModelObject);
      OnPropertyChanged(nameof(EndCap));
      OnMadeDirty();
    }

    private void EhStartCapChanged(IMVCANDController obj)
    {
      _doc = _doc.WithLineStartCap((ILineCap)obj.ProvisionalModelObject);
      OnPropertyChanged(nameof(StartCap));
      OnMadeDirty();
    }

    private void EhEndCapChanged(IMVCANDController obj)
    {
      _doc = _doc.WithLineEndCap((ILineCap)obj.ProvisionalModelObject);
      OnPropertyChanged(nameof(EndCap));
      OnMadeDirty();
    }

    protected override void OnMadeDirty()
    {
      OnPropertyChanged(nameof(Pen));
      base.OnMadeDirty();
    }

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
        _doc = (PenX3D)ctrler.ModelObject;
        OnMadeDirty();
        OnPropertyChanged(nameof(Material));
        OnPropertyChanged(nameof(LineThickness1));
        OnPropertyChanged(nameof(LineThickness2));
        CrossSection.SelectedValue = _doc.CrossSection.GetType();
        OnPropertyChanged(nameof(DashPattern));
        DashStartCap.InitializeDocument(_doc.DashStartCap);
        DashEndCap.InitializeDocument(_doc.DashEndCap);
        StartCap.InitializeDocument(_doc.LineStartCap);
        EndCap.InitializeDocument(_doc.LineEndCap);
        OnPropertyChanged(nameof(LineJoin));
        OnPropertyChanged(nameof(MiterLimit));
        OnMadeDirty();
      }
    }

  }
}
