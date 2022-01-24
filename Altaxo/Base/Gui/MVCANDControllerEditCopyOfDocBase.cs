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

namespace Altaxo.Gui
{
  /// <summary>
  /// Base class of controllers that edit a copy of the document. This means that the document is not connected to the document tree when edited.
  /// This class indicates changes made to the document by firing the <see cref="MadeDirty"/> event.
  /// </summary>
  /// <typeparam name="TModel">The type of the document to edit.</typeparam>
  /// <typeparam name="TView">The type of the view.</typeparam>
  public abstract class MVCANDControllerEditCopyOfDocBase<TModel, TView> : MVCANControllerEditCopyOfDocBase<TModel, TView>, IMVCANDController
    where TModel : class // for structs or immutables use the MVCANDControllerEditImmutableDocBase
    where TView : class
  {
    /// <summary>
    /// Used to suppress the <see cref="MadeDirty"/> event of this controller.
    /// </summary>
    protected Altaxo.Main.SuspendableObject _suppressDirtyEvent = new Altaxo.Main.SuspendableObject();

    /// <summary>
    /// Event fired when the user changed some data that will change the model.
    /// </summary>
    public event Action<IMVCANDController>? MadeDirty;

    /// <summary>
    /// Initialize the controller with the document. If successfull, the function has to return true.
    /// </summary>
    /// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
    /// <returns>
    /// Returns <see langword="true" /> if successfull; otherwise <see langword="false" />.
    /// </returns>
    public override bool InitializeDocument(params object[] args)
    {
      if (args is null || 0 == args.Length || !(args[0] is TModel))
        return false;

      _doc = _originalDoc = (TModel)args[0];
      if (_useDocumentCopy && _originalDoc is ICloneable)
        _doc = (TModel)((ICloneable)_originalDoc).Clone();

      using (var suppressor = _suppressDirtyEvent.SuspendGetToken())
      {
        Initialize(true);
      }
      return true;
    }

    /// <summary>
    /// Returns the Gui element that shows the model to the user.
    /// </summary>
    public override object? ViewObject
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
          using (var suppressor = _suppressDirtyEvent.SuspendGetToken())
          {
            Initialize(false);
            AttachView();
          }
        }
      }
    }

    /// <summary>
    /// Fires the <see cref="MadeDirty"/> event (if it is not suppressed).
    /// </summary>
    protected virtual void OnMadeDirty()
    {
      if (!_suppressDirtyEvent.IsSuspended && MadeDirty is not null)
      {
        MadeDirty(this);
      }
    }

    /// <summary>
    /// Gets the provisional model object. This is the model object that is based on the current user input.
    /// </summary>
    public object ProvisionalModelObject
    {
      get
      {
        if (_doc is null)
          throw NoDocumentException;
        return _doc;
      }
    }
  }
}
