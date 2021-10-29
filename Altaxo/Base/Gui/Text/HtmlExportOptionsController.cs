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
  public interface IHtmlExportOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IHtmlExportOptionsView))]
  [UserControllerForObject(typeof(HtmlExportOptions))]
  public class HtmlExportOptionsController : MVCANControllerEditOriginalDocBase<HtmlExportOptions, IHtmlExportOptionsView>, INotifyPropertyChanged
  {

    public HtmlExportOptionsController()
    {
      CommandSelectOutputFile = new RelayCommand(EhSelectOutputFile);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        if (_doc.EnableRemoveOldContentsOfOutputFolder | _doc.EnableRemoveOldContentsOfImageFolder)
        {
          var answer = Current.Gui.YesNoMessageBox("Please note that either the 'Enable remove of old contents of content folder' or 'Enable remove of old contents of image folder' is checked. " +
            "This can cause unwanted loss of data. Do you want to proceed with this options keeping checked?",
            "Attention - possible loss of data",
            false);

          if (!answer)
          {
            _doc.EnableRemoveOldContentsOfOutputFolder = false;
            _doc.EnableRemoveOldContentsOfImageFolder = false;
          }
        }
      }
      if (_view is not null)
      {
      }
    }

    #region Bindable properties

    public int SplitLevel { get { return _doc.SplitLevel; } set { _doc.SplitLevel = value; OnPropertyChanged(nameof(SplitLevel)); } }
    public bool EnableRemoveOldContentsOfOutputFolder { get { return _doc.EnableRemoveOldContentsOfOutputFolder; } set { _doc.EnableRemoveOldContentsOfOutputFolder = value; OnPropertyChanged(nameof(EnableRemoveOldContentsOfOutputFolder)); } }
    public string ImageFolderName { get { return _doc.ImageFolderName; } set { _doc.ImageFolderName = value; OnPropertyChanged(nameof(ImageFolderName)); } }
    public bool EnableRemoveOldContentsOfImageFolder { get { return _doc.EnableRemoveOldContentsOfImageFolder; } set { _doc.EnableRemoveOldContentsOfImageFolder = value; OnPropertyChanged(nameof(EnableRemoveOldContentsOfImageFolder)); } }
    public bool EnableHtmlEscape { get { return _doc.EnableHtmlEscape; } set { _doc.EnableHtmlEscape = value; OnPropertyChanged(nameof(EnableHtmlEscape)); } }
    public bool EnableAutoOutline { get { return true; } set { OnPropertyChanged(nameof(EnableAutoOutline)); } }
    public bool EnableLinkToPreviousSection { get { return _doc.EnableLinkToPreviousSection; } set { _doc.EnableLinkToPreviousSection = value; OnPropertyChanged(nameof(EnableLinkToPreviousSection)); } }
    public string LinkToPreviousSectionLabelText { get { return _doc.LinkToPreviousSectionLabelText; } set { _doc.LinkToPreviousSectionLabelText = value; OnPropertyChanged(nameof(LinkToPreviousSectionLabelText)); } }
    public bool EnableLinkToNextSection { get { return _doc.EnableLinkToNextSection; } set { _doc.EnableLinkToNextSection = value; OnPropertyChanged(nameof(EnableLinkToNextSection)); } }
    public string LinkToNextSectionLabelText { get { return _doc.LinkToNextSectionLabelText; } set { _doc.LinkToNextSectionLabelText = value; OnPropertyChanged(nameof(LinkToNextSectionLabelText)); } }
    public bool EnableLinkToTableOfContents { get { return _doc.EnableLinkToTableOfContents; } set { _doc.EnableLinkToTableOfContents = value; OnPropertyChanged(nameof(EnableLinkToTableOfContents)); } }
    public string LinkToTableOfContentsLabelText { get { return _doc.LinkToTableOfContentsLabelText; } set { _doc.LinkToTableOfContentsLabelText = value; OnPropertyChanged(nameof(LinkToTableOfContentsLabelText)); } }
    public bool ExpandChildDocuments { get { return _doc.ExpandChildDocuments; } set { _doc.ExpandChildDocuments = value; OnPropertyChanged(nameof(ExpandChildDocuments)); } }
    public bool RenumerateFigures { get { return _doc.RenumerateFigures; } set { _doc.RenumerateFigures = value; OnPropertyChanged(nameof(RenumerateFigures)); } }
    public string BodyTextFontFamilyName { get { return _doc.BodyTextFontFamily; } set { _doc.BodyTextFontFamily = value; OnPropertyChanged(nameof(BodyTextFontFamilyName)); } }
    public double BodyTextFontSize { get { return _doc.BodyTextFontSize; } set { _doc.BodyTextFontSize = value; OnPropertyChanged(nameof(BodyTextFontSize)); } }
    public string OutputFileName { get { return _doc.OutputFileName; } set { _doc.OutputFileName = value; OnPropertyChanged(nameof(OutputFileName)); } }
    public bool OpenHtmlViewer { get { return _doc.OpenHtmlViewer; } set { _doc.OpenHtmlViewer = value; OnPropertyChanged(nameof(OpenHtmlViewer)); } }

    public ICommand CommandSelectOutputFile { get; }

    private void EhSelectOutputFile()
    {
      var (dialogResult, outputFileName) = HtmlExportOptions.ShowGetOutputFileDialog();
      if (dialogResult == true)
      {
        OutputFileName = outputFileName;
      }
    }

    #endregion Bindable properties

    public override bool Apply(bool disposeController)
    {
      var failed = false;

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
