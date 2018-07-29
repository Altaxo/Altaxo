#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Delegate for the <see cref="IGuiTimerService"/>. Note that the argument is the Utc time. If you need local time, you have to convert the
  /// argument to local time.
  /// </summary>
  /// <param name="utcNow">The UTC current time.</param>
  public delegate void GuiTimerServiceHandler(DateTime utcNow);

  /// <summary>
  /// Interface for a timer service that calls back using the Gui thread.
  /// </summary>
  /// <seealso cref="System.IDisposable" />
  public interface IGuiTimerService : IDisposable
  {
    /// <summary>
    /// A Gui timer tick that occurs every 10 ms.
    /// </summary>
    event GuiTimerServiceHandler TickEvery10ms;

    /// <summary>
    /// A Gui timer tick that occurs every 100 ms.
    /// </summary>
    event GuiTimerServiceHandler TickEvery100ms;

    /// <summary>
    /// A Gui timer tick that occurs every 1000 ms.
    /// </summary>
    event GuiTimerServiceHandler TickEvery1000ms;
  }
}
