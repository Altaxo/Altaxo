using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	public struct DocumentNodeAndName
	{
		private IDocumentLeafNode _doc;
		private string _name;

		public DocumentNodeAndName(IDocumentLeafNode doc, string name)
		{
			_doc = doc;
			_name = name;
		}

		public IDocumentLeafNode DocumentNode { get { return _doc; } }

		public string Name { get { return _name; } }

		public bool IsEmpty { get { return null == _doc; } }
	}
}