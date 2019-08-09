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

using System;
using System.ComponentModel;

namespace Altaxo.Gui.Workbench
{
  public abstract class AbstractPadContent : IPadContent, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool _isVisible = true;
    protected bool _isSelected;
    protected bool _isActive;

    protected string _category;
    protected string _iconSource;
    protected string _title;
    protected string _shortCut;
    protected string _contentId;
    protected PadDescriptor _padDescriptor;

    /// <inheritdoc/>
    public abstract object ViewObject { get; set; }

    /// <inheritdoc/>
    public abstract object ModelObject { get; }

    /// <inheritdoc/>
    public virtual object InitiallyFocusedControl
    {
      get
      {
        return null;
      }
    }

    public virtual void Dispose()
    {
      ViewObject = null;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///
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

    public void BringToFront()
    {
      IsVisible = true;
      IsSelected = true;
    }

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
          if (null != _padDescriptor)
            _padDescriptor.Category = value;
          else
            _category = value;

          OnPropertyChanged(nameof(Category));
        }
      }
    }

    public string IconSource
    {
      get
      {
        return _iconSource ?? _padDescriptor?.Icon;
      }
    }

    public string Title
    {
      get
      {
        return _title ?? _padDescriptor?.Title;
      }
    }

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
          if (null != _padDescriptor)
            _padDescriptor.Shortcut = value;
          else
            _shortCut = value;

          OnPropertyChanged(nameof(Shortcut));
        }
      }
    }

    public string ContentId
    {
      get
      {
        return _contentId ?? _padDescriptor?.Class ?? (_contentId = Guid.NewGuid().ToString());
      }
    }

    public DefaultPadPositions DefaultPosition
    {
      get
      {
        return _padDescriptor?.DefaultPosition ?? DefaultPadPositions.Right;
      }
    }

    public virtual object GetService(Type serviceType)
    {
      if (serviceType.IsInstanceOfType(this))
        return this;
      else
        return null;
    }
  }
}
