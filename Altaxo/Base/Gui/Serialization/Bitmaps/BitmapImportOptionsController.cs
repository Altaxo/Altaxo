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
using Altaxo.Gui.Common;
using Altaxo.Serialization.Bitmaps;

namespace Altaxo.Gui.Serialization.Bitmaps
{
  public interface IBitmapImportOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IBitmapImportOptionsView))]
  [UserControllerForObject(typeof(BitmapImportOptions))]
  public class BitmapImportOptionsController : MVCANControllerEditImmutableDocBase<BitmapImportOptions, IBitmapImportOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

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

    private bool _includeDimensionColumns;

    public bool IncludeDimensionColumns
    {
      get => _includeDimensionColumns;
      set
      {
        if (!(_includeDimensionColumns == value))
        {
          _includeDimensionColumns = value;
          OnPropertyChanged(nameof(IncludeDimensionColumns));
        }
      }
    }

    private bool _includePixelNumberColumns;

    public bool IncludePixelNumberColumns
    {
      get => _includePixelNumberColumns;
      set
      {
        if (!(_includePixelNumberColumns == value))
        {
          _includePixelNumberColumns = value;
          OnPropertyChanged(nameof(IncludePixelNumberColumns));
        }
      }
    }

    private bool _importTransposed;

    public bool ImportTransposed
    {
      get => _importTransposed;
      set
      {
        if (!(_importTransposed == value))
        {
          _importTransposed = value;
          OnPropertyChanged(nameof(ImportTransposed));
        }
      }
    }

    private ItemsController<ColorChannel> _colorChannel;

    public ItemsController<ColorChannel> ColorChannel
    {
      get => _colorChannel;
      set
      {
        if (!(_colorChannel == value))
        {
          _colorChannel?.Dispose();
          _colorChannel = value;
          OnPropertyChanged(nameof(ColorChannel));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NeutralColumnName = _doc.NeutralColumnName;
        IncludeFilePathAsProperty = _doc.IncludeFilePathAsProperty;
        IncludePixelNumberColumns = _doc.IncludePixelNumberColumns;
        IncludeDimensionColumns = _doc.IncludeDimensionColumns;
        ImportTransposed = _doc.ImportTransposed;
        ColorChannel = new ItemsController<ColorChannel>(new Collections.SelectableListNodeList(_doc.ColorChannel));
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        NeutralColumnName = string.IsNullOrEmpty(NeutralColumnName) ? "Y" : NeutralColumnName,
        IncludeFilePathAsProperty = IncludeFilePathAsProperty,
        IncludePixelNumberColumns = IncludePixelNumberColumns,
        IncludeDimensionColumns = IncludeDimensionColumns,
        ImportTransposed = ImportTransposed,
        ColorChannel = ColorChannel.SelectedValue,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
