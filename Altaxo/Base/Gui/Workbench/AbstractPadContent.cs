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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Base class for pad content view models.
  /// </summary>
  public abstract class AbstractPadContent : IPadContent, INotifyPropertyChanged
  {
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Stores whether the pad is visible.
    /// </summary>
    protected bool _isVisible = true;

    /// <summary>
    /// Stores whether the pad is selected.
    /// </summary>
    protected bool _isSelected;

    /// <summary>
    /// Stores whether the pad is active.
    /// </summary>
    protected bool _isActive;

    /// <summary>
    /// Stores the pad category when no descriptor is assigned.
    /// </summary>
    protected string? _category;

    /// <summary>
    /// Stores the icon source when no descriptor is assigned.
    /// </summary>
    protected string? _iconSource;

    /// <summary>
    /// Stores the pad title when no descriptor is assigned.
    /// </summary>
    protected string? _title;

    /// <summary>
    /// Stores the shortcut when no descriptor is assigned.
    /// </summary>
    protected string? _shortCut;

    /// <summary>
    /// Stores the content identifier when no descriptor is assigned.
    /// </summary>
    protected string? _contentId;

    /// <summary>
    /// Stores the descriptor that defines the pad metadata.
    /// </summary>
    protected PadDescriptor? _padDescriptor;

    /// <inheritdoc/>
    public abstract object? ViewObject { get; set; }

    /// <inheritdoc/>
    public abstract object ModelObject { get; }

    /// <inheritdoc/>
    public virtual object? InitiallyFocusedControl
    {
      get
      {
        return null;
      }
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
      ViewObject = null;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Gets or sets the visibility of the pad.
    /// If false, the pad is not visible.
    /// If true, the pad may be visible or is collapsed.
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
          OnPropertyChanged(nameof(IsVisible));
        }
      }
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
          _isSelected = value;
          OnPropertyChanged(nameof(IsSelected));
        }
      }
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
          _isActive = value;
          OnPropertyChanged(nameof(IsActive));
        }
      }
    }

    /// <summary>
    /// Brings the pad to the front by making it visible and selected.
    /// </summary>
    public void BringToFront()
    {
      IsVisible = true;
      IsSelected = true;
    }

    /// <inheritdoc/>
    [MaybeNull]
    public virtual PadDescriptor PadDescriptor
    {
      get
      {
        return _padDescriptor;
      }
      set
      {
        _padDescriptor = value;
      }
    }

    /// <inheritdoc/>
    [MaybeNull]
    public string Category
    {
      get
      {
        return _category ?? _padDescriptor?.Category;
      }
      set
      {
        if (!(Category == value))
        {
          if (_padDescriptor is not null)
            _padDescriptor.Category = value;
          else
            _category = value;

          OnPropertyChanged(nameof(Category));
        }
      }
    }

    /// <inheritdoc/>
    public string? IconSource
    {
      get
      {
        return _iconSource ?? _padDescriptor?.Icon;
      }
    }

    /// <inheritdoc/>
    public string? Title
    {
      get
      {
        return _title ?? _padDescriptor?.Title;
      }
    }

    /// <inheritdoc/>
    [MaybeNull]
    public string Shortcut
    {
      get
      {
        return _shortCut ?? _padDescriptor?.Shortcut;
      }
      set
      {
        if (!(Shortcut == value))
        {
          if (_padDescriptor is not null)
            _padDescriptor.Shortcut = value;
          else
            _shortCut = value;

          OnPropertyChanged(nameof(Shortcut));
        }
      }
    }

    /// <inheritdoc/>
    public string ContentId
    {
      get
      {
        return _contentId ?? _padDescriptor?.Class ?? (_contentId = Guid.NewGuid().ToString());
      }
    }

    /// <inheritdoc/>
    public DefaultPadPositions DefaultPosition
    {
      get
      {
        return _padDescriptor?.DefaultPosition ?? DefaultPadPositions.Right;
      }
    }

    /// <summary>
    /// Gets a service provided by this pad content.
    /// </summary>
    /// <param name="serviceType">The requested service type.</param>
    /// <returns>The requested service, or <c>null</c> if it is not available.</returns>
    public virtual object? GetService(Type serviceType)
    {
      if (serviceType.IsInstanceOfType(this))
        return this;
      else
        return null;
    }
  }
}
