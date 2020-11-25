#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
  public class ReadOnlyPartialStreamView : System.IO.Stream
  {
    private Stream _stream;
    private long _offset;
    private long _streamLength;
    private long _myPosition;
    public bool IsStreamOwner { get; set; }
    public bool IsClosedDisposed { get; private set; }


    public ReadOnlyPartialStreamView(Stream stream, long offset, long count)
    {
      _stream = stream;
      _offset = offset;
      _streamLength = count;
      _myPosition = 0;
      _stream.Seek(_offset, SeekOrigin.Begin);
    }

    public override bool CanRead => _stream.CanRead;

    public override bool CanSeek => _stream.CanSeek;

    public override bool CanWrite => false;

    public override long Length => _stream.Length;

    public override long Position
    {
      get => _stream.Position - _offset;
      set
      {
        if (value < 0 || value > _streamLength)
          throw new InvalidOperationException();
        _myPosition = value;
        _stream.Position = value + _offset;
      }
    }


    public override void Flush()
    {
      _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (IsClosedDisposed)
        throw new ObjectDisposedException(nameof(ReadOnlyPartialStreamView));

      if (_stream.Position != _myPosition + _offset)
        throw new InvalidOperationException("Underlying stream was manipulated");

      var maxRead = (int)Math.Min(count, _streamLength - _myPosition);
      var result = _stream.Read(buffer, offset, maxRead);
      _myPosition += result;
      return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          if (offset < 0 || offset > _streamLength)
            throw new InvalidOperationException();
          _myPosition = offset;
          return _stream.Seek(offset, origin) - _offset;

        case SeekOrigin.Current:
          throw new NotImplementedException();
        case SeekOrigin.End:
          throw new NotImplementedException();
        default:
          throw new NotImplementedException();
      }
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override void Close()
    {
      if (IsClosedDisposed)
        return;
      IsClosedDisposed = true;

      base.Close();
      if (IsStreamOwner)
      {
        _stream.Close();
        _stream.Dispose();
        _stream = Stream.Null;
      }
    }

    protected override void Dispose(bool disposing)
    {
      Close();
    }
  }
}
