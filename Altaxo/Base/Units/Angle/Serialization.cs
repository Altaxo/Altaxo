#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Units.Angle
{
	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Pi), 0)]
	public class SerializationSurrogate0_Pi : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Pi.Instance;
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Degree), 0)]
	public class SerializationSurrogate0_Degree : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Degree.Instance;
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Radian), 0)]
	public class SerializationSurrogate0_Radian : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Radian.Instance;
		}
	}
}