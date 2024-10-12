/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
/////////////////////////////////////////////////////////////////////////////

using System.IO;

namespace Altaxo.Serialization
{
  public static class FileIOExtensions
  {
    /// <summary>
    /// Reads the data into a buffer. Ensures that the provided number of bytes is really read. If not, a <see cref="EndOfStreamException"/> is thrown.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The offset where the first read byte should be stored into the buffer.</param>
    /// <param name="length">The number of bytes that must be read.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">length</exception>
    /// <exception cref="System.IO.IOException">Could not read any data from the stream</exception>
    public static void ForcedRead(this Stream stream, byte[] buffer, int offset, int length)
    {
      if (length < 0)
      {
        throw new System.ArgumentOutOfRangeException(nameof(length));
      }
      while (length > 0)
      {
        var read = stream.Read(buffer, offset, length);
        if (read == 0)
        {
          throw new System.IO.EndOfStreamException();
        }
        length -= read;
        offset += read;
      }
    }
  }
}
