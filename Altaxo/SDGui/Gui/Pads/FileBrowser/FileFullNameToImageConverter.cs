using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;


namespace Altaxo.Gui.Pads.FileBrowser
{
	public class FileFullNameToImageConverter : IValueConverter
	{
		#region Interop

		public const uint SHGFI_ICON = 0x100;

		public const uint SHGFI_LARGEICON = 0x0;

		public const uint SHGFI_SMALLICON = 0x1;

		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

		[DllImport("User32.dll")]
		public static extern int DestroyIcon(IntPtr hIcon);

		#endregion

		public ImageSource DefaultImageSource { get; set; }

		public ImageSource GetSmallIcon(string fileName)
		{
			return GetIcon(fileName, SHGFI_SMALLICON);
		}


		public  ImageSource GetIcon(string fileName, uint flags)
		{
			var shinfo = new SHFILEINFO();
			SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | flags);
			IntPtr iconHandle = shinfo.hIcon;
			if (IntPtr.Zero == iconHandle)
				return DefaultImageSource;
			ImageSource img = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			DestroyIcon(iconHandle);
			return img;
		}


		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var s = value as string;
			if (string.IsNullOrEmpty(s))
				return DefaultImageSource;
			else
				return GetSmallIcon(s);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
