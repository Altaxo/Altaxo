#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Gui
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
  /// Concatenation of a <see cref="IMVCController" /> and a <see cref="IApplyController" />.
  /// </summary>
  public interface IMVCAController : IMVCController, IApplyController
  {
  }

  /// <summary>
  /// Enumerates wheter or not a document controlled by a controller is directly changed or not.
  /// </summary>
  public enum UseDocument
  {
     /// <summary>
    /// The document is not used directly. All changes by the user are temporarily stored and committed only when the Apply function of the controller is called.
    /// </summary>
    Copy,

    /// <summary>
    /// The document is used directly. All changes by the user are directly reflected in the controlled document.
    /// </summary>
    Directly

   
  }

  public interface IMVCANController : IMVCAController
  {
    /// <summary>
    /// Initialize the controller with the document. If successfull, the function has to return true.
    /// </summary>
    /// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
    /// <returns>True if successfull, else false.</returns>
    bool InitializeDocument(params object[] args);

    /// <summary>
    /// Sets whether or not a copy of the document is used. If set to true, a copy of the document is used, so if the controller is not applied,
    /// all changes can be reverted. If set to false, no copy must be made. The document is directly changed by the controller, and changes can not be reverted.
    /// Use the last option if a controller up in the hierarchie has already made a copy of the document.
    /// </summary>
    UseDocument UseDocumentCopy { set; }
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

  /// <summary>
  /// Wraps an IMVCController in a wrapper class
  /// </summary>
  public interface IMVCControllerWrapper
  {
    Altaxo.Gui.IMVCController MVCController { get; }
  }

}
