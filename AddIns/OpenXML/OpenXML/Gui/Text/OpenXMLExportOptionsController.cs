﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Text;
using Altaxo.Units;

namespace Altaxo.Gui.Text
{
  public interface IOpenXMLExportOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IOpenXMLExportOptionsView))]
  [UserControllerForObject(typeof(OpenXMLExportOptions))]
  public class OpenXMLExportOptionsController : MVCANControllerEditOriginalDocBase<OpenXMLExportOptions, IOpenXMLExportOptionsView>, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public OpenXMLExportOptionsController()
    {
      CommandSelectOutputFile = new RelayCommand(EhSelectOutputFile);
      CommandSelectTemplateFile = new RelayCommand(EhSelectTemplateFile);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _applyMaximumImageWidth = _doc.MaximumImageWidth != null;
        _maximumImageWidth = _doc.MaximumImageWidth ?? new DimensionfulQuantity(0, Altaxo.Units.Length.Point.Instance);
        _applyMaximumImageHeight = _doc.MaximumImageHeight != null;
        _maximumImageHeight = _doc.MaximumImageHeight ?? new DimensionfulQuantity(0, Altaxo.Units.Length.Point.Instance);
      }
      if (null != _view)
      {
      }
    }

    #region Bindable properties

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private DimensionfulQuantity _maximumImageWidth = new DimensionfulQuantity(0, Altaxo.Units.Length.Point.Instance);
    private bool _applyMaximumImageWidth;
    private DimensionfulQuantity _maximumImageHeight = new DimensionfulQuantity(0, Altaxo.Units.Length.Point.Instance);
    private bool _applyMaximumImageHeight;
    public bool ApplyMaximumImageWidth { get { return _applyMaximumImageWidth; } set { _applyMaximumImageWidth = value; OnPropertyChanged(nameof(ApplyMaximumImageWidth)); } }
    public DimensionfulQuantity MaximumImageWidth { get { return _maximumImageWidth; } set { _maximumImageWidth = value; OnPropertyChanged(nameof(MaximumImageWidth)); } }
    public bool ApplyMaximumImageHeight { get { return _applyMaximumImageHeight; } set { _applyMaximumImageHeight = value; OnPropertyChanged(nameof(ApplyMaximumImageHeight)); } }
    public DimensionfulQuantity MaximumImageHeight { get { return _maximumImageHeight; } set { _maximumImageHeight = value; OnPropertyChanged(nameof(MaximumImageHeight)); } }
    public string ThemeName { get { return _doc.ThemeName; } set { _doc.ThemeName = value; OnPropertyChanged(nameof(ThemeName)); } }
    public string OutputFileName { get { return _doc.OutputFileName; } set { _doc.OutputFileName = value; OnPropertyChanged(nameof(OutputFileName)); } }
    public bool OpenApplication { get { return _doc.OpenApplication; } set { _doc.OpenApplication = value; OnPropertyChanged(nameof(OpenApplication)); } }

    public ICommand CommandSelectTemplateFile { get; }
    public ICommand CommandSelectOutputFile { get; }

    private void EhSelectOutputFile()
    {
      var (dialogResult, outputFileName) = OpenXMLExportOptions.ShowGetOutputFileDialog(OutputFileName);
      if (dialogResult == true)
      {
        OutputFileName = outputFileName;
      }
    }

    private void EhSelectTemplateFile()
    {
      var (dialogResult, templateFileName) = OpenXMLExportOptions.ShowGetTemplateFileDialog(System.IO.Path.IsPathRooted(ThemeName) ? ThemeName : null);
      if (dialogResult == true)
      {
        ThemeName = templateFileName;
      }
    }

    #endregion Bindable properties

    public override bool Apply(bool disposeController)
    {
      bool failed = false;

      if (_applyMaximumImageWidth && _maximumImageWidth.AsValueInSIUnits > 0)
        _doc.MaximumImageWidth = _maximumImageWidth;
      else
        _doc.MaximumImageWidth = null;

      if (_applyMaximumImageHeight && _maximumImageHeight.AsValueInSIUnits > 0)
        _doc.MaximumImageHeight = _maximumImageHeight;
      else
        _doc.MaximumImageHeight = null;


      if (string.IsNullOrEmpty(OutputFileName))
      {
        Current.Gui.ErrorMessageBox("Please provide a name for the output file.");
        failed |= true;
      }
      else if (!System.IO.Path.IsPathRooted(OutputFileName))
      {
        Current.Gui.ErrorMessageBox("The name of the output file must be an absolute path name.");
        failed |= true;
      }

      if (failed)
      {
        if (disposeController)
        {
          _doc = _clonedCopyOfDoc;
        }
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void AttachView()
    {
      _view.DataContext = this;
      base.AttachView();
    }

    protected override void DetachView()
    {
      _view.DataContext = null;
      base.DetachView();
    }
  }
}