#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Serialization.Omnic;

namespace Altaxo.Gui.Serialization.Omnic
{
  /// <summary>
  /// View interface for <see cref="OmnicSPGImportOptionsController"/>.
  /// </summary>
  public interface IOmnicSPGImportOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IOmnicSPGImportOptionsView))]
  [UserControllerForObject(typeof(OmnicSPGImportOptions))]
  /// <summary>
  /// Controller for editing <see cref="OmnicSPGImportOptions"/>.
  /// </summary>
  public class OmnicSPGImportOptionsController : MVCANControllerEditImmutableDocBase<OmnicSPGImportOptions, IOmnicSPGImportOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets a value indicating whether the imported y-column names should be neutral (constant).
    /// </summary>
    public bool UseNeutralColumnName
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseNeutralColumnName));
        }
      }
    }

    /// <summary>
    /// Gets or sets the neutral base name for imported y-columns.
    /// </summary>
    public string NeutralColumnName
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NeutralColumnName));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the imported file path should be stored as a column property.
    /// </summary>
    public bool IncludeFilePathAsProperty
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IncludeFilePathAsProperty));
        }
      }
    }

    /// <summary>
    /// Model class used for binding a single spectrum index in the UI.
    /// </summary>
    public class IndexClass
    {
      /// <summary>
      /// Gets or sets the spectrum index.
      /// </summary>
      public int Index { get; set; }
    }

    /// <summary>
    /// Gets or sets the collection of spectrum indices to import.
    /// </summary>
    public ObservableCollection<IndexClass> IndicesOfImportedSpectra
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IndicesOfImportedSpectra));
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
        IndicesOfImportedSpectra = new ObservableCollection<IndexClass>(_doc.IndicesOfImportedSpectra.Select(x => new IndexClass { Index = x }));

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
        IndicesOfImportedSpectra = IndicesOfImportedSpectra.Select(x => x.Index).ToImmutableList(),

      };

      return ApplyEnd(true, disposeController);
    }

  }
}
