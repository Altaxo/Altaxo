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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.AddInItems;
using Altaxo.Collections;

namespace Altaxo.Gui.Settings
{
  /// <summary>
  /// Implemented by the Gui element that displays the settings (topics as a tree, and the selected topic in a control).
  /// </summary>
  public interface ISettingsView
  {
    /// <summary>Occurs when the user selected another topic.</summary>
    event Action<NGTreeNode> TopicSelectionChanged;

    /// <summary>Occurs when the current topic view was entered.</summary>
    event Action CurrentTopicViewMadeDirty;

    /// <summary>Initializes the topic tree.</summary>
    /// <param name="topics">The topics tree structure.</param>
    void InitializeTopics(NGTreeNodeCollection topics);

    /// <summary>Sets the currently selected topic view.</summary>
    /// <param name="title">Titel (shown above the topic view).</param>
    /// <param name="guiTopicObject">The Gui topic view object.</param>
    void InitializeTopicView(string title, object guiTopicObject);

    /// <summary>Initializes an indicator whether the topic view is dirty or not.</summary>
    /// <param name="dirtyIndicator">The dirty indicator. Zero means: is not dirty, 1: is dirty, and 2 means: the topic view contains errors.</param>
    void InitializeTopicViewDirtyIndicator(int dirtyIndicator);

    /// <summary>Sets the selected node in the topic view.</summary>
    /// <param name="node">The node to select.</param>
    void SetSelectedNode(NGTreeNode node);
  }

  [ExpectedTypeOfView(typeof(ISettingsView))]
  public class SettingsController : IMVCANController
  {
    private ISettingsView _view;
    private NGTreeNode _topics;
    private NGTreeNode _currentNode;

    private HashSet<NGTreeNode> _dirtyTopics = new HashSet<NGTreeNode>();

    public SettingsController()
    {
      Initialize(true);
    }

    public bool InitializeDocument(params object[] args)
    {
      return true;
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        var items = AddInTree.GetTreeNode("/Altaxo/Dialogs/SettingsDialog").BuildChildItems<IOptionPanelDescriptor>(null);
        _topics = new NGTreeNode();
        foreach (var item in items)
          AddTopic(item, _topics.Nodes);
      }

      if (_view is not null)
      {
        _view.InitializeTopics(_topics.Nodes);
        EhTopicSelectionChanged(_topics);
      }
    }

    private void AddTopic(IOptionPanelDescriptor desc, NGTreeNodeCollection nodecoll)
    {
      var newNode = new NGTreeNode(desc.Label)
      {
        Tag = desc
      };
      if (desc.ChildOptionPanelDescriptors is not null)
      {
        foreach (var child in desc.ChildOptionPanelDescriptors)
          AddTopic(child, newNode.Nodes);
      }
      nodecoll.Add(newNode);
    }

    private void EhTopicSelectionChanged(NGTreeNode obj)
    {
      // if this node has a own control, use it, otherwise use the next child
      var node = GetFirstNodeWithControl(obj);
      string title = string.Empty;
      object view = null;
      if (node is not null)
      {
        var desc = (IOptionPanelDescriptor)node.Tag;
        var ctrl = desc.OptionPanel;
        title = desc.Label;
        view = ctrl.ViewObject;
        _currentNode = node;
      }

      _view.InitializeTopicView(title, view);
      _view.InitializeTopicViewDirtyIndicator(_dirtyTopics.Contains(_currentNode) ? 1 : 0);
    }

    private void EhCurrentTopicViewMadeDirty()
    {
      if (!_dirtyTopics.Contains(_currentNode))
        _dirtyTopics.Add(_currentNode);
    }

    private NGTreeNode GetFirstNodeWithControl(NGTreeNode obj)
    {
      // if this node has a own control, use it, otherwise use the next child
      var desc = (IOptionPanelDescriptor)obj.Tag;
      if (desc is not null && desc.OptionPanel is not null)
        return obj;

      if (obj.HasChilds)
      {
        foreach (var child in obj.Nodes)
        {
          var result = GetFirstNodeWithControl(child);
          if (result is not null)
            return result;
        }
      }
      return null; // nothing found
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          _view.TopicSelectionChanged -= EhTopicSelectionChanged;
          _view.CurrentTopicViewMadeDirty -= EhCurrentTopicViewMadeDirty;
        }

        _view = value as ISettingsView;

        if (_view is not null)
        {
          Initialize(false);
          _view.TopicSelectionChanged += EhTopicSelectionChanged;
          _view.CurrentTopicViewMadeDirty += EhCurrentTopicViewMadeDirty;
        }
      }
    }

    public object ModelObject
    {
      get { return null; }
    }

    public void Dispose()
    {
    }

    public bool Apply(bool disposeController)
    {
      // we have to call apply for all dirty topics

      foreach (var node in _dirtyTopics)
      {
        var desc = (IOptionPanelDescriptor)node.Tag;
        var ctrl = desc.OptionPanel;
        if (ctrl is not null && !ctrl.Apply())
        {
          _currentNode = node;
          _view.SetSelectedNode(_currentNode);
          _view.InitializeTopicView(desc.Label, ctrl.ViewObject);
          _view.InitializeTopicViewDirtyIndicator(2);
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }
  }
}
