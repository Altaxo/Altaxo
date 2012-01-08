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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;
using Altaxo.Main;

namespace Altaxo.Gui.Settings
{
	[ExpectedTypeOfView(typeof(ISettingsView))]
	public class SettingsController : IMVCANController
	{
		ISettingsView _view;
		NGTreeNode _topics;

		NGTreeNode _currentNode;

		public SettingsController()
		{
			Initialize(true);
		}

		public bool InitializeDocument(params object[] args)
		{
			return true;
		}

		void Initialize(bool initData)
		{
			if (initData)
			{
				var items = ICSharpCode.Core.AddInTree.GetTreeNode("/Altaxo/Dialogs/SettingsDialog").BuildChildItems<IOptionPanelDescriptor>(null);
				_topics = new NGTreeNode();
				foreach (var item in items)
					AddTopic(item, _topics.Nodes);
			}

			if (null != _view)
			{
				_view.InitializeTopics(_topics.Nodes);
				EhTopicSelectionChanged(_topics);
			}
		}

		void AddTopic(IOptionPanelDescriptor desc, NGTreeNodeCollection nodecoll)
		{
			var newNode = new NGTreeNode(desc.Label);
			newNode.Tag = desc;
			if (null != desc.ChildOptionPanelDescriptors)
			{
				foreach (var child in desc.ChildOptionPanelDescriptors)
					AddTopic(child,newNode.Nodes);
			}
			nodecoll.Add(newNode);
		}

		void EhTopicSelectionChanged(NGTreeNode obj)
		{
			if (_currentNode != null && !object.ReferenceEquals(obj,_currentNode))
			{
				var desc = (IOptionPanelDescriptor)_currentNode.Tag;
				var ctrl = desc.OptionPanel;
				if (!ctrl.Apply())
				{
					_view.SetSelectedNode(_currentNode);
					return;
				}
			}

			// if this node has a own control, use it, otherwise use the next child
			var node = GetFirstNodeWithControl(obj);
			string title = string.Empty;
			object view = null;
			if (node != null)
			{
				var desc = (IOptionPanelDescriptor)node.Tag;
				var ctrl = desc.OptionPanel;
				if(ctrl.ViewObject==null)
					Current.Gui.FindAndAttachControlTo(ctrl);
				title = desc.Label;
				view = ctrl.ViewObject;
				_currentNode = node;
			}

			_view.InitializeTopicView(title,view);
		}


		NGTreeNode GetFirstNodeWithControl(NGTreeNode obj)
		{
			// if this node has a own control, use it, otherwise use the next child
			var desc = (IOptionPanelDescriptor)obj.Tag;
			if (desc!=null && desc.OptionPanel != null)
				return obj;

			if (obj.HasChilds)
			{
				foreach (var child in obj.Nodes)
				{
					var result = GetFirstNodeWithControl(child);
					if (null != result)
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
				if (null != _view)
				{
					_view.TopicSelectionChanged -= EhTopicSelectionChanged;
				}

				_view = value as ISettingsView;

				if (null != _view)
				{
					Initialize(false);
					_view.TopicSelectionChanged += EhTopicSelectionChanged;
				}
			}
		}

	

		public object ModelObject
		{
			get { return null; }
		}

		public bool Apply()
		{
			if (null != _currentNode)
			{
				var desc = (IOptionPanelDescriptor)_currentNode.Tag;
				var ctrl = desc.OptionPanel;
				if (!ctrl.Apply())
					return false;
			}



			return true;
		}
	}
}
