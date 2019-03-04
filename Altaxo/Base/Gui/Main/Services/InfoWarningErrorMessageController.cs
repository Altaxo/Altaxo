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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;
using Altaxo.Gui.Workbench;
using Altaxo.Main.Properties;
using Altaxo.Main.Services;

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

    double[] ColumnWidths { get; set; }
  }

  [ExpectedTypeOfView(typeof(IInfoWarningErrorMessageView))]
  public class InfoWarningErrorMessageController : AbstractPadContent, IMementoCapable
  {
    private bool _viewDirectionRecentIsFirst = true;
    private double[] _columnWidths;
    private IInfoWarningErrorMessageView _view;
    private ObservableCollection<InfoWarningErrorTextMessageItem> _unreversedDoc;
    private Altaxo.Collections.ObservableCollectionReversingWrapper<InfoWarningErrorTextMessageItem> _reversedDoc;
    private IReadOnlyList<InfoWarningErrorTextMessageItem> _currentDoc;

    private IInfoWarningErrorTextMessageService _cachedService;

    private ICommand _commandClearAllMessages;
    private ICommand _commandReverseMessages;

    private CachedService<IShutdownService, IShutdownService> _shutDownService;

    public InfoWarningErrorMessageController()
    {
      Current.ServiceChanged += EhServiceChanged;
      _unreversedDoc = new ObservableCollection<InfoWarningErrorTextMessageItem>();
      _reversedDoc = new Collections.ObservableCollectionReversingWrapper<InfoWarningErrorTextMessageItem>(_unreversedDoc);
      _currentDoc = _unreversedDoc;

      _commandClearAllMessages = new RelayCommand(EhClearAllMessages);
      _commandReverseMessages = new RelayCommand(EhReverseAllMessages);

      _shutDownService = new CachedService<IShutdownService, IShutdownService>(false,
        (shutdownService) => shutdownService.Closed += EhApplicationClosed,
        (shutdownService) => shutdownService.Closed -= EhApplicationClosed);
      _shutDownService.StartCaching();

      var memento = Current.PropertyService.GetValue(PropertyKeyMessageControlState, RuntimePropertyKind.UserAndApplicationAndBuiltin, () => null);
      if (null != memento)
        SetMemento(memento);


      EhServiceChanged();
    }


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

      _viewDirectionRecentIsFirst = tr.ReadElementContentAsBoolean("DirectionRecentFirst", string.Empty);

      count = XmlConvert.ToInt32(tr.GetAttribute("Count"));
      tr.ReadStartElement("ColumnWidths");
      _columnWidths = new double[count];
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
      var colWidths = null != _view ? _view.ColumnWidths : new double[0];
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


    #region Memento

    public override object GetService(Type serviceType)
    {
      // TODO make MementoService availabe if the user chooses to store the state of the online window with the document file
      // and make it unavailable if not ( but then store the state in the properties of the application)
      // like this:
      // if(!wantToStoreMemento) return null; else return base.GetService(serviceType);
      return base.GetService(serviceType);
    }

    private void EhApplicationClosed(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyMessageControlState, (StateMemento)CreateMemento());
    }

    private static readonly PropertyKey<StateMemento> PropertyKeyMessageControlState = new PropertyKey<StateMemento>("1E2D00F9-5A27-4C2C-84C1-842AAC0F5343", "Messages\\ControlState", PropertyLevel.Application);


    public object CreateMemento()
    {
      if (null != _view)
        _columnWidths = _view.ColumnWidths;
      return new StateMemento(_columnWidths);
    }

    public void SetMemento(object memento)
    {
      if (memento is StateMemento m)
        _columnWidths = m.ColumnWidths;
      if (null != _view)
        _view.ColumnWidths = _columnWidths;
    }



    private class StateMemento
    {
      public double[] ColumnWidths { get; private set; }
      public StateMemento(double[] columnWidths)
      {
        ColumnWidths = columnWidths;
      }

      /// <summary>
      /// 2017-09-21 Version 0
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StateMemento), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (StateMemento)obj;

          info.CreateArray("ColumnWidths", s.ColumnWidths.Length);

          foreach (var w in s.ColumnWidths)
            info.AddValue("e", w);

          info.CommitArray();
        }

        public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {
          var s = (StateMemento)o ?? new StateMemento(new double[0]);

          var count = info.OpenArray("ColumnWidths");
          s.ColumnWidths = new double[count];

          for (int i = 0; i < count; ++i)
            s.ColumnWidths[i] = info.GetDouble("e");

          info.CloseArray(count);

          return s;
        }
      }
    }

    #endregion

  }
}
