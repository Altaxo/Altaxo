#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Altaxo.Serialization.TA_Instruments;

namespace Altaxo.Gui.Serialization.TA_Instruments
{
  public interface IQ800ImportOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IQ800ImportOptionsView))]
  [UserControllerForObject(typeof(Q800ImportOptions))]
  public class Q800ImportOptionsController : MVCANControllerEditImmutableDocBase<Q800ImportOptions, IQ800ImportOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _convertUnitsToSIUnits;

    public bool ConvertUnitsToSIUnits
    {
      get => _convertUnitsToSIUnits;
      set
      {
        if (!(_convertUnitsToSIUnits == value))
        {
          _convertUnitsToSIUnits = value;
          OnPropertyChanged(nameof(ConvertUnitsToSIUnits));
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

    private ItemsController<MetadataDestination> _metadataDestination;

    public ItemsController<MetadataDestination> MetadataDestination
    {
      get => _metadataDestination;
      set
      {
        if (!(_metadataDestination == value))
        {
          _metadataDestination?.Dispose();
          _metadataDestination = value;
          OnPropertyChanged(nameof(MetadataDestination));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ConvertUnitsToSIUnits = _doc.ConvertUnitsToSIUnits;
        IncludeFilePathAsProperty = _doc.IncludeFilePathAsProperty;
        MetadataDestination = new ItemsController<MetadataDestination>(new Collections.SelectableListNodeList(_doc.HeaderLinesDestination));
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {

        ConvertUnitsToSIUnits = ConvertUnitsToSIUnits,
        IncludeFilePathAsProperty = IncludeFilePathAsProperty,
        HeaderLinesDestination = MetadataDestination.SelectedValue,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
