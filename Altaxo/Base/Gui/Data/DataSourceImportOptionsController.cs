#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Data
{
  public interface IDataSourceImportOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IDataSourceImportOptionsView))]
  [UserControllerForObject(typeof(IDataSourceImportOptions))]
  public class DataSourceImportOptionsController : MVCANControllerEditOriginalDocBase<DataSourceImportOptions, IDataSourceImportOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    public QuantityWithUnitGuiEnvironment TimeEnvironment => Altaxo.Gui.TimeEnvironment.Instance;

    private DimensionfulQuantity _minimumWaitingTimeAfterUpdate;

    public DimensionfulQuantity MinimumWaitingTimeAfterUpdate
    {
      get => _minimumWaitingTimeAfterUpdate;
      set
      {
        if (!(_minimumWaitingTimeAfterUpdate == value))
        {
          _minimumWaitingTimeAfterUpdate = value;
          OnPropertyChanged(nameof(MinimumWaitingTimeAfterUpdate));
        }
      }
    }

    private DimensionfulQuantity _maximumWaitingTimeAfterUpdate;

    public DimensionfulQuantity MaximumWaitingTimeAfterUpdate
    {
      get => _maximumWaitingTimeAfterUpdate;
      set
      {
        if (!(_maximumWaitingTimeAfterUpdate == value))
        {
          _maximumWaitingTimeAfterUpdate = value;
          OnPropertyChanged(nameof(MaximumWaitingTimeAfterUpdate));
        }
      }
    }

    private DimensionfulQuantity _minimumWaitingTimeAfterFirstTrigger;

    public DimensionfulQuantity MinimumWaitingTimeAfterFirstTrigger
    {
      get => _minimumWaitingTimeAfterFirstTrigger;
      set
      {
        if (!(_minimumWaitingTimeAfterFirstTrigger == value))
        {
          _minimumWaitingTimeAfterFirstTrigger = value;
          OnPropertyChanged(nameof(MinimumWaitingTimeAfterFirstTrigger));
        }
      }
    }

    private DimensionfulQuantity _maximumWaitingTimeAfterFirstTrigger;

    public DimensionfulQuantity MaximumWaitingTimeAfterFirstTrigger
    {
      get => _maximumWaitingTimeAfterFirstTrigger;
      set
      {
        if (!(_maximumWaitingTimeAfterFirstTrigger == value))
        {
          _maximumWaitingTimeAfterFirstTrigger = value;
          OnPropertyChanged(nameof(MaximumWaitingTimeAfterFirstTrigger));
        }
      }
    }

    private DimensionfulQuantity _minimumWaitingTimeAfterLastTrigger;

    public DimensionfulQuantity MinimumWaitingTimeAfterLastTrigger
    {
      get => _minimumWaitingTimeAfterLastTrigger;
      set
      {
        if (!(_minimumWaitingTimeAfterLastTrigger == value))
        {
          _minimumWaitingTimeAfterLastTrigger = value;
          OnPropertyChanged(nameof(MinimumWaitingTimeAfterLastTrigger));
        }
      }
    }

    private bool _doNotSaveTableData;

    public bool DoNotSaveTableData
    {
      get => _doNotSaveTableData;
      set
      {
        if (!(_doNotSaveTableData == value))
        {
          _doNotSaveTableData = value;
          OnPropertyChanged(nameof(DoNotSaveTableData));
        }
      }
    }

    private bool _executeScriptAfterImport;

    public bool ExecuteScriptAfterImport
    {
      get => _executeScriptAfterImport;
      set
      {
        if (!(_executeScriptAfterImport == value))
        {
          _executeScriptAfterImport = value;
          OnPropertyChanged(nameof(ExecuteScriptAfterImport));
        }
      }
    }

    private ItemsController<ImportTriggerSource> _triggerSource;

    public ItemsController<ImportTriggerSource> TriggerSource
    {
      get => _triggerSource;
      set
      {
        if (!(_triggerSource == value))
        {
          _triggerSource?.Dispose();
          _triggerSource = value;
          OnPropertyChanged(nameof(TriggerSource));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        TriggerSource = new ItemsController<ImportTriggerSource>(new Collections.SelectableListNodeList(_doc.ImportTriggerSource));
     
        DoNotSaveTableData = _doc.DoNotSaveCachedTableData;
        ExecuteScriptAfterImport = _doc.ExecuteTableScriptAfterImport;
        MinimumWaitingTimeAfterUpdate = new DimensionfulQuantity(_doc.MinimumWaitingTimeAfterUpdateInSeconds, Altaxo.Units.Time.Second.Instance).AsQuantityIn(TimeEnvironment.DefaultUnit);
        MaximumWaitingTimeAfterUpdate = new DimensionfulQuantity(_doc.MaximumWaitingTimeAfterUpdateInSeconds, Altaxo.Units.Time.Second.Instance).AsQuantityIn(TimeEnvironment.DefaultUnit);
        MinimumWaitingTimeAfterFirstTrigger = new DimensionfulQuantity(_doc.MinimumWaitingTimeAfterFirstTriggerInSeconds, Altaxo.Units.Time.Second.Instance).AsQuantityIn(TimeEnvironment.DefaultUnit);
        MaximumWaitingTimeAfterFirstTrigger = new DimensionfulQuantity(_doc.MaximumWaitingTimeAfterFirstTriggerInSeconds, Altaxo.Units.Time.Second.Instance).AsQuantityIn(TimeEnvironment.DefaultUnit);
        MinimumWaitingTimeAfterLastTrigger = new DimensionfulQuantity(_doc.MinimumWaitingTimeAfterLastTriggerInSeconds, Altaxo.Units.Time.Second.Instance).AsQuantityIn(TimeEnvironment.DefaultUnit);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.DoNotSaveCachedTableData = DoNotSaveTableData;
      _doc.ExecuteTableScriptAfterImport = ExecuteScriptAfterImport;
      _doc.ImportTriggerSource = TriggerSource.SelectedValue;

      var minUpdate = MinimumWaitingTimeAfterUpdate.AsValueIn(Altaxo.Units.Time.Second.Instance);
      var maxUpdate = MaximumWaitingTimeAfterUpdate.AsValueIn(Altaxo.Units.Time.Second.Instance);
      var minFirstTrig = MinimumWaitingTimeAfterFirstTrigger.AsValueIn(Altaxo.Units.Time.Second.Instance);
      var maxFirstTrig =MaximumWaitingTimeAfterFirstTrigger.AsValueIn(Altaxo.Units.Time.Second.Instance);
      var minLastTrig = MinimumWaitingTimeAfterLastTrigger.AsValueIn(Altaxo.Units.Time.Second.Instance);

      if (!(maxUpdate > 0))
      {
        Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterUpdate should be > 0");
        return ApplyEnd(false, disposeController);
      }
      if (!(minUpdate >= 0))
      {
        Current.Gui.ErrorMessageBox("MinimumWaitingTimeAfterUpdate should be >= 0");
        return ApplyEnd(false, disposeController);

      }
      if (!(maxUpdate >= minUpdate))
      {
        Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterUpdate should be >= MinimumWaitingTimeAfterUpdate");
        return ApplyEnd(false, disposeController);
      }

      if (!(minFirstTrig >= 0))
      {
        Current.Gui.ErrorMessageBox("MinimumWaitingTimeAfterFirstTrigger should be > 0");
        return ApplyEnd(false, disposeController);
      }

      if (!(maxFirstTrig >= 0))
      {
        Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterFirstTrigger should be > 0");
        return ApplyEnd(false, disposeController);
      }

      if (!(minLastTrig >= 0))
      {
        Current.Gui.ErrorMessageBox("MinimumWaitingTimeAfterLastTrigger should be > 0");
        return ApplyEnd(false, disposeController);
      }

      if (!(maxFirstTrig >= minFirstTrig))
      {
        Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterFirstTrigger should be >= MinimumWaitingTimeAfterFirstTrigger");
        return ApplyEnd(false, disposeController);
      }

      _doc.MinimumWaitingTimeAfterUpdateInSeconds = minUpdate;
      _doc.MaximumWaitingTimeAfterUpdateInSeconds = maxUpdate;
      _doc.MinimumWaitingTimeAfterFirstTriggerInSeconds = minFirstTrig;
      _doc.MinimumWaitingTimeAfterLastTriggerInSeconds = minLastTrig;
      _doc.MaximumWaitingTimeAfterFirstTriggerInSeconds = maxFirstTrig;

      return ApplyEnd(true, disposeController);
    }
  }
}
