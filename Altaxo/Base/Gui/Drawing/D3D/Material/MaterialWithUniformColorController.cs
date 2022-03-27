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
using Altaxo.Drawing;
using Altaxo.Drawing.D3D.Material;

namespace Altaxo.Gui.Drawing.D3D.Material
{
  public interface IMaterialWithUniformColorView : IMaterialView, IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IMaterialWithUniformColorView))]
  [UserControllerForObject(typeof(MaterialWithUniformColor), 101)]
  public class MaterialWithUniformColorController : MVCANControllerEditImmutableDocBase<MaterialWithUniformColor, IMaterialWithUniformColorView>
  {
    #region Bindings

    private double _indexOfRefraction;

    public double IndexOfRefraction
    {
      get => _indexOfRefraction;
      set
      {
        if (!(_indexOfRefraction == value))
        {
          _indexOfRefraction = value;
          OnPropertyChanged(nameof(IndexOfRefraction));
        }
      }
    }
    private double _smoothness;

    public double Smoothness
    {
      get => _smoothness;
      set
      {
        if (!(_smoothness == value))
        {
          _smoothness = value;
          OnPropertyChanged(nameof(Smoothness));
        }
      }
    }
    private double _metalness;

    public double Metalness
    {
      get => _metalness;
      set
      {
        if (!(_metalness == value))
        {
          _metalness = value;
          OnPropertyChanged(nameof(Metalness));
        }
      }
    }

    private NamedColor _color;

    public NamedColor Color
    {
      get => _color;
      set
      {
        if (!(_color == value))
        {
          _color = value;
          OnPropertyChanged(nameof(Color));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Smoothness = _doc.Smoothness;
        Metalness = _doc.Metalness;
        IndexOfRefraction = _doc.IndexOfRefraction;
        Color = _doc.Color;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = (MaterialWithUniformColor)_doc.WithSpecularProperties(smoothness: Smoothness, metalness: Metalness, indexOfRefraction: IndexOfRefraction).WithColor(Color);
        return ApplyEnd(true, disposeController);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(
          string.Format(
            "Creating the material from your data failed\r\n" +
            "The message is: {0}", ex.Message), "Failure");
        return ApplyEnd(false, disposeController);
      }
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
