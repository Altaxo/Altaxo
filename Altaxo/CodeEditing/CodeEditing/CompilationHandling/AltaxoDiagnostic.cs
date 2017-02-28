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

namespace Altaxo.CodeEditing.CompilationHandling
{
	/// <summary>
	/// Immutable class that wraps a <see cref="Microsoft.CodeAnalysis.Diagnostic"/> instance and provides properties
	/// that can be used to bind a Gui.
	/// </summary>
	public class AltaxoDiagnostic
	{
		/// <summary>
		/// Gets the wrapped diagnostic instance
		/// </summary>
		/// <value>
		/// The diagnostic instance.
		/// </value>
		public Diagnostic Diagnostic { get; private set; }

		/// <summary>Gets the number of the line (1-based, first line number is 1).</summary>
		public int? Line { get; private set; }

		/// <summary>Gets the number of the column (1-based, first column number is 1).</summary>
		public int? Column { get; private set; }

		/// <summary>Gets the caret position.</summary>
		public int? CaretPosition { get; private set; }

		public int Severity { get; private set; }

		/// <summary>Gets a severity string..</summary>
		public string SeverityText { get; private set; }

		/// <summary>Gets the diagnostic message.</summary>
		public string MessageText { get; private set; }

		public AltaxoDiagnostic(Diagnostic d)
		{
			Diagnostic = d;
			Line = d.Location.GetLineSpan().StartLinePosition.Line + 1;
			Column = d.Location.GetLineSpan().StartLinePosition.Character + 1;
			CaretPosition = d.Location.SourceSpan.Start;
			Severity = (int)d.Severity;
			SeverityText = d.Severity.ToString();
			MessageText = d.GetMessage();
		}

		protected AltaxoDiagnostic()
		{
		}

		public static AltaxoDiagnostic CreateInfoMessage(string message)
		{
			return new AltaxoDiagnostic()
			{
				Severity = 1,
				SeverityText = "Info",
				MessageText = message
			};
		}
	}
}