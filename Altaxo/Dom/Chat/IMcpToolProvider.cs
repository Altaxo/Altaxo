#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using ModelContextProtocol.Server;

namespace Altaxo.Chat
{
  /// <summary>
  /// Supplies explicitly declared Mcp tools from an Altaxo add-in.
  /// </summary>
  public interface IMcpToolProvider
  {
    /// <summary>
    /// Gets a stable identifier for the add-in providing the tools.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Creates the Mcp tools contributed by this add-in.
    /// </summary>
    /// <param name="services">The application services available to the provider.</param>
    /// <returns>The tools explicitly contributed by this provider.</returns>
    /// <remarks>
    /// Tool handlers are invoked asynchronously away from the WPF dispatcher. A handler that accesses
    /// dispatcher-affine state must marshal that access to the dispatcher itself. Tool names should be
    /// stable and include the provider identifier to avoid collisions with other add-ins.
    /// </remarks>
    public IReadOnlyList<McpServerTool> CreateTools(IServiceProvider services);
  }
}
