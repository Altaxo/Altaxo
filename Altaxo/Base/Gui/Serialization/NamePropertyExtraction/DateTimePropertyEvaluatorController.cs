﻿#region Copyright

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
using System.Globalization;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.BasicTypes;
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines the view contract for editing date and time property evaluations.
  /// </summary>
  public interface IDateTimePropertyEvaluatorView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="DateTimePropertyEvaluator"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDateTimePropertyEvaluatorView))]
  [UserControllerForObject(typeof(DateTimePropertyEvaluator))]
  public class DateTimePropertyEvaluatorController : MVCANControllerEditImmutableDocBase<DateTimePropertyEvaluator, IDateTimePropertyEvaluatorView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(DateTimeStylesController, () => DateTimeStylesController = null!);
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
    /// Gets or sets the collection of available cultures.
    /// </summary>
    public ItemsController<CultureInfo> Cultures
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Cultures));
        }
      }
    }

    /// <summary>
    /// Gets or sets the date and time styles used for parsing.
    /// </summary>
    public EnumValueController DateTimeStylesController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(DateTimeStylesController));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether DateTimeOffset values should be parsed instead of DateTime values.
    /// </summary>
    public bool UseDateTimeOffset
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseDateTimeOffset));
        }
      }
    }

    /// <summary>
    /// Gets or sets a semicolon-separated list of custom date and time formats.
    /// </summary>
    public string CustomFormatsText
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(CustomFormatsText));
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
        DateTimeStylesController = new EnumValueController(_doc.DateTimeStyles);
        UseDateTimeOffset = _doc.UseDateTimeOffset;
        CustomFormatsText = string.Join("; ", _doc.CustomFormats);
        NumberOfIgnoredCharactersBefore = _doc.NumberOfIgnoredCharactersBefore;
        NumberOfIgnoredCharactersAfter = _doc.NumberOfIgnoredCharactersAfter;

        var cultureList = new SelectableListNodeList();
        foreach (var culture in System.Globalization.CultureInfo.GetCultures(CultureTypes.AllCultures))
        {
          cultureList.Add(new SelectableListNode(culture.DisplayName, culture, false));
        }
        cultureList.SetSelection(cl => _doc.LCID == ((CultureInfo)cl.Tag).LCID);
        Cultures = new ItemsController<CultureInfo>(cultureList);
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (!DateTimeStylesController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }

      if (string.IsNullOrEmpty(PropertyName))
      {
        Current.Gui.ErrorMessageBox("Property name must not be empty.", "Error");
        return ApplyEnd(false, disposeController);
      }

      var customFormats = string.IsNullOrWhiteSpace(CustomFormatsText)
        ? System.Array.Empty<string>()
        : CustomFormatsText.Split(';', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

      _doc = _doc with
      {
        PropertyName = PropertyName,
        DateTimeStyles = (System.Globalization.DateTimeStyles)DateTimeStylesController.ModelObject,
        LCID = Cultures.SelectedValue.LCID,
        UseDateTimeOffset = UseDateTimeOffset,
        CustomFormats = customFormats,
        NumberOfIgnoredCharactersBefore = NumberOfIgnoredCharactersBefore,
        NumberOfIgnoredCharactersAfter = NumberOfIgnoredCharactersAfter,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
