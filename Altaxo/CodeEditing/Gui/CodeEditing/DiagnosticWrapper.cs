#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.CodeEditing
{
	public class DiagnosticWrapper
	{
		private Diagnostic _diagnostic;

		public DiagnosticWrapper(Diagnostic d)
		{
			_diagnostic = d;
		}

		public int Line
		{
			get
			{
				return _diagnostic.Location.GetLineSpan().StartLinePosition.Line;
			}
		}

		public int Column
		{
			get
			{
				return _diagnostic.Location.GetLineSpan().StartLinePosition.Character;
			}
		}

		public string Severity
		{
			get
			{
				switch (_diagnostic.Severity)
				{
					case DiagnosticSeverity.Hidden:

						return "Hidden";

					case DiagnosticSeverity.Info:
						return "Info";

					case DiagnosticSeverity.Warning:
						return "Warning";

					case DiagnosticSeverity.Error:
						return "Error";
				}
				return string.Empty;
			}
		}

		public string Message
		{
			get
			{
				return _diagnostic.GetMessage();
			}
		}
	}
}