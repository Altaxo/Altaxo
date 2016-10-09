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
	public class DecadicExponentialTransformation : ImmutableClassWithoutMembersBase, IVariantToVariantTransformation
	{
		public static DecadicExponentialTransformation Instance { get; private set; } = new DecadicExponentialTransformation();

		#region Serialization

		/// <summary>
		/// 2016-06-24 Initial version.
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DecadicExponentialTransformation), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return DecadicExponentialTransformation.Instance;
			}
		}

		#endregion Serialization

		/// <inheritdoc/>
		public Type InputValueType { get { return typeof(double); } }

		/// <inheritdoc/>
		public Type OutputValueType { get { return typeof(double); } }

		public AltaxoVariant Transform(AltaxoVariant value)
		{
			return Math.Pow(10, value);
		}

		public string RepresentationAsFunction
		{
			get { return GetRepresentationAsFunction("x"); }
		}

		public string GetRepresentationAsFunction(string arg)
		{
			return string.Format("10^({0})", arg);
		}

		public string RepresentationAsOperator
		{
			get { return "10^"; }
		}

		public IVariantToVariantTransformation BackTransformation
		{
			get { return DecadicLogarithmTransformation.Instance; }
		}
	}
}