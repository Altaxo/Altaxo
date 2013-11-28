using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	public interface IGraphicItemsView
	{
		SelectableListNodeList ItemsList { set; }

		event Action SelectedItemsUp;

		event Action SelectedItemsDown;

		event Action SelectedItemsRemove;
	}

	[UserControllerForObject(typeof(GraphicCollection))]
	[ExpectedTypeOfView(typeof(IGraphicItemsView))]
	public class GraphicItemsController : MVCANControllerBase<GraphicCollection, IGraphicItemsView>
	{
		private SelectableListNodeList _itemsList;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_itemsList = new SelectableListNodeList();

				foreach (var item in _doc)
				{
					var node = new SelectableListNode(item.ToString(), item, false);
					_itemsList.Add(node);
				}
			}
			if (null != _view)
			{
				_view.ItemsList = _itemsList;
			}
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.SelectedItemsUp += EhSelectedItemsUp;
			_view.SelectedItemsDown += EhSelectedItemsDown;
			_view.SelectedItemsRemove += EhSelectedItemsRemove;
		}

		protected override void DetachView()
		{
			_view.SelectedItemsUp -= EhSelectedItemsUp;
			_view.SelectedItemsDown -= EhSelectedItemsDown;
			_view.SelectedItemsRemove -= EhSelectedItemsRemove;

			base.DetachView();
		}

		public override bool Apply()
		{
			using (var token = _doc.GetEventDisableToken())
			{
				_doc.Clear();
				foreach (var node in _itemsList)
				{
					_doc.Add((IGraphicBase)node.Tag);
				}
			}
			return true;
		}

		private void EhSelectedItemsRemove()
		{
			_itemsList.RemoveSelectedItems();
		}

		private void EhSelectedItemsDown()
		{
			_itemsList.MoveSelectedItemsDown();
		}

		private void EhSelectedItemsUp()
		{
			_itemsList.MoveSelectedItemsUp();
		}
	}
}