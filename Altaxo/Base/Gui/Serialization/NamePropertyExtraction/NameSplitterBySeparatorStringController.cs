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
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines the view contract for editing separator-based name splitting options.
  /// </summary>
  public interface INameSplitterBySeparatorStringView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="NameSplitterBySeparatorString"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(INameSplitterBySeparatorStringView))]
  [UserControllerForObject(typeof(NameSplitterBySeparatorString))]
  public class NameSplitterBySeparatorStringController : MVCANControllerEditImmutableDocBase<NameSplitterBySeparatorString, INameSplitterBySeparatorStringView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings



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

    /// <summary>
    /// Gets or sets the separator string used to split the name into parts. 
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
        Separator = _doc.Separator;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (string.IsNullOrEmpty(Separator))
      {
        Current.Gui.ErrorMessageBox("The separator string must be specified.", "Error");
        return ApplyEnd(false, disposeController);
      }

      _doc = _doc with
      {
        RemoveEmptyEntries = RemoveEmptyEntries,
        Separator = Separator,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
