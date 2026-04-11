// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Collections.ObjectModel;
using Altaxo.Main.Services.Implementation;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Represents a timestamped informational, warning, or error message.
  /// </summary>
  public struct InfoWarningErrorTextMessageItem
  {
    /// <summary>
    /// Gets or sets the message level.
    /// </summary>
    public MessageLevel Level { get; set; }
    /// <summary>
    /// Gets or sets the message source.
    /// </summary>
    public string Source { get; set; }
    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Gets or sets the message timestamp in UTC.
    /// </summary>
    public DateTime TimeUtc { get; set; }

    /// <summary>
    /// Gets the local time corresponding to <see cref="TimeUtc"/>.
    /// </summary>
    public DateTime Time
    {
      get { return TimeUtc.ToLocalTime(); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoWarningErrorTextMessageItem"/> struct.
    /// </summary>
    /// <param name="level">The message severity level.</param>
    /// <param name="source">The message source.</param>
    /// <param name="message">The message text.</param>
    /// <param name="timeUtc">The message timestamp in UTC.</param>
    public InfoWarningErrorTextMessageItem(MessageLevel level, string source, string message, DateTime timeUtc)
    {
      Level = level;
      Source = source;
      Message = message;
      TimeUtc = timeUtc;
    }
  }

  /// <summary>
  /// Service that usually display infos, warnings, and errors in a pad window at the bottom of the workbench. Typically,
  /// the messages will be displayed in different colors according to their warning level.
  /// </summary>
  [GlobalService("InfoWarningErrorTextMessageService")]
  public interface IInfoWarningErrorTextMessageService
  {
    /// <summary>
    /// Writes a message line.
    /// </summary>
    /// <param name="messageLevel">The message severity.</param>
    /// <param name="source">The message source.</param>
    /// <param name="message">The message text.</param>
    void WriteLine(MessageLevel messageLevel, string source, string message);

    /// <summary>
    /// Writes a formatted message line.
    /// </summary>
    /// <param name="messageLevel">The message severity.</param>
    /// <param name="source">The message source.</param>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void WriteLine(MessageLevel messageLevel, string source, string format, params object[] args);

    /// <summary>
    /// Writes a culture-aware formatted message line.
    /// </summary>
    /// <param name="messageLevel">The message severity.</param>
    /// <param name="source">The message source.</param>
    /// <param name="provider">The format provider.</param>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void WriteLine(MessageLevel messageLevel, string source, System.IFormatProvider provider, string format, params object[] args);

    /// <summary>
    /// Occurs when a message is added.
    /// </summary>
    event Action<InfoWarningErrorTextMessageItem> MessageAdded;
  }
}
