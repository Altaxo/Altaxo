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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ExceptionHandling
{
  public interface IUnhandledExceptionHandler
  {
    /// <summary>
    /// Handles unhandled exceptions from the Wpf dispatcher (see "System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException"). 
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="dispatcher">The dispatcher that was part of the event args.</param>
    /// <param name="exception">The exception that was part of the event args.</param>
    void EhWpfDispatcher_UnhandledException(object sender, object dispatcher, Exception exception);

    /// <summary>
    /// Handles unhandled exceptions from the Windows forms system (see "System.Windows.Forms.Application.ThreadException").
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="ThreadExceptionEventArgs"/> instance containing the event data.</param>
    void EhWindowsFormsApplication_ThreadException(object sender, ThreadExceptionEventArgs e);


    /// <summary>
    /// Handles unhandled exceptions from the current domain (see "AppDomain.CurrentDomain.UnhandledException").
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
    void EhCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e);
  }
}
