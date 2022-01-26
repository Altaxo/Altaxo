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
  /// Base of all controllers that edit the original document directly (live), and that indicates any changes of the document by the <see cref="MadeDirty"/> event.
  /// </summary>
  /// <typeparam name="TModel">The type of the document to edit.</typeparam>
  /// <typeparam name="TView">The type of the view.</typeparam>
  public abstract class MVCANDControllerEditOriginalDocBase<TModel, TView> : MVCANControllerEditOriginalDocBase<TModel, TView>, IMVCANDController
    where TView : class
    where TModel : ICloneable
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
      bool result;

      using (var suspendToken = _suppressDirtyEvent.SuspendGetToken())
      {
        result = base.InitializeDocument(args);

        suspendToken.Resume();
      }

      return result;
    }

    /// <summary>
    /// Returns the Gui element that shows the model to the user. When attaching a new view, the <see cref="MadeDirty"/> event is suppressed.
    /// </summary>
    public override object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (!object.ReferenceEquals(_view, value))
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
            OnPropertyChanged(nameof(ViewObject));
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
        OnPropertyChanged(nameof(ProvisionalModelObject));
      }
    }

    /// <summary>
    /// Gets the provisional model object. This is the model object that is based on the current user input. In this class of controller, it is identical to the edited document (because the original document is edited).
    /// </summary>
    public object ProvisionalModelObject
    {
      get
      {
        if (_doc is null)
          throw CreateNotInitializedException;

        return _doc;
      }
    }
  }
}
