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
  /// <summary>
  /// Defines the view contract for editing table cleaning options.
  /// </summary>
  public interface IDataTableCleaningOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="DataTableCleaningOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDataTableCleaningOptionsView))]
  [UserControllerForObject(typeof(DataTableCleaningOptions))]
  public class DataTableCleaningOptionsController : MVCANControllerEditImmutableDocBase<DataTableCleaningOptions, IDataTableCleaningOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Binding

    private bool _clearData;

    /// <summary>
    /// Gets or sets a value indicating whether data should be cleared.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether data columns should be removed.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether column properties should be cleared.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether column properties should be removed.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether notes should be cleared.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether table properties should be cleared.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the table script should be cleared.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the data source should be cleared.
    /// </summary>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
