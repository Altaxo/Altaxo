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
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Graph.Gdi;
using Altaxo.Graph;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IShapeGroupView
	{
		PointD2D DocPosition { get; set; }
		PointD2D DocSize { get; set; }
		double DocRotation { get; set; }
		double DocShear { get; set; }
		PointD2D DocScale { get; set; }

		void InitializeItemList(SelectableListNodeList list);

		event Action SelectedItemEditing;


	}

	[UserControllerForObject(typeof(ShapeGroup))]
	[ExpectedTypeOfView(typeof(IShapeGroupView))]
	public class ShapeGroupController : MVCANControllerBase<ShapeGroup, IShapeGroupView>
	{
		SelectableListNodeList _itemList;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_itemList = new SelectableListNodeList();

				foreach (var d in _doc.GroupedObjects)
				{
					var node = new SelectableListNode(d.GetType().ToString(), d, false);
					_itemList.Add(node);
				}
			}

			if (_view != null)
			{
				UpdateViewsPosSizeRotShearScale();
				_view.InitializeItemList(_itemList);
			}
		}

		void UpdateViewsPosSizeRotShearScale()
		{
			_view.DocPosition = _doc.Position;
			_view.DocSize = _doc.Size;
			_view.DocRotation = _doc.Rotation;
			_view.DocShear = _doc.Shear;
			_view.DocScale = _doc.Scale;
		}




		public override bool Apply()
		{
			_doc.Position = _view.DocPosition;
			_doc.Size = _view.DocSize;
			_doc.Rotation = _view.DocRotation;
			_doc.Shear = _view.DocShear;
			_doc.Scale = _view.DocScale;

			if (_useDocumentCopy)
				_originalDoc.CopyFrom(_doc);

			return true;
		}


		protected override void AttachView()
		{
			_view.SelectedItemEditing += EhEditSelectedItem;
		}

		protected override void DetachView()
		{
			_view.SelectedItemEditing -= EhEditSelectedItem;
		}


		void EhEditSelectedItem()
		{
			var node = _itemList.FirstSelectedNode;
			if (node == null)
				return;
			var item = (GraphicBase)node.Tag;
			if (Current.Gui.ShowDialog(ref item, "Edit shape group item " + node.Text, true))
			{
				_doc.AdjustPosition();
				UpdateViewsPosSizeRotShearScale();
			}
		}
	}
}
