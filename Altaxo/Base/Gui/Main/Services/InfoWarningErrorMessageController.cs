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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Altaxo.Main.Services;
using System.Collections.ObjectModel;
using Altaxo.Gui.Workbench;
using System.Windows.Input;

namespace Altaxo.Gui.Main.Services
{
  public interface IInfoWarningErrorMessageView
  {
    /// <summary>
    /// Sets the data context for the view.
    /// </summary>
    /// <value>
    /// The data context.
    /// </value>
    object DataContext { set; }

    int[] ColumnWidths { get; set; }
  }

  [ExpectedTypeOfView(typeof(IInfoWarningErrorMessageView))]
  public class InfoWarningErrorMessageController : AbstractPadContent
  {
    private bool _viewDirectionRecentIsFirst = true;
    private int[] _columnWidths;
    private IInfoWarningErrorMessageView _view;
    private ObservableCollection<InfoWarningErrorTextMessageItem> _unreversedDoc;
    private Altaxo.Collections.ObservableCollectionReversingWrapper<InfoWarningErrorTextMessageItem> _reversedDoc;
    private IReadOnlyList<InfoWarningErrorTextMessageItem> _currentDoc;

    private IInfoWarningErrorTextMessageService _cachedService;

    private ICommand _commandClearAllMessages;
    private ICommand _commandReverseMessages;

    #region Bindable properties for view

    public IReadOnlyList<InfoWarningErrorTextMessageItem> MessageItems
    {
      get { return _currentDoc; }
      private set
      {
        if (!object.ReferenceEquals(_currentDoc, value))
        {
          _currentDoc = value;
          OnPropertyChanged(nameof(MessageItems));
        }
      }
    }

    public ICommand CommandClearAllMessages { get { return _commandClearAllMessages; } }

    public ICommand CommandReverseMessageOrder { get { return _commandReverseMessages; } }

    #endregion Bindable properties for view

    public InfoWarningErrorMessageController()
    {
      Current.ServiceChanged += EhServiceChanged;
      _unreversedDoc = new ObservableCollection<InfoWarningErrorTextMessageItem>();
      _reversedDoc = new Collections.ObservableCollectionReversingWrapper<InfoWarningErrorTextMessageItem>(_unreversedDoc);
      _currentDoc = _unreversedDoc;

      _commandClearAllMessages = new RelayCommand(this.EhClearAllMessages);
      _commandReverseMessages = new RelayCommand(this.EhReverseAllMessages);
      EhServiceChanged();
    }

    public override void Dispose()
    {
      Current.ServiceChanged -= EhServiceChanged;

      if (_cachedService != null)
      {
        _cachedService.MessageAdded -= EhMessageAdded;
        _cachedService = null;
      }

      base.Dispose();
    }

    private void EhServiceChanged()
    {
      if (null != _cachedService)
      {
        _cachedService.MessageAdded -= EhMessageAdded;
      }

      _cachedService = Current.GetService<IInfoWarningErrorTextMessageService>();

      if (null != _cachedService)
      {
        _cachedService.MessageAdded += EhMessageAdded;
      }
    }

    private void EhMessageAdded(InfoWarningErrorTextMessageItem msgItem)
    {
      _unreversedDoc.Add(msgItem);
    }

    protected void Initialize(bool initData)
    {
      if (initData)
      {
      }

      if (_view != null)
      {
        if (_columnWidths != null)
        {
          _view.ColumnWidths = _columnWidths;
          _columnWidths = null;
        }
      }
    }

    private void EhClearAllMessages()
    {
      _unreversedDoc.Clear();
    }

    private void EhReverseAllMessages()
    {
      if (object.ReferenceEquals(_currentDoc, _unreversedDoc))
        MessageItems = _reversedDoc;
      else
        MessageItems = _unreversedDoc;
    }

    private void EhDirectionChanged(object para)
    {
      if (para is bool)
        EhDirectionChanged((bool)para);
    }

    private void EhDirectionChanged(bool directionRecentIsFirst)
    {
      var oldCurrentDoc = _currentDoc;

      _viewDirectionRecentIsFirst = directionRecentIsFirst;

      if (_viewDirectionRecentIsFirst)
        _currentDoc = _reversedDoc;
      else
        _currentDoc = _unreversedDoc;

      if (!object.ReferenceEquals(oldCurrentDoc, _currentDoc))
        OnPropertyChanged(nameof(MessageItems));
    }

    protected void AttachView()
    {
      _view.DataContext = this;
    }

    protected void DetachView()
    {
      _view.DataContext = null;
    }

    public override object ViewObject
    {
      get { return _view; }
      set
      {
        if (null != _view)
          DetachView();

        _view = value as IInfoWarningErrorMessageView;

        if (null != _view)
        {
          Initialize(false);
          AttachView();
        }
      }
    }

    public override object ModelObject { get { return _unreversedDoc; } }

    #region IXmlSerializable Members

    public void Load(System.Xml.XmlTextReader tr, string localName)
    {
      int count;
      tr.ReadStartElement(localName);

      this._viewDirectionRecentIsFirst = tr.ReadElementContentAsBoolean("DirectionRecentFirst", string.Empty);

      count = XmlConvert.ToInt32(tr.GetAttribute("Count"));
      tr.ReadStartElement("ColumnWidths");
      _columnWidths = new int[count];
      for (int i = 0; i < count; i++)
        _columnWidths[i] = tr.ReadElementContentAsInt("Width", string.Empty);
      if (count > 0)
        tr.ReadEndElement(); // ColumnWidths
      if (null != _view)
      {
        _view.ColumnWidths = _columnWidths;
        _columnWidths = null;
      }

      tr.ReadEndElement();
    }

    public void Save(System.Xml.XmlTextWriter tw, string localName)
    {
      tw.WriteStartElement(localName);
      tw.WriteAttributeString("Version", "1");

      tw.WriteElementString("DirectionRecentFirst", System.Xml.XmlConvert.ToString(_viewDirectionRecentIsFirst));

      tw.WriteStartElement("ColumnWidths");
      int[] colWidths = null != _view ? _view.ColumnWidths : new int[0];
      tw.WriteAttributeString("Count", XmlConvert.ToString(colWidths.Length));
      for (int i = 0; i < colWidths.Length; i++)
        tw.WriteElementString("Width", XmlConvert.ToString(colWidths[i]));
      tw.WriteEndElement(); // "ColumnWidths"

      tw.WriteEndElement(); // localName
    }

    public void ShowException(Exception ex, string message = null)
    {
      throw new NotImplementedException();
    }

    public void ShowHandledException(Exception ex, string message = null)
    {
      throw new NotImplementedException();
    }

    public void ShowError(string message)
    {
      throw new NotImplementedException();
    }

    public void ShowErrorFormatted(string formatstring, params object[] formatitems)
    {
      throw new NotImplementedException();
    }

    public void ShowWarning(string message)
    {
      throw new NotImplementedException();
    }

    public void ShowWarningFormatted(string formatstring, params object[] formatitems)
    {
      throw new NotImplementedException();
    }

    public void ShowMessage(string message, string caption = null)
    {
      throw new NotImplementedException();
    }

    public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
    {
      throw new NotImplementedException();
    }

    public bool AskQuestion(string question, string caption = null)
    {
      throw new NotImplementedException();
    }

    public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
    {
      throw new NotImplementedException();
    }

    public string ShowInputBox(string caption, string dialogText, string defaultValue)
    {
      throw new NotImplementedException();
    }

    public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
    {
      throw new NotImplementedException();
    }

    public ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
    {
      throw new NotImplementedException();
    }

    public void WriteErrorLine(string source, string message)
    {
      throw new NotImplementedException();
    }

    public void WriteErrorLine(string source, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    #endregion IXmlSerializable Members
  }
}
