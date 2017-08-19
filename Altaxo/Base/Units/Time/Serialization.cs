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

namespace Altaxo.Units.Time
{
	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Second), 0)]
	public class SerializationSurrogate0_Second : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Second.Instance;
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Minute), 0)]
	public class SerializationSurrogate0_Minute : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Minute.Instance;
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Hour), 0)]
	public class SerializationSurrogate0_Hour : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Hour.Instance;
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Day), 0)]
	public class SerializationSurrogate0_Day : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Day.Instance;
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Week), 0)]
	public class SerializationSurrogate0_Week : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			return Week.Instance;
		}
	}
}