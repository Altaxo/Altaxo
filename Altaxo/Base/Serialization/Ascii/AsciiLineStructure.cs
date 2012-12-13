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
using System.Text;
using System.Collections.Generic;

namespace Altaxo.Serialization.Ascii
{
	public enum AsciiColumnType
	{
		DBNull,
		Int64,
		Double,
		DateTime,
		Text
	}


	/// <summary>
	/// Represents the structure of one single line of imported ascii text.
	/// </summary>
	public class AsciiLineStructure : IList<AsciiColumnType>
	{
		/// <summary>
		/// The structure of the line. This list holds <see cref="System.Type" /> values that represent the recognized items in the line.
		/// </summary>
		protected List<AsciiColumnType> _recognizedTypes = new List<AsciiColumnType>();
	


		/// <summary>
		/// If true, the cached data in this class are invalid and needs to be recalculated. 
		/// </summary>
		protected bool _isCachedDataInvalid = true;
		protected int _priorityValue;
		protected int _hashValue;



		/// <summary>
		/// Number of recognized items in the line.
		/// </summary>
		public int Count
		{
			get
			{
				return _recognizedTypes.Count;
			}
		}

		/// <summary>
		/// Adds a recognized item.
		/// </summary>
		/// <param name="o">The recognized item represented by its type, i.e. typeof(double) represents a recognized double number.</param>
		public void Add(AsciiColumnType o)
		{
			_recognizedTypes.Add(o);
			_isCachedDataInvalid = true;
		}

		/// <summary>
		/// Getter / setter of the items of the line.
		/// </summary>
		public AsciiColumnType this[int i]
		{
			get
			{
				return _recognizedTypes[i];
			}
			set
			{
				_recognizedTypes[i] = value;
				_isCachedDataInvalid = true;
			}
		}

	

	

		public int LineStructureScoring
		{
			get
			{
				if (_isCachedDataInvalid)
					CalculateCachedData();
				return _priorityValue;
			}
		}

		protected void CalculateCachedData()
		{
			_isCachedDataInvalid = false;

			// Calculate priority and hash

			int len = Count;
			var stb = new StringBuilder(); // for calculating the hash
			stb.Append(len.ToString());

			_priorityValue = 0;
			for (int i = 0; i < len; i++)
			{
				var t = this[i];
				if (t == AsciiColumnType.DateTime)
				{
					_priorityValue += 15;
					stb.Append('T');
				}
				else if (t == AsciiColumnType.Double)
				{
					_priorityValue += 7;
					stb.Append('D');
				}
				else if (t == AsciiColumnType.Int64)
				{
					_priorityValue += 3;
					stb.Append('D'); // note that it shoud have the same marker than Double, since a column can contain both integer and noninteger numeric data
				}
				else if (t == AsciiColumnType.Text)
				{
					_priorityValue += 2;
					stb.Append('S');
				}
				else if (t == AsciiColumnType.DBNull)
				{
					_priorityValue += 1;
					stb.Append('N');
				}
			} // for

			// calculate hash
			_hashValue = stb.ToString().GetHashCode();
		}



		public override int GetHashCode()
		{
			if (_isCachedDataInvalid)
				CalculateCachedData();
			return _hashValue;
		}
		public bool IsCompatibleWith(AsciiLineStructure ano)
		{
			// our structure can have more columns, but not lesser than ano
			if (this.Count < ano.Count)
				return false;

			for (int i = 0; i < ano.Count; i++)
			{
				if (this[i] == AsciiColumnType.DBNull || ano[i] == AsciiColumnType.DBNull)
					continue;
				if (this[i] == AsciiColumnType.Int64 && ano[i] == AsciiColumnType.Double)
					continue;
				if (this[i] == AsciiColumnType.Double && ano[i] == AsciiColumnType.Int64)
					continue;
				if (this[i] != ano[i])
					return false;
			}
			return true;
		}



		static char ShortFormOfType(AsciiColumnType type)
		{
			switch(type)
			{
				case AsciiColumnType.Double:
					return 'D';
				case AsciiColumnType.Text:
					return 'S';
				case AsciiColumnType.DateTime:
					return 'T';
				case AsciiColumnType.DBNull:
					return 'N';
				case AsciiColumnType.Int64:
					return 'I';
				default:
					throw new ArgumentOutOfRangeException("Option not considered: " + type.ToString());
			}
		}


		public override string ToString()
		{
			var stb = new StringBuilder();

			stb.AppendFormat("C={0} H={1:X8}", Count, GetHashCode());
			for (int i = 0; i < Count; i++)
			{
				stb.Append(' ');
				stb.Append(ShortFormOfType(this[i]));
			}
			return stb.ToString();
		}



		public int IndexOf(AsciiColumnType item)
		{
			return _recognizedTypes.IndexOf(item);
		}

		public void Insert(int index, AsciiColumnType item)
		{
			_recognizedTypes.Insert(index, item);
			_isCachedDataInvalid = true;
		}

		public void RemoveAt(int index)
		{
			_recognizedTypes.RemoveAt(index);
			_isCachedDataInvalid = true;
		}


		public void Clear()
		{
			_recognizedTypes.Clear();
			_isCachedDataInvalid = true;
		}

		public bool Contains(AsciiColumnType item)
		{
			return _recognizedTypes.Contains(item);
		}

		public void CopyTo(AsciiColumnType[] array, int arrayIndex)
		{
			_recognizedTypes.CopyTo(array, arrayIndex);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(AsciiColumnType item)
		{
			var r = _recognizedTypes.Remove(item);
			if (r) _isCachedDataInvalid = true;
			return r;
		}

		public IEnumerator<AsciiColumnType> GetEnumerator()
		{
			return _recognizedTypes.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _recognizedTypes.GetEnumerator();
		}
	} // end class AsciiLineStructure


	/// <summary>
	/// Helper structure to count how many lines have the same structure.
	/// </summary>
	public struct NumberAndStructure
	{
		/// <summary>Number of lines that have the same structure.</summary>
		public int NumberOfLines;

		/// <summary>The structure that these lines have.</summary>
		public AsciiLineStructure LineStructure;
	} // end class

}
