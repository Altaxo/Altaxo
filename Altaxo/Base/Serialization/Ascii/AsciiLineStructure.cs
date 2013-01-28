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
		/// <summary>Number in any form with exception of NumberStyle.Integer and NumberStyle.Float.</summary>
		AnyNumber,
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
			foreach (var colType in _recognizedTypes)
			{
				switch(colType)
				{
					case AsciiColumnType.DateTime:
					_priorityValue += 15;
					stb.Append('T');
						break;
					case AsciiColumnType.Double:
					_priorityValue += 7;
					stb.Append('D');
						break;
					case AsciiColumnType.Int64:
					_priorityValue += 3;
					stb.Append('D'); // note that it shoud have the same marker than Double, since a column can contain both integer and noninteger numeric data
						break;
					case AsciiColumnType.AnyNumber:
					_priorityValue += 3;
					stb.Append('D'); // note that it shoud have the same marker than Double, since a column can contain both integer and noninteger numeric data
						break;
					case AsciiColumnType.Text:
					_priorityValue += 2;
					stb.Append('S');
						break;
					case AsciiColumnType.DBNull:
					_priorityValue += 1;
					stb.Append('N');
						break;
					default:
						throw new ArgumentOutOfRangeException(string.Format("Unconsidered AsciiColumnType: {0}. Please report this error!", colType));
				} // switch
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


		/// <summary>
		/// Determines whether this line structure is is compatible with another line structure.
		/// </summary>
		/// <param name="ano">The other line structure to compare with.</param>
		/// <returns><c>True</c> if this line structure is compatible with the line structure specified in <paramref name="ano"/>; otherwise, <c>false</c>.
		/// It is compatible if the values of all columns of this line structure could be stored in the columns specified by the other line structure.
		/// </returns>
		public bool IsCompatibleWith(AsciiLineStructure ano)
		{
			// our structure can have more columns, but not lesser than ano
			if (this.Count < ano.Count)
				return false;

			for (int i = 0; i < ano.Count; i++)
			{
				if (!IsCompatibleWith(this[i], ano[i]))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the <see cref="AsciiColumnType"/> <paramref name="a"/> is compatible with <paramref name="b"/>.
		/// </summary>
		/// <param name="a">First column type.</param>
		/// <param name="b">Second column type.</param>
		/// <returns><c>True</c> if  <see cref="AsciiColumnType"/> <paramref name="a"/> is compatible with <paramref name="b"/>, i.e. values of type <paramref name="a"/> could be stored in columns of type <paramref name="b"/>; otherwise, <c>false</c>.</returns>
		/// <remarks>
		/// <para>The column type <see cref="AsciiColumnType.DBNull"/> is compatible to all other column types.</para>
		/// <para>Since all numeric types will be stored in Double format, all numeric column types are compatible with each other.</para>
		/// </remarks>
		public static bool IsCompatibleWith(AsciiColumnType a, AsciiColumnType b)
		{
			if (a == AsciiColumnType.DBNull || b == AsciiColumnType.DBNull)
				return true;

			if ((a == AsciiColumnType.AnyNumber || a == AsciiColumnType.Double || a == AsciiColumnType.Int64) &&
				(b == AsciiColumnType.AnyNumber || b == AsciiColumnType.Double || b == AsciiColumnType.Int64))
				return true;

			return a == b;
		}


		static char ShortFormOfType(AsciiColumnType type)
		{
			switch (type)
			{
				case AsciiColumnType.Double:
					return 'D';
				case AsciiColumnType.Text:
					return 'S';
				case AsciiColumnType.DateTime:
					return 'T';
				case AsciiColumnType.DBNull:
					return '_';
				case AsciiColumnType.Int64:
					return 'I';
				case AsciiColumnType.AnyNumber:
					return 'N';
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
