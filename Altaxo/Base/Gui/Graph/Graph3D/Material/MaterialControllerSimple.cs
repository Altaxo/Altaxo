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

using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Graph3D.Material
{
  public interface IMaterialViewSimple
  {
    /// <summary>
    /// Sets a value indicating whether this instance is no material allowed. If true, this instance can return null if no material was selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is no material allowed; otherwise, <c>false</c>.
    /// </value>
    bool IsNoMaterialAllowed { set; }

    IMaterial SelectedMaterial { get; set; }
  }

  [UserControllerForObject(typeof(IMaterial))]
  [ExpectedTypeOfView(typeof(IMaterialViewSimple))]
  public class MaterialControllerSimple : MVCANControllerEditImmutableDocBase<IMaterial, IMaterialViewSimple>
  {
    /// <summary>
    /// Sets a value indicating whether this instance is no material allowed. If true, this instance can return null if no material was selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is no material allowed; otherwise, <c>false</c>.
    /// </value>
    public bool IsNoMaterialAllowed { get; set; }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view != null)
      {
        _view.IsNoMaterialAllowed = IsNoMaterialAllowed;
        _view.SelectedMaterial = _doc;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (null != _view)
        _doc = _view.SelectedMaterial;

      if (IsNoMaterialAllowed && null != _doc && !_doc.IsVisible)
        _doc = null;

      return ApplyEnd(true, disposeController);
    }
  }
}
