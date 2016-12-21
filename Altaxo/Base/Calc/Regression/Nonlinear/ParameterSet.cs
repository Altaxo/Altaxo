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

using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
	public class ParameterSetElement : ICloneable
	{
		public string Name { get; set; }
		public double Parameter { get; set; }
		public double Variance { get; set; }
		public bool Vary { get; set; }

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSetElement), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ParameterSetElement s = (ParameterSetElement)obj;

				info.AddValue("Name", s.Name);
				info.AddValue("Value", s.Parameter);
				info.AddValue("Variance", s.Variance);
				info.AddValue("Vary", s.Vary);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var name = info.GetString("Name");
				var parameter = info.GetDouble("Value");
				var variance = info.GetDouble("Variance");
				var vary = info.GetBoolean("Vary");
				return new ParameterSetElement(name, parameter, variance, vary);
			}
		}

		#endregion Serialization

		/// <summary>
		/// For deserialization purposes only.
		/// </summary>
		protected ParameterSetElement()
		{
		}

		public ParameterSetElement(string name)
		{
			this.Name = name;
			this.Vary = true;
		}

		public ParameterSetElement(string name, double value)
		{
			this.Name = name;
			this.Parameter = value;
			this.Vary = true;
		}

		public ParameterSetElement(string name, double value, double variance, bool vary)
		{
			this.Name = name;
			this.Parameter = value;
			this.Variance = variance;
			this.Vary = vary;
		}

		public ParameterSetElement(ParameterSetElement from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(ParameterSetElement from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this.Name = from.Name;
			this.Parameter = from.Parameter;
			this.Variance = from.Variance;
			this.Vary = from.Vary;
		}

		#region ICloneable Members

		public object Clone()
		{
			return new ParameterSetElement(this);
		}

		#endregion ICloneable Members
	}

	/// <summary>
	/// Summary description for ParameterSet.
	/// </summary>
	public class ParameterSet : System.Collections.CollectionBase, ICloneable
	{
		/// <summary>
		/// Event is fired if the main initialization is finished. This event can be fired
		/// multiple times (every time the set has changed basically.
		/// </summary>
		public event EventHandler InitializationFinished;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSet), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ParameterSet s = (ParameterSet)obj;

				info.CreateArray("Parameters", s.Count);
				for (int i = 0; i < s.Count; ++i)
					info.AddValue("e", s[i]);
				info.CommitArray();
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ParameterSet s = o != null ? (ParameterSet)o : new ParameterSet();

				int arraycount = info.OpenArray();
				for (int i = 0; i < arraycount; ++i)
					s.Add((ParameterSetElement)info.GetValue("e", s));
				info.CloseArray(arraycount);

				return s;
			}
		}

		#endregion Serialization

		public ParameterSet()
		{
		}

		public void OnInitializationFinished()
		{
			if (null != InitializationFinished)
				InitializationFinished(this, EventArgs.Empty);
		}

		public ParameterSetElement this[int i]
		{
			get
			{
				return (ParameterSetElement)this.InnerList[i];
			}
		}

		public void Add(ParameterSetElement ele)
		{
			this.InnerList.Add(ele);
		}

		#region ICloneable Members

		public object Clone()
		{
			ParameterSet result = new ParameterSet();
			for (int i = 0; i < Count; ++i)
				result.Add((ParameterSetElement)this[i].Clone());

			return result;
		}

		#endregion ICloneable Members
	}
}