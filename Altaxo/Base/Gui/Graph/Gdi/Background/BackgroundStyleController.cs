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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi.Background
{
  /// <summary>
  /// Provides the view for editing 2D background styles.
  /// </summary>
  public interface IBackgroundStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controls an <see cref="IBackgroundStyle"/> instance.
  /// </summary>
  [UserControllerForObject(typeof(IBackgroundStyle))]
  [ExpectedTypeOfView(typeof(IBackgroundStyleView))]
  public class BackgroundStyleController : MVCANDControllerEditOriginalDocBase<IBackgroundStyle, IBackgroundStyleView>
  {
    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundStyleController"/> class.
    /// </summary>
    /// <param name="doc">The background style to edit.</param>
    public BackgroundStyleController(IBackgroundStyle? doc)
    {
      _doc = doc;
      Initialize(true);
    }

    /// <summary>
    /// Gets or sets the background style being edited.
    /// </summary>
    public new IBackgroundStyle? Doc
    {
      get => _doc;
      set
      {
        InitializeDocument(value);
      }
    }

    #region Bindings

    private BrushX _backgroundBrush;

    /// <summary>
    /// Gets or sets the brush used for the background.
    /// </summary>
    public BrushX BackgroundBrush
    {
      get => _backgroundBrush;
      set
      {
        if (!(_backgroundBrush == value))
        {
          _backgroundBrush = value;
          OnPropertyChanged(nameof(BackgroundBrush));
          if (_doc is not null && _doc.SupportsBrush)
          {
            _doc.Brush = value;
            OnMadeDirty();
          }
        }
      }
    }

    private bool _isBackgroundBrushEnabled;

    /// <summary>
    /// Gets or sets a value indicating whether the background brush can be edited.
    /// </summary>
    public bool IsBackgroundBrushEnabled
    {
      get => _isBackgroundBrushEnabled;
      set
      {
        if (!(_isBackgroundBrushEnabled == value))
        {
          _isBackgroundBrushEnabled = value;
          OnPropertyChanged(nameof(IsBackgroundBrushEnabled));
        }
      }
    }

    private bool _showPlotColorsOnly;

    /// <summary>
    /// Gets or sets a value indicating whether only plot colors are offered for selection.
    /// </summary>
    public bool ShowPlotColorsOnly
    {
      get => _showPlotColorsOnly;
      set
      {
        if (!(_showPlotColorsOnly == value))
        {
          _showPlotColorsOnly = value;
          OnPropertyChanged(nameof(ShowPlotColorsOnly));
        }
      }
    }


    private ItemsController<Type?> _backgroundStyles;

    /// <summary>
    /// Gets or sets the available background style types.
    /// </summary>
    public ItemsController<Type?> BackgroundStyles
    {
      get => _backgroundStyles;
      set
      {
        if (!(_backgroundStyles == value))
        {
          _backgroundStyles?.Dispose();
          _backgroundStyles = value;
          OnPropertyChanged(nameof(BackgroundStyles));
        }
      }
    }

    /// <summary>
    /// Called when the selected background style changes.
    /// </summary>
    /// <param name="newType">The newly selected background style type.</param>
    public void EhView_BackgroundStyleChanged(Type? newType)
    {
      if (newType is not null)
      {
        _doc = (IBackgroundStyle)Activator.CreateInstance(newType);
      }
      else // is null
      {
        _doc = null;
      }

      if (_doc is not null && _doc.SupportsBrush)
      {
        if (BackgroundBrush.IsVisible)
          _doc.Brush = BackgroundBrush;
        else
          BackgroundBrush = _doc.Brush;

        IsBackgroundBrushEnabled = true;
      }
      else
      {
        IsBackgroundBrushEnabled = false;
      }

      OnMadeDirty();
    }


    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      // base.Initialize, but without test if document is null
      if (initData)
      {
        if (_useDocumentCopy && _suspendToken is null)
          _suspendToken = GetSuspendTokenForControllerDocument();
      }

      if (initData)
      {
        if (BackgroundStyles is null)
        {
          var backgroundStyles = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBackgroundStyle));
          var styles = new SelectableListNodeList();
          styles.Add(new SelectableListNode("None", null, _doc is null));
          foreach (var backtype in backgroundStyles)
            styles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(backtype), backtype, _doc?.GetType() == backtype));
          BackgroundStyles = new ItemsController<Type?>(styles, EhView_BackgroundStyleChanged);
        }
        else
        {
          BackgroundStyles.SelectedValue = _doc?.GetType();
        }

        if (_doc is not null && _doc.SupportsBrush)
          BackgroundBrush = _doc.Brush;
        else
          BackgroundBrush = BrushesX.Transparent;

        IsBackgroundBrushEnabled = (_doc is not null && _doc.SupportsBrush);
      }
    }
    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Overridden because <c>_doc</c> can be <see langword="null"/> here.
    /// </remarks>
    protected override void ThrowIfNotInitialized()
    {
    }
  }
}
