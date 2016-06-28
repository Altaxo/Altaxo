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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data.Transformations
{
	public class CompoundTransformation : IVariantToVariantTransformation
	{
		/// <summary>
		/// The transformations. The innermost (i.e. first transformation to carry out, the rightmost transformation) is located at index 0.
		/// </summary>
		private List<IVariantToVariantTransformation> _transformations = new List<IVariantToVariantTransformation>();

		#region Serialization

		/// <summary>
		/// 2016-06-25 Initial version.
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CompoundTransformation), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CompoundTransformation)obj;
				info.CreateArray("Transformations", s._transformations.Count);
				foreach (var t in s._transformations)
					info.AddValue("e", t);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int count = info.OpenArray("Transformations");
				List<IVariantToVariantTransformation> arr = new List<IVariantToVariantTransformation>(count);
				for (int i = 0; i < count; ++i)
				{
					arr.Add((IVariantToVariantTransformation)info.GetValue("e", null));
				}
				info.CloseArray(count);
				return new CompoundTransformation() { _transformations = arr };
			}
		}

		#endregion Serialization

		private CompoundTransformation()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompoundTransformation"/> class.
		/// </summary>
		/// <param name="transformations">The transformations.</param>
		/// <exception cref="System.ArgumentException">
		/// Enumeration contains no items
		/// or
		/// Enumeration contains only one item. Please use this item directly.
		/// </exception>
		public CompoundTransformation(IEnumerable<IVariantToVariantTransformation> transformations)
		{
			_transformations = new List<IVariantToVariantTransformation>(transformations);

			if (_transformations.Count == 0)
				throw new ArgumentException("Enumeration contains no items", nameof(transformations));
			if (_transformations.Count == 1)
				throw new ArgumentException("Enumeration contains only one item. Please use this item directly.", nameof(transformations));
		}

		/// <summary>
		/// Try to get a compound transformation. Use this function when in doubt how many transformations the enumeration yields.
		/// The behavior is as follows: if the enumeration is null or empty, the return value is null. If the enumeration contains only one
		/// element, the return value is that element. If the enumeration contains multiple elements, the return value is a compound transformation
		/// with all elements.
		/// </summary>
		/// <param name="transformations">Enumeration of transformations.</param>
		/// <returns>If the enumeration is null or empty, the return value is null. If the enumeration contains only one
		/// element, the return value is that element. If the enumeration contains multiple elements, the return value is a compound transformation
		/// with all elements.</returns>
		public static IVariantToVariantTransformation TryGetCompoundTransformation(IEnumerable<IVariantToVariantTransformation> transformations)
		{
			if (null == transformations)
				return null;

			var transformationList = new List<IVariantToVariantTransformation>(transformations);

			if (transformationList.Count == 0)
				return null;
			else if (transformationList.Count == 1)
				return transformationList[0];
			else
				return new CompoundTransformation(transformationList);
		}

		public AltaxoVariant Transform(AltaxoVariant value)
		{
			foreach (var item in _transformations)
				value = item.Transform(value);
			return value;
		}

		public string RepresentationAsFunction
		{
			get { return GetRepresentationAsFunction("x"); }
		}

		public string GetRepresentationAsFunction(string arg)
		{
			var x = arg;
			foreach (var item in _transformations)
				x = item.GetRepresentationAsFunction(x);
			return x;
		}

		public string RepresentationAsOperator
		{
			get
			{
				var stb = new System.Text.StringBuilder();
				for (int i = _transformations.Count - 1; i >= 0; --i)
				{
					stb.Append(_transformations[i].RepresentationAsOperator);
					if (i != 0)
						stb.Append(" ");
				}
				return stb.ToString();
			}
		}

		public IVariantToVariantTransformation BackTransformation
		{
			get
			{
				return new CompoundTransformation(GetTransformationsInReverseOrder().Select(transfo => transfo.BackTransformation));
			}
		}

		public CompoundTransformation WithPrependedTransformation(IVariantToVariantTransformation transformation)
		{
			if (null == transformation)
				throw new ArgumentNullException(nameof(transformation));

			var result = new CompoundTransformation();
			result._transformations = new List<IVariantToVariantTransformation>(this._transformations);
			if (transformation is CompoundTransformation)
			{
				result._transformations.AddRange(((CompoundTransformation)transformation)._transformations);
			}
			else
			{
				result._transformations.Add(transformation);
			}
			return result;
		}

		public CompoundTransformation WithAppendedTransformation(IVariantToVariantTransformation transformation)
		{
			if (null == transformation)
				throw new ArgumentNullException(nameof(transformation));

			var result = new CompoundTransformation();
			result._transformations = new List<IVariantToVariantTransformation>();
			if (transformation is CompoundTransformation)
			{
				result._transformations.AddRange(((CompoundTransformation)transformation)._transformations);
			}
			else
			{
				result._transformations.Add(transformation);
			}

			result._transformations.AddRange(this._transformations);
			return result;
		}

		private IEnumerable<IVariantToVariantTransformation> GetTransformationsInReverseOrder()
		{
			for (int i = _transformations.Count - 1; i >= 0; --i)
				yield return _transformations[i];
		}

		public override bool Equals(object obj)
		{
			var from = obj as CompoundTransformation;
			if (null == from)
				return false;

			if (this._transformations.Count != from._transformations.Count)
				return false;

			for (int i = 0; i < _transformations.Count; ++i)
				if (!_transformations[i].Equals(from._transformations[i]))
					return false;

			return true;
		}

		public override int GetHashCode()
		{
			int len = Math.Min(3, _transformations.Count);
			int result = this.GetType().GetHashCode();
			for (int i = 0; i < len; ++i)
				result += _transformations[i].GetHashCode();

			return result;
		}

		public bool IsEditable { get { return true; } }
	}
}