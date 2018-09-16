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

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  ///
  /// </summary>
  public class FullScreenEnabledWindow : Window
  {
    public static readonly DependencyProperty FullScreenProperty =
        DependencyProperty.Register("FullScreen", typeof(bool), typeof(FullScreenEnabledWindow));

    public bool FullScreen
    {
      get { return (bool)GetValue(FullScreenProperty); }
      set { SetValue(FullScreenProperty, value); }
    }

    private System.Windows.WindowState previousWindowState = System.Windows.WindowState.Maximized;
    private double oldLeft, oldTop, oldWidth, oldHeight;

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == FullScreenProperty)
      {
        if ((bool)e.NewValue)
        {
          // enable fullscreen mode
          // remember previous window state
          if (WindowState == System.Windows.WindowState.Normal || WindowState == System.Windows.WindowState.Maximized)
            previousWindowState = WindowState;
          oldLeft = Left;
          oldTop = Top;
          oldWidth = Width;
          oldHeight = Height;

          var interop = new WindowInteropHelper(this);
          interop.EnsureHandle();
          var screen = Screen.FromHandle(interop.Handle);

          Rect bounds = screen.Bounds.ToWpf().TransformFromDevice(this);

          ResizeMode = ResizeMode.NoResize;
          Left = bounds.Left;
          Top = bounds.Top;
          Width = bounds.Width;
          Height = bounds.Height;
          WindowState = System.Windows.WindowState.Normal;
          WindowStyle = WindowStyle.None;
        }
        else
        {
          ClearValue(WindowStyleProperty);
          ClearValue(ResizeModeProperty);
          ClearValue(MaxWidthProperty);
          ClearValue(MaxHeightProperty);
          WindowState = previousWindowState;

          Left = oldLeft;
          Top = oldTop;
          Width = oldWidth;
          Height = oldHeight;
        }
      }
    }
  }
}
