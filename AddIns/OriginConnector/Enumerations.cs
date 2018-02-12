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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Addins.OriginConnector
{
	public enum OriginObjectType
	{
		None = 0, // Name is not defined.
		Dataset = 1,
		Worksheet = 2,
		GraphWindow = 3,
		NumericVariable = 4,
		Matrix = 5,
		Tool = 7,
		Macro = 8,
		NotesWindow = 9,
	}

	public enum OriginColumnType
	{
		Y = 1,
		Disregard = 2,
		YError = 3,
		X = 4,
		Label = 5,
		Z = 6,
		XError = 7
	}
}
