#region Copyright

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

#nullable disable
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
  public interface IMamlExportOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IMamlExportOptionsView))]
  [UserControllerForObject(typeof(MamlExportOptions))]
  public class MamlExportOptionsController : MVCANControllerEditOriginalDocBase<MamlExportOptions, IMamlExportOptionsView>, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public MamlExportOptionsController()
    {
      CommandSelectOutputFile = new RelayCommand(EhSelectOutputFile);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        if (_doc.EnableRemoveOldContentsOfContentFolder | _doc.EnableRemoveOldContentsOfImageFolder)
        {
          var answer = Current.Gui.YesNoMessageBox("Please note that either the 'Enable remove of old contents of content folder' or 'Enable remove of old contents of image folder' is checked. " +
            "This can cause unwanted loss of data. Do you want to proceed with this options keeping checked?",
            "Attention - possible loss of data",
            false);

          if (!answer)
          {
            _doc.EnableRemoveOldContentsOfContentFolder = false;
            _doc.EnableRemoveOldContentsOfImageFolder = false;
          }
        }
      }
      if (_view is not null)
      {
      }
    }

    #region Bindable properties

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int SplitLevel { get { return _doc.SplitLevel; } set { _doc.SplitLevel = value; OnPropertyChanged(nameof(SplitLevel)); } }
    public string ContentFolderName { get { return _doc.ContentFolderName; } set { _doc.ContentFolderName = value; OnPropertyChanged(nameof(ContentFolderName)); } }
    public bool EnableRemoveOldContentsOfContentFolder { get { return _doc.EnableRemoveOldContentsOfContentFolder; } set { _doc.EnableRemoveOldContentsOfContentFolder = value; OnPropertyChanged(nameof(EnableRemoveOldContentsOfContentFolder)); } }
    public string ContentFileNameBase { get { return _doc.ContentFileNameBase; } set { _doc.ContentFileNameBase = value; OnPropertyChanged(nameof(ContentFileNameBase)); } }
    public string ImageFolderName { get { return _doc.ImageFolderName; } set { _doc.ImageFolderName = value; OnPropertyChanged(nameof(ImageFolderName)); } }
    public bool EnableRemoveOldContentsOfImageFolder { get { return _doc.EnableRemoveOldContentsOfImageFolder; } set { _doc.EnableRemoveOldContentsOfImageFolder = value; OnPropertyChanged(nameof(EnableRemoveOldContentsOfImageFolder)); } }
    public bool EnableHtmlEscape { get { return _doc.EnableHtmlEscape; } set { _doc.EnableHtmlEscape = value; OnPropertyChanged(nameof(EnableHtmlEscape)); } }
    public bool EnableAutoOutline { get { return _doc.EnableAutoOutline; } set { _doc.EnableAutoOutline = value; OnPropertyChanged(nameof(EnableAutoOutline)); } }
    public bool EnableLinkToPreviousSection { get { return _doc.EnableLinkToPreviousSection; } set { _doc.EnableLinkToPreviousSection = value; OnPropertyChanged(nameof(EnableLinkToPreviousSection)); } }
    public string LinkToPreviousSectionLabelText { get { return _doc.LinkToPreviousSectionLabelText; } set { _doc.LinkToPreviousSectionLabelText = value; OnPropertyChanged(nameof(LinkToPreviousSectionLabelText)); } }
    public bool EnableLinkToNextSection { get { return _doc.EnableLinkToNextSection; } set { _doc.EnableLinkToNextSection = value; OnPropertyChanged(nameof(EnableLinkToNextSection)); } }
    public string LinkToNextSectionLabelText { get { return _doc.LinkToNextSectionLabelText; } set { _doc.LinkToNextSectionLabelText = value; OnPropertyChanged(nameof(LinkToNextSectionLabelText)); } }
    public bool RenumerateFigures { get { return _doc.RenumerateFigures; } set { _doc.RenumerateFigures = value; OnPropertyChanged(nameof(RenumerateFigures)); } }
    public bool ExpandChildDocuments { get { return _doc.ExpandChildDocuments; } set { _doc.ExpandChildDocuments = value; OnPropertyChanged(nameof(ExpandChildDocuments)); } }
    public string BodyTextFontFamilyName { get { return _doc.BodyTextFontFamily; } set { _doc.BodyTextFontFamily = value; OnPropertyChanged(nameof(BodyTextFontFamilyName)); } }
    public double BodyTextFontSize { get { return _doc.BodyTextFontSize; } set { _doc.BodyTextFontSize = value; OnPropertyChanged(nameof(BodyTextFontSize)); } }
    public bool IsIntendedForHtmlHelp1File { get { return _doc.IsIntendedForHtmlHelp1File; } set { _doc.IsIntendedForHtmlHelp1File = value; OnPropertyChanged(nameof(IsIntendedForHtmlHelp1File)); } }
    public string OutputFileName { get { return _doc.OutputFileName; } set { _doc.OutputFileName = value; OnPropertyChanged(nameof(OutputFileName)); } }
    public bool OpenHelpFileBuilder { get { return _doc.OpenHelpFileBuilder; } set { _doc.OpenHelpFileBuilder = value; OnPropertyChanged(nameof(OpenHelpFileBuilder)); } }

    public ICommand CommandSelectOutputFile { get; }

    private void EhSelectOutputFile()
    {
      var (dialogResult, outputFileName) = MamlExportOptions.ShowGetOutputFileDialog();
      if (dialogResult == true)
      {
        OutputFileName = outputFileName;
      }
    }

    #endregion Bindable properties

    public override bool Apply(bool disposeController)
    {
      bool failed = false;

      if (string.IsNullOrEmpty(ImageFolderName))
      {
        Current.Gui.ErrorMessageBox("Please provide a name for the image folder.");
        failed |= true;
      }

      if (string.IsNullOrEmpty(BodyTextFontFamilyName))
      {
        Current.Gui.ErrorMessageBox("Please provide a font family name of the body text.");
        failed |= true;
      }

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
