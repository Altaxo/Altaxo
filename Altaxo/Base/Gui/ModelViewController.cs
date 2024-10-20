﻿#region Copyright

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
using System.ComponentModel;

namespace Altaxo.Gui
{
  /// <summary>
  /// The interface that a controller of the MVC (Model-View-Controller) model must implement.
  /// </summary>
  public interface IMVCController : IDisposable
  {
    /// <summary>
    /// Returns the Gui element that shows the model to the user.
    /// </summary>
    object? ViewObject { get; set; }

    /// <summary>
    /// Returns the model (document) that this controller manages.
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
    /// <returns>Returns <see langword="true"/> if successfull; otherwise <see langword="false"/>.</returns>
    bool InitializeDocument(params object[] args);

    /// <summary>
    /// Sets whether or not a copy of the document is used. If set to true, a copy of the document is used, so if the controller is not applied,
    /// all changes can be reverted. If set to false, no copy must be made. The document is directly changed by the controller, and changes can not be reverted.
    /// Use the last option if a controller up in the hierarchie has already made a copy of the document.
    /// </summary>
    UseDocument UseDocumentCopy { set; }
  }

  /// <summary>
  /// Extends the <see cref="IMVCANController"/> by an event to signal that the user changed some data. This can be used for instance to update a preview panel etc.
  /// </summary>
  public interface IMVCANDController : IMVCANController, INotifyPropertyChanged
  {
    /// <summary>Event fired when the user changed some data that will change the model.</summary>
    event Action<IMVCANDController>? MadeDirty;

    /// <summary>Gets the provisional model object. This is the model object that is based on the current user input.</summary>
    object ProvisionalModelObject { get; }
  }

  /// <summary>
  /// Interface that can be optionally implemented by controllers to support some actions when the controller's Apply function is successfully executed.
  /// The controller has to call the event <see cref="SuccessfullyApplied"/> after each successfully apply.
  /// </summary>
  public interface IMVCSupportsApplyCallback
  {
    /// <summary>
    /// Occurs when the controller has sucessfully executed the apply function.
    /// </summary>
    event Action? SuccessfullyApplied;
  }

  public struct ControllerAndSetNullMethod
  {
    private IMVCAController? _doc;
    private Action? _setMemberToNullAction;

    public ControllerAndSetNullMethod(IMVCAController? doc, Action setMemberToNullAction)
    {
      _doc = doc;
      _setMemberToNullAction = setMemberToNullAction;
    }

    public IMVCAController? Controller { get { return _doc; } }

    public Action? SetMemberToNullAction { get { return _setMemberToNullAction; } }

    public bool IsEmpty { get { return _doc is null; } }
  }

  /// <summary>
  /// Interface to a view that utilize a data context.
  /// </summary>
  public interface IDataContextAwareView
  {
    object? DataContext { set; }
  }

  /// <summary>
  /// Must be implemented by views that require a special shell window (other then the standard dialog window with OK, Cancel, Apply).
  /// </summary>
  public interface IViewRequiresSpecialShellWindow
  {
    /// <summary>
    /// Gets the type of shell window required.
    /// </summary>
    /// <value>
    /// The type of shell window required.
    /// </value>
    System.Type TypeOfShellWindowRequired { get; }
  }
}
