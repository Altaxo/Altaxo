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

using Altaxo;
using Altaxo.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Data
{
	/// <summary>
	/// Summary description for Altaxo.Data.TextColumn.
	/// </summary>
	[SerializationSurrogate(0, typeof(Altaxo.Data.TextColumn.SerializationSurrogate0))]
	[SerializationVersion(0)]
	[Serializable]
	public class TextColumn
		:
		Altaxo.Data.DataColumn,
		System.Runtime.Serialization.ISerializable,
		System.Runtime.Serialization.IDeserializationCallback
	{
		private string[] _data;
		private int _capacity; // shortcout to m_Array.Length;
		private int _count;
		public static readonly string NullValue = null;

		public TextColumn()
		{
		}

		public TextColumn(int initialcapacity)
		{
			_count = 0;
			_data = new string[initialcapacity];
			_capacity = initialcapacity;
		}

		public TextColumn(TextColumn from)
		{
			this._count = from._count;
			this._capacity = from._capacity;
			this._data = null == from._data ? null : (string[])from._data.Clone();
		}

		public override object Clone()
		{
			return new TextColumn(this);
		}

		#region "Serialization"

		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				Altaxo.Data.TextColumn s = (Altaxo.Data.TextColumn)obj;
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if (null != ss)
				{
					System.Runtime.Serialization.ISerializationSurrogate surr =
						ss.GetSurrogate(typeof(Altaxo.Data.DataColumn), context, out ss);

					surr.GetObjectData(obj, info, context); // stream the data of the base object
				}
				else
				{
					((DataColumn)s).GetObjectData(info, context);
				}

				if (s._count != s._capacity)
				{
					// instead of the data array itself, stream only the first m_Count
					// array elements, since only they contain data
					string[] streamarray = new string[s._count];
					System.Array.Copy(s._data, streamarray, s._count);
					info.AddValue("Data", streamarray);
				}
				else // if the array is fully filled, we don't need to save a shrinked copy
				{
					info.AddValue("Data", s._data);
				}
			}

			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.TextColumn s = (Altaxo.Data.TextColumn)obj;
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if (null != ss)
				{
					System.Runtime.Serialization.ISerializationSurrogate surr =
						ss.GetSurrogate(typeof(Altaxo.Data.DataColumn), context, out ss);
					surr.SetObjectData(obj, info, context, selector);
				}
				else
				{
					((DataColumn)s).SetObjectData(obj, info, context, selector);
				}
				s._data = (string[])(info.GetValue("Data", typeof(string[])));
				s._capacity = null == s._data ? 0 : s._data.Length;
				s._count = s._capacity;

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.TextColumn), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Altaxo.Data.TextColumn s = (Altaxo.Data.TextColumn)obj;
				// serialize the base class
				info.AddBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn));

				if (null == info.GetProperty("Altaxo.Data.DataColumn.SaveAsTemplate"))
					info.AddArray("Data", s._data, s._count);
				else
					info.AddArray("Data", s._data, 0);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				Altaxo.Data.TextColumn s = null != o ? (Altaxo.Data.TextColumn)o : new Altaxo.Data.TextColumn();

				// deserialize the base class
				info.GetBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn), parent);

				int count = info.GetInt32Attribute("Count");
				s._data = new string[count];
				info.GetArray(s._data, count);
				s._capacity = null == s._data ? 0 : s._data.Length;
				s._count = s._capacity;

				return s;
			}
		}

		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}

		protected TextColumn(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this, info, context, null);
		}

		public new object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
		{
			return new SerializationSurrogate0().SetObjectData(this, info, context, null);
		}

		public new void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			new SerializationSurrogate0().GetObjectData(this, info, context);
		}

		#endregion "Serialization"

		public override int Count
		{
			get
			{
				return _count;
			}
		}

		public string[] Array
		{
			get
			{
				int len = this.Count;
				string[] arr = new string[len];
				System.Array.Copy(_data, 0, arr, 0, len);
				return arr;
			}

			set
			{
				_data = (string[])value.Clone();
				this._count = _data.Length;
				this._capacity = _data.Length;
				this.EhSelfChanged(0, _count, true);
			}
		}

		protected internal string GetValueDirect(int idx)
		{
			return _data[idx];
		}

		public override System.Type GetColumnStyleType()
		{
			return typeof(Altaxo.Worksheet.TextColumnStyle);
		}

		public override void CopyDataFrom(object o)
		{
			var oldCount = _count;
			_count = 0;

			if (o is TextColumn)
			{
				var src = (TextColumn)o;
				_data = null == src._data ? null : (string[])src._data.Clone();
				_capacity = _data.Length;
				_count = src._count;
			}
			else
			{
				if (o is ICollection)
					Realloc((o as ICollection).Count); // Prealloc the array if count of the collection is known beforehand

				if (o is IEnumerable<string>)
				{
					var src = (IEnumerable<string>)o;
					_count = 0;
					foreach (var it in src)
					{
						if (_count >= _capacity)
							Realloc(_count);
						_data[_count++] = it;
					}
				}
				else if (o is IEnumerable)
				{
					var src = (IEnumerable)o;
					_count = 0;
					foreach (var it in src)
					{
						if (_count >= _capacity)
							Realloc(_count);
						_data[_count++] = it.ToString();
					}
				}
				else
				{
					_count = 0;
					if (o == null)
						throw new ArgumentNullException("o");
					else
						throw new ArgumentException("Try to copy " + o.GetType() + " to " + this.GetType(), "o"); // throw exception
				}

				TrimEmptyElementsAtEnd();
			}

			if (oldCount > 0 || _count > 0) // message only if really was a change
				EhSelfChanged(0, oldCount > _count ? (oldCount) : (_count), _count < oldCount);
		}

		private void TrimEmptyElementsAtEnd()
		{
			for (; _count > 0 && _data[_count - 1] != null; _count--) ;
		}

		protected void Realloc(int i)
		{
			int newcapacity1 = (int)(_capacity * _increaseFactor + _addSpace);
			int newcapacity2 = i + _addSpace + 1;
			int newcapacity = newcapacity1 > newcapacity2 ? newcapacity1 : newcapacity2;

			string[] newarray = new string[newcapacity];
			if (_count > 0)
			{
				System.Array.Copy(_data, newarray, _count);
			}

			_data = newarray;
			_capacity = _data.Length;
		}

		// indexers
		public override void SetValueAt(int i, AltaxoVariant val)
		{
			if (val.IsTypeOrNull(AltaxoVariant.Content.VString))
				this[i] = (string)val;
			else
				this[i] = val.ToString();
			// throw new ApplicationException("Error: Try to set " + this.TypeAndName + "[" + i + "] with " + val.ToString());
		}

		public override AltaxoVariant GetVariantAt(int i)
		{
			return new AltaxoVariant(this[i]);
		}

		public override bool IsElementEmpty(int i)
		{
			return i < _count ? (null == _data[i]) : true;
		}

		public override void SetElementEmpty(int i)
		{
			if (i < _count)
				this[i] = NullValue;
		}

		public new string this[int i]
		{
			get
			{
				if (i >= 0 && i < _count)
					return _data[i];
				return "";
			}
			set
			{
				bool bCountDecreased = false;

				if (i < 0)
					throw new ArgumentOutOfRangeException(string.Format("Index<0 (i={0}) while trying to set element of column {1} ({2})", i, Name, FullName));

				if (value == null)
				{
					if (i < _count - 1) // i is inside the used range
					{
						_data[i] = value;
					}
					else if (i == (_count - 1)) // m_Count is then decreasing
					{
						for (_count = i; _count > 0 && (null == _data[_count - 1]); --_count) ;
						bCountDecreased = true; ;
					}
					else // i is above the used area
					{
						return; // no need for a change notification here
					}
				}
				else // value is not empty
				{
					if (i < _count) // i is inside the used range
					{
						_data[i] = value;
					}
					else if (i == _count && i < _capacity) // i is the next value after the used range
					{
						_data[i] = value;
						_count = i + 1;
					}
					else if (i > _count && i < _capacity) // is is outside used range, but inside capacity of array
					{
						for (int k = _count; k < i; k++)
							_data[k] = null; // fill range between used range and new element with voids

						_data[i] = value;
						_count = i + 1;
					}
					else // i is outside of capacity, then realloc the array
					{
						Realloc(i);

						for (int k = _count; k < i; k++)
							_data[k] = null; // fill range between used range and new element with voids

						_data[i] = value;
						_count = i + 1;
					}
				}
				EhSelfChanged(i, i + 1, bCountDecreased);
			} // end set
		} // end indexer

		public override void InsertRows(int nInsBeforeColumn, int nInsCount)
		{
			if (nInsCount <= 0 || nInsBeforeColumn >= Count)
				return; // nothing to do

			int newlen = this._count + nInsCount;
			if (newlen > _capacity)
				Realloc(newlen);

			// copy values from m_Count downto nBeforeColumn
			for (int i = _count - 1, j = newlen - 1; i >= nInsBeforeColumn; i--, j--)
				_data[j] = _data[i];

			for (int i = nInsBeforeColumn + nInsCount - 1; i >= nInsBeforeColumn; i--)
				_data[i] = NullValue;

			this._count = newlen;
			this.EhSelfChanged(nInsBeforeColumn, _count, false);
		}

		public override void RemoveRows(int nDelFirstRow, int nDelCount)
		{
			if (nDelFirstRow < 0)
				throw new ArgumentException("Row number must be greater or equal 0, but was " + nDelFirstRow.ToString(), "nDelFirstRow");

			if (nDelCount <= 0)
				return; // nothing to do here, but we dont catch it

			// we must be careful, since the range to delete can be
			// above the range this column actually holds, but
			// we must handle this the right way
			int i, j;
			for (i = nDelFirstRow, j = nDelFirstRow + nDelCount; j < _count; i++, j++)
				_data[i] = _data[j];

			int prevCount = _count;
			_count = i < _count ? i : _count; // m_Count can only decrease

			if (_count != prevCount) // raise a event only if something really changed
				this.EhSelfChanged(nDelFirstRow, prevCount, true);
		}

		#region "Operators"

		// -----------------------------------------------------------------------------
		//
		//                        Operators
		//
		// -----------------------------------------------------------------------------

		// ----------------------- Addition operator -----------------------------------
		public static Altaxo.Data.TextColumn operator +(Altaxo.Data.TextColumn c1, Altaxo.Data.TextColumn c2)
		{
			int len = c1.Count < c2.Count ? c1.Count : c2.Count;
			Altaxo.Data.TextColumn c3 = new Altaxo.Data.TextColumn(len);
			for (int i = 0; i < len; i++)
			{
				c3._data[i] = c1._data[i] + c2._data[i];
			}
			c3._count = len;
			return c3;
		}

		public static Altaxo.Data.TextColumn operator +(Altaxo.Data.TextColumn c1, Altaxo.Data.DoubleColumn c2)
		{
			int len = c1.Count < c2.Count ? c1.Count : c2.Count;
			Altaxo.Data.TextColumn c3 = new Altaxo.Data.TextColumn(len);
			for (int i = 0; i < len; i++)
			{
				c3._data[i] = c1._data[i] + c2.GetValueDirect(i).ToString();
			}

			c3._count = len;

			return c3;
		}

		public static Altaxo.Data.TextColumn operator +(Altaxo.Data.TextColumn c1, Altaxo.Data.DateTimeColumn c2)
		{
			int len = c1.Count < c2.Count ? c1.Count : c2.Count;
			Altaxo.Data.TextColumn c3 = new Altaxo.Data.TextColumn(len);
			for (int i = 0; i < len; i++)
			{
				c3._data[i] = c1._data[i] + c2.GetValueDirect(i).ToString();
			}

			c3._count = len;

			return c3;
		}

		#endregion "Operators"
	} // end Altaxo.Data.TextColumn
}