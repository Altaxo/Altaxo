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

namespace Altaxo.Gui.Common
{
  public interface IConditionalDocumentView
  {
    bool IsConditionalViewEnabled { get; set; }

    event Action? ConditionalViewEnabledChanged;

    object? ConditionalView { set; }

    string EnablingText { set; }
  }

  [ExpectedTypeOfView(typeof(IConditionalDocumentView))]
  public class ConditionalDocumentController<TModel> : IMVCANController where TModel : notnull
  {
    private Func<TModel> _creationAction;
    private Action _removalAction;
    private Func<TModel, UseDocument, IMVCANController?> _controllerCreationAction;

    private IConditionalDocumentView? _view;
    private IMVCANController? _controller;
    private UseDocument _useDocumentCopy;

    public ConditionalDocumentController(Func<TModel> CreationAction, Action RemovalAction)
      : this(CreationAction, RemovalAction, InternalCreateController)
    {
    }

    public ConditionalDocumentController(Func<TModel> CreationAction, Action RemovalAction, Func<TModel, UseDocument, IMVCANController?> ControllerCreationAction)
    {
      if (CreationAction is null)
        throw new ArgumentNullException("CreationAction");
      if (RemovalAction is null)
        throw new ArgumentNullException("RemovalAction");
      if (ControllerCreationAction is null)
        throw new ArgumentNullException("ControllerCreationAction");

      _creationAction = CreationAction;
      _removalAction = RemovalAction;
      _controllerCreationAction = ControllerCreationAction;
    }

    public IMVCANController? UnderlyingController
    {
      get
      {
        return _controller;
      }
    }

    private static IMVCANController? InternalCreateController(TModel doc, UseDocument useDocumentCopy)
    {
      return (IMVCANController?)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(IMVCANController), useDocumentCopy);
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0 || !(args[0] is TModel))
        return false;

      _controller = _controllerCreationAction((TModel)args[0], _useDocumentCopy);

      Initialize(true);
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set
      {
        _useDocumentCopy = value;
        if (_controller is not null)
          _controller.UseDocumentCopy = value;
      }
    }

    public object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          _view.ConditionalViewEnabledChanged -= EhViewEnabledChanged;
        }
        _view = value as IConditionalDocumentView;

        if (_view is not null)
        {
          Initialize(false);
          _view.ConditionalViewEnabledChanged += EhViewEnabledChanged;
        }
      }
    }

    public object ModelObject
    {
      get { return _controller?.ModelObject ?? new object(); }
    }

    public void Dispose()
    {
    }

    public bool Apply(bool disposeController)
    {
      if (_controller is not null)
        return _controller.Apply(disposeController);
      else
        return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    private void Initialize(bool initData)
    {
      if (_view is not null)
      {
        if (_controller is not null)
        {
          _view.IsConditionalViewEnabled = true;
          _view.ConditionalView = _controller.ViewObject;
        }
        else
        {
          _view.IsConditionalViewEnabled = false;
          _view.ConditionalView = null;
        }
      }
    }

    private void EhViewEnabledChanged()
    {
      if (!(_view is null))
      {
        AnnounceEnabledChanged(_view.IsConditionalViewEnabled);
      }
    }

    public void AnnounceEnabledChanged(bool enableState)
    {
      if (true == enableState && _controller is null)
      {
        if (_controller is null)
        {
          TModel document = _creationAction();
          _controller = _controllerCreationAction(document, _useDocumentCopy);
          if (_view is not null && _controller is not null)
            _view.ConditionalView = _controller.ViewObject;
        }
      }
      else if (false == enableState && _controller is not null) // view is disabled
      {
        _removalAction();
        _controller = null;
        if (_view is not null)
          _view.ConditionalView = null;
      }

      if (_view is not null)
      {
        _view.IsConditionalViewEnabled = enableState;
      }
    }
  }
}
