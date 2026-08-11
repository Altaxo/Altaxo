#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Main.Services;
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines the view contract used by the property extraction tree controller.
  /// </summary>
  public interface IPropertyExtractionTreeView : IDataContextAwareView
  {
  }



  /// <summary>
  /// Controls the display and editing of a property extraction tree.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPropertyExtractionTreeView))]
  [UserControllerForObject(typeof(IPropertyExtractionTreeNode), priority: -1)]
  public class PropertyExtractionTreeController : MVCANDControllerEditImmutableDocBase<IPropertyExtractionTreeNode, IPropertyExtractionTreeView>
  {
    /// <summary>
    /// AvailableNodeTypes represents a node type that can be added to the property extraction tree, along with the command to add it.
    /// </summary>
    public class AvailableNodeType
    {
      /// <summary>
      /// Gets or sets the display name of the node type.
      /// </summary>
      public required string Header { get; init; }

      /// <summary>
      /// Gets or sets the command that adds a node of this type to the property extraction tree.
      /// </summary>
      public required ICommand Command { get; init; }
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    private void EhCmdAddChild(NGTreeNode cmdParameter, Type newType)
    {
      var newNode = (IPropertyExtractionTreeNode)Activator.CreateInstance(newType)!;

      var ngNode = new NGTreeNode
      {
        Tag = (cmdParameter.Count, newNode),
        IsExpanded = true,
        IsSelected = true
      };

      cmdParameter.Nodes.Add(ngNode);

      EhCmdEditNode(ngNode);

      _doc = UpdateDocFromGuiTree(RootNode.Nodes[0]);
      UpdateGuiTreeFromDoc(RootNode.Nodes[0], (0, _doc));
      OnMadeDirty();
    }

    /// <summary>
    /// Gets the command that removes the currently selected node from the tree.
    /// </summary>
    public ICommand CmdRemoveNode => field ??= new RelayCommand<NGTreeNode>(EhCmdRemoveNode);

    private void EhCmdRemoveNode(NGTreeNode cmdParameter)
    {
      if (cmdParameter.ParentNode is not null)
      {
        cmdParameter.Remove();
      }

      _doc = UpdateDocFromGuiTree(RootNode.Nodes[0]);
      UpdateGuiTreeFromDoc(RootNode.Nodes[0], (0, _doc));
      OnMadeDirty();
    }

    /// <summary>
    /// Gets the command that edits the currently selected node.
    /// </summary>
    public ICommand CmdEditNode => field ??= new RelayCommand<NGTreeNode>(EhCmdEditNode);

    private void EhCmdEditNode(NGTreeNode cmdParameter)
    {
      var nodeTuple = ((int IndexOfNamePart, IPropertyExtractionTreeNode Node))cmdParameter.Tag;

      var controller = new IndexAndExtractionNodeController();
      controller.InitializeDocument(nodeTuple);
      if (Current.Gui.ShowDialog(controller, "Edit Node", true))
      {
        var updatedNodeTuple = ((int IndexOfNamePart, IPropertyExtractionTreeNode Node))controller.ModelObject;
        cmdParameter.Tag = updatedNodeTuple;
        _doc = UpdateDocFromGuiTree(RootNode.Nodes[0]);
        UpdateGuiTreeFromDoc(RootNode.Nodes[0], (0, _doc));
        OnMadeDirty();
      }
    }



    private void EhCmdChangeType(NGTreeNode cmdParameter, Type newType)
    {
      var oldNodeTuple = ((int IndexOfNamePart, IPropertyExtractionTreeNode Node))cmdParameter.Tag;

      var newNode = (IPropertyExtractionTreeNode)Activator.CreateInstance(newType)!;

      if (oldNodeTuple.Node is PropertyEvaluatorBase oldPEB && newNode is PropertyEvaluatorBase newPEB)
      {
        newPEB = newPEB with { PropertyName = oldPEB.PropertyName };
      }

      cmdParameter.Tag = (oldNodeTuple.IndexOfNamePart, newNode);

      EhCmdEditNode(cmdParameter);

      _doc = UpdateDocFromGuiTree(RootNode.Nodes[0]);
      UpdateGuiTreeFromDoc(RootNode.Nodes[0], (0, _doc));
      OnMadeDirty();

    }

    /// <summary>
    /// Gets or sets the list of available node types that can be added to the property extraction tree.
    /// </summary>
    public List<AvailableNodeType> AvailableSplitterTypesForAddition
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AvailableSplitterTypesForAddition));
        }
      }
    }

    /// <summary>
    /// Gets or sets the list of available node types that can be added to the property extraction tree.
    /// </summary>
    public List<AvailableNodeType> AvailableEvaluatorTypesForAddition
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AvailableEvaluatorTypesForAddition));
        }
      }
    }

    /// <summary>
    /// Gets or sets the list of available node types that can be added to the property extraction tree.
    /// </summary>
    public List<AvailableNodeType> AvailableTypesForChanging
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AvailableTypesForChanging));
        }
      }
    }


    #region Bindings

    /// <summary>
    /// Gets or sets the root node displayed in the property extraction tree.
    /// </summary>
    public Altaxo.Collections.NGTreeNode RootNode
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RootNode));
        }
      }
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var avTypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(IPropertyExtractionTreeNode));

        var listSplitter = new List<AvailableNodeType>();
        var listEvaluator = new List<AvailableNodeType>();
        foreach (var t in avTypes)
        {
          var ant = new AvailableNodeType
          {
            Header = t.Name,
            Command = new RelayCommand<NGTreeNode>(n => EhCmdAddChild(n, t))
          };

          if (t.IsAssignableTo(typeof(IPropertyEvaluator)))
          {
            listEvaluator.Add(ant);
          }
          else if (t.IsAssignableTo(typeof(INameSplitter)))
          {
            listSplitter.Add(ant);
          }
        }
        AvailableEvaluatorTypesForAddition = listEvaluator;
        AvailableSplitterTypesForAddition = listSplitter;

        var list = new List<AvailableNodeType>();
        foreach (var t in avTypes)
        {
          list.Add(new AvailableNodeType
          {
            Header = t.Name,
            Command = new RelayCommand<NGTreeNode>(n => EhCmdChangeType(n, t))
          });

        }
        AvailableTypesForChanging = list;


        var rn = new NGTreeNode() { IsExpanded = true };
        UpdateGuiTreeFromDoc(rn, (0, _doc));
        RootNode = new NGTreeNode();
        RootNode.Nodes.Add(rn);
      }
    }

    void UpdateGuiTreeFromDoc(NGTreeNode guiNode, (int IndexOfNamePart, IPropertyExtractionTreeNode Node) docNodeTuple)
    {
      var (index, docNode) = docNodeTuple;
      guiNode.Text = GetHeader(docNodeTuple);
      guiNode.Tag = docNodeTuple;
      guiNode.LimitChildCountTo(docNode.Children.Count);

      if (docNode.Children.Count > 0)
      {
        foreach (var child in docNode.Children.Index())
        {
          if (child.Index < guiNode.Count)
          {
            var childGuiNode = guiNode.Nodes[child.Index];
            UpdateGuiTreeFromDoc(childGuiNode, child.Item);
          }
          else
          {
            var childGuiNode = new NGTreeNode();
            childGuiNode.IsExpanded = true;
            guiNode.Nodes.Add(childGuiNode);
            UpdateGuiTreeFromDoc(childGuiNode, child.Item);
          }
        }
      }
    }

    string GetHeader((int IndexOfNamePart, IPropertyExtractionTreeNode Node) docNodeTuple)
    {
      var (index, docNode) = docNodeTuple;
      if (docNode is PropertyEvaluatorBase peb)
      {
        return $"[{docNodeTuple.IndexOfNamePart}] \"{peb.PropertyName}\": {docNode.ToString()}";
      }
      else
      {
        return $"[{docNodeTuple.IndexOfNamePart}] {docNode.ToString()}";
      }
    }

    IPropertyExtractionTreeNode UpdateDocFromGuiTree(NGTreeNode node)
    {
      var list = new List<(int IndexOfNamePart, IPropertyExtractionTreeNode Node)>();

      foreach (var child in node.Nodes)
      {
        var childTuple = ((int IndexOfNamePart, IPropertyExtractionTreeNode Node))child.Tag;
        var updatedChildNode = UpdateDocFromGuiTree(child);
        list.Add((childTuple.IndexOfNamePart, updatedChildNode));
      }

      var thisNodeTuple = ((int IndexOfNamePart, IPropertyExtractionTreeNode Node))node.Tag;

      if (thisNodeTuple.Node is NameSplitterBase nsb)
      {
        return nsb with { Children = list.ToImmutableList() };
      }
      else
      {
        return thisNodeTuple.Node;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    #endregion
  }
}
