using Altaxo.Gui;

namespace Altaxo.Main.Commands
{
  public abstract class ResizeBaseClass : SimpleCommand
  {
    public void Execute(int width, int height)
    {
      var mainWindow = System.Windows.Application.Current.MainWindow;

      mainWindow.Visibility = System.Windows.Visibility.Visible;
      mainWindow.ShowInTaskbar = true;
      mainWindow.BringIntoView();

      // how to bring a window to the front ?
      // see http://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf

      if (!mainWindow.IsVisible)
      {
        mainWindow.Show();
      }

      if (mainWindow.WindowState == System.Windows.WindowState.Minimized)
      {
        mainWindow.WindowState = System.Windows.WindowState.Normal;
      }
      else if (mainWindow.WindowState == System.Windows.WindowState.Maximized)
      {
        mainWindow.WindowState = System.Windows.WindowState.Normal;
      }

      mainWindow.Width = width + 14;
      mainWindow.Height = height + 7;

      mainWindow.Top = 0;
      mainWindow.Left = -7;

      mainWindow.Activate();
      mainWindow.Topmost = true;  // important
      mainWindow.Topmost = false; // important
      mainWindow.Focus();         // important
    }
  }

  public class ResizeMainWindowTo1920x1080 : ResizeBaseClass
  {
    public override void Execute(object? parameter)
    {
      Execute(1920, 1080);
    }
  }

  public class ResizeMainWindowTo1024x768 : ResizeBaseClass
  {
    public override void Execute(object? parameter)
    {
      Execute(1024, 768);
    }
  }

  public class ResizeMainWindowTo800x600 : ResizeBaseClass
  {
    public override void Execute(object? parameter)
    {
      Execute(800, 600);
    }
  }
}

