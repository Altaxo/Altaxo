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
using System.Text;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Background;

namespace Altaxo.Gui.Graph.Gdi.Background
{
  #region Interfaces

  public interface IBackgroundStyleView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Controls a IBackgroundStyle instance. 
  /// </summary>
  [UserControllerForObject(typeof(IBackgroundStyle))]
  [ExpectedTypeOfView(typeof(IBackgroundStyleView))]
  public class BackgroundStyleController : MVCANDControllerEditOriginalDocBase<IBackgroundStyle, IBackgroundStyleView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public BackgroundStyleController(IBackgroundStyle? doc)
    {
      _doc = doc;
      Initialize(true);
    }

    #region Bindings

    private BrushX _backgroundBrush;

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

    private SelectableListNodeList _backgroundStyles = new SelectableListNodeList();

    /// <summary>
    /// Gets or sets the list background styles. Tag is of type <see cref="System.Type"/>, namely the type of the background style.
    /// </summary>
    public SelectableListNodeList BackgroundStyles
    {
      get => _backgroundStyles;
      set
      {
        if (!(_backgroundStyles == value))
        {
          _backgroundStyles = value;
          OnPropertyChanged(nameof(BackgroundStyles));
        }
      }
    }

    private Type _selectedBackgroundStyle;

    public Type SelectedBackgroundStyle
    {
      get => _selectedBackgroundStyle;
      set
      {
        if (!(_selectedBackgroundStyle == value))
        {
          _selectedBackgroundStyle = value;
          OnPropertyChanged(nameof(SelectedBackgroundStyle));
          EhView_BackgroundStyleChanged(value);
        }
      }
    }

    /// <summary>
    /// Called if the background style changed.
    /// </summary>
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
       var backgroundStyles = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBackgroundStyle));
        _backgroundStyles.Clear();
        _backgroundStyles.Add(new SelectableListNode("None", null, _doc is null));
        foreach (var backtype in backgroundStyles)
          _backgroundStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(backtype), backtype, _doc?.GetType() == backtype));
        _selectedBackgroundStyle = _doc?.GetType();

        if (_doc is not null && _doc.SupportsBrush)
          _backgroundBrush = _doc.Brush;
        else
          _backgroundBrush = BrushesX.Transparent;

        _isBackgroundBrushEnabled = (_doc is not null && _doc.SupportsBrush);
      }
    }
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    public override object ModelObject => _doc; // override because the return value can be null

    public override object ProvisionalModelObject // override because the return value can be null
    {
      get
      {
        return _doc;
      }
    }
  }
}
