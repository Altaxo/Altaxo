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

namespace Altaxo.Chat
{
  /// <summary>
  /// Hosts the in-process Mcp server for Altaxo.
  /// </summary>
  public interface IMcpServerHost : IDisposable
  {
    /// <summary>
    /// Starts listening for Mcp clients on the configured named pipe.
    /// </summary>
    public void Start();

    /// <summary>
    /// Gets the name of the named pipe used by the server.
    /// </summary>
    public string PipeName { get; }
  }
}
