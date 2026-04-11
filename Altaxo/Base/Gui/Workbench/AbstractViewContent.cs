// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Altaxo.Main;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Provides a default implementation for the IViewContent interface.
  /// It provides a files collection that, by default, automatically registers the view with the
  /// files added to it.
  /// Several properties have default implementation that depend on the contents of the files collection,
  /// e.g. IsDirty is true when at least one of the files in the collection is dirty.
  /// To support the changed event, this class registers event handlers with the members of the files collection.
  ///
  /// When used with an empty Files collection, IsViewOnly will return true and this class can be used as a base class
  /// for view contents not using files.
  /// </summary>
  public abstract class AbstractViewContent : IViewContent
  {
    /// <summary>
    /// Stores whether the view content is active.
    /// </summary>
    protected bool _isActive;

    /// <summary>
    /// Stores whether the view content is selected.
    /// </summary>
    protected bool _isSelected;

    /// <summary>
    /// Stores whether the view content is visible.
    /// </summary>
    protected bool _isVisible = true;

    /// <summary>
    /// Stores the current title.
    /// </summary>
    protected string? _title;

    /// <summary>
    /// Stores the localized title source.
    /// </summary>
    protected LanguageDependentString? _titleToBeLocalized;

    private string? _infoTip;

    private LanguageDependentString? _infoTipToBeLocalized;

    private ICommand? _closeCommand;

    private bool _isDirty;

    private IServiceContainer? _services;

    /// <summary>
    /// Gets or sets a value indicating whether disposal is in progress.
    /// </summary>
    public bool IsDisposeInProgress { get; private set; }
    private bool _isDisposed;

    /// <summary>
    /// Occurs when the dirty state changes.
    /// </summary>
    public event EventHandler? IsDirtyChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public virtual bool CloseWithSolution
    {
      get
      {
        return true;
      }
    }

    /// <inheritdoc/>
    public abstract object? ViewObject
    {
      get; set;
    }

    /// <inheritdoc/>
    public virtual object? InitiallyFocusedControl
    {
      get { return null; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the view of this instance is active in the UI.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the view of this instance is active in the UI; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive
    {
      get
      {
        return _isActive;
      }
      set
      {
        if (!(_isActive == value))
        {
          var oldIsContentVisible = IsContentVisible;

          _isActive = value;

          if (!(IsDisposeInProgress || IsDisposed)) // Fire events only if not dispose is in progress
          {
            OnPropertyChanged(nameof(IsActive));

            if (oldIsContentVisible != IsContentVisible)
              OnPropertyChanged(nameof(IsContentVisible));
          }
        }
      }
    }

    /// <summary>
    /// Gets if the view content is read-only (can be saved only when choosing another file name).
    /// </summary>
    public virtual bool IsReadOnly
    {
      get { return false; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the view of this instance is selected (e.g. inside its container).
    /// </summary>
    /// <value>
    ///   <c>true</c> if the view of this instance is selected; otherwise, <c>false</c>.
    /// </value>
    public bool IsSelected
    {
      get
      {
        return _isSelected;
      }
      set
      {
        if (!(_isSelected == value))
        {
          var oldIsContentVisible = IsContentVisible;
          _isSelected = value;

          if (!(IsDisposeInProgress || IsDisposed)) // Fire events only if not dispose is in progress
          {
            OnPropertyChanged(nameof(IsSelected));

            if (oldIsContentVisible != IsContentVisible)
              OnPropertyChanged(nameof(IsContentVisible));
          }
        }
      }
    }

    /// <summary>
    /// Gets if the view content is view-only (cannot be saved at all).
    /// </summary>
    public virtual bool IsViewOnly
    {
      get { return false; }
    }

    /// <summary>
    /// Gets or sets the visibility of the document.
    /// If false, the document tab header is not visible (but the document itself maybe visible !).
    /// If true, the document tab header is visible (if it fits in the bar),
    /// and the document is visible, if it is selected, too.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    public bool IsVisible
    {
      get
      {
        return _isVisible;
      }
      set
      {
        if (!(_isVisible == value))
        {
          _isVisible = value;

          if (!(IsDisposeInProgress || IsDisposed)) // Fire events only if not dispose is in progress
          {
            OnPropertyChanged(nameof(IsVisible));
          }
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the content of this document window is visible (it is if either <see cref="IsActive"/> or <see cref="IsSelected"/> is visible.
    /// </summary>
    public bool IsContentVisible
    {
      get
      {
        return _isActive | _isSelected;
      }
    }

    #region Title

    /// <summary>
    /// Gets/Sets the title of the current tab page.
    /// This value will be passed through the string parser before being displayed.
    /// </summary>
    public virtual string Title
    {
      get
      {
        return _title ?? string.Empty;
      }
      set
      {
        if (_titleToBeLocalized is not null)
        {
          _titleToBeLocalized.ValueChanged -= EhTitleLocalizationChanged;
          _titleToBeLocalized = null;
        }

        if (!(_title == value))
        {
          _title = value;
          OnPropertyChanged(nameof(Title));
        }
      }
    }

    /// <summary>
    /// Gets or sets the icon source.
    /// </summary>
    public virtual string? IconSource
    {
      get
      {
        return null;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Sets a localized title that will update automatically when the language changes.
    /// </summary>
    /// <param name="text">The input to the string parser which will localize title.</param>
    protected virtual void SetLocalizedTitle(string text)
    {
      _titleToBeLocalized = new LanguageDependentString(text);
      _titleToBeLocalized.ValueChanged += EhTitleLocalizationChanged;
      EhTitleLocalizationChanged(_titleToBeLocalized, EventArgs.Empty);
    }

    /// <summary>
    /// Updates the title when the localized title changes.
    /// </summary>
    /// <param name="sender">The localization source that raised the change notification.</param>
    /// <param name="e">The event data.</param>
    protected virtual void EhTitleLocalizationChanged(object? sender, EventArgs e)
    {
      var value = _titleToBeLocalized?.Value;
      if (!(_title == value))
      {
        _title = value;
        OnPropertyChanged(nameof(Title));
      }
    }

    #endregion Title

    #region ContentId

    /// <summary>
    /// Gets or sets the content identifier. Here, the content identifier is calculated of the reference hash of the document.
    /// Setting is not implemented here.
    /// </summary>
    /// <value>
    /// The content identifier.
    /// </value>
    public virtual string ContentId
    {
      get
      {
        var model = ModelObject;

        // Get the project item, either directly or via its presentation model
        var projectItem = model as IProjectItem;
        if (projectItem is null && model is IProjectItemPresentationModel pipModel)
        {
          projectItem = pipModel.Document;
        }

        if (projectItem is not null)
        {
          var name = Altaxo.Main.AbsoluteDocumentPath.GetPathString(projectItem, 16);
          // System.Diagnostics.Debug.WriteLine("GetName: " + name);
          return name;
        }
        else if (model is not null)
        {
          return "ContentHash:" + RuntimeHelpers.GetHashCode(model).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
          return "ContentHash:Empty";
        }
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Can be used to notify the view content that the <see cref="ContentId"/> maybe has changed.
    /// </summary>
    public virtual void NotifyContentIdChanged()
    {
      OnPropertyChanged(nameof(ContentId));
    }

    #endregion ContentId

    /// <inheritdoc/>
    public virtual INavigationPoint? BuildNavPoint()
    {
      return null;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region CloseCommand

    /// <summary>
    /// Gets the command that closes the view content.
    /// </summary>
    public System.Windows.Input.ICommand CloseCommand
    {
      get
      {
        if (_closeCommand is null)
        {
          _closeCommand = Current.Gui.NewRelayCommand(OnClose, CanClose);
        }

        return _closeCommand;
      }
    }

    /// <summary>
    /// Determines whether the view content can be closed.
    /// </summary>
    /// <returns><c>true</c> if the view content can be closed; otherwise, <c>false</c>.</returns>
    protected virtual bool CanClose()
    {
      return true;
    }

    /// <summary>
    /// Closes the view content.
    /// </summary>
    protected virtual void OnClose()
    {
      Current.Workbench.CloseContent(this);
    }

    #endregion CloseCommand

    #region InfoTip

    /// <summary>
    /// Gets or sets the informational tooltip for the view content.
    /// </summary>
    public string? InfoTip
    {
      get { return _infoTip; }
      set
      {
        if (_infoTipToBeLocalized is not null)
        {
          _infoTipToBeLocalized.ValueChanged -= EhInfoTipLocalizationChanged;
          _infoTipToBeLocalized = null;
        }

        if (!(_infoTip == value))
        {
          _infoTip = value;
          OnPropertyChanged(nameof(InfoTip));
        }
      }
    }

    /// <summary>
    /// Sets a localized info tip that will update automatically when the language changes.
    /// </summary>
    /// <param name="text">The input to the string parser which will localize info tip.</param>
    protected void SetLocalizedInfoTip(string text)
    {
      _infoTipToBeLocalized = new LanguageDependentString(text);
      _infoTipToBeLocalized.ValueChanged += EhInfoTipLocalizationChanged;
      EhInfoTipLocalizationChanged(_infoTipToBeLocalized, EventArgs.Empty);
    }

    private void EhInfoTipLocalizationChanged(object? sender, EventArgs e)
    {
      var value = _infoTipToBeLocalized?.Value;
      if (!(_infoTip == value))
      {
        _infoTip = value;
        OnPropertyChanged(nameof(InfoTip));
      }
    }

    #endregion InfoTip

    #region IDisposable

    /// <summary>
    /// Occurs when the view content has been disposed.
    /// </summary>
    public event EventHandler? Disposed;

    /// <summary>
    /// Gets a value indicating whether the view content has been disposed.
    /// </summary>
    public bool IsDisposed
    {
      get { return _isDisposed; }
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
      _isDisposed = true;

      Disposed?.Invoke(this, EventArgs.Empty);
      OnPropertyChanged(nameof(IsDisposed));
    }

    /// <inheritdoc/>
    public void SetDisposeInProgress()
    {
      IsDisposeInProgress = true;
    }

    #endregion IDisposable

    #region IsDirty

    /// <inheritdoc/>
    public virtual bool IsDirty
    {
      get { return _isDirty; }
    }

    /// <summary>
    /// Raises notifications that the dirty state has changed.
    /// </summary>
    public virtual void OnIsDirtyChanged()
    {
      IsDirtyChanged?.Invoke(this, EventArgs.Empty);
      OnPropertyChanged(nameof(IsDirty));
    }

    /// <summary>
    /// Clears the dirty state.
    /// </summary>
    public void ClearIsDirty()
    {
      if (!(_isDirty == false))
      {
        _isDirty = false;
        OnIsDirtyChanged();
      }
    }

    /// <summary>
    /// Sets the dirty state.
    /// </summary>
    public void SetDirty()
    {
      if (!(_isDirty == true))
      {
        _isDirty = true;
        OnIsDirtyChanged();
      }
    }

    #endregion IsDirty

    #region IServiceProvider

    /// <summary>
    /// Gets or sets the service container for the view content.
    /// </summary>
    public IServiceContainer? Services
    {
      get
      {
        return _services ?? (_services = new ServiceContainer());
      }
      protected set
      {
        _services = value ?? throw new ArgumentNullException(nameof(value));
      }
    }

    /// <inheritdoc/>
    public abstract object ModelObject { get; }

    /// <summary>
    /// Gets a required service of the specified type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The requested service.</returns>
    public T GetService<T>()
    {
      return (T)(GetService(typeof(T)) ?? throw new InvalidOperationException($"Service of type {typeof(T)} not found on {this}"));
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType)
    {
      var obj = _services?.GetService(serviceType);

      if (obj is not null)
        return obj;

      if (serviceType.IsInstanceOfType(this))
        return this;

      return null;
    }

    #endregion IServiceProvider
  }
}
