#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  public interface IDataTableCleaningOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IDataTableCleaningOptionsView))]
  [UserControllerForObject(typeof(DataTableCleaningOptions))]
  public class DataTableCleaningOptionsController : MVCANControllerEditImmutableDocBase<DataTableCleaningOptions, IDataTableCleaningOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Binding

    private bool _clearData;

    public bool ClearData
    {
      get => _clearData;
      set
      {
        if (!(_clearData == value))
        {
          _clearData = value;
          OnPropertyChanged(nameof(ClearData));
        }
      }
    }

    private bool _removeDataColumns;

    public bool RemoveDataColumns
    {
      get => _removeDataColumns;
      set
      {
        if (!(_removeDataColumns == value))
        {
          _removeDataColumns = value;
          OnPropertyChanged(nameof(RemoveDataColumns));
        }
      }
    }

    private bool _clearColumnProperties;

    public bool ClearColumnProperties
    {
      get => _clearColumnProperties;
      set
      {
        if (!(_clearColumnProperties == value))
        {
          _clearColumnProperties = value;
          OnPropertyChanged(nameof(ClearColumnProperties));
        }
      }
    }

    private bool _removeColumnProperties;

    public bool RemoveColumnProperties
    {
      get => _removeColumnProperties;
      set
      {
        if (!(_removeColumnProperties == value))
        {
          _removeColumnProperties = value;
          OnPropertyChanged(nameof(RemoveColumnProperties));
        }
      }
    }

    private bool _clearNotes;

    public bool ClearNotes
    {
      get => _clearNotes;
      set
      {
        if (!(_clearNotes == value))
        {
          _clearNotes = value;
          OnPropertyChanged(nameof(ClearNotes));
        }
      }
    }

    private bool _clearTableProperties;

    public bool ClearTableProperties
    {
      get => _clearTableProperties;
      set
      {
        if (!(_clearTableProperties == value))
        {
          _clearTableProperties = value;
          OnPropertyChanged(nameof(ClearTableProperties));
        }
      }
    }

    private bool _clearTableScript;

    public bool ClearTableScript
    {
      get => _clearTableScript;
      set
      {
        if (!(_clearTableScript == value))
        {
          _clearTableScript = value;
          OnPropertyChanged(nameof(ClearTableScript));
        }
      }
    }

    private bool _clearDataSource;

    public bool ClearDataSource
    {
      get => _clearDataSource;
      set
      {
        if (!(_clearDataSource == value))
        {
          _clearDataSource = value;
          OnPropertyChanged(nameof(ClearDataSource));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ClearData = _doc.ClearData;
        RemoveDataColumns = _doc.RemoveDataColumns;
        ClearColumnProperties = _doc.ClearColumnProperties;
        RemoveColumnProperties = _doc.RemoveColumnProperties;
        ClearNotes = _doc.ClearNotes;
        ClearTableProperties = _doc.ClearTableProperties;
        ClearTableScript = _doc.ClearTableScript;
        ClearDataSource = _doc.ClearDataSource;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        ClearData = ClearData,
        RemoveDataColumns = RemoveDataColumns,
        ClearColumnProperties = ClearColumnProperties,
        RemoveColumnProperties = RemoveColumnProperties,
        ClearNotes = ClearNotes,
        ClearTableProperties = ClearTableProperties,
        ClearTableScript = ClearTableScript,
        ClearDataSource = ClearDataSource,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
