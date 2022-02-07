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
using Altaxo.Graph.Gdi.Plot.ColorProvider;

namespace Altaxo.Gui.Graph.Gdi.Plot.ColorProvider
{
  public interface IColorProviderARGBGradientView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IColorProviderARGBGradientView))]
  [UserControllerForObject(typeof(ColorProviderARGBGradient), 110)]
  public class ColorProviderARGBGradientController : MVCANDControllerEditImmutableDocBase<ColorProviderARGBGradient, IColorProviderARGBGradientView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_baseController, () => _baseController = null);
    }

    #region Bindings

    private ColorProviderBaseController _baseController;

    public ColorProviderBaseController BaseController
    {
      get => _baseController;
      set
      {
        if (!(_baseController == value))
        {
          _baseController?.Dispose();
          _baseController = value;
          OnPropertyChanged(nameof(BaseController));
          OnMadeDirty();
        }
      }
    }

    private decimal _red0;

    public decimal Red0
    {
      get => _red0;
      set
      {
        if (!(_red0 == value))
        {
          _red0 = value;
          OnPropertyChanged(nameof(Red0));
          OnMadeDirty();
        }
      }
    }
    private decimal _red1;

    public decimal Red1
    {
      get => _red1;
      set
      {
        if (!(_red1 == value))
        {
          _red1 = value;
          OnPropertyChanged(nameof(Red1));
          OnMadeDirty();
        }
      }
    }
    private decimal _green0;

    public decimal Green0
    {
      get => _green0;
      set
      {
        if (!(_green0 == value))
        {
          _green0 = value;
          OnPropertyChanged(nameof(Green0));
          OnMadeDirty();
        }
      }
    }
    private decimal _green1;

    public decimal Green1
    {
      get => _green1;
      set
      {
        if (!(_green1 == value))
        {
          _green1 = value;
          OnPropertyChanged(nameof(Green1));
          OnMadeDirty();
        }
      }
    }
    private decimal _blue0;

    public decimal Blue0
    {
      get => _blue0;
      set
      {
        if (!(_blue0 == value))
        {
          _blue0 = value;
          OnPropertyChanged(nameof(Blue0));
          OnMadeDirty();
        }
      }
    }
    private decimal _blue1;

    public decimal Blue1
    {
      get => _blue1;
      set
      {
        if (!(_blue1 == value))
        {
          _blue1 = value;
          OnPropertyChanged(nameof(Blue1));
          OnMadeDirty();
        }
      }
    }

    private decimal _opaqueness0;

    public decimal Opaqueness0
    {
      get => _opaqueness0;
      set
      {
        if (!(_opaqueness0 == value))
        {
          _opaqueness0 = value;
          OnPropertyChanged(nameof(Opaqueness0));
          OnMadeDirty();
        }
      }
    }
    private decimal _opaqueness1;

    public decimal Opaqueness1
    {
      get => _opaqueness1;
      set
      {
        if (!(_opaqueness1 == value))
        {
          _opaqueness1 = value;
          OnPropertyChanged(nameof(Opaqueness1));
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
        _baseController = new ColorProviderBaseController() { UseDocumentCopy = UseDocument.Directly };
        _baseController.InitializeDocument(_doc);
        _baseController.MadeDirty += EhBaseControllerChanged;
       

        Red0 = (decimal)_doc.Red0;
        Red1 = (decimal)_doc.Red1;
        Green0 = (decimal)_doc.Green0;
        Green1 = (decimal)_doc.Green1;
        Blue0 = (decimal)_doc.Blue0;
        Blue1 = (decimal)_doc.Blue1;
        Opaqueness0 = (decimal)_doc.Opaqueness0;
        Opaqueness1 = (decimal)_doc.Opaqueness1;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_baseController.Apply(disposeController))
        return false;

      _doc = (ColorProviderARGBGradient)_baseController.ModelObject;

      _doc = _doc
            .WithRed0((double)Red0)
            .WithRed1((double)Red1)
            .WithGreen0((double)Green0)
            .WithGreen1((double)Green1)
            .WithBlue0((double)Blue0)
            .WithBlue1((double)Blue1)
            .WithOpaqueness0((double)Opaqueness0)
            .WithOpaqueness1((double)Opaqueness1);

      return ApplyEnd(true, disposeController);
    }

    private void EhBaseControllerChanged(IMVCANDController ctrl)
    {
      OnMadeDirty();
    }
  }
}
