#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Serialization;

namespace Altaxo.Gui.Serialization
{

  public interface IStorageSettingsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IStorageSettingsView))]
  [UserControllerForObject(typeof(StorageSettings))]
  public class StorageSettingsController : MVCANControllerEditImmutableDocBase<StorageSettings, IStorageSettingsView>
  {
    #region Bindings

    public bool UseProgressiveStorage
    {
      get => _doc.AllowProgressiveStorage;
      set
      {
        if (!(UseProgressiveStorage == value))
        {
          _doc.AllowProgressiveStorage = value;
          OnPropertyChanged(nameof(UseProgressiveStorage));
        }
      }
    }

    public bool IsCompressionLevelUncompressed
    {
      get => _doc.ZipCompressionLevel == System.IO.Compression.CompressionLevel.NoCompression;
      set
      {
        var oldValue = _doc.ZipCompressionLevel;
        _doc.ZipCompressionLevel = System.IO.Compression.CompressionLevel.NoCompression;

        if (oldValue != _doc.ZipCompressionLevel)
        {
          OnPropertyChanged(nameof(IsCompressionLevelUncompressed));
          OnPropertyChanged(nameof(IsCompressionLevelMedium));
          OnPropertyChanged(nameof(IsCompressionLevelOptimal));
        }
      }
    }

    public bool IsCompressionLevelMedium
    {
      get => _doc.ZipCompressionLevel == System.IO.Compression.CompressionLevel.Fastest;
      set
      {
        var oldValue = _doc.ZipCompressionLevel;
        _doc.ZipCompressionLevel = System.IO.Compression.CompressionLevel.Fastest;

        if (oldValue != _doc.ZipCompressionLevel)
        {
          OnPropertyChanged(nameof(IsCompressionLevelUncompressed));
          OnPropertyChanged(nameof(IsCompressionLevelMedium));
          OnPropertyChanged(nameof(IsCompressionLevelOptimal));
        }
      }
    }

    public bool IsCompressionLevelOptimal
    {
      get => _doc.ZipCompressionLevel == System.IO.Compression.CompressionLevel.Optimal;
      set
      {
        var oldValue = _doc.ZipCompressionLevel;
        _doc.ZipCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;

        if (oldValue != _doc.ZipCompressionLevel)
        {
          OnPropertyChanged(nameof(IsCompressionLevelUncompressed));
          OnPropertyChanged(nameof(IsCompressionLevelMedium));
          OnPropertyChanged(nameof(IsCompressionLevelOptimal));
        }
      }
    }

    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);
    }


    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void AttachView()
    {
      _view!.DataContext = this;
      base.AttachView();
    }

    protected override void DetachView()
    {
      _view!.DataContext = null;
      base.DetachView();
    }
  }
}
