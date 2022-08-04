#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy.Raman;

namespace Altaxo.Gui.Analysis.Spectroscopy.Raman
{
  public abstract class OptionsAndDestinationTable<T> where T: class
  {
    public virtual string OptionsLabel { get; } 

    public T Options { get; set; }

    public DataTable DestinationTable { get; set; }
  }

  

  public class NeonCalibrationOptionsAndDestinationTable : OptionsAndDestinationTable<NeonCalibrationOptions>
  {
    public override string OptionsLabel => "Neon calibration options:";

    public NeonCalibrationOptionsAndDestinationTable()
    {
      Options = new NeonCalibrationOptions();
    }
  }

  public class SiliconCalibrationOptionsAndDestinationTable : OptionsAndDestinationTable<SiliconCalibrationOptions>
  {
    public override string OptionsLabel => "Silicon calibration options:";

    public SiliconCalibrationOptionsAndDestinationTable()
      {
      Options = new SiliconCalibrationOptions();
      }
  }

  public interface IOptionsAndDestinationTableView : IDataContextAwareView
  {
  }


  [ExpectedTypeOfView(typeof(IOptionsAndDestinationTableView))]
  public class OptionsAndDestinationTableController<T> : MVCANControllerEditImmutableDocBase<OptionsAndDestinationTable<T>, IOptionsAndDestinationTableView> where T: class
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_optionsController, () => OptionsController = null);
    }

    #region Bindings


    private string _optionsLabel;

    public string OptionsLabel
    {
      get => _optionsLabel;
      set
      {
        if (!(_optionsLabel == value))
        {
          _optionsLabel = value;
          OnPropertyChanged(nameof(OptionsLabel));
        }
      }
    }


    private IMVCANController _optionsController;

    public IMVCANController OptionsController
    {
      get => _optionsController;
      set
      {
        if (!(_optionsController == value))
        {
          _optionsController?.Dispose();
          _optionsController = value;
          OnPropertyChanged(nameof(OptionsController));
        }
      }
    }


    private ItemsController<DataTable> _destinationTable;

    public ItemsController<DataTable> DestinationTable
    {
      get => _destinationTable;
      set
      {
        if (!(_destinationTable == value))
        {
          _destinationTable = value;
          OnPropertyChanged(nameof(DestinationTable));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        OptionsLabel = _doc.OptionsLabel;
        OptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.Options }, typeof(IMVCANController));

        var list = new SelectableListNodeList();
        list.Add(new SelectableListNode("<<New table>>", null, true));
        DataTable? youngestTable = null;
        foreach(var t in Current.Project.DataTableCollection)
        {
          if(t.DataSource is RamanCalibrationDataSource)
          {
            list.Add(new SelectableListNode(t.Name, t, false));

            if (youngestTable is null || t.CreationTimeUtc > youngestTable.CreationTimeUtc)
            {
              youngestTable = t;
            }
          }
        }
        DestinationTable = new ItemsController<DataTable>(list);
        if (youngestTable is not null)
        {
          DestinationTable.SelectedValue = youngestTable;
        }
      }
    }


    public override bool Apply(bool disposeController)
    {
      if (!OptionsController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.Options = (T)OptionsController.ModelObject;
      _doc.DestinationTable = DestinationTable.SelectedValue;

      return ApplyEnd(true, disposeController);
    }

    
  }
}
