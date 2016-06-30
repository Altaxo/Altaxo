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
	internal class TransformedReadableColumnProxyForStandaloneColumns : Main.SuspendableDocumentLeafNodeWithEventArgs, IReadableColumnProxy
	{
		private IReadableColumn _underlyingColumn;
		private IVariantToVariantTransformation _transformation;
		private IReadableColumn _cachedResultingColumn;

		public static TransformedReadableColumnProxyForStandaloneColumns FromColumn(ITransformedReadableColumn column)
		{
			if (null == column)
				throw new ArgumentNullException(nameof(column));

			var colAsDocumentNode = column.UnderlyingReadableColumn as IDocumentLeafNode;
			if (null != colAsDocumentNode)
				throw new ArgumentException(string.Format("Column does implement {0}. The actual type of column is {1}", typeof(IDocumentLeafNode), column.GetType()));

			return new TransformedReadableColumnProxyForStandaloneColumns(column); ;
		}

		/// <summary>
		/// Constructor by giving a numeric column.
		/// </summary>
		/// <param name="column">The numeric column to hold.</param>
		protected TransformedReadableColumnProxyForStandaloneColumns(ITransformedReadableColumn column)
		{
			_underlyingColumn = column.UnderlyingReadableColumn;
			_transformation = column.Transformation;
			_cachedResultingColumn = column;
		}

		protected TransformedReadableColumnProxyForStandaloneColumns(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		#region Serialization

		/// <summary>
		/// 2016-06-24 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TransformedReadableColumnProxyForStandaloneColumns), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (TransformedReadableColumnProxyForStandaloneColumns)obj;
				info.AddValue("Column", s._underlyingColumn);
				info.AddValue("Transformation", s._transformation);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (TransformedReadableColumnProxyForStandaloneColumns)o ?? new TransformedReadableColumnProxyForStandaloneColumns(info);
				s._underlyingColumn = (IReadableColumn)info.GetValue("Column", s);
				s._transformation = (IVariantToVariantTransformation)info.GetValue("Transformation", s);
				if (null != s._underlyingColumn)
					s._cachedResultingColumn = new TransformedReadableColumn(s._underlyingColumn, s._transformation);

				return s;
			}
		}

		#endregion Serialization

		public IReadableColumn Document
		{
			get
			{
				return _cachedResultingColumn;
			}
		}

		public object Clone()
		{
			return (TransformedReadableColumnProxyForStandaloneColumns)this.MemberwiseClone();
		}

		public bool IsEmpty
		{
			get { return null == _underlyingColumn; }
		}

		public string GetName(int level)
		{
			if (null == _underlyingColumn)
				return string.Empty;
			else
				return (_transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction) + " " + _underlyingColumn.FullName;
		}

		public object DocumentObject
		{
			get { return _cachedResultingColumn; }
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