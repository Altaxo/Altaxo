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
using System.Linq;
using System.Text;

namespace Altaxo
{
	/// <summary>
	/// Designates a resource key that will be used to retrieve a resource string. This struct is immutable. Store instances of this struct in static fields. This will help external programs to prove that for all resource keys there exists corresponding entries.
	/// </summary>
	public struct StringResourceKey
	{
		readonly string _key;
		readonly string _exampleValue;
		readonly string _description;

		/// <summary>
		/// Initializes a new instance of the <see cref="StringResourceKey"/> struct.
		/// </summary>
		/// <param name="key">The resource key.</param>
		/// <param name="exampleValue">An example of the resource string (always in the english language).</param>
		/// <param name="description">The description of the resource string to help translating it into other languages.</param>
		public StringResourceKey(string key, string exampleValue, string description)
		{
			_key = key;
			_exampleValue = exampleValue;
			_description = description;
		}

		/// <summary>
		/// Gets the resource key.
		/// </summary>
		public string Key { get { return _key; } }

		/// <summary>
		/// Gets an example of the resource string value (always in english).
		/// </summary>
		public string ExampleStringValue { get { return _exampleValue; } }

		/// <summary>
		/// Gets the description of the resource string to help translating it into other languages.
		/// </summary>
		public string Description { get { return _description; } }


		public override string ToString()
		{
			return this.Key;
		}
	}



}
