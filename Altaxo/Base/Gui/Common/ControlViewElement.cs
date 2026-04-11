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
  /// Describes a view element backed by a controller.
  /// </summary>
  public class ControlViewElement : ViewDescriptionElement, ICloneable
  {
    /// <summary>
    /// The controller responsible for applying changes.
    /// </summary>
    public IApplyController Controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlViewElement"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public ControlViewElement(ControlViewElement from)
      : base(from)
    {
      Controller = from.Controller;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlViewElement"/> class.
    /// </summary>
    /// <param name="title">The title of the view element.</param>
    /// <param name="controller">The controller that applies the changes.</param>
    /// <param name="view">The view object associated with the controller.</param>
    public ControlViewElement(string title, IApplyController controller, object view)
      : base(title, view)
    {
      Controller = controller;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlViewElement"/> class.
    /// </summary>
    /// <param name="title">The title of the view element.</param>
    /// <param name="controller">The controller that provides the view.</param>
    public ControlViewElement(string title, IMVCAController controller)
      : base(title, controller.ViewObject ?? throw new InvalidOperationException("The controller provided in the argument has no view yet!"))
    {
      Controller = controller;
    }

    /// <summary>
    /// Creates a typed clone of this instance.
    /// </summary>
    /// <returns>A cloned <see cref="ControlViewElement"/> instance.</returns>
    public new ControlViewElement Clone()
    {
      return new ControlViewElement(this);
    }

    #region ICloneable Members

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new ControlViewElement(this);
    }

    #endregion ICloneable Members
  }
}
