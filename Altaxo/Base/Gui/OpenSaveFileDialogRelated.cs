using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	public class OpenFileOptions
	{
		List<KeyValuePair<string, string>> _filterList = new List<KeyValuePair<string, string>>();

		public void AddFilter(string filter, string description)
		{
  		_filterList.Add(new KeyValuePair<string, string>(filter, description));
		}

		public IList<KeyValuePair<string,string>> FilterList
		{
			get
			{
				return _filterList.AsReadOnly();
			}
		}
		public string Title { get; set; }
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
