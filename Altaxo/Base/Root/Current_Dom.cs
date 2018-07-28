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

    private static Altaxo.Main.IPrintingService _printingService;

    /// <summary>
    /// Returns the printing service, which provides methods for page setup and printing.
    /// </summary>
    public static Altaxo.Main.IPrintingService PrintingService
    {
      get { return _printingService ?? (_printingService = GetRequiredService<Altaxo.Main.IPrintingService>()); }
    }

    #region Data display

    private static Altaxo.Main.Services.IDataDisplayService _dataDisplayService;

    /// <summary>
    /// Returns the data display window, which is used to show the data obtained from the data reader tool.
    /// </summary>
    public static Altaxo.Main.Services.IDataDisplayService DataDisplay
    {
      get
      {
        return _dataDisplayService ?? (_dataDisplayService = GetRequiredService<Altaxo.Main.Services.IDataDisplayService>());
      }
    }

    #endregion Data display

    #region Fit function service

    private static Altaxo.Main.Services.IFitFunctionService _fitFunctionService;

    /// <summary>
    /// Returns the fit function service, which is used to obtain the file based user defined fit functions.
    /// </summary>
    public static Altaxo.Main.Services.IFitFunctionService FitFunctionService
    {
      get { return _fitFunctionService ?? (_fitFunctionService = GetRequiredService<Altaxo.Main.Services.IFitFunctionService>()); }
    }

    #endregion Fit function service

    #region Timer queue

    private static Altaxo.Main.Services.ITimerQueue _timerQueue;

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
        return _timerQueue ?? (_timerQueue = GetRequiredService<Altaxo.Main.Services.ITimerQueue>());
      }
    }

    #endregion Timer queue

    #region High resolution clock

    private static Altaxo.Main.Services.IHighResolutionClock _highResolutionClock;

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
        return _highResolutionClock ?? (_highResolutionClock = GetRequiredService<Altaxo.Main.Services.IHighResolutionClock>());
      }
    }

    #endregion High resolution clock
  }
}
