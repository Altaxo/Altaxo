using System;

namespace Altaxo.Graph.GUI
{
	#region Interfaces
	public interface ILineScatterPlotDataController : Main.GUI.IApplyController, Main.GUI.IMVCController
	{
		ILineScatterPlotDataView View { get; set; }
	
		void SetElements(bool bInit);

		void EhView_TableSelectionChanged(int selindex, string seltable);

		void EhView_ToX(int tableindex, string tablename, int columnindex, string columnname);
		void EhView_ToY(int tableindex, string tablename, int columnindex, string columnname);
		void EhView_EraseX();
		void EhView_EraseY();

		bool EhView_RangeFrom(int val);
		bool EhView_RangeTo(int val);
	}

	public interface ILineScatterPlotDataView : Main.GUI.IMVCView
	{

		/// <summary>
		/// Get/sets the controller of this view.
		/// </summary>
		ILineScatterPlotDataController Controller { get; set; }

		/// <summary>
		/// Gets the hosting parent form of this view.
		/// </summary>
		System.Windows.Forms.Form Form	{	get; }


		void Tables_Initialize(string[] tables, int selectedTable);

		void Columns_Initialize(string[] colnames, int selectedColumn);

		void XColumn_Initialize(string colname);
		void YColumn_Initialize(string colname);

		void PlotRangeFrom_Initialize(int from);
		void PlotRangeTo_Initialize(int to);

	}
	#endregion

	/// <summary>
	/// Summary description for LineScatterPlotDataController.
	/// </summary>
	public class LineScatterPlotDataController : ILineScatterPlotDataController
	{
		ILineScatterPlotDataView m_View=null;
		XYColumnPlotData m_PlotAssociation;


		bool m_bDirty = false;
		int m_PlotRange_From;
		int m_PlotRange_To;
		Altaxo.Data.IReadableColumn m_xCol;
		Altaxo.Data.IReadableColumn m_yCol;
		int m_MaxPossiblePlotRange_To;

		public LineScatterPlotDataController(XYColumnPlotData pa)
		{
			m_PlotAssociation = pa;
			SetElements(true);
		}

		public void SetDirty()
		{
			m_bDirty = true;
		}

		#region ILineScatterPlotDataController Members

		public ILineScatterPlotDataView View
		{ 
			get 
			{
				return m_View;
			}

			set
			{
				if(null!=m_View)
					m_View.Controller = null;
				
				m_View = value;

				if(null!=m_View)
				{
					m_View.Controller = this;
					SetElements(false); // set only the view elements, dont't initialize the variables
				}
			}
		}


		public void SetElements(bool bInit)
		{
			if(bInit)
			{
				m_xCol = m_PlotAssociation.XColumn;
				m_yCol = m_PlotAssociation.YColumn;
				m_PlotRange_From = m_PlotAssociation.PlotRangeStart;
				m_PlotRange_To   = m_PlotAssociation.PlotRangeLength==int.MaxValue ? int.MaxValue : m_PlotAssociation.PlotRangeStart+m_PlotAssociation.PlotRangeLength-1;
				CalcMaxPossiblePlotRangeTo();
			}

			if(null!=View)
			{
				string[] tables = App.Current.Doc.TableCollection.GetSortedTableNames();
				View.Tables_Initialize(tables,0);
				
				string[] columns = App.Current.Doc.TableCollection[tables[0]].DataColumns.GetColumnNames();
				View.Columns_Initialize(columns,0);

				View.XColumn_Initialize(m_xCol==null ? String.Empty : m_xCol.FullName);
				View.YColumn_Initialize(m_yCol==null ? String.Empty : m_yCol.FullName);

				View.PlotRangeFrom_Initialize(m_PlotRange_From);
				CalcMaxPossiblePlotRangeTo();
			}
		}

		void CalcMaxPossiblePlotRangeTo()
		{
			int len = int.MaxValue;
			if(m_xCol is Altaxo.Data.IDefinedCount)
				len = Math.Min(len,((Altaxo.Data.IDefinedCount)m_xCol).Count);
			if(m_yCol is Altaxo.Data.IDefinedCount)
				len = Math.Min(len,((Altaxo.Data.IDefinedCount)m_yCol).Count);

			m_MaxPossiblePlotRange_To = len-1;

			if(null!=View)
				View.PlotRangeTo_Initialize(Math.Min(this.m_PlotRange_To, m_MaxPossiblePlotRange_To));
		}

		public void EhView_TableSelectionChanged(int tableindex, string tablename)
		{
			if(null!=View)
			{
				string[] columns = App.Current.Doc.TableCollection[tablename].DataColumns.GetColumnNames();
				View.Columns_Initialize(columns,0);
			}
		}

		public void EhView_ToX(int tableindex, string tablename, int columnindex, string columnname)
		{
			SetDirty();
			m_xCol = App.Current.Doc.TableCollection[tablename][columnname];
			if(null!=View)
				View.XColumn_Initialize(m_xCol==null ? String.Empty : m_xCol.FullName);
		}
		public void EhView_ToY(int tableindex, string tablename, int columnindex, string columnname)
		{
			SetDirty();
			m_yCol = App.Current.Doc.TableCollection[tablename][columnname];
			if(null!=View)
				View.YColumn_Initialize(m_xCol==null ? String.Empty : m_yCol.FullName);
		}
		public void EhView_EraseX()
		{
			SetDirty();
			m_xCol = null;
			if(null!=View)
				View.XColumn_Initialize(m_xCol==null ? String.Empty : m_xCol.FullName);
		}
		public void EhView_EraseY()
		{
			SetDirty();
			m_yCol = null;
			if(null!=View)
				View.YColumn_Initialize(m_xCol==null ? String.Empty : m_yCol.FullName);
		}

		public bool EhView_RangeFrom(int val)
		{
			SetDirty();
			this.m_PlotRange_From = val;
			return false;
		}
		public bool EhView_RangeTo(int val)
		{
			SetDirty();
			this.m_PlotRange_To = val;
			return false;
		}


		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			m_PlotAssociation.XColumn = m_xCol;
			m_PlotAssociation.YColumn = m_yCol;
			m_PlotAssociation.PlotRangeStart = this.m_PlotRange_From;
			m_PlotAssociation.PlotRangeLength = this.m_PlotRange_To >= this.m_MaxPossiblePlotRange_To ? int.MaxValue : this.m_PlotRange_To+1-this.m_PlotRange_From;

			m_bDirty = false;
			return true; // successfull
		}

		#endregion

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return View;
			}
			set
			{
				View = value as ILineScatterPlotDataView;
			}
		}

		#endregion
	}
}
