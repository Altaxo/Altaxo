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
using Altaxo.Graph.Gdi.Plot.ColorProvider;

namespace Altaxo.Gui.Graph.Gdi.Plot.ColorProvider
{
  using Altaxo.Drawing;
  using Altaxo.Graph;

  
  /// <summary>
  /// Interface for Gui elements that show the properties of the <see cref="ColorProviderBase"/> class.
  /// </summary>
  public interface IColorProviderBaseView : IDataContextAwareView
  {
   
  }

  [ExpectedTypeOfView(typeof(IColorProviderBaseView))]
  [UserControllerForObject(typeof(ColorProviderBase))]
  public class ColorProviderBaseController : MVCANDControllerEditImmutableDocBase<ColorProviderBase, IColorProviderBaseView>
  {
    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private NamedColor _colorBelow;

    public NamedColor ColorBelow
    {
      get => _colorBelow;
      set
      {
        if (!(_colorBelow == value))
        {
          _colorBelow = value;
          OnPropertyChanged(nameof(ColorBelow));
          OnMadeDirty();
        }
      }
    }
    private NamedColor _colorAbove;

    public NamedColor ColorAbove
    {
      get => _colorAbove;
      set
      {
        if (!(_colorAbove == value))
        {
          _colorAbove = value;
          OnPropertyChanged(nameof(ColorAbove));
          OnMadeDirty();
        }
      }
    }
    private NamedColor _colorInvalid;

    public NamedColor ColorInvalid
    {
      get => _colorInvalid;
      set
      {
        if (!(_colorInvalid == value))
        {
          _colorInvalid = value;
          OnPropertyChanged(nameof(ColorInvalid));
          OnMadeDirty();
        }
      }
    }


    private decimal _transparency;

    public decimal Transparency
    {
      get => _transparency;
      set
      {
        if (!(_transparency == value))
        {
          _transparency = value;
          OnPropertyChanged(nameof(Transparency));
          OnMadeDirty();
        }
      }
    }

    private int _colorSteps;

    public int ColorSteps
    {
      get => _colorSteps;
      set
      {
        if (!(_colorSteps == value))
        {
          _colorSteps = value;
          OnPropertyChanged(nameof(ColorSteps));
          OnMadeDirty();
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view is not null)
      {
        ColorBelow = _doc.ColorBelow;
        ColorAbove = _doc.ColorAbove;
        ColorInvalid = _doc.ColorInvalid;
        Transparency = (decimal)(_doc.Transparency*100);
        ColorSteps = _doc.ColorSteps;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc
            .WithColorBelow(ColorBelow)
            .WithColorAbove(ColorAbove)
            .WithColorInvalid(ColorInvalid)
            .WithTransparency((double)(Transparency/100))
            .WithColorSteps(ColorSteps);

      return ApplyEnd(true, disposeController);
    }

   
  }
}
