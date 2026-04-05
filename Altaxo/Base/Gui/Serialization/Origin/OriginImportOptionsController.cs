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
using Altaxo.Serialization.Origin;

namespace Altaxo.Gui.Serialization.Origin
{
  /// <summary>
  /// View interface for editing <see cref="OriginImportOptions"/>.
  /// </summary>
  public interface IOriginImportOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for editing <see cref="OriginImportOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IOriginImportOptionsView))]
  [UserControllerForObject(typeof(OriginImportOptions))]
  public class OriginImportOptionsController : MVCANControllerEditImmutableDocBase<OriginImportOptions, IOriginImportOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _useNeutralColumnName;

    /// <summary>
    /// Gets or sets a value indicating whether the imported y-column names should be neutral (constant).
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
    /// Gets or sets the neutral base name for imported y-columns.
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
    /// Gets or sets a value indicating whether the imported file path should be stored as a column property.
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

    private bool _ignoreSecondaryData;

    /// <summary>
    /// Gets or sets a value indicating whether secondary data should be ignored.
    /// </summary>
    public bool IgnoreSecondaryData
    {
      get => _ignoreSecondaryData;
      set
      {
        if (!(_ignoreSecondaryData == value))
        {
          _ignoreSecondaryData = value;
          OnPropertyChanged(nameof(IgnoreSecondaryData));
        }
      }
    }

    /// <summary>
    /// Model class used for binding a single Origin path string in the UI.
    /// </summary>
    public class IndexClass
    {
      /// <summary>
      /// Gets or sets the origin path identifier.
      /// </summary>
      public string Index { get; set; }
    }

    private ObservableCollection<IndexClass> _indicesOfGraphs;

    /// <summary>
    /// Gets or sets the list of graph indices / paths to import.
    /// </summary>
    public ObservableCollection<IndexClass> IndicesOfGraphs
    {
      get => _indicesOfGraphs;
      set
      {
        if (!(_indicesOfGraphs == value))
        {
          _indicesOfGraphs = value;
          OnPropertyChanged(nameof(IndicesOfGraphs));
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
        IndicesOfGraphs = new ObservableCollection<IndexClass>(_doc.PathsOfImportedData.Select(x => new IndexClass { Index = x }));
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
        PathsOfImportedData = IndicesOfGraphs.Select(x => x.Index).ToImmutableList(),
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
