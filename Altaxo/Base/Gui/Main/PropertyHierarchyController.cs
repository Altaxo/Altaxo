#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Main
{
	using Altaxo.Collections;
	using Altaxo.Main.Properties;

	public interface IPropertyHierarchyView
	{
		SelectableListNodeList PropertyList { set; }

		/// <summary>
		/// Occurs when the selected item should be edited.
		/// </summary>
		event Action ItemEditing;
	}

	[ExpectedTypeOfView(typeof(IPropertyHierarchyView))]
	[UserControllerForObject(typeof(PropertyHierarchy))]
	public class PropertyHierarchyController : MVCANControllerBase<PropertyHierarchy, IPropertyHierarchyView>
	{
		#region Inner types

		private class MyListNode : SelectableListNode
		{
			private string[] _subText = new string[3];

			public MyListNode(string text, object tag)
				: base(text, tag, false)
			{
			}

			public override int SubItemCount
			{
				get
				{
					return _subText.Length;
				}
			}

			public override string SubItemText(int i)
			{
				return _subText[i - 1];
			}

			public string Text1S { set { _subText[0] = value; } }

			public string Text2S { set { _subText[1] = value; } }

			public string Text3S { set { _subText[2] = value; } }
		}

		#endregion Inner types

		private SelectableListNodeList _propertyList;

		private SelectableListNodeList _availableProperties;

		public override bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is PropertyHierarchy))
				return false;

			_doc = _originalDoc = (PropertyHierarchy)args[0];
			if (_useDocumentCopy)
				_doc = _originalDoc.CreateCopyWithOnlyTopmostBagCloned();

			Initialize(true);
			return true;
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				Altaxo.Main.Services.ReflectionService.ForceRegisteringOfAllPropertyKeys();
				InitializePropertyNodeList();
			}
			if (null != _view)
			{
				_view.PropertyList = _propertyList;
			}
		}

		private void InitializeAvailablePropertyList()
		{
			_availableProperties = new SelectableListNodeList();

			foreach (var prop in Altaxo.Main.Properties.PropertyKeyBase.AllRegisteredPropertyKeys)
			{
				//var node = new SelectableListNode(prop.PropertyName, )
			}
		}

		private void InitializePropertyNodeList()
		{
			var sortedNames = new List<KeyValuePair<string, string>>(); //
			foreach (var key in _doc.GetAllPropertyNames())
			{
				string keyName = PropertyKeyBase.GetPropertyName(key);
				if (null == keyName)
					keyName = key;
				sortedNames.Add(new KeyValuePair<string, string>(key, keyName));
			}
			sortedNames.Sort(((entry1, entry2) => string.Compare(entry1.Value, entry2.Value))); // sort the entries not by the key, but by the keyname

			_propertyList = new SelectableListNodeList();

			foreach (var entry in sortedNames)
			{
				object value;
				IPropertyBag bag;
				PropertyBagInformation bagInfo;

				if (_doc.TryGetValue<object>(entry.Key, out value, out bag, out bagInfo))
				{
					var node = new MyListNode(entry.Value, new Tuple<string, IPropertyBag>(entry.Key, bag))
					{
						Text1S = value == null ? "n.a." : value.GetType().Name,
						Text2S = value == null ? "null" : value.ToString(),
						Text3S = bagInfo.Name
					};

					_propertyList.Add(node);
				}
			}
		}

		public override bool Apply()
		{
			if (!object.ReferenceEquals(_doc, _originalDoc))
			{
				_originalDoc.TopmostBag.CopyFrom(_doc.TopmostBag);
			}
			return true;
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.ItemEditing += EhItemEditing;
		}

		protected override void DetachView()
		{
			_view.ItemEditing -= EhItemEditing;
			base.DetachView();
		}

		private void EhItemEditing()
		{
			var node = _propertyList.FirstSelectedNode;
			if (null == node)
				return;

			var nodeTag = (Tuple<string, IPropertyBag>)node.Tag;
			var propertyKey = nodeTag.Item1;

			object value;
			IPropertyBag bag;
			PropertyBagInformation bagInfo;

			_doc.TryGetValue(propertyKey, out value, out bag, out bagInfo);

			var controller = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { value }, typeof(IMVCAController), UseDocument.Copy);

			if (null == controller)
				return;

			if (Current.Gui.ShowDialog(controller, "Edit property", false))
			{
				var newValue = controller.ModelObject;
				_doc.TopmostBag.SetValue(propertyKey, newValue);
				InitializePropertyNodeList();
				if (null != _view)
					_view.PropertyList = _propertyList;
			}
		}
	}
}