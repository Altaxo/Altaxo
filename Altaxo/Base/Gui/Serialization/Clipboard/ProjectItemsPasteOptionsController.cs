#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Serialization.Clipboard
{
  using Altaxo.Serialization.Clipboard;

  public interface IProjectItemsPasteOptionsView
  {
    /// <summary>If true, references will be relocated in the same way as the project items will be relocated.</summary>
    /// <value><c>true</c> if references should be relocated, <c>false</c> otherwise</value>
    bool RelocateReferences { get; set; }

    /// <summary>
    /// When true, at serialization the internal references are tried to keep internal, i.e. if for instance a table have to be renamed, the plot items in the deserialized graphs
    /// will be relocated to the renamed table.
    /// </summary>
    bool TryToKeepInternalReferences { get; set; }
  }

  [ExpectedTypeOfView(typeof(IProjectItemsPasteOptionsView))]
  [UserControllerForObject(typeof(ProjectItemsPasteOptions))]
  public class ProjectItemsPasteOptionsController : MVCANControllerEditOriginalDocBase<ProjectItemsPasteOptions, IProjectItemsPasteOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (null != _view)
      {
        _view.RelocateReferences = _doc.RelocateReferences.HasValue ? _doc.RelocateReferences.Value : true;
        _view.TryToKeepInternalReferences = _doc.TryToKeepInternalReferences.HasValue ? _doc.TryToKeepInternalReferences.Value : true;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.RelocateReferences = _view.RelocateReferences;
      _doc.TryToKeepInternalReferences = _view.TryToKeepInternalReferences;

      return ApplyEnd(true, disposeController);
    }
  }
}
