using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Data;

	public class ExchangeTablesOfPlotItemsDocument
	{
		List<Altaxo.Graph.Gdi.Plot.IGPlotItem> _itemsToChange = new List<Altaxo.Graph.Gdi.Plot.IGPlotItem>();

		Dictionary<Main.DocumentPath, DataTable> _tablesToChange = new Dictionary<Main.DocumentPath, DataTable>();


		public List<Altaxo.Graph.Gdi.Plot.IGPlotItem> PlotItemsToChange
		{
			get { return _itemsToChange; }
		}

		public Dictionary<Main.DocumentPath, DataTable> TablesToChange
		{
			get { return _tablesToChange; }
			set { _tablesToChange = value; }
		}


		public static ExchangeTablesOfPlotItemsDocument CreateFromGraph(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			var result = new ExchangeTablesOfPlotItemsDocument();
			doc.VisitDocumentReferences(result.CollectDataColumnFromProxyVisit);
			result.CollectPlotItemsForGraph(doc);
			return result;
		}


		public static ExchangeTablesOfPlotItemsDocument CreateFromGraphs(IEnumerable<Altaxo.Graph.Gdi.GraphDocument> docs)
		{
			var result = new ExchangeTablesOfPlotItemsDocument();
			foreach (var doc in docs)
			{
				doc.VisitDocumentReferences(result.CollectDataColumnFromProxyVisit);
				result.CollectPlotItemsForGraph(doc);
			}

			
			return result;
		}

		void CollectPlotItemsForGraph(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			foreach (var layer in doc.Layers)
			{
				var pis = layer.PlotItems.Flattened;
				foreach (var pi in pis)
				{
					if (!_itemsToChange.Contains(pi))
						_itemsToChange.Add(pi);
				}
			}
		}


		void CollectDataColumnFromProxyVisit(Main.DocNodeProxy proxy, object owner, string propertyName)
		{
			if (proxy.DocumentObject is Altaxo.Data.DataColumn)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)proxy.DocumentObject);
				if (table != null)
				{
				var tablePath = Main.DocumentPath.GetAbsolutePath(table);
				if(!_tablesToChange.ContainsKey(tablePath))
					_tablesToChange.Add(tablePath, null);
				}
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumnCollection)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumnCollection)proxy.DocumentObject);
				if (table != null)
				{
				var tablePath = Main.DocumentPath.GetAbsolutePath(table);
				if(!_tablesToChange.ContainsKey(tablePath))
					_tablesToChange.Add(tablePath, null);
				}
			}
			else if (proxy.DocumentObject is DataTable)
			{
				var table = proxy.DocumentObject as DataTable;
				if (table != null)
				{
					var tablePath = Main.DocumentPath.GetAbsolutePath(table);
					if (!_tablesToChange.ContainsKey(tablePath))
					_tablesToChange.Add(tablePath, null);
				}
			}
			else if ((proxy is Altaxo.Data.NumericColumnProxy) || (proxy is Altaxo.Data.ReadableColumnProxy))
			{
				var path = proxy.DocumentPath;
				if (path.Count >= 2 && path.StartsWith(Main.DocumentPath.GetPath(Current.Project.DataTableCollection,int.MaxValue)))
				{
					var tablePath = path.SubPath(2);
					if (!_tablesToChange.ContainsKey(tablePath))
						_tablesToChange.Add(tablePath, null);
				}
			}

		}


		public void ApplyTableExchanges()
		{
			foreach (var pi in _itemsToChange)
			{
				pi.VisitDocumentReferences(ExchangeTablesProxyVisit);
			}
		}


	

		/// <summary>Exchanges the tables stored in < proxy visit.</summary>
		/// <param name="proxy">The proxy.</param>
		/// <param name="owner">The owner.</param>
		/// <param name="propertyName">Name of the property.</param>
		void ExchangeTablesProxyVisit(Main.DocNodeProxy proxy, object owner, string propertyName)
		{
			Altaxo.Data.DataTable substituteTable;

			if (proxy.DocumentObject is Altaxo.Data.DataColumn)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)proxy.DocumentObject);
				if (table != null)
				{
					var tablePath = Main.DocumentPath.GetAbsolutePath(table);
					if (_tablesToChange.TryGetValue(tablePath, out substituteTable) && null!=substituteTable)
					{
						
						proxy.ReplacePathParts(tablePath, Main.DocumentPath.GetAbsolutePath(substituteTable));

					}
				}
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumnCollection)
			{
				throw new NotImplementedException();
			}
			else if (proxy.DocumentObject is DataTable)
			{
				throw new NotImplementedException();
			}
			else if ((proxy is Altaxo.Data.NumericColumnProxy) || (proxy is Altaxo.Data.ReadableColumnProxy))
			{
				var path = proxy.DocumentPath;
				throw new NotImplementedException();
			}

		}

	}

	public interface IExchangeTablesOfPlotItemsView
	{
		void InitializeExchangeTableList(SelectableListNodeList list);
		event Action ChooseTableForSelectedItems;
		event Action ChooseFolderForSelectedItems;
	}

	[UserControllerForObject(typeof(ExchangeTablesOfPlotItemsDocument))]
	[ExpectedTypeOfView(typeof(IExchangeTablesOfPlotItemsView))]
	public class ExchangeTablesOfPlotItemsController : IMVCANController
	{
		#region Inner classes

		class MyXTableListNode : SelectableListNode
		{
			DataTable _newTable;

			public MyXTableListNode(string text, object tag, bool isSelected)
				: base(text, tag, isSelected)
			{
			}

			public DataTable NewTable
			{
				get { return _newTable; }
				set
				{
					var oldValue = _newTable;
					_newTable = value;
					if (!object.ReferenceEquals(_newTable, oldValue))
					{
						OnPropertyChanged("Text1");
					}
				}
			}

			public override string Text1
			{
				get
				{
					return null == _newTable ? "" : _newTable.Name;
				}
			}
		}



		#endregion

		IExchangeTablesOfPlotItemsView _view;
		ExchangeTablesOfPlotItemsDocument _doc;

		SelectableListNodeList _tableList;

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is ExchangeTablesOfPlotItemsDocument))
				return false;

			_doc = args[0] as ExchangeTablesOfPlotItemsDocument;
			Initialize(true);
			return true;
		}

		void Initialize(bool initData)
		{
			if (initData)
			{
				_tableList = new SelectableListNodeList();
				foreach (var tablePath in _doc.TablesToChange.Keys)
				{
					_tableList.Add(new MyXTableListNode(tablePath.ToString(),tablePath,false));
				}
			}
			if (null != _view)
			{
				_view.InitializeExchangeTableList(_tableList);
			}
		}

		void EhChooseTableForSelectedItems()
		{
			var tableSel = new Altaxo.Gui.Worksheet.SingleTableChoice();

			if (Current.Gui.ShowDialog(ref tableSel, "Choose new table for selected items", false))
			{
				var newTable = tableSel.SelectedTable;
				foreach (var entry in _tableList)
				{
					if (entry.IsSelected)
					{
						var tableToChange = (Main.DocumentPath)entry.Tag;
						_doc.TablesToChange[tableToChange] = newTable;
						((MyXTableListNode)entry).NewTable = newTable;
					}
				}
			}
		}

		void EhChooseFolderForSelectedItems()
		{
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
					_view.ChooseTableForSelectedItems -= this.EhChooseTableForSelectedItems;
					_view.ChooseFolderForSelectedItems -= this.EhChooseFolderForSelectedItems;
				}

				_view = value as IExchangeTablesOfPlotItemsView;

				if (null != _view)
				{
					Initialize(false);

					_view.ChooseTableForSelectedItems += this.EhChooseTableForSelectedItems;
					_view.ChooseFolderForSelectedItems += this.EhChooseFolderForSelectedItems;
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public bool Apply()
		{
			_doc.ApplyTableExchanges();
			return true;
		}
	}
}
