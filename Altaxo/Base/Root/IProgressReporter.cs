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
	/// Allows a thread to report text to a receiver. Additionally, the thread can look to the property <see cref="CancellationPending" />, and
	/// if it is <c>true</c>, return in a safe way.
	/// </summary>
	public interface IProgressReporter
	{
		/// <summary>
		/// True if we should report the progress now.
		/// </summary>
		bool ShouldReportNow { get; }

		/// <summary>
		/// Reports the progress as a text string.
		/// </summary>
		/// <param name="text">Report text</param>
		void ReportProgress(string text);

		/// <summary>
		/// Reports the progress as a text string.
		/// </summary>
		/// <param name="text">Report text</param>
		/// <param name="progressPercentage">The progress as fraction (0..1).</param>
		void ReportProgress(string text, double progressValue);


		/// <summary>
		/// Returns true if the activity was cancelled by the user. The script has to check this value periodically. If it is set to true, the script should return.
		/// </summary>
		bool CancellationPending { get; }
	}
}
