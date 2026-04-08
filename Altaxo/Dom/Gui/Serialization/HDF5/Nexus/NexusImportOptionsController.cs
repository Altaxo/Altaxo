#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Altaxo.Serialization.HDF5.Nexus;

namespace Altaxo.Gui.Serialization.HDF5.Nexus
{
  /// <summary>
  /// Defines the view for editing NeXus import options.
  /// </summary>
  public interface INexusImportOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controls the editing of <see cref="NexusImportOptions"/> instances.
  /// </summary>
  [ExpectedTypeOfView(typeof(INexusImportOptionsView))]
  [UserControllerForObject(typeof(NexusImportOptions))]
  public class NexusImportOptionsController : MVCANControllerEditImmutableDocBase<NexusImportOptions, INexusImportOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _useNeutralColumnName;

    /// <summary>
    /// Gets or sets a value indicating whether a neutral column name should be used.
    /// </summary>
    public bool UseNeutralColumnName
    {
      get => _useNeutralColumnName;
      set
      {
        if (!(_useNeutralColumnName == value))
        {
          _useNeutralColumnName = value;
          OnPropertyChanged(nameof(UseNeutralColumnName));
        }
      }
    }

    private string _neutralColumnName;

    /// <summary>
    /// Gets or sets the neutral column name.
    /// </summary>
    public string NeutralColumnName
    {
      get => _neutralColumnName;
      set
      {
        if (!(_neutralColumnName == value))
        {
          _neutralColumnName = value;
          OnPropertyChanged(nameof(NeutralColumnName));
        }
      }
    }

    private bool _includeFilePathAsProperty;

    /// <summary>
    /// Gets or sets a value indicating whether the file path should be stored as a property.
    /// </summary>
    public bool IncludeFilePathAsProperty
    {
      get => _includeFilePathAsProperty;
      set
      {
        if (!(_includeFilePathAsProperty == value))
        {
          _includeFilePathAsProperty = value;
          OnPropertyChanged(nameof(IncludeFilePathAsProperty));
        }
      }
    }

    private bool _includeNXentryNameAsProperty;

    /// <summary>
    /// Gets or sets a value indicating whether the <c>NXentry</c> name should be stored as a property.
    /// </summary>
    public bool IncludeNXentryNameAsProperty
    {
      get => _includeNXentryNameAsProperty;
      set
      {
        if (!(_includeNXentryNameAsProperty == value))
        {
          _includeNXentryNameAsProperty = value;
          OnPropertyChanged(nameof(IncludeNXentryNameAsProperty));
        }
      }
    }
    private bool _includeNXentryIndexAsProperty;

    /// <summary>
    /// Gets or sets a value indicating whether the <c>NXentry</c> index should be stored as a property.
    /// </summary>
    public bool IncludeNXentryIndexAsProperty
    {
      get => _includeNXentryIndexAsProperty;
      set
      {
        if (!(_includeNXentryIndexAsProperty == value))
        {
          _includeNXentryIndexAsProperty = value;
          OnPropertyChanged(nameof(IncludeNXentryIndexAsProperty));
        }
      }
    }

    private bool _includeTitleAsProperty;

    /// <summary>
    /// Gets or sets a value indicating whether the entry title should be stored as a property.
    /// </summary>
    public bool IncludeTitleAsProperty
    {
      get => _includeTitleAsProperty;
      set
      {
        if (!(_includeTitleAsProperty == value))
        {
          _includeTitleAsProperty = value;
          OnPropertyChanged(nameof(IncludeTitleAsProperty));
        }
      }
    }

    private bool _includeLongNameAndUnitAsProperty;

    /// <summary>
    /// Gets or sets a value indicating whether the long name and unit should be stored as properties.
    /// </summary>
    public bool IncludeLongNameAndUnitAsProperty
    {
      get => _includeLongNameAndUnitAsProperty;
      set
      {
        if (!(_includeLongNameAndUnitAsProperty == value))
        {
          _includeLongNameAndUnitAsProperty = value;
          OnPropertyChanged(nameof(IncludeLongNameAndUnitAsProperty));
        }
      }
    }

    private bool _includeMetaDataAsProperties;

    /// <summary>
    /// Gets or sets a value indicating whether metadata should be imported as properties.
    /// </summary>
    public bool IncludeMetaDataAsProperties
    {
      get => _includeMetaDataAsProperties;
      set
      {
        if (!(_includeMetaDataAsProperties == value))
        {
          _includeMetaDataAsProperties = value;
          OnPropertyChanged(nameof(IncludeMetaDataAsProperties));
        }
      }
    }
    /// <summary>
    /// Represents an index entry shown in the UI.
    /// </summary>
    public class IndexClass
    {
      /// <summary>
      /// Gets or sets the entry index.
      /// </summary>
      public int Index { get; set; }
    }

    private ObservableCollection<IndexClass> _indicesOfGraphs;

    /// <summary>
    /// Gets or sets the indices of the entries to import.
    /// </summary>
    public ObservableCollection<IndexClass> IndicesOfEntries
    {
      get => _indicesOfGraphs;
      set
      {
        if (!(_indicesOfGraphs == value))
        {
          _indicesOfGraphs = value;
          OnPropertyChanged(nameof(IndicesOfEntries));
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
        UseNeutralColumnName = _doc.UseNeutralColumnName;
        NeutralColumnName = _doc.NeutralColumnName;
        IncludeFilePathAsProperty = _doc.IncludeFilePathAsProperty;
        IncludeNXentryNameAsProperty = _doc.IncludeNXentryNameAsProperty;
        IncludeNXentryIndexAsProperty = _doc.IncludeNXentryIndexAsProperty;
        IncludeTitleAsProperty = _doc.IncludeTitleAsProperty;
        IncludeLongNameAndUnitAsProperty = _doc.IncludeLongNameAndUnitAsProperty;
        IncludeMetaDataAsProperties = _doc.IncludeMetaDataAsProperties;
        IndicesOfEntries = new ObservableCollection<IndexClass>(_doc.IndicesOfImportedEntries.Select(x => new IndexClass { Index = x }));
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {

        UseNeutralColumnName = UseNeutralColumnName,
        NeutralColumnName = string.IsNullOrEmpty(NeutralColumnName) ? "Y" : NeutralColumnName,
        IncludeFilePathAsProperty = IncludeFilePathAsProperty,
        IncludeNXentryNameAsProperty = IncludeNXentryNameAsProperty,
        IncludeNXentryIndexAsProperty = IncludeNXentryIndexAsProperty,
        IncludeTitleAsProperty = IncludeTitleAsProperty,
        IncludeLongNameAndUnitAsProperty = IncludeLongNameAndUnitAsProperty,
        IncludeMetaDataAsProperties = IncludeMetaDataAsProperties,
        IndicesOfImportedEntries = IndicesOfEntries.Select(x => x.Index).ToImmutableList(),
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
