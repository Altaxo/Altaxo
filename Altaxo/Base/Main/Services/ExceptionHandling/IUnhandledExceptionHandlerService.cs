#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

#nullable enable

namespace Altaxo.Main.Services.ExceptionHandling
{
  /// <summary>
  /// Interface to a service that registers one or more handlers that are able to handle (otherwise) unhandled exceptions in the application.
  /// </summary>
  public interface IUnhandledExceptionHandlerService
  {
    /// <summary>
    /// Registers a handler and puts them on top of the list. If the flag <paramref name="isExclusive"/> is set to true, all
    /// handlers below the newly registered handler are not called.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    /// <param name="isExclusive">if set to <c>true</c>, the handlers below this handler are not called (until this handler unregisters).</param>
    void AddHandler(IUnhandledExceptionHandler handler, bool isExclusive);

    /// <summary>
    /// Unregisters the handler.
    /// </summary>
    /// <param name="handler">The handler to unregister.</param>
    /// <returns>True if the unregistering was successfull.</returns>
    bool RemoveHandler(IUnhandledExceptionHandler handler);
  }
}
