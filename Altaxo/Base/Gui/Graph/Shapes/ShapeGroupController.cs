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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IShapeGroupView
	{
		object LocationView { set; }

		void InitializeItemList(SelectableListNodeList list);

		event Action SelectedItemEditing;
	}

	[UserControllerForObject(typeof(ShapeGroup))]
	[ExpectedTypeOfView(typeof(IShapeGroupView))]
	public class ShapeGroupController : MVCANControllerEditOriginalDocBase<ShapeGroup, IShapeGroupView>
	{
		private SelectableListNodeList _itemList;

		private IMVCANController _locationController;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_locationController, () => _locationController = null);
		}

		public override void Dispose(bool isDisposing)
		{
			_itemList = null;
			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_itemList = new SelectableListNodeList();

				foreach (var d in _doc.GroupedObjects)
				{
					var node = new SelectableListNode(d.GetType().ToString(), d, false);
					_itemList.Add(node);
				}

				_locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
				Current.Gui.FindAndAttachControlTo(_locationController);
			}

			if (_view != null)
			{
				_view.InitializeItemList(_itemList);

				_view.LocationView = _locationController.ViewObject;
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_locationController.Apply(disposeController))
				return false;

			if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
				_doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			_view.SelectedItemEditing += EhEditSelectedItem;
		}

		protected override void DetachView()
		{
			_view.SelectedItemEditing -= EhEditSelectedItem;
		}

		private void EhEditSelectedItem()
		{
			_locationController.Apply(false);

			var node = _itemList.FirstSelectedNode;
			if (node == null)
				return;
			var item = (GraphicBase)node.Tag;
			if (Current.Gui.ShowDialog(ref item, "Edit shape group item " + node.Text, true))
			{
				_doc.AdjustPosition();
				UpdateLocationView();
			}
		}

		private void UpdateLocationView()
		{
			_locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
			Current.Gui.FindAndAttachControlTo(_locationController);
			_view.LocationView = _locationController.ViewObject;
		}
	}
}