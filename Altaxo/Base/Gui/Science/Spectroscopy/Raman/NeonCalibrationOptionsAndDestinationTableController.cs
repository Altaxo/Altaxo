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

namespace Altaxo.Gui.Science.Spectroscopy.Raman
{
  /// <summary>
  /// Combines processing options with a destination <see cref="DataTable"/>.
  /// </summary>
  /// <typeparam name="T">Type of the options document.</typeparam>
  public abstract class OptionsAndDestinationTable<T> where T: class
  {
    /// <summary>
    /// Gets the label that should be shown in the UI for the options section.
    /// </summary>
    public virtual string OptionsLabel { get; } 

    /// <summary>
    /// Gets or sets the options document.
    /// </summary>
    public T Options { get; set; }

    /// <summary>
    /// Gets or sets the destination table.
    /// </summary>
    public DataTable DestinationTable { get; set; }
  }

  

  /// <summary>
  /// UI model that combines <see cref="NeonCalibrationOptions"/> with a destination table.
  /// </summary>
  public class NeonCalibrationOptionsAndDestinationTable : OptionsAndDestinationTable<NeonCalibrationOptions>
  {
    /// <inheritdoc/>
    public override string OptionsLabel => "Neon calibration options:";

    /// <summary>
    /// Initializes a new instance of the <see cref="NeonCalibrationOptionsAndDestinationTable"/> class.
    /// </summary>
    public NeonCalibrationOptionsAndDestinationTable()
    {
      Options = new NeonCalibrationOptions();
    }
  }

  /// <summary>
  /// UI model that combines <see cref="SiliconCalibrationOptions"/> with a destination table.
  /// </summary>
  public class SiliconCalibrationOptionsAndDestinationTable : OptionsAndDestinationTable<SiliconCalibrationOptions>
  {
    /// <inheritdoc/>
    public override string OptionsLabel => "Silicon calibration options:";

    /// <summary>
    /// Initializes a new instance of the <see cref="SiliconCalibrationOptionsAndDestinationTable"/> class.
    /// </summary>
    public SiliconCalibrationOptionsAndDestinationTable()
      {
      Options = new SiliconCalibrationOptions();
      }
  }

  /// <summary>
  /// View interface for editing an options-and-destination-table document.
  /// </summary>
  public interface IOptionsAndDestinationTableView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Controller for editing an <see cref="OptionsAndDestinationTable{T}"/> instance.
  /// </summary>
  /// <typeparam name="T">Type of the wrapped options document.</typeparam>
  [ExpectedTypeOfView(typeof(IOptionsAndDestinationTableView))]
  public class OptionsAndDestinationTableController<T> : MVCANControllerEditImmutableDocBase<OptionsAndDestinationTable<T>, IOptionsAndDestinationTableView> where T: class
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_optionsController, () => OptionsController = null);
    }

    #region Bindings


    private string _optionsLabel;

    /// <summary>
    /// Gets or sets the label shown for the options section.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the controller responsible for editing the options document.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the controller that provides the selectable destination tables.
    /// </summary>
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

    /// <inheritdoc/>
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


    /// <inheritdoc/>
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
