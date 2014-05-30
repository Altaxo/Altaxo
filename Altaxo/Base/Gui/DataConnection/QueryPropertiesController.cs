using Altaxo.Collections;
using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface IQueryPropertiesView
	{
		void UpdateDialogValues(bool isDistinct, int topN, SelectableListNodeList groupBy);

		int GetTopN();

		bool GetDistinct();
	}

	[ExpectedTypeOfView(typeof(IQueryPropertiesView))]
	public class QueryPropertiesController : IMVCAController
	{
		private IQueryPropertiesView _view;
		private QueryBuilder _builder;
		private SelectableListNodeList _groupByChoices;

		public QueryPropertiesController(QueryBuilder builder)
		{
			_builder = builder;
			Initialize(true);
		}

		public QueryBuilder QueryBuilder
		{
			get { return _builder; }
			set
			{
				if (_builder != value)
				{
					_builder = value;
					Initialize(false);
				}
			}
		}

		private void Initialize(bool initData)
		{
			if (initData)
			{
				_groupByChoices = new SelectableListNodeList();
				_groupByChoices.FillWithEnumeration(_builder.GroupByExtension);
			}
			if (null != _view)
			{
				_groupByChoices.RemoveSelectedItems();
				if (_builder.GroupBy)
				{
					_groupByChoices[(int)_builder.GroupByExtension].IsSelected = true;
				}
				_view.UpdateDialogValues(_builder.Distinct, _builder.Top, _groupByChoices);
			}
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
				}
				_view = value as IQueryPropertiesView;
				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return null; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply()
		{
			_builder.Top = _view.GetTopN();
			if (_builder.GroupBy)
			{
				_builder.GroupByExtension = (GroupByExtension)_groupByChoices.FirstSelectedNode.Tag;
			}
			_builder.Distinct = true == _view.GetDistinct();

			return true;
		}
	}
}