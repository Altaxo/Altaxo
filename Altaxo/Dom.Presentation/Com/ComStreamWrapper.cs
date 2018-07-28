#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Com
{
  /// <summary>
  /// Wraps any Com IStream (<see cref="System.Runtime.InteropServices.ComTypes.IStream"/>) into a <see cref="System.IO.Stream"/>.
  /// </summary>
  public class ComStreamWrapper : System.IO.Stream
  {
    private System.Runtime.InteropServices.ComTypes.IStream _istream;
    private System.Runtime.InteropServices.ComTypes.STATSTG _streamStatus;

    private IntPtr _int64Ptr;
    private IntPtr _int32Ptr;

    /// <summary>
    /// True when this instance is the stream owner and is responsible for disposing the stream.
    /// </summary>
    private bool _isStreamOwner;

#if COMSTREAMLOGGING
		public StringBuilder _debugWriter = new StringBuilder();
#endif

    /// <summary>
    /// Reports an informational message for stream debugging.
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="args">Additional arguments.</param>
    [Conditional("COMSTREAMLOGGING")]
    public void ReportDebugStream(string format, params object[] args)
    {
#if COMSTREAMLOGGING
			_debugWriter.AppendFormat(format, args);
#endif
    }

    /// <summary>
    /// Reports stream debugging info about the first and last bytes read or written.
    /// </summary>
    /// <param name="readOrWrite">Either 'read' or 'written'</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The buffer offset.</param>
    /// <param name="bytesReadOrWritten">The bytes read from the stream or written to the stream.</param>
    [Conditional("COMSTREAMLOGGING")]
    public void ReportDebugStreamBuffer(string readOrWrite, byte[] buffer, int offset, int bytesReadOrWritten)
    {
#if COMSTREAMLOGGING
			_debugWriter?.AppendFormat(".... {0}: {1} byte", readOrWrite, bytesReadOrWritten);
			_debugWriter.AppendFormat(" (");
			if (bytesReadOrWritten <= 4)
			{
				for (int i = 0; i < bytesReadOrWritten; ++i)
					_debugWriter.AppendFormat(" {0:X}", buffer[offset + i]);
			}
			else
			{
				for (int i = 0; i < 2; ++i)
					_debugWriter.AppendFormat(" {0:X}", buffer[offset + i]);
				_debugWriter.AppendFormat("...");
				for (int i = bytesReadOrWritten - 2; i < bytesReadOrWritten; ++i)
					_debugWriter.AppendFormat(" {0:X}", buffer[offset + i]);
			}
			_debugWriter.AppendFormat(")\r\n");
#endif
    }

    private static class STREAM_SEEK
    {
      public const int SET = 0;
      public const int CUR = 1;
      public const int END = 2;
    }

    private static class STATFLAG
    {
      public const int DEFAULT = 0;
      public const int NONAME = 1;
      public const int NOOPEN = 2;
    }

    private static class STGC
    {
      public const int DEFAULT = 0;
      public const int OVERWRITE = 1;
      public const int ONLYIFCURRENT = 2;
      public const int DANGEROUSLYCOMMITMERELYTODISKCACHE = 4;
      public const int CONSOLIDATE = 8;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComStreamWrapper"/> class.
    /// </summary>
    /// <param name="istream">The istream to wrap.</param>
    /// <param name="isStreamOwner">If set to <c>true</c>, this instance should be the owner of the wrapped stream and thus is responsible for disposing it.</param>
    /// <exception cref="System.ArgumentNullException">istream is null.</exception>
    public ComStreamWrapper(System.Runtime.InteropServices.ComTypes.IStream istream, bool isStreamOwner)
    {
      _istream = istream ?? throw new ArgumentNullException(nameof(istream));
      _isStreamOwner = isStreamOwner;

      _int64Ptr = Marshal.AllocCoTaskMem(8);
      _int32Ptr = Marshal.AllocCoTaskMem(4);

      _streamStatus = new System.Runtime.InteropServices.ComTypes.STATSTG();
      _istream.Stat(out _streamStatus, STATFLAG.NONAME);
    }

    public override bool CanSeek
    {
      get
      {
        var result = true;
        ReportDebugStream("StreamWrapper.CanSeek: {0}\r\n", result);
        return result;
      }
    }

    public override bool CanRead
    {
      get
      {
        var result = (1 != (_streamStatus.grfMode & 0x03));
        ReportDebugStream("StreamWrapper.CanRead: {0}\r\n", result);
        return result;
      }
    }

    public override bool CanWrite
    {
      get
      {
        var result = (0 != (_streamStatus.grfMode & 0x03));
        ReportDebugStream("StreamWrapper.CanWrite: {0}\r\n", result);
        return result;
      }
    }

    public override long Length
    {
      get
      {
        var stat = new System.Runtime.InteropServices.ComTypes.STATSTG();
        _istream.Stat(out stat, STATFLAG.NONAME);
        var result = stat.cbSize;
        ReportDebugStream("StreamWrapper.Length: {0}\r\n", result);
        return result;
      }
    }

    public override long Position
    {
      get
      {
        _istream.Seek(0, STREAM_SEEK.CUR, _int64Ptr);
        var result = Marshal.ReadInt64(_int64Ptr);
        ReportDebugStream("StreamWrapper.Position.get: {0}\r\n", result);
        return result;
      }
      set
      {
        _istream.Seek(value, STREAM_SEEK.SET, _int64Ptr);
        ReportDebugStream("StreamWrapper.Position.set: {0} -> read back: {1}\r\n", value, Marshal.ReadInt64(_int64Ptr));
      }
    }

    public override void SetLength(long value)
    {
      ReportDebugStream("StreamWrapper.SetLength: {0}\r\n", value);
      _istream.SetSize(value);
    }

    public override long Seek(long offset, System.IO.SeekOrigin origin)
    {
      ReportDebugStream("StreamWrapper.Seek, offset={0}, origin={1}", offset, origin);

      int seekType;
      switch (origin)
      {
        case System.IO.SeekOrigin.Begin:
          seekType = STREAM_SEEK.SET;
          break;

        case System.IO.SeekOrigin.Current:
          seekType = STREAM_SEEK.CUR;
          break;

        case System.IO.SeekOrigin.End:
          seekType = STREAM_SEEK.END;
          break;

        default:
          seekType = STREAM_SEEK.SET;
          break;
      }

      _istream.Seek(offset, seekType, _int64Ptr);
      var result = Marshal.ReadInt64(_int64Ptr);
      ReportDebugStream("... result: {0}\r\n", result);
      return result;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      ReportDebugStream("StreamWrapper.Read, bufferlength={0}, offset={1}, count={2}", buffer.Length, offset, count);
      int result = 0;
      if (0 == offset)
      {
        _istream.Read(buffer, count, _int32Ptr);
        result = Marshal.ReadInt32(_int32Ptr);
      }
      else
      {
        var len = Math.Max(0, Math.Min(count, buffer.Length - offset));
        var tempBuffer = new byte[len];
        _istream.Read(tempBuffer, len, _int32Ptr);
        result = Marshal.ReadInt32(_int32Ptr);
        Buffer.BlockCopy(tempBuffer, 0, buffer, offset, result);
      }
      ReportDebugStreamBuffer("read", buffer, offset, result);
      return result;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      ReportDebugStream("StreamWrapper.Write, bufferlength={0}, offset={1}, count={2}\r\n", buffer.Length, offset, count);
      if (0 == offset)
      {
        _istream.Write(buffer, count, _int32Ptr);
      }
      else
      {
        var tempBuffer = new byte[count];
        Buffer.BlockCopy(buffer, offset, tempBuffer, 0, count);
        _istream.Write(tempBuffer, count, _int32Ptr);
      }
    }

    public override void Flush()
    {
      ReportDebugStream("StreamWrapper.Flush\r\n");
    }

    public override void Close()
    {
      if (null != _istream)
      {
        //_istream.Commit(0);
        if (_isStreamOwner)
        {
          Marshal.ReleaseComObject(_istream);
          ComDebug.ReportInfo("ComStreamWrapper.Close: istream released");

          ReportDebugStream("StreamWrapper.Close");
        }
        _istream = null;
      }
      base.Close();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.IO.Stream" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      ComDebug.ReportInfo("ComStreamWrapper.Dispose({0})", disposing);
      ReportDebugStream("StreamWrapper.Dispose({0})\r\n", disposing);

      base.Dispose(disposing);

      Marshal.FreeCoTaskMem(_int64Ptr);
      _int64Ptr = IntPtr.Zero;

      Marshal.FreeCoTaskMem(_int32Ptr);
      _int32Ptr = IntPtr.Zero;

      if (null != _istream)
      {
        if (_isStreamOwner)
        {
          Marshal.ReleaseComObject(_istream);
          ComDebug.ReportInfo("ComStreamWrapper.Dispose: istream released");
        }
        _istream = null;
      }
    }
  }
}
