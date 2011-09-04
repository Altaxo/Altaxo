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

using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
	public class SuppressedTicks : ICloneable
	{
		List<AltaxoVariant> _suppressedTickValues;
		List<int> _suppressedTicksByNumber;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SuppressedTicks), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SuppressedTicks s = (SuppressedTicks)obj;

				info.CreateArray("ByValues", s._suppressedTickValues.Count);
				foreach (AltaxoVariant v in s._suppressedTickValues)
					info.AddValue("e", (object)v);
				info.CommitArray();


				info.CreateArray("ByNumbers", s._suppressedTicksByNumber.Count);
				foreach (int v in s._suppressedTicksByNumber)
					info.AddValue("e", v);
				info.CommitArray();

			

			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				SuppressedTicks s = SDeserialize(o, info, parent);
				return s;
			}


			protected virtual SuppressedTicks SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				SuppressedTicks s = null != o ? (SuppressedTicks)o : new SuppressedTicks();

				int count;

				count = info.OpenArray("ByValues");
				for (int i = 0; i < count; i++)
					s._suppressedTickValues.Add((AltaxoVariant)info.GetValue("e", s));
				info.CloseArray(count);

				count = info.OpenArray("ByNumbers");
				for (int i = 0; i < count; i++)
					s._suppressedTicksByNumber.Add(info.GetInt32("e"));
				info.CloseArray(count);

			

				return s;
			}
		}
		#endregion

		public SuppressedTicks()
		{
			_suppressedTickValues = new List<AltaxoVariant>();
			_suppressedTicksByNumber = new List<int>();
		}

		public SuppressedTicks(SuppressedTicks from)
			: this()
		{
			CopyFrom(from);
		}

		public void CopyFrom(SuppressedTicks from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			_suppressedTickValues.Clear();
			_suppressedTicksByNumber.Clear();

			_suppressedTickValues.AddRange(from._suppressedTickValues);
			_suppressedTicksByNumber.AddRange(from._suppressedTicksByNumber);
		}

		public object Clone()
		{
			return new SuppressedTicks(this);
		}

    public bool IsEmpty
    {
      get
      {
        return _suppressedTickValues.Count == 0 && _suppressedTicksByNumber.Count == 0;
      }
    }

		public IList<AltaxoVariant> ByValues
		{
			get
			{
				return _suppressedTickValues;
			}
		}

		public IList<int> ByNumbers
		{
			get
			{
				return _suppressedTicksByNumber;
			}
		}
	}
}
