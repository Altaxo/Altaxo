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

namespace Altaxo.Graph.Scales.Rescaling
{
	public class AngularRescaleConditions : ICloneable, Altaxo.Main.IChangedEventSource
	{
		public event EventHandler Changed;

		/// <summary>Origin of the scale in degrees.</summary>
		protected int _scaleOrigin; 

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularRescaleConditions), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AngularRescaleConditions s = (AngularRescaleConditions)obj;

				info.AddValue("ScaleOrigin", s._scaleOrigin);

			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularRescaleConditions s = SDeserialize(o, info, parent);
				return s;
			}


			protected virtual AngularRescaleConditions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularRescaleConditions s = null != o ? (AngularRescaleConditions)o : new AngularRescaleConditions();

				s._scaleOrigin = info.GetInt32("ScaleOrigin");

				return s;
			}
		}
		#endregion

		public AngularRescaleConditions()
		{
		}

		public AngularRescaleConditions(AngularRescaleConditions from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(AngularRescaleConditions from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this._scaleOrigin = from._scaleOrigin;
		}



		public int ScaleOrigin
		{
			get
			{
				return _scaleOrigin;
			}
			set
			{
				_scaleOrigin = value;
			}
		}


	

		#region ICloneable Members

		public object Clone()
		{
			return new AngularRescaleConditions(this);
		}

		#endregion
	}
}
