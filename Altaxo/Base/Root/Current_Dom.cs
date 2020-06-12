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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo
{
  public partial class Current
  {
    public static Altaxo.Main.IAltaxoProjectService ProjectService
    {
      get
      {
        return (Altaxo.Main.IAltaxoProjectService)IProjectService;
      }
    }

    public static Altaxo.AltaxoDocument Project
    {
      get
      {
        return ProjectService.CurrentOpenProject;
      }
    }

    private static Altaxo.Main.IPrintingService? _printingService;

    /// <summary>
    /// Returns the printing service, which provides methods for page setup and printing.
    /// </summary>
    public static Altaxo.Main.IPrintingService PrintingService
    {
      get { return _printingService ??= GetRequiredService<Altaxo.Main.IPrintingService>(); }
    }

    #region Data display

    private static Altaxo.Main.Services.IDataDisplayService? _dataDisplayService;

    /// <summary>
    /// Returns the data display window, which is used to show the data obtained from the data reader tool.
    /// </summary>
    public static Altaxo.Main.Services.IDataDisplayService DataDisplay
    {
      get
      {
        return _dataDisplayService ??= GetRequiredService<Altaxo.Main.Services.IDataDisplayService>();
      }
    }

    #endregion Data display

    #region Fit function service

    private static Altaxo.Main.Services.IFitFunctionService? _fitFunctionService;

    /// <summary>
    /// Returns the fit function service, which is used to obtain the file based user defined fit functions.
    /// </summary>
    public static Altaxo.Main.Services.IFitFunctionService FitFunctionService
    {
      get { return _fitFunctionService ??= GetRequiredService<Altaxo.Main.Services.IFitFunctionService>(); }
    }

    #endregion Fit function service

    #region Timer queue

    private static Altaxo.Main.Services.ITimerQueue? _timerQueue;

    /// <summary>
    /// Gets an application wide timer queue to add actions to be scheduled.
    /// </summary>
    /// <value>
    /// The timer queue.
    /// </value>
    public static Altaxo.Main.Services.ITimerQueue TimerQueue
    {
      get
      {
        return _timerQueue ??= GetRequiredService<Altaxo.Main.Services.ITimerQueue>();
      }
    }

    #endregion Timer queue

    #region High resolution clock

    private static Altaxo.Main.Services.IHighResolutionClock? _highResolutionClock;

    /// <summary>
    /// Gets a high resolution clock that delivers relative values (TimeSpan values relative to the start of the clock). Those values are guaranteed to be continuously incresing, even
    /// if the computer's clock time is changed backwards.
    /// </summary>
    /// <value>
    /// The high resolution clock.
    /// </value>
    public static Altaxo.Main.Services.IHighResolutionClock HighResolutionClock
    {
      get
      {
        return _highResolutionClock ??= GetRequiredService<Altaxo.Main.Services.IHighResolutionClock>();
      }
    }

    #endregion High resolution clock
  }
}
