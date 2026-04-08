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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Provides WinForms-related extension methods.
  /// </summary>
  public static class WinFormsExtensions
  {
    /// <summary>
    /// Gets a bitmap from the resource service.
    /// This method returns an existing bitmap, do not dispose it!
    /// </summary>
    /// <param name="resourceService">The resource service.</param>
    /// <param name="resourceName">The name of the bitmap resource.</param>
    /// <returns>The bitmap.</returns>
    /// <exception cref="ResourceNotFoundException">Resource with the specified name does not exist</exception>
    public static Bitmap GetBitmap(this IResourceService resourceService, string resourceName)
    {
      return Altaxo.Current.GetRequiredService<IWinFormsService>().GetResourceServiceBitmap(resourceName);
    }

    /// <summary>
    /// Gets an icon from the resource service.
    /// This method returns an existing icon, do not dispose it!
    /// </summary>
    /// <param name="resourceService">The resource service.</param>
    /// <param name="resourceName">The name of the icon resource.</param>
    /// <returns>The icon.</returns>
    /// <exception cref="ResourceNotFoundException">Resource with the specified name does not exist</exception>
    public static Icon GetIcon(this IResourceService resourceService, string resourceName)
    {
      return Altaxo.Current.GetRequiredService<IWinFormsService>().GetResourceServiceIcon(resourceName);
    }

    #region System.Drawing <-> WPF conversions

    /// <summary>
    /// Converts a WPF point to a <see cref="System.Drawing.Point"/>.
    /// </summary>
    /// <param name="p">The WPF point.</param>
    /// <returns>The converted point.</returns>
    public static System.Drawing.Point ToSystemDrawing(this System.Windows.Point p)
    {
      return new System.Drawing.Point((int)p.X, (int)p.Y);
    }

    /// <summary>
    /// Converts a WPF size to a <see cref="System.Drawing.Size"/>.
    /// </summary>
    /// <param name="s">The WPF size.</param>
    /// <returns>The converted size.</returns>
    public static System.Drawing.Size ToSystemDrawing(this System.Windows.Size s)
    {
      return new System.Drawing.Size((int)s.Width, (int)s.Height);
    }

    /// <summary>
    /// Converts a WPF rectangle to a <see cref="System.Drawing.Rectangle"/>.
    /// </summary>
    /// <param name="r">The WPF rectangle.</param>
    /// <returns>The converted rectangle.</returns>
    public static System.Drawing.Rectangle ToSystemDrawing(this System.Windows.Rect r)
    {
      return new System.Drawing.Rectangle(r.TopLeft.ToSystemDrawing(), r.Size.ToSystemDrawing());
    }

    /// <summary>
    /// Converts a WPF color to a <see cref="System.Drawing.Color"/>.
    /// </summary>
    /// <param name="c">The WPF color.</param>
    /// <returns>The converted color.</returns>
    public static System.Drawing.Color ToSystemDrawing(this System.Windows.Media.Color c)
    {
      return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    /// <summary>
    /// Converts a <see cref="System.Drawing.Point"/> to a WPF point.
    /// </summary>
    /// <param name="p">The drawing point.</param>
    /// <returns>The converted point.</returns>
    public static System.Windows.Point ToWpf(this System.Drawing.Point p)
    {
      return new System.Windows.Point(p.X, p.Y);
    }

    /// <summary>
    /// Converts a <see cref="System.Drawing.Size"/> to a WPF size.
    /// </summary>
    /// <param name="s">The drawing size.</param>
    /// <returns>The converted size.</returns>
    public static System.Windows.Size ToWpf(this System.Drawing.Size s)
    {
      return new System.Windows.Size(s.Width, s.Height);
    }

    /// <summary>
    /// Converts a <see cref="System.Drawing.Rectangle"/> to a WPF rectangle.
    /// </summary>
    /// <param name="rect">The drawing rectangle.</param>
    /// <returns>The converted rectangle.</returns>
    public static System.Windows.Rect ToWpf(this System.Drawing.Rectangle rect)
    {
      return new System.Windows.Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
    }

    /// <summary>
    /// Converts a <see cref="System.Drawing.Color"/> to a WPF color.
    /// </summary>
    /// <param name="c">The drawing color.</param>
    /// <returns>The converted color.</returns>
    public static System.Windows.Media.Color ToWpf(this System.Drawing.Color c)
    {
      return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    #endregion System.Drawing <-> WPF conversions
  }
}
