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
	/// <summary>
	/// Proxy for a <see cref="DataColumn"/> that is part of the document, and a transformation.
	/// </summary>
	/// <seealso cref="Altaxo.Main.DocNodeProxy" />
	/// <seealso cref="Altaxo.Data.IReadableColumnProxy" />
	internal class TransformedReadableColumnProxy : DocNodeProxy, IReadableColumnProxy
	{
		private IVariantToVariantTransformation _transformation = null;

		#region Serialization

		/// <summary>
		/// 2016-06-24 intial version.
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ReadableColumnProxy), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (TransformedReadableColumnProxy)obj;
				info.AddBaseValueEmbedded(obj, typeof(DocNodeProxy)); // serialize the base class
				info.AddValue("Transformation", s._transformation);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (TransformedReadableColumnProxy)o ?? new TransformedReadableColumnProxy(info);
				info.GetBaseValueEmbedded(s, typeof(DocNodeProxy), parent);         // deserialize the base class
				s._transformation = (IVariantToVariantTransformation)info.GetValue("Transformation", s);
				return s;
			}
		}

		#endregion Serialization

		public static TransformedReadableColumnProxy FromColumn(ITransformedReadableColumn column)
		{
			if (null == column)
				throw new ArgumentNullException(nameof(column));
			var colAsDocumentNode = column.OriginalReadableColumn as IDocumentLeafNode;
			if (null == colAsDocumentNode)
				throw new ArgumentException(string.Format("column does not implement {0}. The actual type of column is {1}", typeof(IDocumentLeafNode), column.GetType()));

			return new TransformedReadableColumnProxy(column);
		}

		protected TransformedReadableColumnProxy(ITransformedReadableColumn column)
			: base((IDocumentLeafNode)column.OriginalReadableColumn)
		{
			_transformation = column.Transformation;
		}

		/// <summary>
		/// For deserialization purposes only.
		/// </summary>
		protected TransformedReadableColumnProxy(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
			: base(info)
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="from">Object to clone from.</param>
		public TransformedReadableColumnProxy(TransformedReadableColumnProxy from)
			: base(from)
		{
			_transformation = from._transformation; // transformation is immutable
		}

		protected override bool IsValidDocument(object obj)
		{
			return (obj is IReadableColumn) || obj == null;
		}

		public IReadableColumn Document
		{
			get
			{
				var originalColumn = (IReadableColumn)base.DocumentObject;
				if (null == originalColumn)
					return null;
				else
					return new TransformedReadableColumn(originalColumn, _transformation);
			}
		}

		public override object Clone()
		{
			return new TransformedReadableColumnProxy(this);
		}

		public string GetName(int level)
		{
			string trans = _transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction;
			return trans + " " + ReadableColumnProxy.GetName(level, (IReadableColumn)base.DocumentObject, InternalDocumentPath);
		}
	}
}