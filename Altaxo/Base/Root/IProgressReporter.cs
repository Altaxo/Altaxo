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
		/// Returns true if the activity was cancelled by the user. The script has to check this value periodically. If it is set to true, the script should return.
		/// </summary>
		bool CancellationPending { get; }
	}
}
