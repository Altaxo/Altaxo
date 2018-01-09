using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Altaxo.Com
{
	/// <summary>
	/// Wrapper to debug a stream. Wraps around another stream, and logs all calls to that stream.
	/// </summary>
	/// <seealso cref="System.IO.Stream" />
	public class DebugStreamWrapper : System.IO.Stream
	{
		private Stream _istream;
		private bool _isStreamOwner;

		private TextWriter _debugWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DebugStreamWrapper"/> class.
		/// </summary>
		/// <param name="istream">The istream to wrap.</param>
		/// <param name="isStreamOwner">If set to <c>true</c>, this wrapper is considerd the owner of this stream. That means that during closing of this stream wrapper, the wrapped stream is also closed.</param>
		/// <param name="debugWriter">The stream that logs the debug information.</param>
		public DebugStreamWrapper(Stream istream, bool isStreamOwner, TextWriter debugWriter)
		{
			_istream = istream;
			_isStreamOwner = isStreamOwner;
			_debugWriter = debugWriter;
		}

		/// <inheritdoc/>
		public override bool CanSeek
		{
			get
			{
				var result = _istream.CanSeek;
				_debugWriter?.WriteLine("StreamWrapper.CanSeek: {0}", result);
				_debugWriter?.Flush();
				return result;
			}
		}

		/// <inheritdoc/>
		public override bool CanRead
		{
			get
			{
				var result = _istream.CanRead;
				_debugWriter?.WriteLine("StreamWrapper.CanRead: {0}", result);
				_debugWriter?.Flush();
				return result;
			}
		}

		/// <inheritdoc/>
		public override bool CanWrite
		{
			get
			{
				var result = _istream.CanWrite;
				_debugWriter?.WriteLine("StreamWrapper.CanWrite: {0}", result);
				_debugWriter?.Flush();
				return result;
			}
		}

		/// <inheritdoc/>
		public override long Length
		{
			get
			{
				var result = _istream.Length;

				_debugWriter?.WriteLine("StreamWrapper.Length: {0}", result);
				_debugWriter?.Flush();
				return result;
			}
		}

		/// <inheritdoc/>
		public override long Position
		{
			get
			{
				var result = _istream.Position;
				_debugWriter?.WriteLine("StreamWrapper.Position.get: {0}", result);
				_debugWriter?.Flush();
				return result;
			}
			set
			{
				_debugWriter?.WriteLine("StreamWrapper.Position.set: {0}", value);
				_debugWriter?.Flush();
				_istream.Position = value;
			}
		}

		/// <inheritdoc/>
		public override void SetLength(long value)
		{
			_debugWriter?.WriteLine("StreamWrapper.SetLength: {0}", value);
			_debugWriter?.Flush();
			_istream.SetLength(value);
		}

		/// <inheritdoc/>
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			_debugWriter?.Write("StreamWrapper.Seek, offset={0}, origin={1}", offset, origin);
			var result = _istream.Seek(offset, origin);
			_debugWriter?.WriteLine("... result: {0}", result);
			_debugWriter?.Flush();
			return result;
		}

		/// <inheritdoc/>
		public override int Read(byte[] buffer, int offset, int count)
		{
			_debugWriter?.Write("StreamWrapper.Read, bufferlength={0}, offset={1}, count={2}", buffer.Length, offset, count);
			var result = _istream.Read(buffer, offset, count);

			if (null != _debugWriter)
			{
				_debugWriter?.Write(".... read: {0} byte", result);
				_debugWriter.Write(" (");
				if (result <= 4)
				{
					for (int i = 0; i < result; ++i)
						_debugWriter.Write(" {0:X}", buffer[offset + i]);
				}
				else
				{
					for (int i = 0; i < 2; ++i)
						_debugWriter.Write(" {0:X}", buffer[offset + i]);
					_debugWriter.Write("...");
					for (int i = result - 2; i < result; ++i)
						_debugWriter.Write(" {0:X}", buffer[offset + i]);
				}
				_debugWriter.WriteLine(")");
			}

			_debugWriter?.Flush();
			return result;
		}

		/// <inheritdoc/>
		public override void Write(byte[] buffer, int offset, int count)
		{
			_debugWriter?.WriteLine("StreamWrapper.Write, bufferlength={0}, offset={1}, count={2}", buffer.Length, offset, count);
			_debugWriter?.Flush();
		}

		/// <inheritdoc/>
		public override void Flush()
		{
			_debugWriter.WriteLine("StreamWrapper.Flush");
			_debugWriter.Flush();
			_istream.Flush();
		}

		/// <inheritdoc/>
		public override void Close()
		{
			if (null != _istream)
			{
				//_istream.Commit(0);
				if (_isStreamOwner)
				{
					_istream.Close();
					_debugWriter?.WriteLine("StreamWrapper.Close");
					_debugWriter?.Flush();
				}
				_istream = null;
			}
			base.Close();
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			_debugWriter?.WriteLine("StreamWrapper.Dispose({0})", disposing);

			if (disposing)
			{
				_debugWriter?.Dispose();
				_debugWriter = null;

				if (_isStreamOwner)
					_istream?.Dispose();
				_istream = null;
			}

			base.Dispose();
		}
	}
}