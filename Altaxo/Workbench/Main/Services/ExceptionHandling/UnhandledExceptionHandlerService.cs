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

  /// <summary>
  /// Implementation of the <see cref="IUnhandledExceptionHandlerService"/>.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.ExceptionHandling.IUnhandledExceptionHandlerService" />
  public class UnhandledExceptionHandlerService : IUnhandledExceptionHandlerService
  {
    private List<(IUnhandledExceptionHandler Handler, bool IsExclusive)> _handlerList;
    private object _listLocker;

    /// <summary>
    /// Registers a handler and puts them on top of the list. If the flag <paramref name="isExclusive" /> is set to true, all
    /// handlers below the newly registered handler are not called.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    /// <param name="isExclusive">if set to <c>true</c>, the handlers below this handler are not called (until this handler unregisters).</param>
    /// <exception cref="ArgumentNullException">handler</exception>
    public void AddHandler(IUnhandledExceptionHandler handler, bool isExclusive)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));

      lock (_listLocker)
      {
        _handlerList.Add((handler, isExclusive));
      }
    }

    /// <summary>
    /// Unregisters the handler.
    /// </summary>
    /// <param name="handler">The handler to unregister.</param>
    /// <returns>
    /// True if the unregistering was successfull.
    /// </returns>
    /// <exception cref="ArgumentNullException">handler</exception>
    public bool RemoveHandler(IUnhandledExceptionHandler handler)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));

      lock (_listLocker)
      {
        for (int i = _handlerList.Count - 1; i >= 0; --i)
        {
          if (object.ReferenceEquals(_handlerList[i].Handler, handler))
          {
            _handlerList.RemoveAt(i);
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledExceptionHandlerService"/> class.
    /// </summary>
    public UnhandledExceptionHandlerService()
    {
      _handlerList = new List<(IUnhandledExceptionHandler Handler, bool IsExclusive)>();
      _listLocker = new object();
      System.Windows.Forms.Application.ThreadException += EhFormsApplication_ThreadException;
      AppDomain.CurrentDomain.UnhandledException += EhCurrentDomain_UnhandledException;
      System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException += EhWpfDispatcher_UnhandledException;
    }

    private void EhWpfDispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
      for (int i = _handlerList.Count - 1; i >= 0; --i)
      {
        try
        {
          _handlerList[i].Handler.EhWpfDispatcher_UnhandledException(sender, e.Dispatcher, e.Exception);
          if (_handlerList[i].IsExclusive)
            break;
        }
        catch
        {
        }
      }
      e.Handled = _handlerList.Count > 0;
    }

    private void EhFormsApplication_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      for (int i = _handlerList.Count - 1; i >= 0; --i)
      {
        try
        {
          _handlerList[i].Handler.EhWindowsFormsApplication_ThreadException(sender, e);
          if (_handlerList[i].IsExclusive)
            break;
        }
        catch
        {
        }
      }
    }

    private void EhCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      for (int i = _handlerList.Count - 1; i >= 0; --i)
      {
        try
        {
          _handlerList[i].Handler.EhCurrentDomain_UnhandledException(sender, e);
          if (_handlerList[i].IsExclusive)
            break;
        }
        catch
        {
        }
      }
    }

  }
}
