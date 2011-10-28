using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Altaxo.Gui.Graph
{
	using Altaxo.Data;

	public class ExchangeTablesOfPlotItemsDocument
	{
		List<Altaxo.Graph.Gdi.Plot.IGPlotItem> _itemsToChange;

		Dictionary<DataTable, DataTable> _tablesToChange = new Dictionary<DataTable, DataTable>();

		public List<Altaxo.Graph.Gdi.Plot.IGPlotItem> PlotItemsToChange
		{
			get { return _itemsToChange; }
		}


		static void CreateFromGraph(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			var result = new ExchangeTablesOfPlotItemsDocument();
			doc.VisitDocumentReferences(result.Visit);
		}

		void GetTables()
		{
			DataTable table;
			foreach (var item in _itemsToChange.OfType<Altaxo.Graph.Gdi.Plot.XYColumnPlotItem>())
			{
				var style = item.Style;


				if(item.Data.XColumn is DataColumn)
					table = DataTable.GetParentDataTableOf((DataColumn)item.Data.XColumn);

			}
		}


		public void Visit(Main.DocNodeProxy proxy, object owner, string propertyName)
		{
			if (proxy.DocumentObject is Altaxo.Data.DataColumn)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)proxy.DocumentObject);
				if (table != null && !_tablesToChange.ContainsKey(table))
					_tablesToChange.Add(table, null);
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumnCollection)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumnCollection)proxy.DocumentObject);
				if (table != null && !_tablesToChange.ContainsKey(table))
					_tablesToChange.Add(table, null);
			}
			else if (proxy.DocumentObject is DataTable)
			{
				var table = proxy.DocumentObject as DataTable;
				if (table != null && !_tablesToChange.ContainsKey(table))
					_tablesToChange.Add(table, null);
			}

		}
	}

	public interface IExchangeTablesOfPlotItemsView
	{
	}

	[ExpectedTypeOfView(typeof(IExchangeTablesOfPlotItemsView))]
	public class ExchangeTablesOfPlotItemsController : IMVCANController
	{
		IExchangeTablesOfPlotItemsView _view;

		public bool InitializeDocument(params object[] args)
		{
			throw new NotImplementedException();
		}

		public UseDocument UseDocumentCopy
		{
			set { throw new NotImplementedException(); }
		}

		public object ViewObject
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public object ModelObject
		{
			get { throw new NotImplementedException(); }
		}

		public bool Apply()
		{
			throw new NotImplementedException();
		}
	}
}
