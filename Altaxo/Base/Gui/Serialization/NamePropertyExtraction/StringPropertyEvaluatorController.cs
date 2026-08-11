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
  /// Defines the view contract for editing string property evaluations.
  /// </summary>
  public interface IStringPropertyEvaluatorView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="StringPropertyEvaluator"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IStringPropertyEvaluatorView))]
  [UserControllerForObject(typeof(StringPropertyEvaluator))]
  public class StringPropertyEvaluatorController : MVCANControllerEditImmutableDocBase<StringPropertyEvaluator, IStringPropertyEvaluatorView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the name of the property that is evaluated.
    /// </summary>
    public string PropertyName
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(PropertyName));
        }
      }
    }

    /// <summary>
    /// Gets or sets the number of characters to ignore before the evaluated value.
    /// </summary>
    public int NumberOfIgnoredCharactersBefore
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfIgnoredCharactersBefore));
        }
      }
    }

    /// <summary>
    /// Gets or sets the number of characters to ignore after the evaluated value.
    /// </summary>
    public int NumberOfIgnoredCharactersAfter
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfIgnoredCharactersAfter));
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
        PropertyName = _doc.PropertyName;
        NumberOfIgnoredCharactersBefore = _doc.NumberOfIgnoredCharactersBefore;
        NumberOfIgnoredCharactersAfter = _doc.NumberOfIgnoredCharactersAfter;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (string.IsNullOrEmpty(PropertyName))
      {
        Current.Gui.ErrorMessageBox("Property name must not be empty.", "Error");
        return ApplyEnd(false, disposeController);
      }

      _doc = _doc with
      {
        PropertyName = PropertyName,
        NumberOfIgnoredCharactersBefore = NumberOfIgnoredCharactersBefore,
        NumberOfIgnoredCharactersAfter = NumberOfIgnoredCharactersAfter,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
