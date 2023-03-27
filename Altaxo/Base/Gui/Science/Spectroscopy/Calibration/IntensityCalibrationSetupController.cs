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

using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Data;

namespace Altaxo.Gui.Science.Spectroscopy.Calibration
{

  public record IntensityCalibrationSetup
  {
    /// <summary>
    /// Gets or sets the x column. The x-column is shared among both the YSignal column and the YDark column.
    /// </summary>
    public DataColumn XColumn { get; set; }

    /// <summary>
    /// Gets or sets the column containing the spectrum of the known source.
    /// </summary>
    public DataColumn YSignal { get; set; }

    /// <summary>
    /// Gets or sets the column containing the dark spectrum. This column is optional, as for some signals the dark signal is
    /// already subtracted.
    /// </summary>
    public DataColumn? YDark { get; set; }
  }

  public interface IIntensityCalibrationSetupView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IIntensityCalibrationSetupView))]
  [UserControllerForObject(typeof(IntensityCalibrationSetup))]
  public class IntensityCalibrationSetupController : MVCANControllerEditImmutableDocBase<IntensityCalibrationSetup, IIntensityCalibrationSetupView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public IntensityCalibrationSetupController()
    {
      CmdSwapYColumns = new RelayCommand(EhSwapYColumns, EhCanSwapYColumns);
    }



    #region Bindings

    public ICommand CmdSwapYColumns { get; }

    private string _xColumn;

    public string XColumn
    {
      get => _xColumn;
      set
      {
        if (!(_xColumn == value))
        {
          _xColumn = value;
          OnPropertyChanged(nameof(XColumn));
        }
      }
    }

    private DataColumn _signalColumn;

    public DataColumn SignalColumn
    {
      get => _signalColumn;
      set
      {
        if (!(_signalColumn == value))
        {
          _signalColumn = value;
          OnPropertyChanged(nameof(SignalColumn));
        }
      }
    }

    private DataColumn _darkColumn;

    public DataColumn DarkColumn
    {
      get => _darkColumn;
      set
      {
        if (!(_darkColumn == value))
        {
          _darkColumn = value;
          OnPropertyChanged(nameof(DarkColumn));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        XColumn = _doc.XColumn.Name;
        SignalColumn = _doc.YSignal;
        DarkColumn = _doc.YDark;
      }
    }

    public override bool Apply(bool disposeController)
    {

      _doc = new IntensityCalibrationSetup
      {
        XColumn = _doc.XColumn,
        YSignal = SignalColumn,
        YDark = DarkColumn,
      };

      return ApplyEnd(true, disposeController);
    }

    private void EhSwapYColumns()
    {
      (DarkColumn, SignalColumn) = (SignalColumn, DarkColumn);
    }

    private bool EhCanSwapYColumns()
    {
      return DarkColumn is not null;
    }

  }
}
