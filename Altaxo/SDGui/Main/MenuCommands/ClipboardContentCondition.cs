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

using ICSharpCode.Core;
using System;

namespace Altaxo.Main.Commands
{
	/// <summary>
	/// This helps in conditions where the number of selected data columns cares.
	/// Valid values are all (all columns must be selected, none (no column must be selected),
	/// one (exactly one column must be selected), any (one or more columns must be selected),
	/// or the number of columns.
	/// </summary>
	public class ClipboardContentConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string expectedcontent = condition.Properties["content"].ToLower();

			var dao = System.Windows.Forms.Clipboard.GetDataObject();
			string[] formats = dao.GetFormats();

			foreach (string format in formats)
				if (0 == string.Compare(expectedcontent, format, true))
					return true;

			return false;
		}
	}
}