#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Gui
{
  /// <summary>
  /// Base class of controllers that edit a copy of the document. This means that the document is not connected to the document tree when edited.
  /// If you need the document connected to the document tree during editing, consider using <see cref="MVCANControllerEditOriginalDocBase{TModel, TView>"/> instead.
  /// </summary>
  /// <typeparam name="TModel">The type of the document to edit.</typeparam>
  /// <typeparam name="TView">The type of the view.</typeparam>
  public abstract class MVCANControllerEditCopyOfDocBase<TModel, TView> : ControllerBase, IMVCANController
    where TModel : class // for structs please use MVCANControllerEditImmutableDocBase
    where TView : class
  {
    /// <summary>The document to edit. If <see cref="_useDocumentCopy"/> is true, this is a copy of the original document; otherwise, it is the original document itself.</summary>
    protected TModel? _doc;

    /// <summary>The original document. If <see cref="_useDocumentCopy"/> is false, it maybe has been edited by this controller.</summary>
    protected TModel? _originalDoc;

    /// <summary>The Gui view of this controller</summary>
    protected TView? _view;

    /// <summary>If true, a copy of the document is made before editing; this copy can later be used to revert the state of the document to the original state.</summary>
    protected bool _useDocumentCopy;

    /// <summary>Set to true if this controller is already disposed.</summary>
    private bool _isDisposed;

    /// <summary>
    /// Enumerates the sub controllers. This function is called on <see cref="Dispose(bool)"/> of this controller to dispose the subcontrollers too.
    /// By overriding this function, there is no need to override <see cref="Dispose(bool)"/>
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<ControllerAndSetNullMethod> GetSubControllers();

    /// <summary>
    /// Initialize the controller with the document. If successfull, the function has to return true.
    /// </summary>
    /// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
    /// <returns>
    /// Returns <see langword="true" /> if successfull; otherwise <see langword="false" />.
    /// </returns>
    public virtual bool InitializeDocument(params object[] args)
    {
      if (IsDisposed)
        throw new ObjectDisposedException("The controller was already disposed. Type: " + GetType().FullName);

      if (args is null || 0 == args.Length || !(args[0] is TModel))
        return false;

      _doc = _originalDoc = (TModel)args[0];
      if (_useDocumentCopy && _originalDoc is ICloneable cloneableDoc)
        _doc = (TModel)cloneableDoc.Clone();

      Initialize(true);
      return true;
    }

    /// <summary>
    /// Basic initialization of the document.
    /// </summary>
    /// <param name="initData">If set to <c>true</c>, it indicates that the controller should initialize its model classes..</param>
    /// <exception cref="System.InvalidOperationException">This controller was not initialized with a document.</exception>
    /// <exception cref="System.ObjectDisposedException">The controller was already disposed.</exception>
    [MemberNotNull(nameof(_doc))]
    protected virtual void Initialize(bool initData)
    {
      if (IsDisposed)
        throw new ObjectDisposedException("The controller was already disposed. Type: " + GetType().FullName);
      if (_doc is null)
        throw new InvalidOperationException("This controller was not initialized with a document.");
    }



    /// <summary>Throws an exception if the controller is not initialized with a document.</summary>
    /// <exception cref="InvalidOperationException">Controller was not initialized with a document</exception>
    [MemberNotNull(nameof(_originalDoc), nameof(_doc))]
    protected void ThrowIfNotInitialized()
    {
      if (_originalDoc is null || _doc is null)
        throw NoDocumentException;
    }

    /// <summary>
    /// Called when the user input has to be applied to the document being controlled. Returns true if Apply is successfull.
    /// </summary>
    /// <param name="disposeController">If the Apply operation was successfull, and this argument is <c>true</c>, the controller should release all temporary resources, because they are not needed any more.
    /// If this argument is <c>false</c>, the controller should be reinitialized with the current model (the model that results from the Apply operation).</param>
    /// <returns>
    /// True if the apply operation was successfull, otherwise false. If false is returned, the <paramref name="disposeController" /> argument is ignored: thus the controller is not disposed.
    /// </returns>
    /// <remarks>
    /// This function is called in two cases: Either the user pressed OK or the user pressed Apply.
    /// </remarks>
    public abstract bool Apply(bool disposeController);

    protected virtual bool ApplyEnd(bool applyResult, bool disposeController)
    {
      ThrowIfNotInitialized();

      if (applyResult == true)
      {
        if (!object.ReferenceEquals(_doc, _originalDoc))
        {
          if (_originalDoc is ICloneable orgDoc)
          {

            CopyHelper.Copy(ref orgDoc, (ICloneable)_doc);
            _originalDoc = (TModel)orgDoc;
          }
        }

        if (disposeController)
        {
          Dispose();
        }
      }

      return applyResult;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public virtual bool Revert(bool disposeController)
    {
      if (disposeController)
      {
        Dispose();
      }

      return false;
    }

    /// <summary>
    /// Override this function to attach the view to the controller, either by subscribing to events of the view, or by setting the controller object on the view.
    /// </summary>
    protected virtual void AttachView()
    {
    }

    /// <summary>
    /// Override this function to detach the view from the controller, either by unsubscribing to events of the view, or by setting the controller object on the view to null.
    /// </summary>
    protected virtual void DetachView()
    {
    }

    /// <summary>Get a value indication whether  this controller is already disposed.</summary>
    public bool IsDisposed { get { return _isDisposed; } }

    /// <summary>
    /// Sets whether or not a copy of the document is used. If set to true, a copy of the document is used, so if the controller is not applied,
    /// all changes can be reverted. If set to false, no copy must be made. The document is directly changed by the controller, and changes can not be reverted.
    /// Use the last option if a controller up in the hierarchie has already made a copy of the document.
    /// </summary>
    public UseDocument UseDocumentCopy
    {
      set { _useDocumentCopy = value == UseDocument.Copy; }
    }

    /// <summary>
    /// Returns the Gui element that shows the model to the user.
    /// </summary>
    public virtual object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          DetachView();
        }

        _view = value as TView;

        if (_view is not null)
        {
          Initialize(false);
          AttachView();
        }
      }
    }

    /// <summary>
    /// Returns the document that this controller has edited. Here the state of the document has changed only after calling <see cref="Apply"/>.
    /// </summary>
    public virtual object ModelObject
    {
      get
      {
        ThrowIfNotInitialized();
        return _originalDoc;
      }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    public virtual void Dispose(bool isDisposing)
    {
      if (!IsDisposed)
      {
        foreach (var subControllerItem in GetSubControllers())
        {
          if (subControllerItem.Controller is not null)
            subControllerItem.Controller.Dispose();
          if (subControllerItem.SetMemberToNullAction is not null)
            subControllerItem.SetMemberToNullAction();
        }

        ViewObject = null;

        _isDisposed = true;
      }
    }
  }
}
