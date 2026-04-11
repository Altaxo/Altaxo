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
using Altaxo.Main.Services.Implementation;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Interface to a logging service which typically logs some events to a log file.
  /// </summary>
  [GlobalService("Log", FallbackImplementation = typeof(FallbackLoggingService))]
  public interface ILoggingService
  {
    /// <summary>
    /// Writes a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(object message);

    /// <summary>
    /// Writes a formatted debug message.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void DebugFormatted(string format, params object[] args);

    /// <summary>
    /// Writes an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Info(object message);

    /// <summary>
    /// Writes a formatted informational message.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void InfoFormatted(string format, params object[] args);

    /// <summary>
    /// Writes a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Warn(object message);

    /// <summary>
    /// Writes a warning message together with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The related exception.</param>
    void Warn(object message, Exception exception);

    /// <summary>
    /// Writes a formatted warning message.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void WarnFormatted(string format, params object[] args);

    /// <summary>
    /// Writes an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Error(object message);

    /// <summary>
    /// Writes an error message together with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The related exception.</param>
    void Error(object message, Exception exception);

    /// <summary>
    /// Writes a formatted error message.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void ErrorFormatted(string format, params object[] args);

    /// <summary>
    /// Writes a fatal message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Fatal(object message);

    /// <summary>
    /// Writes a fatal message together with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The related exception.</param>
    void Fatal(object message, Exception? exception);

    /// <summary>
    /// Writes a formatted fatal message.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void FatalFormatted(string format, params object[] args);

    /// <summary>
    /// Gets a value indicating whether debug logging is enabled.
    /// </summary>
    bool IsDebugEnabled { get; }
    /// <summary>
    /// Gets a value indicating whether informational logging is enabled.
    /// </summary>
    bool IsInfoEnabled { get; }
    /// <summary>
    /// Gets a value indicating whether warning logging is enabled.
    /// </summary>
    bool IsWarnEnabled { get; }
    /// <summary>
    /// Gets a value indicating whether error logging is enabled.
    /// </summary>
    bool IsErrorEnabled { get; }
    /// <summary>
    /// Gets a value indicating whether fatal logging is enabled.
    /// </summary>
    bool IsFatalEnabled { get; }
  }

  /// <summary>
  /// Fallback logging service that writes trace output when no other logging service is available.
  /// </summary>
  internal sealed class FallbackLoggingService : TextWriterLoggingService
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FallbackLoggingService"/> class.
    /// </summary>
    public FallbackLoggingService() : base(new TraceTextWriter())
    {
    }
  }
}
