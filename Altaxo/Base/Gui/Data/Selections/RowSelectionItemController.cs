#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data.Selections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Data.Selections
{
  public interface IRowSelectionItemView : IDataContextAwareView
  {
  }

  public class RowSelectionItemController : MVCANControllerEditImmutableDocBase<IRowSelection, IRowSelectionItemView>
  {
    RowSelectionController _parent;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_detailsController, () => DetailsController = null);
    }

    public RowSelectionItemController(IRowSelection doc, RowSelectionController parent)
    {
      _doc = _originalDoc = doc;
      _parent = parent;
      Initialize(true);
    }

    #region Bindings


    private IMVCANController _detailsController;

    public IMVCANController DetailsController
    {
      get => _detailsController;
      set
      {
        if (!(_detailsController == value))
        {
          _detailsController?.Dispose();
          _detailsController = value;
          OnPropertyChanged(nameof(DetailsController));
        }
      }
    }

    private ItemsController<Type> _rowSelectionTypes;

    public ItemsController<Type> RowSelectionTypes
    {
      get => _rowSelectionTypes;
      set
      {
        if (!(_rowSelectionTypes == value))
        {
          _rowSelectionTypes?.Dispose();
          _rowSelectionTypes = value;
          OnPropertyChanged(nameof(RowSelectionTypes));
        }
      }
    }

    ICommand _cmdAddNewSelection;
    public ICommand CmdAddNewSelection => _cmdAddNewSelection ??= new RelayCommand(EhAddNewSelection);



    ICommand _cmdUnindentSelection;
    public ICommand CmdUnindentSelection => _cmdUnindentSelection ??= new RelayCommand(EhUnindentSelection);

    ICommand _cmdIndentSelection;
    public ICommand CmdIndentSelection => _cmdIndentSelection ??= new RelayCommand(EhIndentSelection);

    ICommand _cmdRemoveSelection;
    public ICommand CmdRemoveSelection => _cmdRemoveSelection ??= new RelayCommand(EhRemoveSelection);

    private int _indentationLevel;

    public int IndentationLevel
    {
      get => _indentationLevel;
      set
      {
        if (!(_indentationLevel == value))
        {
          _indentationLevel = value;
          OnPropertyChanged(nameof(IndentationLevel));
        }
      }
    }

    private bool _isInterSection;

    /// <summary>
    /// If true, this item is connected with the next item on the same level by intersection. If false, it is connected by union.
    /// </summary>
    public bool IsInterSection
    {
      get => _isInterSection;
      set
      {
        if (!(_isInterSection == value))
        {
          _isInterSection = value;
          OnPropertyChanged(nameof(IsInterSection));
        }
      }
    }


    public IRowSelection RowSelection
    {
      get => _doc;
      set
      {
        if (!(_doc == value))
        {
          _doc = value;
          OnPropertyChanged(nameof(RowSelection));
        }
      }
    }


    #endregion

    SelectableListNodeList _rowSelectionSimpleTypes;
    SelectableListNodeList _rowSelectionCollectionTypes;

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        // available row selection types
        var types = ReflectionService.GetNonAbstractSubclassesOf(typeof(IRowSelection));
        _rowSelectionSimpleTypes = new SelectableListNodeList();
        _rowSelectionCollectionTypes = new SelectableListNodeList();
        foreach (var type in types)
        {
          if (typeof(IRowSelectionCollection).IsAssignableFrom(type))
            _rowSelectionCollectionTypes.Add(new SelectableListNode(type.Name, type, false));
          else
            _rowSelectionSimpleTypes.Add(new SelectableListNode(type.Name, type, false));
        }

        if(_doc is IRowSelectionCollection)
        {
          RowSelectionTypes = new ItemsController<Type>(_rowSelectionCollectionTypes, EhRowSelectionTypeChanged);
        }
        else
        {
          RowSelectionTypes = new ItemsController<Type>(_rowSelectionSimpleTypes, EhRowSelectionTypeChanged);
        }
        RowSelectionTypes.SelectedValue = _doc?.GetType();
      }

      CreateDetailsController(_doc);
    }

   

    public override bool Apply(bool disposeController)
    {
      if(DetailsController is not null)
      {
        if (false == DetailsController.Apply(disposeController))
          return ApplyEnd(false, disposeController);
        else
          _doc = (IRowSelection)DetailsController.ModelObject;
      }

      return ApplyEnd(true, disposeController);
    }

    private void EhRowSelectionTypeChanged(Type newRowSelectionType)
    {
      if (newRowSelectionType is not null && newRowSelectionType != _doc.GetType())
      {
        // Create an item of the new type
        _doc = (IRowSelection)Activator.CreateInstance(newRowSelectionType);
        CreateDetailsController(_doc);
        _parent.EhSelectionTypeChanged(this);
      }
    }

    private void CreateDetailsController(IRowSelection doc)
    {
      var controller = (IMVCANController)Current.Gui.GetController(new object[] { doc, _parent._supposedParentDataTable }, typeof(IMVCANController));
      if (controller is RowSelectionController)
        controller = null;

      if (controller is not null)
        Current.Gui.FindAndAttachControlTo(controller);

      DetailsController = controller;
    }


    private void EhAddNewSelection()
    {
      _parent.EhCmdAddNewSelection(this);
    }

    private void EhUnindentSelection()
    {
      _parent.EhCmdUnindentSelection(this);
    }

    private void EhIndentSelection()
    {
      _parent.EhCmdIndentSelection(this);
    }

    private void EhRemoveSelection()
    {
      _parent.EhCmdRemoveSelection(this);
    }

  }
}
