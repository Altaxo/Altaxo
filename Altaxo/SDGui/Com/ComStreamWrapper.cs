using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Com
{
	/// <summary>
	/// Wraps any Com IStream (<see cref="System.Runtime.InteropServices.ComTypes.IStream"/>) into a <see cref="System.IO.Stream"/>.
	/// </summary>
	public class ComStreamWrapper : System.IO.Stream
	{
		System.Runtime.InteropServices.ComTypes.IStream _istream;

		System.Runtime.InteropServices.ComTypes.STATSTG _streamStatus;

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


		public ComStreamWrapper(System.Runtime.InteropServices.ComTypes.IStream istream)
		{
			if (null == istream)
				throw new ArgumentNullException("istream");

			_istream = istream;

			_streamStatus = new System.Runtime.InteropServices.ComTypes.STATSTG();
			_istream.Stat(out _streamStatus, STATFLAG.NONAME);
		}

		#region Implementation of System.IO.Stream

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return 0 != (_streamStatus.grfMode & 1); }
		}

		public override void Flush()
		{
			_istream.Commit(STGC.DEFAULT);
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
				IntPtr p = new IntPtr();
				_istream.Seek(0, STREAM_SEEK.CUR, p);
				return p.ToInt64();
			}
			set
			{
				IntPtr p = new IntPtr();
				_istream.Seek(0, STREAM_SEEK.SET, p);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			IntPtr pcbRead = new IntPtr();
			if (0 == offset)
			{
				_istream.Read(buffer, count, pcbRead);
			}
			else
			{
				var len = Math.Max(0,Math.Min(count, buffer.Length-offset));
				var tempBuffer = new byte[len];
				_istream.Read(tempBuffer, len, pcbRead);
				Buffer.BlockCopy(tempBuffer,0,buffer,offset, pcbRead.ToInt32());
			}
			return pcbRead.ToInt32();
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

			IntPtr result = new IntPtr(0L);
			_istream.Seek(offset, seekType, result);
			return result.ToInt64();
		}

		public override void SetLength(long value)
		{
			_istream.SetSize(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			IntPtr pcbWritten = new IntPtr();
			if(0==offset)
			{
				_istream.Write(buffer, count, pcbWritten);
			}
			else
			{
				var tempBuffer = new byte[count];
				Buffer.BlockCopy(buffer, offset, tempBuffer, 0, count);
				_istream.Write(tempBuffer, count, pcbWritten);
			}
		}

		#endregion

	}
}
