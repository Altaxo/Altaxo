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
using System.Text;
using System.Linq;

namespace Altaxo.Serialization.Ascii
{
	/// <summary>
	/// Provides a method to separate tokens in a line of ascii text.
	/// </summary>
	public interface IAsciiSeparationStrategy : Main.ICopyFrom
	{
		/// <summary>
		/// For a given line of ascii text, this gives the separated tokens as an enumerable list of strings.
		/// </summary>
		/// <param name="line">The ascii text line (should be a single line, because most of the methods assume that no
		/// line feeds occur).</param>
		/// <returns>List of separated strings (tokens).</returns>
		IEnumerable<string> GetTokens(string line);
	}

	/// <summary>
	/// This separation strategy returns the entire line as a single token. This strategy can be used for instance when you have text containing spaces, that should be imported in one single text column.
	/// </summary>
	public class SingleLineSeparationStrategy : IAsciiSeparationStrategy
	{

		public SingleLineSeparationStrategy()
		{
		}

		public IEnumerable<string> GetTokens(string line)
		{
			yield return line;
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as SingleLineSeparationStrategy;
			if (null != from)
			{
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return new SingleLineSeparationStrategy();
		}
	}

	/// <summary>
	/// This strategy assumes that the tokens are separated by exactly one (!) separation char. The separation character has to
	/// be provided in the constructor.
	/// </summary>
	public class SingleCharSeparationStrategy : IAsciiSeparationStrategy
	{
		char _separatorChar;

		/// <summary>
		/// Constructor for this strategy. You must provide a separator char.
		/// </summary>
		/// <param name="separator">The separator char used.</param>
		public SingleCharSeparationStrategy(char separator)
		{
			_separatorChar = separator;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SingleCharSeparationStrategy"/> class with tabulator as the separator char.
		/// </summary>
		public SingleCharSeparationStrategy()
			: this('\t')
		{
		}

		public char SeparatorChar { get { return _separatorChar; } set { _separatorChar = value; } }

		public IEnumerable<string> GetTokens(string line)
		{

			int len = line.Length;
			int ix = 0;
			for (int start = 0; start <= len; start = ix + 1)
			{
				ix = line.IndexOf(_separatorChar, start, len - start);
				if (ix == -1)
				{
					ix = len;
				}
				yield return line.Substring(start, ix - start);
			}
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as SingleCharSeparationStrategy;
			if (null != from)
			{
				this._separatorChar = from._separatorChar;
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return new SingleCharSeparationStrategy(this._separatorChar);
		}
	}


	/// <summary>
	/// This strategy assumes that the tokens fill the string at fixed positions in the string and have a fixed length.
	/// The starting position of the first token is always zero. The starting positions of each subsequent token (beginning with the second token) has to be provided in the constructor.
	/// </summary>
	public class FixedColumnWidthWithoutTabSeparationStrategy : IAsciiSeparationStrategy
	{
		int[] _startPositions;

		/// <summary>
		/// Initializes a new instance of the <see cref="FixedColumnWidthWithoutTabSeparationStrategy"/> class with an empty list of start positions.
		/// </summary>
		public FixedColumnWidthWithoutTabSeparationStrategy()
			: this(new int[] { })
		{
		}

		/// <summary>
		/// Constructor for this strategy. You must provide the start positions of the tokens. The first token implicitely has position 0.
		/// </summary>
		/// <param name="startPositions">List of starting positions.</param>
		public FixedColumnWidthWithoutTabSeparationStrategy(IList<int> startPositions)
		{
			_startPositions = startPositions.ToArray();
		}

		public int[] StartPositions
		{
			get
			{
				return _startPositions;
			}
			set
			{
				_startPositions = value;
			}
		}

		public IEnumerable<string> GetTokens(string line)
		{
			int len = line.Length;
			int stringPos = 0;
			for (int i = 0; i < _startPositions.Length; i++)
			{
				int startStringPos = stringPos;
				stringPos = _startPositions[i];
				if (stringPos > len)
				{
					yield return line.Substring(startStringPos, len - startStringPos);
					break;
				}
				else
				{
					yield return line.Substring(startStringPos, stringPos - startStringPos);
				}
			}
		}


		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as FixedColumnWidthWithoutTabSeparationStrategy;
			if (null != from)
			{
				this._startPositions = (int[])from._startPositions.Clone();
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return new FixedColumnWidthWithoutTabSeparationStrategy(this._startPositions);
		}
	}


	/// <summary>
	/// This strategy assumes that the tokens fill the printout (!) at fixed positions and have a fixed length.
	/// For the printout position, we have to assume a certain tabulator with. Each tabulator char in the string advances the printout position by a certain amount depending on the current printout
	/// position.
	/// The starting printout position of the first token is always zero. The starting printout positions of each subsequent token (beginning with the second token) has to be provided in the constructor.
	/// </summary>
	/// <remarks>For a tab width of 1, this strategy is identical to the <see cref="FixedColumnWidthWithoutTabSeparationStrategy" />.</remarks>
	public class FixedColumnWidthWithTabSeparationStrategy : IAsciiSeparationStrategy
	{
		int[] _startPositions;
		int _tabSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="FixedColumnWidthWithTabSeparationStrategy"/> class with a TabSize of 8 and an empty list of start positions.
		/// </summary>
		public FixedColumnWidthWithTabSeparationStrategy()
			: this(new int[] { }, 8)
		{
		}

		/// <summary>
		/// Constructor for this strategy. You must provide the start positions of the tokens. The first token implicitely has position 0.
		/// Furthermore you must provide a tab size that is used to calculate the tabbed positions.
		/// </summary>
		/// <param name="startPositions">List of starting tabbed positions.</param>
		/// <param name="tabSize">Size of the tabulator (i.e. how many spaces at maximum substitutes one tabulator)</param>
		public FixedColumnWidthWithTabSeparationStrategy(IList<int> startPositions, int tabSize)
		{
			if (tabSize < 1)
				throw new ArgumentOutOfRangeException("TabSize have to be >=1");

			_startPositions = startPositions.ToArray();
			_tabSize = tabSize;
		}

		public int TabSize
		{
			get { return _tabSize; }
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("TabSize have to be >=1");
				_tabSize = value;
			}
		}

		public int[] StartPositions
		{
			get
			{
				return _startPositions;
			}
			set
			{
				_startPositions = value;
			}
		}

		public IEnumerable<string> GetTokens(string line)
		{
			int len = line.Length;
			int stringPos = 0;
			int tabbedPos = 0;

			for (int i = 0; i < _startPositions.Length; i++)
			{
				int startStringPos = stringPos;
				int tabbedend = _startPositions[i];

				// now we have to look for the string position corresponding to the tabbedend
				for (; (tabbedPos < tabbedend) && (stringPos < len); stringPos++)
				{
					if (line[stringPos] == '\t')
						tabbedPos += _tabSize - (tabbedPos % _tabSize);
					else
						tabbedPos++;
				}

				yield return line.Substring(startStringPos, stringPos - startStringPos);
			}
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as FixedColumnWidthWithTabSeparationStrategy;
			if (null != from)
			{
				this._startPositions = (int[])from._startPositions.Clone();
				this._tabSize = from._tabSize;
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return new FixedColumnWidthWithTabSeparationStrategy(this._startPositions, this._tabSize);
		}
	}

	/// <summary>
	/// This stategy assumes that the tokens are separated by one or more whitespace chars (tabs and spaces).
	/// </summary>
	public class SkipWhiteSpaceSeparationStrategy : IAsciiSeparationStrategy
	{
		static char[] separators = { ' ', '\t' };


		/// <summary>
		/// Initializes a new instance of the <see cref="SkipWhiteSpaceSeparationStrategy"/> class.
		/// </summary>
		public SkipWhiteSpaceSeparationStrategy()
		{
		}

		public IEnumerable<string> GetTokens(string line)
		{
			return line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as SkipWhiteSpaceSeparationStrategy;
			if (null != from)
			{
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return new SkipWhiteSpaceSeparationStrategy();
		}

	}

}
