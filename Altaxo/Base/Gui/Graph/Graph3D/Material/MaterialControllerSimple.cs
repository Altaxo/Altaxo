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
using System.Text;
using Altaxo.Drawing.D3D;
using Altaxo.Drawing.D3D.Material;
using Altaxo.Graph.Graph3D;

namespace Altaxo.Gui.Graph.Graph3D.Material
{
  public interface IMaterialViewSimple : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(IMaterial))]
  [ExpectedTypeOfView(typeof(IMaterialViewSimple))]
  public class MaterialControllerSimple : MVCANControllerEditImmutableDocBase<IMaterial, IMaterialViewSimple>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    
    private bool _isNoMaterialAllowed;

   /// <summary>
    /// Sets a value indicating whether this instance is no material allowed. If true, this instance can return null if no material was selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is no material allowed; otherwise, <c>false</c>.
    /// </value>
    public bool IsNoMaterialAllowed
    {
      get => _isNoMaterialAllowed;
      set
      {
        if (!(_isNoMaterialAllowed == value))
        {
          _isNoMaterialAllowed = value;
          OnPropertyChanged(nameof(IsNoMaterialAllowed));
        }
      }
    }

    private IMaterial _selectedMaterial;

    public IMaterial SelectedMaterial
    {
      get => _selectedMaterial;
      set
      {
        if (!(_selectedMaterial == value))
        {
          _selectedMaterial = value;
          OnPropertyChanged(nameof(SelectedMaterial));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IsNoMaterialAllowed = IsNoMaterialAllowed;
        SelectedMaterial = _doc;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = SelectedMaterial ?? MaterialInvisible.Instance;

      if (!IsNoMaterialAllowed && !_doc.IsVisible)
        return ApplyEnd(false, disposeController);

      return ApplyEnd(true, disposeController);
    }
  }
}
