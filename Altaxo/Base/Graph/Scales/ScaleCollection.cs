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
using System.Collections.Generic;

namespace Altaxo.Graph.Scales
{
	[Serializable]
	public class ScaleCollection
	:
	Main.SuspendableDocumentNodeWithSetOfEventArgs,
	Main.ICopyFrom,
	IEnumerable<Scale>
	{
		private Scale[] _scales;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.ScaleCollection", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				ScaleCollection s = (ScaleCollection)obj;

				info.CreateArray("Members", s._scales.Length);
				for (int i = 0; i < s._scales.Length; ++i)
					info.AddValue("e", s._scales[i]);
				info.CommitArray();
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScaleCollection s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual ScaleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScaleCollection s = null != o ? (ScaleCollection)o : new ScaleCollection(info);

				int count = info.OpenArray("Members");
				s._scales = new Scale[count];
				for (int i = 0; i < count; ++i)
				{
					info.OpenElement(); // e
					var scale = (Scale)info.GetValue("Scale", s);
					var tickspacing = (Ticks.TickSpacing)info.GetValue("TickSpacing", s);
					scale.TickSpacing = tickspacing;
					scale.ParentObject = s;
					s._scales[i] = scale;
					info.CloseElement();
				}
				info.CloseArray(count);

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaleCollection), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ScaleCollection s = (ScaleCollection)obj;

				info.CreateArray("Members", s._scales.Length);
				for (int i = 0; i < s._scales.Length; ++i)
					info.AddValue("e", s._scales[i]);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScaleCollection s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual ScaleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScaleCollection s = null != o ? (ScaleCollection)o : new ScaleCollection(info);

				int count = info.OpenArray("Members");
				s._scales = new Scale[count];
				for (int i = 0; i < count; ++i)
				{
					s._scales[i] = (Scale)info.GetValue("e", s);
					s._scales[i].ParentObject = s;
				}
				info.CloseArray(count);

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// For deserialization only: Initializes a new instance of the <see cref="ScaleCollection"/> class.
		/// </summary>
		/// <param name="info">The deserialization information.</param>
		protected ScaleCollection(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		public ScaleCollection()
			: this(2)
		{
		}

		public ScaleCollection(int numberOfScales)
		{
			if (numberOfScales <= 0)
				throw new ArgumentOutOfRangeException(nameof(numberOfScales) + " must be >= 1");

			_scales = new Scale[numberOfScales];
			for (int i = 0; i < numberOfScales; ++i)
				this[i] = new LinearScale();
		}

		public ScaleCollection(ScaleCollection from)
		{
			_scales = new Scale[from._scales.Length];
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as ScaleCollection;

			if (null == from)
				return false;

			using (var suspendToken = this.SuspendGetToken())
			{
				int len = Math.Min(this._scales.Length, from._scales.Length);
				for (int i = 0; i < len; i++)
				{
					this[i] = (Scale)from._scales[i].Clone();
				}

				suspendToken.Resume();
			}

			return true;
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _scales)
			{
				for (int i = _scales.Length - 1; i >= 0; --i)
				{
					if (null != _scales[i])
						yield return new Main.DocumentNodeAndName(_scales[i], "Scale" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				var scales = _scales;
				_scales = null;
				if (null != scales)
				{
					for (int i = 0; i < scales.Length; ++i)
					{
						if (null != scales[i])
							scales[i].Dispose();
					}
				}
			}

			base.Dispose(isDisposing);
		}

		object ICloneable.Clone()
		{
			return new ScaleCollection(this);
		}

		public ScaleCollection Clone()
		{
			return new ScaleCollection(this);
		}

		public int Count
		{
			get
			{
				return _scales.Length;
			}
		}

		public string GetName(int idx)
		{
			switch (idx)
			{
				case 0:
					return "X";

				case 1:
					return "Y";

				case 2:
					return "Z";

				default:
					return "Scale" + idx.ToString();
			}
		}

		public Scale X
		{
			get
			{
				return this[0];
			}
			set
			{
				this[0] = value;
			}
		}

		public Scale Y
		{
			get
			{
				return this[1];
			}
			set
			{
				this[1] = value;
			}
		}

		public Scale Z
		{
			get
			{
				return this[2];
			}
			set
			{
				this[2] = value;
			}
		}

		public Scale this[int i]
		{
			get
			{
				return _scales[i];
			}
			set
			{
				var oldScale = _scales[i];
				if (ChildSetMember(ref _scales[i], value))
					EhSelfChanged(new Altaxo.Graph.ScaleInstanceChangedEventArgs(oldScale, _scales[i]));
			}
		}

		public int IndexOf(Scale ax)
		{
			for (int i = 0; i < _scales.Length; i++)
			{
				if (object.ReferenceEquals(_scales[i], ax))
					return i;
			}

			return -1;
		}

		public IEnumerator<Scale> GetEnumerator()
		{
			for (int i = 0; i < _scales.Length; ++i)
				yield return _scales[i];
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _scales.Length; ++i)
				yield return _scales[i];
		}
	}
}