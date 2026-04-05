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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Altaxo.Main.Services.Implementation
{
  /// <summary>
  /// <see cref="TextWriter"/> implementation that writes to <see cref="System.Diagnostics.Trace"/>.
  /// </summary>
  public class TraceTextWriter : TextWriter
  {
    /// <inheritdoc/>
    public override Encoding Encoding
    {
      get
      {
        return Encoding.Unicode;
      }
    }

    /// <inheritdoc/>
    public override void Write(char value)
    {
      Trace.Write(value.ToString());
    }

    /// <inheritdoc/>
    public override void Write(char[] buffer, int index, int count)
    {
      Trace.Write(new string(buffer, index, count));
    }

    /// <inheritdoc/>
    public override void Write(string? value)
    {
      Trace.Write(value);
    }

    /// <inheritdoc/>
    public override void WriteLine()
    {
      Trace.WriteLine(string.Empty);
    }

    /// <inheritdoc/>
    public override void WriteLine(string? value)
    {
      Trace.WriteLine(value);
    }
  }
}
