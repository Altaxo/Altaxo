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
using System.Collections.Generic;

namespace Altaxo.Gui.Serialization.Clipboard
{
  using Altaxo.Serialization.Clipboard;

  public interface IProjectItemsPasteOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IProjectItemsPasteOptionsView))]
  [UserControllerForObject(typeof(ProjectItemsPasteOptions))]
  public class ProjectItemsPasteOptionsController : MVCANControllerEditOriginalDocBase<ProjectItemsPasteOptions, IProjectItemsPasteOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _relocateReferences;

    public bool RelocateReferences
    {
      get => _relocateReferences;
      set
      {
        if (!(_relocateReferences == value))
        {
          _relocateReferences = value;
          OnPropertyChanged(nameof(RelocateReferences));
        }
      }
    }

    private bool _tryToKeepInternalReferences;

    public bool TryToKeepInternalReferences
    {
      get => _tryToKeepInternalReferences;
      set
      {
        if (!(_tryToKeepInternalReferences == value))
        {
          _tryToKeepInternalReferences = value;
          OnPropertyChanged(nameof(TryToKeepInternalReferences));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        RelocateReferences = _doc.RelocateReferences.HasValue ? _doc.RelocateReferences.Value : true;
        TryToKeepInternalReferences = _doc.TryToKeepInternalReferences.HasValue ? _doc.TryToKeepInternalReferences.Value : true;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.RelocateReferences = RelocateReferences;
      _doc.TryToKeepInternalReferences = TryToKeepInternalReferences;

      return ApplyEnd(true, disposeController);
    }
  }
}
