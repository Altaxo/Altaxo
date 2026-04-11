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

using System;
using System.ComponentModel;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Controller that shows two documents: one in the enabled state that can be changed, and one in the disabled state that cannot be changed.
  /// </summary>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  [ExpectedTypeOfView(typeof(IConditionalDocumentView))]
  public class ConditionalDocumentControllerWithDisabledView<TModel> : IConditionalDocumentController, IMVCANController where TModel : notnull
  {
    private Func<TModel> _documentCreationActionForEnabledState;
    private Func<TModel> _documentCreationActionForDisabledState;
    private Func<TModel, UseDocument, IMVCANController?> _controllerCreationAction;

    private IConditionalDocumentView? _view;
    private IMVCANController? _controllerForEnabledState;
    private IMVCANController? _controllerForDisabledState;
    private UseDocument _useDocumentCopy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalDocumentControllerWithDisabledView{TModel}"/> class.
    /// </summary>
    /// <param name="DocumentCreationActionForEnabledState">The action that creates the document for the enabled state.</param>
    /// <param name="DocumentCreationActionForDisabledState">The action that creates the document for the disabled state.</param>
    public ConditionalDocumentControllerWithDisabledView(
      Func<TModel> DocumentCreationActionForEnabledState,
      Func<TModel> DocumentCreationActionForDisabledState
      )
      : this(DocumentCreationActionForEnabledState, DocumentCreationActionForDisabledState, InternalCreateController)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalDocumentControllerWithDisabledView{TModel}"/> class.
    /// </summary>
    /// <param name="DocumentCreationActionForEnabledState">The action that creates the document for the enabled state.</param>
    /// <param name="DocumentCreationActionForDisabledState">The action that creates the document for the disabled state.</param>
    /// <param name="ControllerCreationAction">The action that creates a controller for the document.</param>
    public ConditionalDocumentControllerWithDisabledView(
      Func<TModel> DocumentCreationActionForEnabledState,
      Func<TModel> DocumentCreationActionForDisabledState,
      Func<TModel, UseDocument, IMVCANController?> ControllerCreationAction)
    {
      if (DocumentCreationActionForEnabledState is null)
        throw new ArgumentNullException(nameof(DocumentCreationActionForEnabledState));
      if (DocumentCreationActionForDisabledState is null)
        throw new ArgumentNullException(nameof(DocumentCreationActionForDisabledState));
      if (ControllerCreationAction is null)
        throw new ArgumentNullException(nameof(ControllerCreationAction));

      _documentCreationActionForEnabledState = DocumentCreationActionForEnabledState;
      _documentCreationActionForDisabledState = DocumentCreationActionForDisabledState;
      _controllerCreationAction = ControllerCreationAction;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    #region Binding

    private IMVCANController? _controller;

    /// <summary>
    /// Gets or sets the underlying controller that is currently active.
    /// </summary>
    public IMVCANController? UnderlyingController
    {
      get
      {
        return _controller;
      }
      set
      {
        if (!object.ReferenceEquals(_controller, value))
        {
          _controller = value;
          OnPropertyChanged(nameof(UnderlyingController));
          OnPropertyChanged(nameof(UnderlyingView));
          OnPropertyChanged(nameof(IsConditionalViewEnabled));
        }
      }
    }

    /// <summary>
    /// Gets the view object of the currently active underlying controller.
    /// </summary>
    public object? UnderlyingView => _controller?.ViewObject;

    /// <summary>
    /// Gets or sets a value indicating whether the enabled-state controller is active.
    /// </summary>
    public bool IsConditionalViewEnabled
    {
      get => _controller is not null && object.ReferenceEquals(_controller, _controllerForEnabledState);
      set
      {
        if (!(IsConditionalViewEnabled == value))
        {
          OnEnabledChanged(value);
        }
      }
    }

    private string _enablingText = "Enable";

    /// <summary>
    /// Gets or sets the text used to enable the conditional view.
    /// </summary>
    public string EnablingText
    {
      get => _enablingText;
      set
      {
        if (!(_enablingText == value))
        {
          _enablingText = value;
          OnPropertyChanged(nameof(EnablingText));
        }
      }
    }

    #endregion




    private static IMVCANController? InternalCreateController(TModel doc, UseDocument useDocumentCopy)
    {
      return (IMVCANController?)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(IMVCANController), useDocumentCopy);
    }

    /// <summary>
    /// Initialize the controller with the document. If successful, the function has to return true.
    /// Here, you can give two arguments. The first is the document for the enabled state, the second is the document to show in the disabled state.
    /// </summary>
    /// <param name="args">The arguments necessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
    /// <returns>
    /// Returns <see langword="true" /> if successful; otherwise <see langword="false" />.
    /// </returns>
    public bool InitializeDocument(params object[] args)
    {
      if (args is not null && args.Length >= 1 && args[0] is not null && (args[0] is TModel))
      {
        _controllerForEnabledState = _controllerCreationAction((TModel)args[0], _useDocumentCopy);
      }
      else
      {
      }

      if (args is not null && args.Length >= 2 && args[1] is not null && (args[1] is TModel))
      {
        _controllerForDisabledState = _controllerCreationAction((TModel)args[1], _useDocumentCopy);
      }

      UnderlyingController = _controllerForEnabledState ?? _controllerForDisabledState;

      Initialize(true);
      return true;
    }

    /// <inheritdoc/>
    public UseDocument UseDocumentCopy
    {
      set
      {
        _useDocumentCopy = value;
        if (_controllerForEnabledState is not null)
          _controllerForEnabledState.UseDocumentCopy = value;
        if (_controllerForDisabledState is not null)
          _controllerForDisabledState.UseDocumentCopy = value;
      }
    }

    /// <inheritdoc/>
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
          _view.DataContext = null;
        }
        _view = value as IConditionalDocumentView;

        if (_view is not null)
        {
          Initialize(false);
          _view.DataContext = this;
        }
      }
    }

    /// <inheritdoc/>
    public object ModelObject
    {
      get
      {
        object? result = null;

        if (IsConditionalViewEnabled)
          result = _controllerForEnabledState?.ModelObject;

        return result ?? new object();
      }
    }

    /// <summary>
    /// Gets the model object for the enabled controller, or <c>null</c> if none exists.
    /// </summary>
    public object? ModelObjectOrNull
    {
      get => _controllerForEnabledState?.ModelObject;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      _controller = null;
      _controllerForDisabledState?.Dispose();
      _controllerForEnabledState?.Dispose();
    }

    /// <inheritdoc/>
    public bool Apply(bool disposeController)
    {
      try
      {
        if (_controllerForEnabledState is not null)
          return _controllerForEnabledState.Apply(disposeController);
        else
          return true;
      }
      finally
      {
        if (disposeController)
        {
          Dispose();
        }
      }
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>true</c> if the revert operation was successful; otherwise, <c>false</c> if the revert operation was not possible.
    /// </returns>
    public bool Revert(bool disposeController)
    {
      if (disposeController)
      {
        Dispose();
      }
      return false;
    }

    private void Initialize(bool initData)
    {
    }

    /// <summary>
    /// Switches the active controller according to the enabled state.
    /// </summary>
    /// <param name="enableState">If set to <c>true</c>, the enabled controller is activated; otherwise, the disabled controller is activated.</param>
    public void OnEnabledChanged(bool enableState)
    {
      if (enableState)
      {
        if (_controllerForEnabledState is null)
        {
          TModel document = _documentCreationActionForEnabledState();
          _controllerForEnabledState = _controllerCreationAction(document, _useDocumentCopy);
        }
        UnderlyingController = _controllerForEnabledState;
      }
      else // disabled
      {
        if (_controllerForDisabledState is null)
        {
          TModel document = _documentCreationActionForDisabledState();
          _controllerForDisabledState = _controllerCreationAction(document, _useDocumentCopy);
        }
        UnderlyingController = _controllerForDisabledState;
      }
    }
  }
}
