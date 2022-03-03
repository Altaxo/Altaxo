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
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D.Background;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Graph3D.Background
{
  
  public interface IBackgroundStyleView : IDataContextAwareView
  {
  }

 
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

    public new IBackgroundStyle? Doc
    {
      get => _doc;
      set
      {
        InitializeDocument(value);
      }
    }

    #region Bindings

    private IMaterial _backgroundMaterial;

    public IMaterial BackgroundMaterial
    {
      get => _backgroundMaterial;
      set
      {
        if (!(_backgroundMaterial == value))
        {
          _backgroundMaterial = value;
          OnPropertyChanged(nameof(BackgroundMaterial));
          if (_doc is not null && _doc.SupportsUserDefinedMaterial)
          {
            _doc.Material = value;
            OnMadeDirty();
          }
        }
      }
    }

    private bool _isBackgroundMaterialEnabled;

    public bool IsBackgroundMaterialEnabled
    {
      get => _isBackgroundMaterialEnabled;
      set
      {
        if (!(_isBackgroundMaterialEnabled == value))
        {
          _isBackgroundMaterialEnabled = value;
          OnPropertyChanged(nameof(IsBackgroundMaterialEnabled));
        }
      }
    }

    private bool _showPlotColorsOnly;

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

      if (_doc is not null && _doc.SupportsUserDefinedMaterial)
      {
        if (BackgroundMaterial.IsVisible)
          _doc.Material = BackgroundMaterial;
        else
          BackgroundMaterial = _doc.Material;

        IsBackgroundMaterialEnabled = true;
      }
      else
      {
        IsBackgroundMaterialEnabled = false;
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

        if (_doc is not null && _doc.SupportsUserDefinedMaterial)
          BackgroundMaterial = _doc.Material;
        else
          BackgroundMaterial = Materials.GetNoMaterial();

        IsBackgroundMaterialEnabled = (_doc is not null && _doc.SupportsUserDefinedMaterial);
      }
    }
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    /// <summary>
    /// Overridden because here _doc can be null.
    /// </summary>
    protected override void ThrowIfNotInitialized()
    {
    }
  }
}
