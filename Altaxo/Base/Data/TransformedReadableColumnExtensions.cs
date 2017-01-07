#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	public static class TransformedReadableColumnExtensions
	{
		/// <summary>
		/// Gets the underlying data column (of type <see cref="DataColumn"/>) or the default value null.
		/// </summary>
		/// <param name="c">The transformed column for which to search the underlying <see cref="DataColumn"/>.</param>
		/// <returns>The underlying data column (of type <see cref="DataColumn"/>) or the default value null.</returns>
		public static DataColumn GetUnderlyingDataColumnOrDefault(this IReadableColumn c)
		{
			if (c is DataColumn)
			{
				return (DataColumn)c;
			}
			else if (c is ITransformedReadableColumn it)
			{
				while (null != it)
				{
					if (it.UnderlyingReadableColumn is DataColumn)
						return (DataColumn)it.UnderlyingReadableColumn;
					else if (it.UnderlyingReadableColumn is ITransformedReadableColumn)
						it = (ITransformedReadableColumn)it.UnderlyingReadableColumn;
				}
			}
			return null;
		}
	}
}