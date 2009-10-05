using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Interface for a data object to put data on the clipboard.
	/// </summary>
	public interface IClipboardSetDataObject
	{
		void SetImage(System.Drawing.Image image);
		void SetFileDropList(System.Collections.Specialized.StringCollection filePaths);
		void SetData(string format, object data);
		void SetData(Type format, object data);
		void SetCommaSeparatedValues(string text);

	}

	/// <summary>
	/// Interface for a data object to get data from the clipboard.
	/// </summary>
	public interface IClipboardGetDataObject
	{
		string[] GetFormats();
		bool GetDataPresent(string format);
		bool GetDataPresent(System.Type type);
		object GetData(string format);
		object GetData(System.Type type);
		bool ContainsFileDropList();
		System.Collections.Specialized.StringCollection GetFileDropList();
		bool ContainsImage();
		System.Drawing.Image GetImage();
	}
}
