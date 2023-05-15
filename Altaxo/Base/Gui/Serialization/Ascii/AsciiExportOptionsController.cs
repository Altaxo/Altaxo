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

using System;
using System.Collections.Generic;
using System.Globalization;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
  public interface IAsciiExportOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IAsciiExportOptionsView))]
  [UserControllerForObject(typeof(AsciiExportOptions))]
  public class AsciiExportOptionsController : MVCANControllerEditImmutableDocBase<AsciiExportOptions, IAsciiExportOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private char _SeparatorChar;

    public char SeparatorChar
    {
      get => _SeparatorChar;
      set
      {
        if (!(_SeparatorChar == value))
        {
          _SeparatorChar = value;
          OnPropertyChanged(nameof(SeparatorChar));
        }
      }
    }

    private char _SubstituteChar;

    public char SubstituteChar
    {
      get => _SubstituteChar;
      set
      {
        if (!(_SubstituteChar == value))
        {
          _SubstituteChar = value;
          OnPropertyChanged(nameof(SubstituteChar));
        }
      }
    }

    private bool _ExportDataColumnNames;

    public bool ExportDataColumnNames
    {
      get => _ExportDataColumnNames;
      set
      {
        if (!(_ExportDataColumnNames == value))
        {
          _ExportDataColumnNames = value;
          OnPropertyChanged(nameof(ExportDataColumnNames));
        }
      }
    }

    private bool _ExportPropertyColumns;

    public bool ExportPropertyColumns
    {
      get => _ExportPropertyColumns;
      set
      {
        if (!(_ExportPropertyColumns == value))
        {
          _ExportPropertyColumns = value;
          OnPropertyChanged(nameof(ExportPropertyColumns));
        }
      }
    }

    private bool _ExportPropertiesWithName;

    public bool ExportPropertiesWithName
    {
      get => _ExportPropertiesWithName;
      set
      {
        if (!(_ExportPropertiesWithName == value))
        {
          _ExportPropertiesWithName = value;
          OnPropertyChanged(nameof(ExportPropertiesWithName));
        }
      }
    }

    private ItemsController<CultureInfo> _culture;

    public ItemsController<CultureInfo> Culture
    {
      get => _culture;
      set
      {
        if (!(_culture == value))
        {
          _culture = value;
          OnPropertyChanged(nameof(Culture));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        SeparatorChar = _doc.SeparatorChar;
        SubstituteChar = _doc.SubstituteForSeparatorChar;
        ExportDataColumnNames = _doc.ExportDataColumnNames;
        ExportPropertyColumns = _doc.ExportPropertyColumns;
        ExportPropertiesWithName = _doc.ExportPropertiesWithName;

        var _availableCulturesList = new SelectableListNodeList();
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        Array.Sort(cultures, (x, y) => string.Compare(x.DisplayName, y.DisplayName));
        _availableCulturesList.Add(new SelectableListNode(CultureInfo.InvariantCulture.DisplayName, CultureInfo.InvariantCulture, false));
        foreach (var cult in cultures)
          _availableCulturesList.Add(new SelectableListNode(cult.DisplayName, cult, false));

        Culture = new ItemsController<CultureInfo>(_availableCulturesList);
        Culture.SelectedValue = _doc.Culture;
      }
    }


    public override bool Apply(bool disposeController)
    {
      if (SeparatorChar == SubstituteChar)
      {
        Current.Gui.ErrorMessageBox("SeparatorChar must be different from SubstituteChar");
        return ApplyEnd(false, disposeController);
      }

      try
      {
        _doc = _doc with
        {
          SeparatorAndSubstituteChar = (SeparatorChar, SubstituteChar),
          ExportDataColumnNames = ExportDataColumnNames,
          ExportPropertyColumns = ExportPropertyColumns,
          ExportPropertiesWithName = ExportPropertiesWithName,
          Culture = Culture.SelectedValue,
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }

  }
}
