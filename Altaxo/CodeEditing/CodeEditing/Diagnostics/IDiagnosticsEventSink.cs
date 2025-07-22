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

#if !NoDiagnostics

using Microsoft.CodeAnalysis.Diagnostics;

namespace Altaxo.CodeEditing.Diagnostics;

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
  internal void OnDiagnosticsUpdated(object sender, DiagnosticsUpdatedArgs diagnosticsUpdatedArgs);
}

#endif
