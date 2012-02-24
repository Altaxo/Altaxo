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

namespace Altaxo.Main.Services.PropertyReflection
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// <para>This class originated from the 'WPG Property Grid' project (<see href="http://wpg.codeplex.com"/>), licensed under Ms-PL.</para>
	/// </remarks>
	public class PropertyCategory : PropertyCollection
	{
		#region Initialization

		public PropertyCategory()
		{
			this._categoryName = "Misc";
		}

		public PropertyCategory(string categoryName)
		{
			this._categoryName = categoryName;
		}

		public string Category
		{
			get { return _categoryName; }
		}

		#endregion

		#region Fields

		private readonly string _categoryName;

		#endregion
	}
}
