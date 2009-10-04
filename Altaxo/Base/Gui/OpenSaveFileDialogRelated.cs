using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	public class OpenFileOptions
	{
		List<KeyValuePair<string, string>> _filterList = new List<KeyValuePair<string, string>>();

		public void AddFilter(params string[] values)
		{
			if (values.Length % 2 != 0)
				throw new ArgumentException("values array has odd length");

			for (int i = 0; i < values.Length / 2; i++)
				_filterList.Add(new KeyValuePair<string, string>(values[2 * i], values[2 * i + 1]));
		}

		public IList<KeyValuePair<string,string>> FilterList
		{
			get
			{
				return _filterList.AsReadOnly();
			}
		}
		public string DialogTitle { get; set; }
		public int FilterIndex { get; set; }
		public bool Multiselect { get; set; }
		public bool RestoreDirectory { get; set; }
		public string InitialDirectory { get; set; }
		public string[] FileNames { get; set; }
		public string FileName { get; set; }
	}
	public class SaveFileOptions : OpenFileOptions
	{
	}
}
