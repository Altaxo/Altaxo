#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	internal class ReadableColumnProxyForStandaloneColumns : Main.SuspendableDocumentLeafNodeWithEventArgs, IReadableColumnProxy
	{
		private IReadableColumn _column;

		public static ReadableColumnProxyForStandaloneColumns FromColumn(IReadableColumn column)
		{
			var colAsDocumentNode = column as IDocumentLeafNode;
			if (null != colAsDocumentNode)
				throw new ArgumentException(string.Format("column does implement {0}. The actual type of column is {1}", typeof(IDocumentLeafNode), column.GetType()));

			return new ReadableColumnProxyForStandaloneColumns(column); ;
		}

		/// <summary>
		/// Constructor by giving a numeric column.
		/// </summary>
		/// <param name="column">The numeric column to hold.</param>
		protected ReadableColumnProxyForStandaloneColumns(IReadableColumn column)
		{
			_column = column;
		}

		#region Serialization

		/// <summary>
		/// 2014-12-26 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ReadableColumnProxyForStandaloneColumns), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ReadableColumnProxyForStandaloneColumns)obj;
				info.AddValue("Column", s._column);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (ReadableColumnProxyForStandaloneColumns)o ?? new ReadableColumnProxyForStandaloneColumns(null);
				object node = info.GetValue("Column", s);
				s._column = (IReadableColumn)node;
				return s;
			}
		}

		#endregion Serialization

		public IReadableColumn Document
		{
			get { return _column; }
		}

		public object Clone()
		{
			return FromColumn(_column);
		}

		public bool IsEmpty
		{
			get { return null == _column; }
		}

		public string GetName(int level)
		{
			return _column == null ? string.Empty : _column.ToString();
		}

		public object DocumentObject
		{
			get { return _column; }
		}

		public AbsoluteDocumentPath DocumentPath
		{
			get { return AbsoluteDocumentPath.DocumentPathOfRootNode; }
		}

		public bool ReplacePathParts(AbsoluteDocumentPath partToReplace, AbsoluteDocumentPath newPart, IDocumentLeafNode rootNode)
		{
			return false;
		}
	}
}