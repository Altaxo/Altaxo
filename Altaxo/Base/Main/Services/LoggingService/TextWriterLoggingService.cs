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
using System.IO;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Logging service implementation that writes log output to a <see cref="TextWriter"/>.
  /// </summary>
  public class TextWriterLoggingService : ILoggingService
  {
    private readonly TextWriter writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextWriterLoggingService"/> class.
    /// </summary>
    /// <param name="writer">The writer that receives the log output.</param>
    public TextWriterLoggingService(TextWriter writer)
    {
      if (writer is null)
        throw new ArgumentNullException("writer");
      this.writer = writer;
      IsFatalEnabled = true;
      IsErrorEnabled = true;
      IsWarnEnabled = true;
      IsInfoEnabled = true;
      IsDebugEnabled = true;
    }

    private void Write(object message, Exception? exception)
    {
      if (message is not null)
      {
        writer.WriteLine(message.ToString());
      }
      if (exception is not null)
      {
        writer.WriteLine(exception.ToString());
      }
    }

    /// <inheritdoc/>
    public bool IsDebugEnabled { get; set; }
    /// <inheritdoc/>
    public bool IsInfoEnabled { get; set; }
    /// <inheritdoc/>
    public bool IsWarnEnabled { get; set; }
    /// <inheritdoc/>
    public bool IsErrorEnabled { get; set; }
    /// <inheritdoc/>
    public bool IsFatalEnabled { get; set; }

    /// <inheritdoc/>
    public void Debug(object message)
    {
      if (IsDebugEnabled)
      {
        Write(message, null);
      }
    }

    /// <inheritdoc/>
    public void DebugFormatted(string format, params object[] args)
    {
      Debug(string.Format(format, args));
    }

    /// <inheritdoc/>
    public void Info(object message)
    {
      if (IsInfoEnabled)
      {
        Write(message, null);
      }
    }

    /// <inheritdoc/>
    public void InfoFormatted(string format, params object[] args)
    {
      Info(string.Format(format, args));
    }

    /// <inheritdoc/>
    public void Warn(object message)
    {
      Warn(message, null);
    }

    /// <inheritdoc/>
    public void Warn(object message, Exception? exception)
    {
      if (IsWarnEnabled)
      {
        Write(message, exception);
      }
    }

    /// <inheritdoc/>
    public void WarnFormatted(string format, params object[] args)
    {
      Warn(string.Format(format, args));
    }

    /// <inheritdoc/>
    public void Error(object message)
    {
      Error(message, null);
    }

    /// <inheritdoc/>
    public void Error(object message, Exception? exception)
    {
      if (IsErrorEnabled)
      {
        Write(message, exception);
      }
    }

    /// <inheritdoc/>
    public void ErrorFormatted(string format, params object[] args)
    {
      Error(string.Format(format, args));
    }

    /// <inheritdoc/>
    public void Fatal(object message)
    {
      Fatal(message, null);
    }

    /// <inheritdoc/>
    public void Fatal(object message, Exception? exception)
    {
      if (IsFatalEnabled)
      {
        Write(message, exception);
      }
    }

    /// <inheritdoc/>
    public void FatalFormatted(string format, params object[] args)
    {
      Fatal(string.Format(format, args));
    }
  }
}
