using System;
using System.Collections.Generic;
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
			if (null == istream)
				throw new ArgumentNullException("istream");

			_isStreamOwner = isStreamOwner;
			_istream = istream;

			_int64Ptr = Marshal.AllocCoTaskMem(8);
			_int32Ptr = Marshal.AllocCoTaskMem(4);

			_streamStatus = new System.Runtime.InteropServices.ComTypes.STATSTG();
			_istream.Stat(out _streamStatus, STATFLAG.NONAME);
		}

		public override void Close()
		{
			if (null != _istream)
			{
				//_istream.Commit(0);
				if (_isStreamOwner)
				{
					Marshal.ReleaseComObject(_istream);
#if COMLOGGING
					Debug.ReportInfo("ComStreamWrapper.Close: istream released");
#endif
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
#if COMLOGGING
			Debug.ReportInfo("ComStreamWrapper.Dispose({0})", disposing);
#endif

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
#if COMLOGGING
					Debug.ReportInfo("ComStreamWrapper.Dispose: istream released");
#endif
				}
				_istream = null;
			}
		}

		#region Implementation of System.IO.Stream

		public override bool CanRead
		{
			get { return 1 != (_streamStatus.grfMode & 0x03); }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return 0 != (_streamStatus.grfMode & 0x03); }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get
			{
				var stat = new System.Runtime.InteropServices.ComTypes.STATSTG();
				_istream.Stat(out stat, STATFLAG.NONAME);
				return stat.cbSize;
			}
		}

		public override long Position
		{
			get
			{
				_istream.Seek(0, STREAM_SEEK.CUR, _int64Ptr);
				return Marshal.ReadInt64(_int64Ptr);
			}
			set
			{
				Marshal.WriteInt64(_int64Ptr, value);
				_istream.Seek(0, STREAM_SEEK.SET, _int64Ptr);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = 0;
			if (0 == offset)
			{
				_istream.Read(buffer, count, _int32Ptr);
				read = Marshal.ReadInt32(_int32Ptr);
			}
			else
			{
				var len = Math.Max(0, Math.Min(count, buffer.Length - offset));
				var tempBuffer = new byte[len];
				_istream.Read(tempBuffer, len, _int32Ptr);
				read = Marshal.ReadInt32(_int32Ptr);
				Buffer.BlockCopy(tempBuffer, 0, buffer, offset, read);
			}
			return read;
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
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
			return Marshal.ReadInt64(_int64Ptr);
		}

		public override void SetLength(long value)
		{
			_istream.SetSize(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
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

		#endregion Implementation of System.IO.Stream
	}
}