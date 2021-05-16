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

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public interface IDependentVariableTransformationView : IDataContextAwareView
  {
    /// <summary>
    /// Shows a popup menu for the column corresponding to <paramref name="tag"/>, questioning whether to add the
    /// selected transformation as single transformation, as prepending transformation, or as appending transformation.
    /// </summary>
    void ShowTransformationSinglePrependAppendPopup(bool isOpen);

    event CanStartDragDelegate AvailableTransformations_CanStartDrag;

    event StartDragDelegate AvailableTransformations_StartDrag;

    event DragEndedDelegate AvailableTransformations_DragEnded;

    event DragCancelledDelegate AvailableTransformations_DragCancelled;

    event DropCanAcceptDataDelegate PlotItemColumn_DropCanAcceptData;

    event DropDelegate PlotItemColumn_Drop;

  }

  [ExpectedTypeOfView(typeof(IDependentVariableTransformationView))]
  public class DependentVariableTransformationController : MVCANControllerEditImmutableDocBase<IDoubleToDoubleTransformation?, IDependentVariableTransformationView>
  {
    /// <summary>All types of available column transformations.</summary>
    protected SelectableListNodeList _availableTransformations = new SelectableListNodeList();
    string _variableName = "y";

    public DependentVariableTransformationController()
    {
    }

    public DependentVariableTransformationController(IDoubleToDoubleTransformation? doc, string variableName="y")
    {
      _doc = doc;
      _variableName = variableName ?? "y";
      Initialize(true);
    }

    protected override void CheckDocumentInitialized<T>([AllowNull, NotNull] ref T doc)
    {
    }

    protected override void ThrowIfNotInitialized()
    {
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.AvailableTransformations_CanStartDrag += EhAvailableTransformations_CanStartDrag;
      _view.AvailableTransformations_StartDrag += EhAvailableTransformations_StartDrag;
      _view.AvailableTransformations_DragEnded += EhAvailableTransformations_DragEnded;
      _view.AvailableTransformations_DragCancelled += EhAvailableTransformations_DragCancelled;

      _view.PlotItemColumn_DropCanAcceptData += EhColumnDropCanAcceptData;
      _view.PlotItemColumn_Drop += EhColumnDrop;


    }

    protected override void DetachView()
    {
      _view.AvailableTransformations_CanStartDrag -= EhAvailableTransformations_CanStartDrag;
      _view.AvailableTransformations_StartDrag -= EhAvailableTransformations_StartDrag;
      _view.AvailableTransformations_DragEnded -= EhAvailableTransformations_DragEnded;
      _view.AvailableTransformations_DragCancelled -= EhAvailableTransformations_DragCancelled;

      _view.PlotItemColumn_DropCanAcceptData -= EhColumnDropCanAcceptData;
      _view.PlotItemColumn_Drop -= EhColumnDrop;


      base.DetachView();
    }

    protected override void Initialize(bool initData)
    {
      if (IsDisposed)
        throw new ObjectDisposedException("The controller was already disposed. Type: " + GetType().FullName);

      if(initData)
      {
        Controller_AvailableTransformations_Initialize();
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

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
          _view?.ShowTransformationSinglePrependAppendPopup(true); // this will eventually fire one of three commands to add as single, as prepend or as append transformation
        }
      }
      Update();
    }

    public void EhView_TransformationAddAsSingle()
    {
      _view.ShowTransformationSinglePrependAppendPopup(false);

      var node = _availableTransformations.FirstSelectedNode;
      if (node is not null)
      {
        EhTransformation_AddMultiple((Type)node.Tag, 0);
      }
    }

    public void EhView_TransformationAddAsPrepending()
    {
      _view.ShowTransformationSinglePrependAppendPopup(false);


      var node = _availableTransformations.FirstSelectedNode;
      if (node is not null)
      {
        EhTransformation_AddMultiple((Type)node.Tag, 1);
      }
    }

    public void EhView_TransformationAddAsAppending()
    {
      _view.ShowTransformationSinglePrependAppendPopup(false);


      var node = _availableTransformations.FirstSelectedNode;
      if (node is not null)
      {
        EhTransformation_AddMultiple((Type)node.Tag, 2);
      }
    }

    public void EhView_TransformationEdit()
    {
      _doc = EditAvailableTransformation(_doc, out var wasEdited);
      Update();
    }

    public void EhView_TransformationErase()
    {
      _doc = null;
      Update();
    }

    #endregion Transformation

    public void Update()
    {
      OnPropertyChanged(nameof(TransformationTextToShow));
      OnPropertyChanged(nameof(TransformationToolTip));
    }

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

    public SelectableListNodeList AvailableTransformations => _availableTransformations;

    #region ColumnAddTo command

    private ICommand _columnAddToCommand;
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

    public ICommand TransformationAddAsAppendingCommand
    {
      get
      {
        return _transformationAddAsAppendingCommand ??= new RelayCommand(EhView_TransformationAddAsAppending); 
      }
    }

    #endregion TransformationAddAsAppending command

    #region AvailableTransformations drag handler

    private bool EhAvailableTransformations_CanStartDrag(IEnumerable items)
    {
      var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
      // to start a drag, at least one item must be selected
      return selNode is not null;
    }

    private StartDragData EhAvailableTransformations_StartDrag(IEnumerable items)
    {
      var node = items.OfType<SelectableListNode>().FirstOrDefault();

      return new StartDragData
      {
        Data = node.Tag,
        CanCopy = true,
        CanMove = true
      };
    }

    private void EhAvailableTransformations_DragEnded(bool isCopy, bool isMove)
    {
    }

    private void EhAvailableTransformations_DragCancelled()

    {
    }

    #endregion AvailableTransformations drag handler

    #region ColumnDrop hander

    /// <summary>
    ///
    /// </summary>
    /// <param name="data">The data to accept.</param>
    /// <param name="nonGuiTargetItem">Object that can identify the drop target, for instance a non gui tree node or list node, or a tag.</param>
    /// <param name="insertPosition">The insert position.</param>
    /// <param name="isCtrlKeyPressed">if set to <c>true</c> [is control key pressed].</param>
    /// <param name="isShiftKeyPressed">if set to <c>true</c> [is shift key pressed].</param>
    /// <returns></returns>
    public DropCanAcceptDataReturnData EhColumnDropCanAcceptData(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
    {
      // investigate data

      return new DropCanAcceptDataReturnData
      {
        CanCopy = true,
        CanMove = true,
        ItemIsSwallowingData = false
      };
    }

    public DropReturnData EhColumnDrop(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
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
          _availableTransformations.ClearSelectionsAll(); // we artificially select the node that holds that type
          var nodeToSelect = _availableTransformations.FirstOrDefault(node => (Type)node.Tag == (Type)data);
          if (nodeToSelect is not null)
          {
            nodeToSelect.IsSelected = true;
            EhView_TransformationAddTo();
          }
        }

        Update();
      }
      

      return new DropReturnData
      {
        IsCopy = true,
        IsMove = false
      };
    }

    #endregion ColumnDrop hander

  }
}
