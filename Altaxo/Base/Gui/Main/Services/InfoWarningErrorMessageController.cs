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

#nullable enable
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
  /// <summary>
  /// View interface for displaying informational, warning, and error messages.
  /// </summary>
  public interface IInfoWarningErrorMessageView
  {
    /// <summary>
    /// Sets the data context for the view.
    /// </summary>
    /// <value>
    /// The data context.
    /// </value>
    object? DataContext { set; }

    /// <summary>
    /// Gets or sets the column widths.
    /// </summary>
    double[] ColumnWidths { get; set; }
  }

  /// <summary>
  /// Controller for the message pad that displays informational, warning, and error messages.
  /// </summary>
  [ExpectedTypeOfView(typeof(IInfoWarningErrorMessageView))]
  public class InfoWarningErrorMessageController : AbstractPadContent, IMementoCapable
  {
    private bool _viewDirectionRecentIsFirst = true;
    private double[]? _columnWidths;
    private IInfoWarningErrorMessageView? _view;
    private ObservableCollection<InfoWarningErrorTextMessageItem> _unreversedDoc;
    private Altaxo.Collections.ObservableCollectionReversingWrapper<InfoWarningErrorTextMessageItem> _reversedDoc;
    private IReadOnlyList<InfoWarningErrorTextMessageItem> _currentDoc;

    private IInfoWarningErrorTextMessageService? _cachedService;

    private ICommand _commandClearAllMessages;
    private ICommand _commandReverseMessages;

    private CachedService<IShutdownService, IShutdownService> _shutDownService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoWarningErrorMessageController"/> class.
    /// </summary>
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

      var memento = Current.PropertyService.GetValue(PropertyKeyMessageControlState, RuntimePropertyKind.UserAndApplicationAndBuiltin, null);
      if (memento is not null)
        SetMemento(memento);


      EhServiceChanged();
    }


    #region Bindable properties for view

    /// <summary>
    /// Gets the message items currently displayed.
    /// </summary>
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

    /// <summary>
    /// Gets the command that clears all messages.
    /// </summary>
    public ICommand CommandClearAllMessages { get { return _commandClearAllMessages; } }

    /// <summary>
    /// Gets the command that reverses the message order.
    /// </summary>
    public ICommand CommandReverseMessageOrder { get { return _commandReverseMessages; } }

    #endregion Bindable properties for view


    /// <inheritdoc/>
    public override void Dispose()
    {
      Current.ServiceChanged -= EhServiceChanged;

      if (_cachedService is not null)
      {
        _cachedService.MessageAdded -= EhMessageAdded;
        _cachedService = null;
      }

      base.Dispose();
    }

    private void EhServiceChanged()
    {
      if (_cachedService is not null)
      {
        _cachedService.MessageAdded -= EhMessageAdded;
      }

      _cachedService = Current.GetService<IInfoWarningErrorTextMessageService>();

      if (_cachedService is not null)
      {
        _cachedService.MessageAdded += EhMessageAdded;
      }
    }

    private void EhMessageAdded(InfoWarningErrorTextMessageItem msgItem)
    {
      if (Current.Dispatcher.InvokeRequired)
        Current.Dispatcher.InvokeAndForget(() => _unreversedDoc.Add(msgItem));
      else
      _unreversedDoc.Add(msgItem);
    }

    /// <summary>
    /// Initializes the controller state and transfers cached view settings to the attached view.
    /// </summary>
    /// <param name="initData">If set to <c>true</c>, the controller initializes its data state.</param>
    protected void Initialize(bool initData)
    {
      if (initData)
      {
      }

      if (_view is not null)
      {
        if (_columnWidths is not null)
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

    /// <summary>
    /// Attaches the current view to this controller.
    /// </summary>
    protected void AttachView()
    {
      _view!.DataContext = this;
    }

    /// <summary>
    /// Detaches the current view from this controller.
    /// </summary>
    protected void DetachView()
    {
      _view!.DataContext = null;
    }

    /// <inheritdoc/>
    public override object? ViewObject
    {
      get { return _view; }
      set
      {
        if (_view is not null)
          DetachView();

        _view = value as IInfoWarningErrorMessageView;

        if (_view is not null)
        {
          Initialize(false);
          AttachView();
        }
      }
    }

    /// <inheritdoc/>
    public override object ModelObject { get { return _unreversedDoc; } }

    #region IXmlSerializable Members

    /// <summary>
    /// Loads the controller state from XML.
    /// </summary>
    /// <param name="tr">The XML reader.</param>
    /// <param name="localName">The local element name.</param>
    public void Load(System.Xml.XmlTextReader tr, string localName)
    {
      int count;
      tr.ReadStartElement(localName);

      _viewDirectionRecentIsFirst = tr.ReadElementContentAsBoolean("DirectionRecentFirst", string.Empty);

      count = XmlConvert.ToInt32(tr.GetAttribute("Count") ?? throw new InvalidOperationException($"Attribute 'Count' is mandatory here!"));
      tr.ReadStartElement("ColumnWidths");
      _columnWidths = new double[count];
      for (int i = 0; i < count; i++)
        _columnWidths[i] = tr.ReadElementContentAsInt("Width", string.Empty);
      if (count > 0)
        tr.ReadEndElement(); // ColumnWidths
      if (_view is not null)
      {
        _view.ColumnWidths = _columnWidths;
        _columnWidths = null;
      }

      tr.ReadEndElement();
    }

    /// <summary>
    /// Saves the controller state to XML.
    /// </summary>
    /// <param name="tw">The XML writer.</param>
    /// <param name="localName">The local element name.</param>
    public void Save(System.Xml.XmlTextWriter tw, string localName)
    {
      tw.WriteStartElement(localName);
      tw.WriteAttributeString("Version", "1");

      tw.WriteElementString("DirectionRecentFirst", System.Xml.XmlConvert.ToString(_viewDirectionRecentIsFirst));

      tw.WriteStartElement("ColumnWidths");
      var colWidths = _view is not null ? _view.ColumnWidths : new double[0];
      tw.WriteAttributeString("Count", XmlConvert.ToString(colWidths.Length));
      for (int i = 0; i < colWidths.Length; i++)
        tw.WriteElementString("Width", XmlConvert.ToString(colWidths[i]));
      tw.WriteEndElement(); // "ColumnWidths"

      tw.WriteEndElement(); // localName
    }

    /// <summary>
    /// Shows an exception message.
    /// </summary>
    /// <param name="ex">The exception to show.</param>
    /// <param name="message">An optional custom message.</param>
    public void ShowException(Exception ex, string? message = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows a handled exception message.
    /// </summary>
    /// <param name="ex">The handled exception.</param>
    /// <param name="message">An optional custom message.</param>
    public void ShowHandledException(Exception ex, string? message = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    public void ShowError(string message)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows a formatted error message.
    /// </summary>
    /// <param name="formatstring">The composite format string.</param>
    /// <param name="formatitems">The format arguments.</param>
    public void ShowErrorFormatted(string formatstring, params object[] formatitems)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    /// <param name="message">The warning message.</param>
    public void ShowWarning(string message)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows a formatted warning message.
    /// </summary>
    /// <param name="formatstring">The composite format string.</param>
    /// <param name="formatitems">The format arguments.</param>
    public void ShowWarningFormatted(string formatstring, params object[] formatitems)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows an informational message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="caption">An optional caption.</param>
    public void ShowMessage(string message, string? caption = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows a formatted informational message.
    /// </summary>
    /// <param name="formatstring">The composite format string.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="formatitems">The format arguments.</param>
    public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Asks the user a question.
    /// </summary>
    /// <param name="question">The question text.</param>
    /// <param name="caption">An optional caption.</param>
    /// <returns><c>true</c> if the user answered yes; otherwise, <c>false</c>.</returns>
    public bool AskQuestion(string question, string? caption = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows a custom dialog with the specified buttons.
    /// </summary>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="dialogText">The dialog text.</param>
    /// <param name="acceptButtonIndex">The index of the accept button.</param>
    /// <param name="cancelButtonIndex">The index of the cancel button.</param>
    /// <param name="buttontexts">The button captions.</param>
    /// <returns>The index of the pressed button.</returns>
    public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows an input box.
    /// </summary>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="dialogText">The prompt text.</param>
    /// <param name="defaultValue">The default input value.</param>
    /// <returns>The entered value.</returns>
    public string ShowInputBox(string caption, string dialogText, string defaultValue)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Informs the user about a save error.
    /// </summary>
    /// <param name="fileName">The file that could not be saved.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="dialogName">The dialog name.</param>
    /// <param name="exceptionGot">The exception that occurred.</param>
    public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Lets the user choose how to handle a save error.
    /// </summary>
    /// <param name="fileName">The file that could not be saved.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="dialogName">The dialog name.</param>
    /// <param name="exceptionGot">The exception that occurred.</param>
    /// <param name="chooseLocationEnabled">If set to <c>true</c>, choosing another location is allowed.</param>
    /// <returns>The selected save-error handling option.</returns>
    public ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Writes an error line.
    /// </summary>
    /// <param name="source">The source of the error.</param>
    /// <param name="message">The error message.</param>
    public void WriteErrorLine(string source, string message)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Writes a formatted error line.
    /// </summary>
    /// <param name="source">The source of the error.</param>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    public void WriteErrorLine(string source, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    #endregion IXmlSerializable Members


    #region Memento

    /// <inheritdoc/>
    public override object? GetService(Type serviceType)
    {
      // TODO make MementoService availabe if the user chooses to store the state of the online window with the document file
      // and make it unavailable if not ( but then store the state in the properties of the application)
      // like this:
      // if(!wantToStoreMemento) return null; else return base.GetService(serviceType);
      return base.GetService(serviceType);
    }

    private void EhApplicationClosed(object? sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyMessageControlState, (StateMemento)CreateMemento());
    }

    private static readonly PropertyKey<StateMemento> PropertyKeyMessageControlState = new PropertyKey<StateMemento>("1E2D00F9-5A27-4C2C-84C1-842AAC0F5343", "Messages\\ControlState", PropertyLevel.Application);


    /// <inheritdoc/>
    public object CreateMemento()
    {
      if (_view is not null)
        _columnWidths = _view.ColumnWidths;
      return new StateMemento(_columnWidths ?? new double[0]);
    }

    /// <inheritdoc/>
    public void SetMemento(object memento)
    {
      if (memento is StateMemento m)
        _columnWidths = m.ColumnWidths;
      if (_view is not null && _columnWidths is { } cw && cw.Length > 0)
        _view.ColumnWidths = cw;
    }



    /// <summary>
    /// Stores the state of the message pad.
    /// </summary>
    private class StateMemento
    {
      /// <summary>
      /// Gets the column widths.
      /// </summary>
      public double[] ColumnWidths { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="StateMemento"/> class.
      /// </summary>
      /// <param name="columnWidths">The column widths.</param>
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
        /// <inheritdoc/>
        public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (StateMemento)o;

          info.CreateArray("ColumnWidths", s.ColumnWidths.Length);

          foreach (var w in s.ColumnWidths)
            info.AddValue("e", w);

          info.CommitArray();
        }

        /// <inheritdoc/>
        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (StateMemento?)o ?? new StateMemento(new double[0]);

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
