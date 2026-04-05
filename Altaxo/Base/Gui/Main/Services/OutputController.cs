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
using System.Linq;
using System.Text;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Main.Services
{
  /// <summary>
  /// View interface for the output window.
  /// </summary>
  public interface IOutputView
  {
    /// <summary>
    /// Sets the displayed text.
    /// </summary>
    /// <param name="text">The text.</param>
    void SetText(string text);

    /// <summary>
    /// Gets or sets a value indicating whether output logging is enabled.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Occurs when the enabled state changes.
    /// </summary>
    event Action EnabledChanged;
  }

  /// <summary>
  /// Controller for the output pad.
  /// </summary>
  [ExpectedTypeOfView(typeof(IOutputView))]
  public class OutputController : IMVCController, ITextOutputService
  {
    private IOutputView? _view;
    private bool _isLoggingEnabled = false; // always start with false, only the user can enable this service
    private StringBuilder _logText = new StringBuilder();

    /// <summary>
    /// Writes text internally if logging is enabled.
    /// </summary>
    /// <param name="text">The text.</param>
    public void InternalWrite(string text)
    {
      if (_isLoggingEnabled)
      {
        _logText.Append(text);

        if (_view is not null)
          _view.SetText(_logText.ToString());
      }
    }

    /// <inheritdoc/>
    public void Write(string text)
    {
      InternalWrite(text);
    }

    /// <inheritdoc/>
    public void WriteLine()
    {
      InternalWrite(System.Environment.NewLine);
    }

    /// <inheritdoc/>
    public void WriteLine(string text)
    {
      InternalWrite(text + System.Environment.NewLine);
    }

    /// <inheritdoc/>
    public void WriteLine(string format, params object[] args)
    {
      InternalWrite(string.Format(format, args) + System.Environment.NewLine);
    }

    /// <inheritdoc/>
    public void WriteLine(System.IFormatProvider provider, string format, params object[] args)
    {
      InternalWrite(string.Format(provider, format, args) + System.Environment.NewLine);
    }

    /// <inheritdoc/>
    public void Write(string format, params object[] args)
    {
      InternalWrite(string.Format(format, args));
    }

    /// <inheritdoc/>
    public void Write(System.IFormatProvider provider, string format, params object[] args)
    {
      InternalWrite(string.Format(provider, format, args));
    }

    private void Initialize(bool initData)
    {
      if (_view is not null)
      {
        _view.SetText(_logText.ToString());
      }
    }

    private void EhEnabledChanged()
    {
      if (_view is { } view)
        _isLoggingEnabled = view.IsEnabled;
    }

    /// <inheritdoc/>
    public object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          _view.EnabledChanged -= EhEnabledChanged;
        }

        _view = (IOutputView?)value;

        if (_view is not null)
        {
          Initialize(false);
          _view.EnabledChanged += EhEnabledChanged;
        }
      }
    }

    #region IMVCController Members

    /// <inheritdoc/>
    public object ModelObject
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion IMVCController Members

    #region IXmlSerializable Members

    /// <summary>
    /// Loads the controller state from XML.
    /// </summary>
    /// <param name="tr">The XML reader.</param>
    /// <param name="localName">The local element name.</param>
    public void Load(System.Xml.XmlTextReader tr, string localName)
    {
      tr.ReadStartElement(localName);

      tr.ReadElementContentAsBoolean("IsEnabled", string.Empty);

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

      tw.WriteElementString("IsEnabled", System.Xml.XmlConvert.ToString(_isLoggingEnabled));

      tw.WriteEndElement(); // localName
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      throw new NotImplementedException();
    }

    #endregion IXmlSerializable Members
  }
}
