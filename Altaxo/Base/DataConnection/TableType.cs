#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Original Copyright (C) Bernardo Castilho
//    Original Source: http://www.codeproject.com/Articles/43171/A-Visual-SQL-Query-Designer
//		Licence: The Code Project Open License (CPOL) 1.02, http://www.codeproject.com/info/cpol10.aspx
//
//    Modified 2014 by Dr. Dirk Lellinger for
//    Altaxo:  a data processing and data plotting program
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.DataConnection
{
  /// <summary>
  /// Represents types of data tables.
  /// </summary>
  public enum TableType
  {
    Table,
    View,
    Procedure
  }
}
