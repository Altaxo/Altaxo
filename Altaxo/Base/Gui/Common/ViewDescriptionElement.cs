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

#nullable enable
using System;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Bundles a view instance with a title.
  /// </summary>
  public class ViewDescriptionElement : ICloneable
  {
    /// <summary>
    /// The title of the view.
    /// </summary>
    public string Title;
    /// <summary>
    /// Gets the view instance.
    /// </summary>
    public object View { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewDescriptionElement"/> class as a copy of another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public ViewDescriptionElement(ViewDescriptionElement from)
    {
      Title = from.Title;
      View = from.View;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewDescriptionElement"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="view">The view instance.</param>
    public ViewDescriptionElement(string title, object view)
    {
      Title = title;
      View = view;
    }

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>A copy of this instance.</returns>
    public ViewDescriptionElement Clone()
    {
      return new ViewDescriptionElement(this);
    }

    #region ICloneable Members

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new ViewDescriptionElement(this);
    }

    #endregion ICloneable Members
  }
}
