#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Altaxo.Gui
{
	/// <summary>
	/// Encapsulates the logic to map a Gdi bitmap on a Wpf bitmap source.
	/// </summary>
	public class GdiToWpfBitmap : IDisposable, System.ComponentModel.INotifyPropertyChanged
	{
		#region Native calls

		[DllImport("kernel32.dll", SetLastError = true)]

		static extern IntPtr CreateFileMapping(IntPtr hFile,

																					 IntPtr lpFileMappingAttributes,

																					 uint flProtect,

																					 uint dwMaximumSizeHigh,

																					 uint dwMaximumSizeLow,

																					 string lpName);



		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,

																			 uint dwDesiredAccess,

																			 uint dwFileOffsetHigh,

																			 uint dwFileOffsetLow,

																			 uint dwNumberOfBytesToMap);

		[DllImport("kernel32", EntryPoint = "UnmapViewOfFile", SetLastError = true)]
		private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);


		[DllImport("kernel32", EntryPoint = "CloseHandle", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr handle);



		// Windows constants

		static uint FILE_MAP_ALL_ACCESS = 0xF001F;

		static uint PAGE_READWRITE = 0x04;

		static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

		#endregion

		static long _totalBytesAllocated; // only for debugging, designates the total number of bytes allocated in all those objects

		const int BytesPerPixel = 4; // it is only possible to use ARGB format, otherwise Imaging.CreateBitmapSourceFromMemorySection would copy the bitmap instead of mapping it

		IntPtr _section;
		IntPtr _map;
		System.Drawing.Bitmap _bmp;
		System.Windows.Interop.InteropBitmap _interopBmp;
		int _width, _height;
		/// <summary>
		/// Creates an instance of given width and height. Note that the GidBitmap is created during this calling thread. (the WpfBitmap is created later during the first access to WpfBitmap).
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public GdiToWpfBitmap(int width, int height)
		{
			InternalAllocate(width, height);
		}

		public GdiToWpfBitmap()
		{

		}

		/// <summary>
		/// Resizes the bitmap. Note that the GidBitmap is created during this calling thread. (the WpfBitmap is created later during the first access to WpfBitmap).
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void Resize(int width, int height)
		{
			InternalDeallocate(true);
			InternalAllocate(width, height);
		}

		void InternalAllocate(int width, int height)
		{
			width = Math.Max(width, 1);
			height = Math.Max(height, 1);

			_width = width;
			_height = height;
			uint numBytesToAllocate = (uint)(width * height * BytesPerPixel);

			_section = CreateFileMapping(INVALID_HANDLE_VALUE,
																				 IntPtr.Zero,
																				 PAGE_READWRITE,
																				 0,
																				 numBytesToAllocate,
																				 null);

			if (_section == IntPtr.Zero)
				throw new InvalidOperationException(string.Format("Unable to create file mapping for {0} x {1} pixel", width, height));

			_map = MapViewOfFile(_section, FILE_MAP_ALL_ACCESS, 0, 0, numBytesToAllocate);

			if (IntPtr.Zero == _map)
			{
				System.GC.Collect();
				_map = MapViewOfFile(_section, FILE_MAP_ALL_ACCESS, 0, 0, numBytesToAllocate);
			}

			if (_map == IntPtr.Zero)
			{
				CloseHandle(_section);
				_section = IntPtr.Zero;
				string exception = string.Format("Unable to create view of file for {0} x {1} pixel", width, height);
				width = height = 0;
				throw new InvalidOperationException(exception);
			}


			_bmp = new System.Drawing.Bitmap(_width, _height, _width * BytesPerPixel, System.Drawing.Imaging.PixelFormat.Format32bppArgb, _map);

			GC.AddMemoryPressure(numBytesToAllocate);
			_totalBytesAllocated += numBytesToAllocate;

			_interopBmp = (System.Windows.Interop.InteropBitmap)System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(_section, _width, _height, System.Windows.Media.PixelFormats.Bgra32, _width * BytesPerPixel, 0);
		}

		void InternalDeallocate(bool disposing)
		{
			if (disposing)
			{
				// free managed resources
				if (_bmp != null)
				{
					_bmp.Dispose();
					_bmp = null;
				}

				if (_interopBmp != null)
				{
					_interopBmp = null;
				}
			}

			// now free all unmanaged resources
			if (IntPtr.Zero != _map)
			{
				UnmapViewOfFile(_map);
				_map = IntPtr.Zero;
			}

			if (IntPtr.Zero != _section)
			{

				CloseHandle(_section);
				_section = IntPtr.Zero;
			}

			GC.RemoveMemoryPressure(_width * _height * BytesPerPixel);
			_totalBytesAllocated -= (_width * _height * BytesPerPixel);

			_width = 0;
			_height = 0;
		}


		public System.Drawing.Bitmap GdiBitmap
		{
			get
			{
				return _bmp;
			}
		}

		public System.Drawing.Graphics BeginGdiPainting()
		{
			return System.Drawing.Graphics.FromImage(_bmp);
		}

		public void EndGdiPainting()
		{
			_interopBmp.Invalidate();
			OnPropertyChanged("WpfBitmapSource");
		}


		public System.Drawing.Rectangle GdiRectangle
		{
			get
			{
				return new System.Drawing.Rectangle(0, 0, _width, _height);
			}
		}

		
		public System.Windows.Interop.InteropBitmap WpfBitmap
		{
			get
			{
				return _interopBmp;
			}
		}
		
		public System.Windows.Media.Imaging.BitmapSource WpfBitmapSource
		{
			get
			{
				_interopBmp.Invalidate();
				return ( System.Windows.Media.Imaging.BitmapSource)_interopBmp.GetAsFrozen();
			}
		}
		

		#region IDisposable Members

		public void Dispose()
		{
			InternalDeallocate(true);
			GC.SuppressFinalize(this);
		}

		~GdiToWpfBitmap()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			InternalDeallocate(false);
		}


		#endregion

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		public virtual void OnPropertyChanged(string name)
		{
			if (null != PropertyChanged)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
		}
	}
}
