﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  /// <summary>
  /// Represents a storage for text that can be used like a text console.
  /// </summary>
  public interface ITextBackedConsole
    :
    ICloneable,
    System.ComponentModel.INotifyPropertyChanged // notify if the Text property changed
  {
    /// <summary>Writes the specified string value to the text backed console.</summary>
    /// <param name="value">The string to write. </param>
    void Write(string value);

    /// <summary>Writes the text representation of the specified array of objects to the text backed console using the specified format information.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using <paramref name="format" />. </param>
    void Write(string format, params object[] args);

    /// <summary>Writes the current line terminator to the text backed console.</summary>
    void WriteLine();

    /// <summary>Writes the specified string value, followed by the current line terminator, to the text backed console.</summary>
    /// <param name="value">The value to write. </param>
    void WriteLine(string value);

    /// <summary>Writes the text representation of the specified array of objects, followed by the current line terminator, to the text backed console using the specified format information.</summary>
    /// <param name="format">A composite format string. </param>
    /// <param name="args">An array of objects to write using <paramref name="format" />. </param>
    void WriteLine(string format, params object[] args);

    /// <summary>Removes all characters from the current text backed console.</summary>
    void Clear();

    /// <summary>Gets or sets the entire text of the text backed console. If setting the text, and if the text is different from the text that is currently stored in the instance, a property changed event (see <see cref="System.ComponentModel.PropertyChangedEventHandler"/>) is fired with 'Text' as parameter.</summary>
    /// <value>The text of this console.</value>
    string Text { get; set; }
  }

  /// <summary>
  /// Implementation of <see cref="ITextBackedConsole"/>, where the text is stored in a <see cref="System.Text.StringBuilder"/> instance.
  /// </summary>
  public class TextBackedConsole
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ITextBackedConsole
  {
    private object _synchronizingObject;
    private StringBuilder _stb;

    /// <summary>Support for binding to a view. At least this event must be called when the Text property has changed.</summary>
    [field: NonSerialized]
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBackedConsole"/> class with empty text.
    /// </summary>
    public TextBackedConsole()
    {
      _synchronizingObject = new object();
      _stb = new StringBuilder();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBackedConsole"/> class. The text is copied from the provided text backed console.
    /// </summary>
    /// <param name="from">The instance to copy the text from.</param>
    public TextBackedConsole(TextBackedConsole from)
    {
      _synchronizingObject = new object();
      _stb = new StringBuilder(from._stb.ToString());
    }

    object ICloneable.Clone()
    {
      return new TextBackedConsole(this);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public TextBackedConsole Clone()
    {
      return new TextBackedConsole(this);
    }

    public void Write(string value)
    {
      _stb.Append(value);
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Writes the text representation of the specified array of objects to the text backed console using the specified format information.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
    public void Write(string format, params object?[] args)
    {
      _stb.AppendFormat(format, args);
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Writes the current line terminator to the text backed console.
    /// </summary>
    public void WriteLine()
    {
      _stb.AppendLine();
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the text backed console.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteLine(string value)
    {
      _stb.AppendLine(value);
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Writes the text representation of the specified array of objects, followed by the current line terminator, to the text backed console using the specified format information.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
    public void WriteLine(string format, params object?[] args)
    {
      _stb.AppendFormat(format, args);
      _stb.AppendLine();
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Removes all characters from the current text backed console.
    /// </summary>
    public void Clear()
    {
      Text = string.Empty;
    }

    /// <summary>
    /// Gets or sets the entire text of the text backed console. If setting the text, and if the text is different from the text that is currently stored in the instance, a property changed event (see <see cref="System.ComponentModel.PropertyChangedEventHandler"/>) is fired with 'Text' as parameter.
    /// </summary>
    /// <value>
    /// The text of this console.
    /// </value>
    public string Text
    {
      get
      {
        return _stb.ToString();
      }
      set
      {
        value ??= string.Empty;

        var isDifferent = _stb.Length != value.Length || 0 != string.CompareOrdinal(value, _stb.ToString());
        if (isDifferent)
        {
          lock (_synchronizingObject)
          {
            _stb.Clear();
            _stb.Append(value);
          }
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    protected override void OnChanged(EventArgs e)
    {
      PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Text)));
      base.OnChanged(e);
    }
  }
}
