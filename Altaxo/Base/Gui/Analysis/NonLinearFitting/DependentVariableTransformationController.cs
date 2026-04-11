#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data.Transformations;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Defines the view contract for editing dependent-variable transformations.
  /// </summary>
  public interface IDependentVariableTransformationView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing a dependent-variable transformation.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDependentVariableTransformationView))]
  public class DependentVariableTransformationController : MVCANControllerEditImmutableDocBase<IDoubleToDoubleTransformation?, IDependentVariableTransformationView>
  {
    /// <summary>All types of available column transformations.</summary>
    protected SelectableListNodeList _availableTransformations = new SelectableListNodeList();
    string _variableName = "y";

    /// <summary>
    /// Initializes a new instance of the <see cref="DependentVariableTransformationController"/> class.
    /// </summary>
    public DependentVariableTransformationController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependentVariableTransformationController"/> class.
    /// </summary>
    /// <param name="doc">The transformation to edit.</param>
    /// <param name="variableName">The dependent variable name.</param>
    public DependentVariableTransformationController(IDoubleToDoubleTransformation? doc, string variableName="y")
    {
      _doc = doc;
      _variableName = variableName ?? "y";
      Initialize(true);
    }

    /// <inheritdoc/>
    protected override void CheckDocumentInitialized<T>([AllowNull, NotNull] ref T doc)
    {
    }

    /// <inheritdoc/>
    protected override void ThrowIfNotInitialized()
    {
    }

    

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      if (IsDisposed)
        throw new ObjectDisposedException("The controller was already disposed. Type: " + GetType().FullName);

      if(initData)
      {
        Controller_AvailableTransformations_Initialize();
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Transformation

    private void Controller_AvailableTransformations_Initialize()
    {
      _availableTransformations.Clear();

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IDoubleToDoubleTransformation));

      foreach (var t in types)
      {
        if (t.IsNestedPrivate)
          continue; // types that are declared private will not be listed

        if (!(true == t.GetConstructor(Type.EmptyTypes)?.IsPublic))
          continue; // don't has an empty public constructor

        _availableTransformations.Add(new SelectableListNode(t.Name, t, false));
      }
    }

    private static IDoubleToDoubleTransformation EditAvailableTransformation(IDoubleToDoubleTransformation createdTransformation, out bool wasEdited)
    {
      wasEdited = false;
      if (createdTransformation is not null && createdTransformation.IsEditable)
      {
        var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { createdTransformation }, typeof(IMVCANController));
        if (controller is not null && controller.ViewObject is not null)
        {
          if (Current.Gui.ShowDialog(controller, "Edit " + createdTransformation.GetType().Name))
          {
            createdTransformation = (IDoubleToDoubleTransformation)controller.ModelObject;
            wasEdited = true;
          }
        }
      }
      return createdTransformation;
    }

    private void EhTransformation_AddMultiple(Type transformationType, int multipleType)
    {
      // make sure we can create that transformation
      IDoubleToDoubleTransformation createdTransformation = null;
      try
      {
        createdTransformation = (IDoubleToDoubleTransformation)System.Activator.CreateInstance(transformationType);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("This column could not be created, message: " + ex.ToString(), "Error");
        return;
      }

      createdTransformation = EditAvailableTransformation(createdTransformation, out var wasEdited);

      switch (multipleType)
      {
        case 0: // as single
          _doc = createdTransformation;
          break;

        case 1: // prepend
          if (_doc is Altaxo.Data.Transformations.CompoundTransformation ct1)
            _doc = ct1.WithPrependedTransformation(createdTransformation);
          else if (_doc is not null)
            _doc = new Altaxo.Data.Transformations.CompoundTransformation(new[] { _doc, createdTransformation });
          else
            _doc = createdTransformation;
          break;

        case 2: // append
          if (_doc is Altaxo.Data.Transformations.CompoundTransformation ct2)
            _doc = ct2.WithAppendedTransformation(createdTransformation);
          else if (_doc is not null)
            _doc = new Altaxo.Data.Transformations.CompoundTransformation(new[] { createdTransformation, _doc });
          else
            _doc = createdTransformation;
          break;

        default:
          throw new NotImplementedException();
      }
      Update();
    }

    /// <summary>
    /// Adds the selected transformation.
    /// </summary>
    public void EhView_TransformationAddTo()
    {
      var node = _availableTransformations.FirstSelectedNode;
      if (node is not null)
      {
        if (_doc is null)
        {
          EhTransformation_AddMultiple((Type)node.Tag, 0);
        }
        else
        {
          IsTransformationPopupOpen = true; // this will eventually fire one of three commands to add as single, as prepend or as append transformation
        }
      }
      Update();
    }

    /// <summary>
    /// Replaces the current transformation with the selected transformation.
    /// </summary>
    public void EhView_TransformationAddAsSingle()
    {
      IsTransformationPopupOpen = false;

      var node = _availableTransformations.FirstSelectedNode;
      if (node is not null)
      {
        EhTransformation_AddMultiple((Type)node.Tag, 0);
      }
    }

    /// <summary>
    /// Prepends the selected transformation.
    /// </summary>
    public void EhView_TransformationAddAsPrepending()
    {
      IsTransformationPopupOpen = false;


      var node = _availableTransformations.FirstSelectedNode;
      if (node is not null)
      {
        EhTransformation_AddMultiple((Type)node.Tag, 1);
      }
    }

    /// <summary>
    /// Appends the selected transformation.
    /// </summary>
    public void EhView_TransformationAddAsAppending()
    {
      IsTransformationPopupOpen = false;

      var node = _availableTransformations.FirstSelectedNode;
      if (node is not null)
      {
        EhTransformation_AddMultiple((Type)node.Tag, 2);
      }
    }

    /// <summary>
    /// Edits the current transformation.
    /// </summary>
    public void EhView_TransformationEdit()
    {
      _doc = EditAvailableTransformation(_doc, out var wasEdited);
      Update();
    }

    /// <summary>
    /// Removes the current transformation.
    /// </summary>
    public void EhView_TransformationErase()
    {
      _doc = null;
      Update();
    }

    #endregion Transformation

    /// <summary>
    /// Refreshes the text shown for the current transformation.
    /// </summary>
    public void Update()
    {
      OnPropertyChanged(nameof(TransformationTextToShow));
      OnPropertyChanged(nameof(TransformationToolTip));
    }

    /// <summary>
    /// Gets the transformation text shown in the view.
    /// </summary>
    public string TransformationTextToShow
    {
      get
      {
        if (_doc is { } doc)
        {
          return doc.RepresentationAsOperator ?? doc.RepresentationAsFunction;
        }
        else
        {
          return string.Empty;
        }
      }
    }

    /// <summary>
    /// Gets the tooltip text for the current transformation.
    /// </summary>
    public string TransformationToolTip
    {
      get
      {
        if (_doc is { } doc)
        {
          return $"Transforms the fit function output by {_variableName}'={doc.GetRepresentationAsFunction(_variableName)}";
        }
        else
        {
          return "No transformation applied";
        }
      }
    }

    private bool _isTransformationPopupOpen;

    /// <summary>
    /// Gets or sets a value indicating whether the transformation popup is open.
    /// </summary>
    public bool IsTransformationPopupOpen
    {
      get => _isTransformationPopupOpen;
      set
      {
        if (!(_isTransformationPopupOpen == value))
        {
          _isTransformationPopupOpen = value;
          OnPropertyChanged(nameof(IsTransformationPopupOpen));
        }
      }
    }

    /// <summary>
    /// Gets the available transformations.
    /// </summary>
    public SelectableListNodeList AvailableTransformations => _availableTransformations;

    #region ColumnAddTo command

    private ICommand _columnAddToCommand;
    /// <summary>
    /// Gets the command for adding the selected transformation.
    /// </summary>
    public ICommand ColumnAddToCommand
    {
      get
      {
        return _columnAddToCommand ??= new RelayCommand(EhView_TransformationAddTo);
      }
    }

    #endregion ColumnAddTo command

    #region TransformationEdit command

    private ICommand _transformationEditCommand;

    /// <summary>
    /// Gets the command for editing the current transformation.
    /// </summary>
    public ICommand TransformationEditCommand
    {
      get
      {
        return _transformationEditCommand ??= new RelayCommand(EhView_TransformationEdit); 
      }
    }



    #endregion TransformationEdit command

    #region TransformationErase command

    private ICommand _transformationEraseCommand;

    /// <summary>
    /// Gets the command for erasing the current transformation.
    /// </summary>
    public ICommand TransformationEraseCommand
    {
      get
      {
       
        return _transformationEraseCommand ??= new RelayCommand(EhView_TransformationErase);
      }
    }


    #endregion TransformationErase command

    #region TransformationAddAsSingle command

    private ICommand _transformationAddAsSingleCommand;

    /// <summary>
    /// Gets the command for replacing the current transformation.
    /// </summary>
    public ICommand TransformationAddAsSingleCommand
    {
      get
      {
        return _transformationAddAsSingleCommand ??= new RelayCommand(EhView_TransformationAddAsSingle);
      }
    }

    #endregion TransformationAddAsSingle command

    #region TransformationAddAsPrepending command

    private ICommand _transformationAddAsPrependingCommand;

    /// <summary>
    /// Gets the command for prepending a transformation.
    /// </summary>
    public ICommand TransformationAddAsPrependingCommand
    {
      get
      {
        return _transformationAddAsPrependingCommand ??= new RelayCommand(EhView_TransformationAddAsPrepending);
      }
    }


    #endregion TransformationAddAsPrepending command

    #region TransformationAddAsAppending command

    private ICommand _transformationAddAsAppendingCommand;

    /// <summary>
    /// Gets the command for appending a transformation.
    /// </summary>
    public ICommand TransformationAddAsAppendingCommand
    {
      get
      {
        return _transformationAddAsAppendingCommand ??= new RelayCommand(EhView_TransformationAddAsAppending); 
      }
    }

    #endregion TransformationAddAsAppending command

    #region CmdCloseTransformationPopup

    ICommand _cmdCloseTransformationPopup;
    /// <summary>
    /// Gets the command for closing the transformation popup.
    /// </summary>
    public ICommand CmdCloseTransformationPopup => _cmdCloseTransformationPopup ??= new RelayCommand(() => IsTransformationPopupOpen = false);

    #endregion

    #region AvailableTransformations drag handler

    AvailableTransformationsDragHandlerImpl _availableTransformationDragHandler;
    /// <summary>
    /// Gets the drag handler for available transformations.
    /// </summary>
    public AvailableTransformationsDragHandlerImpl AvailableTransformationDragHandler => _availableTransformationDragHandler ??= new AvailableTransformationsDragHandlerImpl();

    /// <summary>
    /// Drag handler for available transformations.
    /// </summary>
    public class AvailableTransformationsDragHandlerImpl : IMVVMDragHandler
    {
      /// <inheritdoc/>
      public bool CanStartDrag(IEnumerable items)
      {
        var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
        // to start a drag, at least one item must be selected
        return selNode is not null;
      }

     

      /// <inheritdoc/>
      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
        var node = items.OfType<SelectableListNode>().FirstOrDefault();
        data = node.Tag;
        canCopy = true;
        canMove = true;
      }

      /// <inheritdoc/>
      public void DragCancelled()
      {
        
      }

      /// <inheritdoc/>
      public void DragEnded(bool isCopy, bool isMove)
      {
        
      }
    }

    #endregion AvailableTransformations drag handler

    #region ColumnDrop hander


    private ColumnDropHandlerImpl _columnDropHandler;
    /// <summary>
    /// Gets the drop handler for transformations.
    /// </summary>
    public ColumnDropHandlerImpl ColumnDropHandler => _columnDropHandler ??= new ColumnDropHandlerImpl(this);

    /// <summary>
    /// Drop handler for adding transformations.
    /// </summary>
    public class ColumnDropHandlerImpl : IMVVMDropHandler
    {
      DependentVariableTransformationController _parent;

      /// <summary>
      /// Initializes a new instance of the <see cref="ColumnDropHandlerImpl"/> class.
      /// </summary>
      /// <param name="parent">The parent controller.</param>
      public ColumnDropHandlerImpl(DependentVariableTransformationController parent)
      {
        _parent = parent;
      }

      /// <inheritdoc/>
      public void DropCanAcceptData(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
      {
        // investigate data
        canCopy = true;
        canMove = true;
        itemIsSwallowingData = false;
      }


      /// <inheritdoc/>
      public void Drop(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        if (data is Type)
        {
          object createdObj = null;
          try
          {
            createdObj = System.Activator.CreateInstance((Type)data);
          }
          catch (Exception ex)
          {
            Current.Gui.ErrorMessageBox("This object could not be dropped, message: " + ex.ToString(), "Error");
          }

          if (createdObj is IDoubleToDoubleTransformation)
          {
            _parent._availableTransformations.ClearSelectionsAll(); // we artificially select the node that holds that type
            var nodeToSelect = _parent._availableTransformations.FirstOrDefault(node => (Type)node.Tag == (Type)data);
            if (nodeToSelect is not null)
            {
              nodeToSelect.IsSelected = true;
              _parent.EhView_TransformationAddTo();
            }
          }

          _parent.Update();
        }


        
          isCopy = true;
        isMove = false;
        
      }

    }

    #endregion ColumnDrop hander

  }
}
