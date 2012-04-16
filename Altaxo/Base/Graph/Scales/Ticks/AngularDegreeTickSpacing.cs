#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	public class AngularDegreeTickSpacing : AngularTickSpacing
	{
			#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularDegreeTickSpacing), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, typeof(AngularTickSpacing));
				AngularDegreeTickSpacing s = (AngularDegreeTickSpacing)obj;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularDegreeTickSpacing s = SDeserialize(o, info, parent);
				OnAfterDeserialization(s);
				return s;
			}

			protected virtual void OnAfterDeserialization(AngularDegreeTickSpacing s)
			{
				
			}

			protected virtual AngularDegreeTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularDegreeTickSpacing s = null!=o ? (AngularDegreeTickSpacing)o : new AngularDegreeTickSpacing();
				info.GetBaseValueEmbedded(s, typeof(AngularTickSpacing), s);
				return s;
			}
		}
		#endregion


		public AngularDegreeTickSpacing()
		{
		}

		public AngularDegreeTickSpacing(AngularDegreeTickSpacing from)
			: base(from) // everything is done here, since CopyFrom is virtual!
		{
		}

		public override object Clone()
		{
			return new AngularDegreeTickSpacing(this);
		}

		public override bool UseDegree
		{
			get { return true; }
		}
	}
}
