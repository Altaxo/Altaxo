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
using System.ComponentModel;
using System.Linq;
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines the view contract for editing separator-based name splitting options.
  /// </summary>
  public interface INameSplitterBySeparatorStringsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="NameSplitterBySeparatorStrings"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(INameSplitterBySeparatorStringsView))]
  [UserControllerForObject(typeof(NameSplitterBySeparatorStrings))]
  public class NameSplitterBySeparatorStringsController : MVCANControllerEditImmutableDocBase<NameSplitterBySeparatorStrings, INameSplitterBySeparatorStringsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Defines a wrapper class for a separator string that implements INotifyPropertyChanged.
    /// </summary>
    public class SeparatorString : INotifyPropertyChanged
    {
      /// <inheritdoc/>
      public event PropertyChangedEventHandler? PropertyChanged;

      /// <summary>
      /// Raises the PropertyChanged event for the specified property name. 
      /// </summary>
      /// <param name="propertyName">The name of the property that changed.</param>
      protected void OnPropertyChanged(string propertyName)
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      /// <summary>
      /// The separator string value.
      /// </summary>
      public string Separator
      {
        get => field;
        set
        {
          if (!(field == value))
          {
            field = value;
            OnPropertyChanged(nameof(Separator));
            OnPropertyChanged(nameof(DisplayName));
          }
        }
      }

      /// <summary>
      /// Gets a display-friendly name for the separator string, handling special cases for empty strings, spaces, and tabs.
      /// </summary>
      public string DisplayName => Separator switch
      {
        "" => "(empty string)",
        " " => "(space)",
        "\t" => "(tab)",
        _ => Separator
      };

    }

    /// <summary>
    /// Gets the collection of separator strings used to split names.
    /// </summary>
    public ObservableCollection<SeparatorString> Separators { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether empty entries should be removed after splitting.
    /// </summary>
    public bool RemoveEmptyEntries
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RemoveEmptyEntries));
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    override protected void Initialize(bool initData)
    {
      base.Initialize(initData);
      if (initData)
      {
        RemoveEmptyEntries = _doc.RemoveEmptyEntries;
        Separators.Clear();
        foreach (var separator in _doc.Separators)
        {
          Separators.Add(new SeparatorString { Separator = separator });
        }
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      var separators = Separators.Select(s => s.Separator).Where(s => !string.IsNullOrEmpty(s)).ToImmutableList();

      if (separators.Count == 0)
      {
        Current.Gui.ErrorMessageBox("At least one separator string must be specified.", "Error");
        return ApplyEnd(false, disposeController);
      }

      _doc = _doc with
      {
        RemoveEmptyEntries = RemoveEmptyEntries,
        Separators = separators,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
