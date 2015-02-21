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

#endregion Copyright

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	public class AdditionalTicks
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		Main.ICopyFrom
	{
		private List<AltaxoVariant> _additionalTicks;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AdditionalTicks), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AdditionalTicks s = (AdditionalTicks)obj;

				info.CreateArray("ByValues", s._additionalTicks.Count);
				foreach (AltaxoVariant v in s._additionalTicks)
					info.AddValue("e", (object)v);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AdditionalTicks s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual AdditionalTicks SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AdditionalTicks s = null != o ? (AdditionalTicks)o : new AdditionalTicks();

				int count;

				count = info.OpenArray("ByValues");
				for (int i = 0; i < count; i++)
					s._additionalTicks.Add((AltaxoVariant)info.GetValue("e", s));
				info.CloseArray(count);

				return s;
			}
		}

		#endregion Serialization

		public AdditionalTicks()
		{
			_additionalTicks = new List<AltaxoVariant>();
		}

		public AdditionalTicks(AdditionalTicks from)
		{
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as AdditionalTicks;
			if (null == from)
				return false;

			_additionalTicks = new List<AltaxoVariant>(from._additionalTicks);
			EhSelfChanged();

			return true;
		}

		public object Clone()
		{
			return new AdditionalTicks(this);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			else if (!(obj is AdditionalTicks))
				return false;
			else
			{
				var from = (AdditionalTicks)obj;
				return _additionalTicks.SequenceEqual(from._additionalTicks);
			}
		}

		public override int GetHashCode()
		{
			return _additionalTicks.GetHashCode();
		}

		public bool IsEmpty
		{
			get
			{
				return _additionalTicks.Count == 0;
			}
		}

		public AltaxoVariant this[int idx]
		{
			get
			{
				return _additionalTicks[idx];
			}
			set
			{
				var oldValue = _additionalTicks[idx];
				_additionalTicks[idx] = value;
				if (value != oldValue)
					EhSelfChanged();
			}
		}

		public void Clear()
		{
			var oldCount = _additionalTicks.Count;
			_additionalTicks.Clear();
			if (0 != oldCount)
				EhSelfChanged();
		}

		public void Add(AltaxoVariant additionalTick)
		{
			_additionalTicks.Add(additionalTick);
			EhSelfChanged();
		}

		public IEnumerable<AltaxoVariant> Values
		{
			get
			{
				return _additionalTicks;
			}
		}
	}
}