using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Worksheet.Commands;

namespace Altaxo.Gui.Worksheet
{
	

	public interface IPlotCommonColumnsView
	{
		void InitializeXCommonColumns(SelectableListNodeList list);
		void InitializeYCommonColumns(SelectableListNodeList list);
		bool UseCurrentXColumn { get; set; }
	}

	[ExpectedTypeOfView(typeof(IPlotCommonColumnsView))]
	[UserControllerForObject(typeof(PlotCommonColumnsCommand))]
	public class PlotCommonColumnsController : IMVCANController
	{
		IPlotCommonColumnsView _view;
		PlotCommonColumnsCommand _doc;

		SelectableListNodeList _xCommonColumns;
		SelectableListNodeList _yCommonColumns;


		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0 || !(args[0] is PlotCommonColumnsCommand))
				return false;

			_doc = (PlotCommonColumnsCommand)args[0];
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}


		void Initialize(bool initData)
		{
			if (initData)
			{
				_xCommonColumns = new SelectableListNodeList();
				_yCommonColumns = new SelectableListNodeList();

				var names = _doc.GetCommonColumnNamesOrderedByAppearanceInFirstTable();
				bool isFirst = true;
				foreach (var name in names)
				{
					_xCommonColumns.Add(new SelectableListNode(name, name, name == _doc.XCommonColumnNameForPlot || (isFirst && null == _doc.XCommonColumnNameForPlot)));
					_yCommonColumns.Add(new SelectableListNode(name, name, false));
					isFirst = false;
				}
			}

			if (null != _view)
			{
				_view.InitializeXCommonColumns(_xCommonColumns);
				_view.InitializeYCommonColumns(_yCommonColumns);
				_view.UseCurrentXColumn = _doc.XCommonColumnNameForPlot == null;
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

				_view = value as IPlotCommonColumnsView;

				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public bool Apply()
		{
			if (_view.UseCurrentXColumn)
				_doc.XCommonColumnNameForPlot = null;
			else
				_doc.XCommonColumnNameForPlot = _xCommonColumns.FirstSelectedNode == null ? null : (string)_xCommonColumns.FirstSelectedNode.Tag;

			_doc.YCommonColumnNamesForPlotting.Clear();
			foreach (var node in _yCommonColumns.Where(x => x.IsSelected))
				_doc.YCommonColumnNamesForPlotting.Add((string)node.Tag);


			return true;
		}


	}
}

