// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable disable warnings
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;

namespace Altaxo.Gui.Workbench
{
  public class StatusBarStandard : System.Windows.Controls.Primitives.StatusBar, IStatusBarView
  {
    // Panels from left to right
    private StatusBarItem _txtStatusBarPanel;

    private StatusBarItem _jobNamePanel;
    private StatusBarItem _statusProgressBarItem;
    private StatusBarItem _cursorStatusBarPanel;
    private StatusBarItem _selectionStatusBarPanel;
    private StatusBarItem _modeStatusBarPanel;

    // For progress displaying
    private ProgressBar _statusProgressBar;

    private bool _statusProgressBarIsVisible;
    private string _currentTaskName;
    private OperationStatus _currentProgressStatus;
    private SolidColorBrush _progressForegroundBrush;

    #region IStatusBarView

    public bool IsStatusBarVisible { set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; } }
    public object CursorStatusBarPanelContent { set { _cursorStatusBarPanel.Content = value; } }
    public object SelectionStatusBarPanelContent { set { _selectionStatusBarPanel.Content = value; } }
    public object ModeStatusBarPanelContent { set { _modeStatusBarPanel.Content = value; } }

    #endregion IStatusBarView

    public StatusBarItem CursorStatusBarPanel
    {
      get
      {
        return _cursorStatusBarPanel;
      }
    }

    public StatusBarItem SelectionStatusBarPanel
    {
      get
      {
        return _selectionStatusBarPanel;
      }
    }

    public StatusBarItem ModeStatusBarPanel
    {
      get
      {
        return _modeStatusBarPanel;
      }
    }

    public StatusBarStandard()
    {
      // from left to right
      _txtStatusBarPanel = new StatusBarItem() { Name = nameof(_txtStatusBarPanel) };

      _jobNamePanel = new StatusBarItem() { Name = nameof(_jobNamePanel) };
      _statusProgressBarItem = new StatusBarItem() { Name = nameof(_statusProgressBarItem), Width = 100, Visibility = Visibility.Hidden, VerticalContentAlignment = VerticalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch };
      _cursorStatusBarPanel = new StatusBarItem() { Name = nameof(_cursorStatusBarPanel), Width = 150 };
      _selectionStatusBarPanel = new StatusBarItem() { Name = nameof(_selectionStatusBarPanel), Width = 50 };
      _modeStatusBarPanel = new StatusBarItem() { Name = nameof(_modeStatusBarPanel), Width = 65 };
      // For progress displaying
      _statusProgressBar = new ProgressBar() { Name = nameof(_statusProgressBar), Minimum = 0, Maximum = 1 };
      _statusProgressBarItem.Content = _statusProgressBar;

      DockPanel.SetDock(_modeStatusBarPanel, Dock.Right);
      DockPanel.SetDock(_selectionStatusBarPanel, Dock.Right);
      DockPanel.SetDock(_cursorStatusBarPanel, Dock.Right);
      DockPanel.SetDock(_statusProgressBarItem, Dock.Right);
      DockPanel.SetDock(_jobNamePanel, Dock.Right);

      Items.Add(_modeStatusBarPanel);
      Items.Add(_selectionStatusBarPanel);
      Items.Add(_cursorStatusBarPanel);
      Items.Add(_statusProgressBarItem);
      Items.Add(_jobNamePanel);

      Items.Add(_txtStatusBarPanel);
    }

    public void SetMessage(string message, bool highlighted, object icon = null)
    {
      Action setMessageAction = delegate
      {
        if (highlighted)
        {
          _txtStatusBarPanel.Background = SystemColors.HighlightBrush;
          _txtStatusBarPanel.Foreground = SystemColors.HighlightTextBrush;
        }
        else
        {
          _txtStatusBarPanel.Background = SystemColors.ControlBrush;
          _txtStatusBarPanel.Foreground = SystemColors.ControlTextBrush;
        }
        _txtStatusBarPanel.Content = message;
      };
      if (Current.Dispatcher.InvokeRequired)
      {
        Current.Dispatcher.InvokeAndForget(setMessageAction);
      }
      else
      {
        setMessageAction();
      }
    }

    public void DisplayProgress(string taskName, double workDone, OperationStatus status)
    {
      //			Current.Log.Debug("DisplayProgress(\"" + taskName + "\", " + workDone + ", " + status + ")");
      if (!_statusProgressBarIsVisible)
      {
        _statusProgressBarItem.Visibility = Visibility.Visible;
        _statusProgressBarIsVisible = true;
        StopHideProgress();
      }

      TaskbarItemProgressState taskbarProgressState;
      if (double.IsNaN(workDone))
      {
        _statusProgressBar.IsIndeterminate = true;
        status = OperationStatus.Normal; // indeterminate doesn't support foreground color
        taskbarProgressState = TaskbarItemProgressState.Indeterminate;
      }
      else
      {
        _statusProgressBar.IsIndeterminate = false;
        _statusProgressBar.Value = workDone;

        if (status == OperationStatus.Error)
          taskbarProgressState = TaskbarItemProgressState.Error;
        else
          taskbarProgressState = TaskbarItemProgressState.Normal;
      }

      TaskbarItemInfo taskbar = ((Window)(Current.Gui.MainWindowObject)).TaskbarItemInfo;
      if (taskbar is not null)
      {
        taskbar.ProgressState = taskbarProgressState;
        taskbar.ProgressValue = workDone;
      }

      if (status != _currentProgressStatus)
      {
        if (_progressForegroundBrush is null)
        {
          var defaultForeground = _statusProgressBar.Foreground as SolidColorBrush;
          _progressForegroundBrush = new SolidColorBrush(defaultForeground is not null ? defaultForeground.Color : Colors.Blue);
        }

        if (status == OperationStatus.Error)
        {
          _statusProgressBar.Foreground = _progressForegroundBrush;
          _progressForegroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(
              Colors.Red, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd));
        }
        else if (status == OperationStatus.Warning)
        {
          _statusProgressBar.Foreground = _progressForegroundBrush;
          _progressForegroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(
              Colors.YellowGreen, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd));
        }
        else
        {
          _statusProgressBar.ClearValue(ProgressBar.ForegroundProperty);
          _progressForegroundBrush = null;
        }
        _currentProgressStatus = status;
      }

      if (_currentTaskName != taskName)
      {
        _currentTaskName = taskName;
        _jobNamePanel.Content = taskName;
      }
    }

    public void HideProgress()
    {
      //			Current.Log.Debug("HideProgress()");
      _statusProgressBarIsVisible = false;
      // to allow the user to see the red progress bar as a visual clue of a failed
      // build even if it occurs close to the end of the build, we'll hide the progress bar
      // with a bit of time delay
      Current.Dispatcher.InvokeLaterAndForget(
           TimeSpan.FromMilliseconds(_currentProgressStatus == OperationStatus.Error ? 500 : 150),
           new Action(DoHideProgress));
    }

    private void DoHideProgress()
    {
      if (!_statusProgressBarIsVisible)
      {
        // make stuff look nice and delay it a little more by using an animation
        // on the progress bar
        var timeSpan = TimeSpan.FromSeconds(0.25);
        var animation = new DoubleAnimation(0, new Duration(timeSpan), FillBehavior.HoldEnd);
        _statusProgressBarItem.BeginAnimation(OpacityProperty, animation);
        _jobNamePanel.BeginAnimation(OpacityProperty, animation);
        Current.Dispatcher.InvokeLaterAndForget(
            timeSpan,
            delegate
            {
              if (!_statusProgressBarIsVisible)
              {
                _statusProgressBarItem.Visibility = Visibility.Collapsed;
                _jobNamePanel.Content = _currentTaskName = "";
                var taskbar = ((Window)Current.Gui.MainWindowObject).TaskbarItemInfo;
                if (taskbar is not null)
                  taskbar.ProgressState = TaskbarItemProgressState.None;
                StopHideProgress();
              }
            });
      }
    }

    private void StopHideProgress()
    {
      _statusProgressBarItem.BeginAnimation(OpacityProperty, null);
      _jobNamePanel.BeginAnimation(OpacityProperty, null);
    }
  }
}
