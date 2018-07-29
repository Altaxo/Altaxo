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
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Thread that can be invoked, i.e. code can be executed using <see cref="Invoke"/> or <see cref="InvokeAsync"/> always from this thread. This is especially important
  /// for objects which are thread sensitive. These objects must be created and it's functions must be called always from the same thread.
  /// </summary>
  public interface IInvokeableThread : IDisposable
  {
    /// <summary>
    /// Executes the provided action synchronously. This means that this function returns only after the provided action was executed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void Invoke(Action action);

    /// <summary>
    /// Executes the provided action asynchronously. This means that this function immediately returns, without waiting for the action to be executed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void InvokeAsync(Action action);
  }
}
