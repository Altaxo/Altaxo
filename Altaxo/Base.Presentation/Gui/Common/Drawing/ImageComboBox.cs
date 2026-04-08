#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// ComboBox base class that provides image support for its items.
  /// </summary>
  public class ImageComboBox : ComboBox
  {
    private static double _standardHeight;

    /// <summary>
    /// Relative width factor used for images rendered in the control.
    /// </summary>
    protected double _relativeImageWidth = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageComboBox"/> class.
    /// </summary>
    public ImageComboBox()
    {
    }

    /// <summary>
    /// Gets the standard item height used for image rendering.
    /// </summary>
    public static double StandardHeight
    {
      get
      {
        return 0 != _standardHeight ? _standardHeight : 24;
      }
    }

    /// <summary>
    /// Gets or sets the relative width of the rendered image.
    /// </summary>
    public double RelativeImageWidth
    {
      get
      {
        return _relativeImageWidth;
      }
      set
      {
        _relativeImageWidth = value;
      }
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size constraint)
    {
      var result = base.MeasureOverride(constraint);

      return result;
    }

    /// <inheritdoc/>
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      if (_standardHeight == 0)
        _standardHeight = sizeInfo.NewSize.Height;

      base.OnRenderSizeChanged(sizeInfo);
    }

    /// <summary>
    /// Gets the image representation for the specified item.
    /// </summary>
    /// <param name="item">The item to visualize.</param>
    /// <returns>The image representation, or <see langword="null"/> if no image is available.</returns>
    public virtual ImageSource GetItemImage(object item)
    {
      return null;
    }

    /// <summary>
    /// Gets the text representation for the specified item.
    /// </summary>
    /// <param name="item">The item to describe.</param>
    /// <returns>The text representation.</returns>
    public virtual string GetItemText(object item)
    {
      return string.Empty;
    }
  }
}
