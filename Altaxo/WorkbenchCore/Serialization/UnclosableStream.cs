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

using System;
using System.IO;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Wraps another stream. Closing this stream does not close the base stream.
  /// </summary>
  public class UnclosableStream : Stream
  {
    private Stream baseStream;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnclosableStream"/> class.
    /// </summary>
    /// <param name="baseStream">The wrapped base stream.</param>
    public UnclosableStream(Stream baseStream)
    {
      if (baseStream is null)
        throw new ArgumentNullException("baseStream");
      this.baseStream = baseStream;
    }

    /// <inheritdoc/>
    public override bool CanRead
    {
      get { return baseStream.CanRead; }
    }

    /// <inheritdoc/>
    public override bool CanSeek
    {
      get { return baseStream.CanSeek; }
    }

    /// <inheritdoc/>
    public override bool CanWrite
    {
      get { return baseStream.CanWrite; }
    }

    /// <inheritdoc/>
    public override long Length
    {
      get { return baseStream.Length; }
    }

    /// <inheritdoc/>
    public override long Position
    {
      get { return baseStream.Position; }
      set { baseStream.Position = value; }
    }

    /// <inheritdoc/>
    public override void Flush()
    {
      baseStream.Flush();
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
      return baseStream.Seek(offset, origin);
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
      baseStream.SetLength(value);
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
      return baseStream.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
      baseStream.Write(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
      return baseStream.BeginRead(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
      return baseStream.BeginWrite(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    public override bool CanTimeout
    {
      get { return baseStream.CanTimeout; }
    }

    /// <inheritdoc/>
    public override int EndRead(IAsyncResult asyncResult)
    {
      return baseStream.EndRead(asyncResult);
    }

    /// <inheritdoc/>
    public override void EndWrite(IAsyncResult asyncResult)
    {
      baseStream.EndWrite(asyncResult);
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
      return baseStream.ReadByte();
    }

    /// <inheritdoc/>
    public override int ReadTimeout
    {
      get { return baseStream.ReadTimeout; }
      set { baseStream.ReadTimeout = value; }
    }

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
      baseStream.WriteByte(value);
    }

    /// <inheritdoc/>
    public override int WriteTimeout
    {
      get { return baseStream.WriteTimeout; }
      set { baseStream.WriteTimeout = value; }
    }
  }
}
