#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Main.GUI
{
  /// <summary>
  /// The interface that a controller of the MVC (Model-View-Controller) model must implement.
  /// </summary>
  public interface IMVCController
  {
    /// <summary>
    /// Returns the view that shows the model.
    /// </summary>
    object ViewObject { get; set; }

    /// <summary>
    /// Returns the model (document) that this controller controls
    /// </summary>
    object ModelObject { get; }
  
  }

  /// <summary>
  /// Concatenation of a <see>IMVCController</see> and a <see>IApplyController</see>.
  /// </summary>
  public interface IMVCAController : IMVCController, IApplyController
  {
  }

  /// <summary>
  /// Extends IMVCController by the possibility to create a default view for it.
  /// </summary>
  public interface IMVCControllerEx : IMVCController
  {
    /// <summary>
    /// Creates a default view object and assign it to the controller.
    /// </summary>
    /// <returns>The default view object, or null if there is no default view object.</returns>
    /// <remarks>Don't forget not only to create the default view object, but also assign it to the controller!</remarks>
    object CreateDefaultViewObject();
  }
  /// <summary>
  /// The interface that a view of the MVC (Model-View-Controller) model must implement.
  /// </summary>
  public interface IMVCView
  {
    /// <summary>
    /// Returns the controller object that controls this view.
    /// </summary>
    object ControllerObject { get; set; }
  }
}
