using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.Diagnostics
{
	/// <summary>
	/// Must be implemented by workspaces that want to receive diagnostics messages.
	/// </summary>
	public interface IDiagnosticsEventSink
	{
		/// <summary>
		/// Is called by the diagnostics service if the diagnostics for a document of this workspace has been updated.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="diagnosticsUpdatedArgs">The diagnostics updated arguments.</param>
		void OnDiagnosticsUpdated(object sender, DiagnosticsUpdatedArgs diagnosticsUpdatedArgs);
	}
}