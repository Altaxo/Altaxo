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
  public interface IColorProviderAHSBGradientView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IColorProviderAHSBGradientView))]
  [UserControllerForObject(typeof(ColorProviderAHSBGradient), 110)]
  public class ColorProviderAHSBGradientController : MVCANDControllerEditImmutableDocBase<ColorProviderAHSBGradient, IColorProviderAHSBGradientView>
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


    private decimal _hue0;

    public decimal Hue0
    {
      get => _hue0;
      set
      {
        if (!(_hue0 == value))
        {
          _hue0 = value;
          OnPropertyChanged(nameof(Hue0));
          OnMadeDirty();
        }
      }
    }

    private decimal _hue1;

    public decimal Hue1
    {
      get => _hue1;
      set
      {
        if (!(_hue1 == value))
        {
          _hue1 = value;
          OnPropertyChanged(nameof(Hue1));
          OnMadeDirty();
        }
      }
    }

    private decimal _saturation0;

    public decimal Saturation0
    {
      get => _saturation0;
      set
      {
        if (!(_saturation0 == value))
        {
          _saturation0 = value;
          OnPropertyChanged(nameof(Saturation0));
          OnMadeDirty();
        }
      }
    }
    private decimal _saturation1;

    public decimal Saturation1
    {
      get => _saturation1;
      set
      {
        if (!(_saturation1 == value))
        {
          _saturation1 = value;
          OnPropertyChanged(nameof(Saturation1));
          OnMadeDirty();
        }
      }
    }
    private decimal _brightness0;

    public decimal Brightness0
    {
      get => _brightness0;
      set
      {
        if (!(_brightness0 == value))
        {
          _brightness0 = value;
          OnPropertyChanged(nameof(Brightness0));
          OnMadeDirty();
        }
      }
    }
    private decimal _brightness1;

    public decimal Brightness1
    {
      get => _brightness1;
      set
      {
        if (!(_brightness1 == value))
        {
          _brightness1 = value;
          OnPropertyChanged(nameof(Brightness1));
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


        Hue0 = (decimal)_doc.Hue0;
        Hue1 = (decimal)_doc.Hue1;
        Saturation0 = (decimal)_doc.Saturation0;
        Saturation1 = (decimal)_doc.Saturation1;
        Brightness0 = (decimal)_doc.Brightness0;
        Brightness1 = (decimal)_doc.Brightness1;
        Opaqueness0 = (decimal)_doc.Opaqueness0;
        Opaqueness1 = (decimal)_doc.Opaqueness1;
      }
      
    }

    public override bool Apply(bool disposeController)
    {
      if (!_baseController.Apply(disposeController))
        return false;

      _doc = (ColorProviderAHSBGradient)_baseController.ModelObject;

      _doc = _doc
              .WithHue0((double)Hue0)
              .WithHue1((double)Hue1)
              .WithSaturation0((double)Saturation0)
              .WithSaturation1((double)Saturation1)
              .WithBrightness0((double)Brightness0)
              .WithBrightness1((double)Brightness1)
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
