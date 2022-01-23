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
  public interface IConditionalDocumentView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Interface for model in the view.
  /// </summary>
  public interface IConditionalDocumentController : INotifyPropertyChanged, IMVCANController
  {
    object? UnderlyingView { get; }
    string EnablingText { get; }

    bool IsConditionalViewEnabled { get; }

    object? ModelObjectOrNull { get; }
  }

  [ExpectedTypeOfView(typeof(IConditionalDocumentView))]
  public class ConditionalDocumentController<TModel> : IConditionalDocumentController, IMVCANController where TModel : notnull
  {
    private Func<TModel> _creationAction;
    private Action _removalAction;
    private Func<TModel, UseDocument, IMVCANController?> _controllerCreationAction;

    private IConditionalDocumentView? _view;
    private IMVCANController? _controller;
    private UseDocument _useDocumentCopy;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ConditionalDocumentController(Func<TModel> CreationAction, Action RemovalAction)
      : this(CreationAction, RemovalAction, InternalCreateController)
    {
    }

    public ConditionalDocumentController(Func<TModel> CreationAction, Action RemovalAction, Func<TModel, UseDocument, IMVCANController?> ControllerCreationAction)
    {
      if (CreationAction is null)
        throw new ArgumentNullException(nameof(CreationAction));
      if (RemovalAction is null)
        throw new ArgumentNullException(nameof(RemovalAction));
      if (ControllerCreationAction is null)
        throw new ArgumentNullException(nameof(ControllerCreationAction));

      _creationAction = CreationAction;
      _removalAction = RemovalAction;
      _controllerCreationAction = ControllerCreationAction;
    }

    #region Binding

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

    public object? UnderlyingView => _controller?.ViewObject;

    public bool IsConditionalViewEnabled
    {
      get => _controller is not null;
      set
      {
        if (!(IsConditionalViewEnabled == value))
        {
          OnEnabledChanged(value);
        }
      }
    }

    private string _enablingText = "Enable";

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

    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0 || args[0] is not TModel)
        return false;

      UnderlyingController = _controllerCreationAction((TModel)args[0], _useDocumentCopy);

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
        if (_view is { } oldView)
        {
          oldView.DataContext = null;
        }

        _view = value as IConditionalDocumentView;

        if (_view is not null)
        {
          Initialize(false);
          _view.DataContext = this;
        }
      }
    }

    public object ModelObject
    {
      get { return _controller?.ModelObject ?? new object(); }
    }

    public object? ModelObjectOrNull => _controller?.ModelObject;

    public void Dispose()
    {
      _controller?.Dispose();
    }

    public bool Apply(bool disposeController)
    {
      bool result;
      if (_controller is not null)
        result = _controller.Apply(disposeController);
      else
        result = true;

      if (disposeController)
      {
        Dispose();
      }
      return result;
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
      if (disposeController)
      {
        Dispose();
      }

      return false;
    }

    private void Initialize(bool initData)
    {
    }

    protected void OnEnabledChanged(bool enableState)
    {
      if (true == enableState && _controller is null)
      {
        if (_controller is null)
        {
          TModel document = _creationAction();
          UnderlyingController = _controllerCreationAction(document, _useDocumentCopy);
        }
      }
      else if (false == enableState && _controller is not null) // view is disabled
      {
        _removalAction();
        UnderlyingController = null;
      }
    }
  }
}
