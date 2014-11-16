#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.Clipboard
{
	public class ProjectItemsPasteOptions : ICloneable
	{
		/// <summary>If true, references will be relocated in the same way as the project items will be relocated.</summary>
		/// <value><c>true</c> if references should be relocated, <c>false</c> otherwise</value>
		public bool? RelocateReferences { get; set; }

		/// <summary>
		/// When true, at serialization the internal references are tried to keep internal, i.e. if for instance a table have to be renamed, the plot items in the deserialized graphs
		/// will be relocated to the renamed table.
		/// </summary>
		public bool? TryToKeepInternalReferences { get; set; }

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}