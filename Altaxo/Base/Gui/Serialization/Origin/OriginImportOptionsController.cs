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
  public interface IOriginImportOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IOriginImportOptionsView))]
  [UserControllerForObject(typeof(OriginImportOptions))]
  public class OriginImportOptionsController : MVCANControllerEditImmutableDocBase<OriginImportOptions, IOriginImportOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _useNeutralColumnName;

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

    public class IndexClass
    {
      public string Index { get; set; }
    }

    private ObservableCollection<IndexClass> _indicesOfGraphs;

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
