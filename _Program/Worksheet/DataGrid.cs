/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.Serialization;
using Altaxo.Serialization;

namespace Altaxo.Worksheet
{
	/// <summary>
	/// Summary description for AltaxoDataGrid.
	/// </summary>
	[SerializationSurrogate(0,typeof(DataGrid.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataGrid : System.Windows.Forms.UserControl
	{
		public enum SelectionType { Nothing, DataRowSelection, DataColumnSelection, PropertyColumnSelection }

		public class MyGridPanel : System.Windows.Forms.Panel
		{
			public MyGridPanel() : base()
			{/*
				SetStyle(ControlStyles.DoubleBuffer,true); // to avoid flickering during redraw
				SetStyle(ControlStyles.ResizeRedraw,false); // redraw anything if resized
				SetStyle(ControlStyles.AllPaintingInWmPaint,true); // all work is done in OnPaint
				SetStyle(ControlStyles.UserPaint,true); // dito
			*/}
		}


		protected SelectionType m_LastSelectionType = SelectionType.Nothing;

		protected Altaxo.Data.DataTable m_DataTable = null;
		
		/// <summary>
		/// defaultColumnsStyles stores the default column Styles in a Hashtable
		/// the key for the hash table is the Type of the ColumnStyle
		/// </summary>
		protected System.Collections.Hashtable m_DefaultColumnStyles = new System.Collections.Hashtable();

		/// <summary>
		/// m_ColumnStyles stores the column styles for each data column individually,
		/// key is the data column itself.
		/// There is no need to store a column style here if the column is styled as default,
		/// instead the defaultColumnStyle is used in this case
		/// </summary>
		protected internal System.Collections.Hashtable m_ColumnStyles = new System.Collections.Hashtable();
		

		/// <summary>
		/// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
		/// </summary>
		//public    System.Collections.Hashtable columnScripts = new System.Collections.Hashtable();

		protected RowHeaderStyle m_RowHeaderStyle = new RowHeaderStyle(); // holds the style of the row header (leftmost column of data grid)
		protected ColumnHeaderStyle m_ColumnHeaderStyle = new ColumnHeaderStyle(); // the style of the column header (uppermost row of datagrid)
		protected ColumnHeaderStyle m_PropertyColumnHeaderStyle = new ColumnHeaderStyle();
		/// <summary>
		/// holds the positions (int) of the right boundarys of the __visible__ (!) columns
		/// i.e. columnBordersCache[0] is the with of the rowHeader plus the width of column[0]
		/// </summary>
		protected ColumnStyleCache m_ColumnStyleCache = new ColumnStyleCache();
		private int m_HorzScrollPos;
		private int m_VertScrollPos;
		
		private int  m_LastVisibleColumn=0;
		private int  m_LastFullyVisibleColumn=0;
		public IndexSelection m_SelectedColumns = new Altaxo.Worksheet.IndexSelection(); // holds the selected columns
		public IndexSelection m_SelectedRows    = new Altaxo.Worksheet.IndexSelection(); // holds the selected rows
		private int m_NumberOfTableRows=0; // cached number of rows of the table
		private int m_NumberOfPropertyCols; // cached number of property  columnsof the table
		private bool m_ShowColumnProperties=true; // are the property columns visible?
		private int m_NumberOfTableCols=0;
		private System.Windows.Forms.TextBox m_CellEditControl; // cached number of cols of the table

		private Point m_MouseDownPosition; // holds the position of a double click
		private int  m_DragColumnWidth_ColumnNumber=int.MinValue; // stores the column number if mouse hovers over separator
		private int  m_DragColumnWidth_OriginalPos = 0;
		private int  m_DragColumnWidth_OriginalWidth=0;
		private bool m_DragColumnWidth_InCapture=false;
	



		private int                          m_CellEdit_nRow; // Row wich is edited by the control
		private int                          m_CellEdit_nCol;
		private bool                         m_CellEdit_IsArmed=false;



		private System.Windows.Forms.HScrollBar m_HorzScrollBar;
		private System.Windows.Forms.VScrollBar m_VertScrollBar;
		private Altaxo.Worksheet.DataGrid.MyGridPanel m_GridPanel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DataGrid()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			m_CellEditControl = new System.Windows.Forms.TextBox();
			m_CellEditControl.AcceptsTab = true;
			m_CellEditControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			m_CellEditControl.Location = new System.Drawing.Point(392, 0);
			m_CellEditControl.Multiline = true;
			m_CellEditControl.Name = "m_CellEditControl";
			m_CellEditControl.TabIndex = 0;
			m_CellEditControl.Text = "";
			m_CellEditControl.Hide();
			m_CellEditControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnCellEditControl_KeyDown);
			m_CellEditControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnCellEditControl_KeyPress);
			m_GridPanel.Controls.Add(m_CellEditControl);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_HorzScrollBar = new System.Windows.Forms.HScrollBar();
			this.m_VertScrollBar = new System.Windows.Forms.VScrollBar();
			this.m_GridPanel = new Altaxo.Worksheet.DataGrid.MyGridPanel();
			this.SuspendLayout();
			// 
			// m_HorzScrollBar
			// 
			this.m_HorzScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_HorzScrollBar.Location = new System.Drawing.Point(0, 344);
			this.m_HorzScrollBar.Name = "m_HorzScrollBar";
			this.m_HorzScrollBar.Size = new System.Drawing.Size(424, 16);
			this.m_HorzScrollBar.TabIndex = 0;
			this.m_HorzScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnHorzScrollBar_Scroll);
			// 
			// m_VertScrollBar
			// 
			this.m_VertScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.m_VertScrollBar.Location = new System.Drawing.Point(408, 0);
			this.m_VertScrollBar.Name = "m_VertScrollBar";
			this.m_VertScrollBar.Size = new System.Drawing.Size(16, 344);
			this.m_VertScrollBar.TabIndex = 1;
			this.m_VertScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnVertScrollBar_Scroll);
			// 
			// m_GridPanel
			// 
			this.m_GridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_GridPanel.Name = "m_GridPanel";
			this.m_GridPanel.Size = new System.Drawing.Size(408, 344);
			this.m_GridPanel.TabIndex = 2;
			this.m_GridPanel.Click += new System.EventHandler(this.OnGridPanel_Click);
			this.m_GridPanel.Resize += new System.EventHandler(this.OnGridPanel_Resize);
			this.m_GridPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnGridPanel_MouseUp);
			this.m_GridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnGridPanel_Paint);
			this.m_GridPanel.DoubleClick += new System.EventHandler(this.OnGridPanel_DoubleClick);
			this.m_GridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnGridPanel_MouseMove);
			this.m_GridPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnGridPanel_MouseDown);
			// 
			// DataGrid
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.m_GridPanel,
																																	this.m_VertScrollBar,
																																	this.m_HorzScrollBar});
			this.Name = "DataGrid";
			this.Size = new System.Drawing.Size(424, 360);
			this.ResumeLayout(false);

		}
		#endregion

		private void OnGridPanel_Resize(object sender, System.EventArgs e)
		{
			m_ColumnStyleCache.Update(this);
		}


		#region "Serialization"
		// -------------------------------------------------------------------------------------------------------------------
		//
		// Serialization of a Windows Form object requires special measures. I decided to use a special trick:
		// 1. For Serialization I use a SerializationSurrogate which casts the type of serialization object to itself's type 
		//    (and __not__ the type of the data grid)
		// 2. So during deserialization this serializationSurrogate type is deserialized, which requires that this type
		//    derives from ISerializable, and the special deserialization constructor is used for deserialization
		// 3. the data grid object itself can be created only after the entire object graph is deserialized
		//    this is only ensured after the deserialization of Altaxo.Document
		//    then the function GetRealObject is called on the child objects of Altaxo.Document, which are itself free to
		//    call GetRealObject
		//
		// -------------------------------------------------------------------------------------------------------------------
		public class SerializationSurrogate0 : IDeserializationSubstitute, System.Runtime.Serialization.ISerializationSurrogate, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
		{
			protected Altaxo.Data.DataTable m_DataTable;
			protected System.Collections.Hashtable m_ColumnStyles;
			protected System.Collections.Hashtable m_DefaultColumnStyles;
			protected RowHeaderStyle m_RowHeaderStyle;
			protected ColumnHeaderStyle m_ColumnHeaderStyle;


			// we need a empty constructor
			public SerializationSurrogate0() {}
			// not used for deserialization, since the ISerializable constructor is used for that
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector){return obj;}
			// not used for serialization, instead the ISerializationSurrogate is used for that
			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)	{}

			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{	
				//info.SetType(typeof(DataGridDeserializer));
				info.SetType(this.GetType());
				DataGrid s = (DataGrid)obj;
				info.AddValue("DataTable",s.m_DataTable);
				info.AddValue("DefColumnStyles",s.m_DefaultColumnStyles);
				info.AddValue("ColumnStyles",s.m_ColumnStyles);
				info.AddValue("RowHeaderStyle",s.m_RowHeaderStyle);
				info.AddValue("ColumnHeaderStyle",s.m_ColumnHeaderStyle);
			}


			public SerializationSurrogate0(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
			{
				m_DataTable = (Altaxo.Data.DataTable)info.GetValue("DataTable",typeof(Altaxo.Data.DataTable));
				m_DefaultColumnStyles= (System.Collections.Hashtable)info.GetValue("DefColumnStyles",typeof(System.Collections.Hashtable));
				m_ColumnStyles = (System.Collections.Hashtable)info.GetValue("ColumnStyles",typeof(System.Collections.Hashtable));
				m_RowHeaderStyle = (RowHeaderStyle)info.GetValue("RowHeaderStyle",typeof(RowHeaderStyle));
				m_ColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("ColumnHeaderStyle",typeof(ColumnHeaderStyle));
			}

			public void OnDeserialization(object o)
			{
			}

			public object GetRealObject(object parent)
			{
				DataGrid dg = new DataGrid();
				DeserializationFinisher finisher = new DeserializationFinisher(dg);
			
				if(null!=parent)
					dg.Parent = (System.Windows.Forms.Control)parent;
			

				dg.m_ColumnStyles = m_ColumnStyles;
				dg.m_DefaultColumnStyles = this.m_DefaultColumnStyles;
				dg.m_RowHeaderStyle = this.m_RowHeaderStyle; this.m_RowHeaderStyle=null;
				dg.m_ColumnHeaderStyle = this.m_ColumnHeaderStyle; this.m_ColumnHeaderStyle=null;
				
	
				// finish deserialization of styles
				foreach(ColumnStyle cs in dg.m_ColumnStyles.Values) 
					cs.OnDeserialization(finisher);
				
				foreach(ColumnStyle cs in dg.m_DefaultColumnStyles.Values)
					cs.OnDeserialization(finisher);
				dg.m_RowHeaderStyle.OnDeserialization(finisher);
				dg.m_ColumnHeaderStyle.OnDeserialization(finisher);

				// now finish the datatable and set it
				m_DataTable.OnDeserialization(finisher);
				dg.DataTable = m_DataTable; // this also restores the event links to the datatable

				return dg;
			}
		} // end SerializationSurrogate0

		#endregion

		#region "public properties"

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Altaxo.Data.DataTable DataTable
		{
			get
			{
				return m_DataTable;
			}
			set
			{
				if(null!=m_DataTable)
				{
					m_DataTable.FireDataChanged -= new Altaxo.Data.DataTable.OnDataChanged(this.OnTableDataChanged);
					m_DataTable.PropCols.FireDataChanged -= new Altaxo.Data.DataTable.OnDataChanged(this.OnPropertyDataChanged);
				}

				m_DataTable = value;
				if(null!=m_DataTable)
				{
					m_DataTable.FireDataChanged += new Altaxo.Data.DataTable.OnDataChanged(this.OnTableDataChanged);
					m_DataTable.PropCols.FireDataChanged += new Altaxo.Data.DataTable.OnDataChanged(this.OnPropertyDataChanged);
					this.m_NumberOfTableCols = m_DataTable.ColumnCount;
					this.m_NumberOfTableRows = m_DataTable.RowCount;
					this.m_NumberOfPropertyCols = m_DataTable.PropCols.ColumnCount;

					SetScrollPositionTo(0,0);
					m_ColumnStyleCache.ForceUpdate(this);
					m_GridPanel.Invalidate();
				}
				else // Data table is null
				{
					this.m_NumberOfTableCols = 0;
					this.m_NumberOfTableRows = 0;
					m_ColumnStyleCache.Clear();
					SetScrollPositionTo(0,0);
					m_GridPanel.Invalidate();
				}
			}	
		}
		

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HorzScrollPos
		{
			get { return m_HorzScrollPos; }
			set
			{
				int oldValue = m_HorzScrollPos;
				m_HorzScrollPos=value;

				if(value!=oldValue)
				{

					if(m_CellEditControl.Visible)
					{
						this.ReadCellEditContent();
						m_CellEditControl.Hide();
					}

					this.m_HorzScrollBar.Value = value;
					this.m_ColumnStyleCache.ForceUpdate(this);
					m_GridPanel.Invalidate();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VertScrollPos
		{
			get { return m_VertScrollPos; }
			set
			{
				int oldValue = m_VertScrollPos;
				m_VertScrollPos=value;

				if(value!=oldValue)
				{
					if(m_CellEditControl.Visible)
					{
						this.ReadCellEditContent();
						m_CellEditControl.Hide();
					}

					this.m_VertScrollBar.Value = value;
					m_GridPanel.Invalidate();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisibleColumn
		{
			get
			{
				return HorzScrollPos;
			}
			set
			{
				HorzScrollPos=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleColumns
		{
			get
			{
				return this.m_LastVisibleColumn>=FirstVisibleColumn ? 1+m_LastVisibleColumn-FirstVisibleColumn : 0;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisibleColumns
		{
			get
			{
				return m_LastFullyVisibleColumn>=FirstVisibleColumn ? 1+m_LastFullyVisibleColumn-FirstVisibleColumn : 0;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastVisibleColumn
		{
			get
			{
				return FirstVisibleColumn + VisibleColumns -1;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisibleColumn
		{
			get
			{
				return FirstVisibleColumn + FullyVisibleColumns -1;
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisibleTableRow
		{
			get
			{
				return Math.Max(0,VertScrollPos - TotalEnabledPropertyColumns);
			}
			set
			{
				VertScrollPos = TotalEnabledPropertyColumns + Math.Max(0,value);
				m_GridPanel.Invalidate();
			}
		}


		public int GetFirstVisibleTableRow(int top)
		{
			int firstTotRow = (int)Math.Max(RemainingEnabledPropertyColumns,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height));
			return FirstVisibleTableRow + Math.Max(0,firstTotRow-RemainingEnabledPropertyColumns);
		}

		public int GetVisibleTableRows(int top, int bottom)
		{
			int firstTotRow = (int)Math.Max(RemainingEnabledPropertyColumns,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height));
			int lastTotRow  = (int)Math.Ceiling((bottom-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height)-1;
			return Math.Max(0,1 + lastTotRow - firstTotRow);
		}

		public int GetFullyVisibleTableRows(int top, int bottom)
		{
			int firstTotRow = (int)Math.Max(RemainingEnabledPropertyColumns,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height));
			int lastTotRow  = (int)Math.Floor((bottom-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height)-1;
			return Math.Max(0, 1+ lastTotRow - firstTotRow);
		}

		public int GetTopCoordinateOfTableRow(int nRow)
		{
			return		m_ColumnHeaderStyle.Height 
				+ RemainingEnabledPropertyColumns*m_RowHeaderStyle.Height
				+ (nRow-FirstVisibleTableRow)*m_RowHeaderStyle.Height;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleTableRows
		{
			get
			{
				return GetVisibleTableRows(0,m_GridPanel.Height);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisibleTableRows
		{
			get
			{
				return GetFullyVisibleTableRows(0,m_GridPanel.Height);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastVisibleTableRow
		{
			get
			{
				return FirstVisibleTableRow + VisibleTableRows -1;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisibleTableRow
		{
			get
			{
				return FirstVisibleTableRow + FullyVisibleTableRows - 1;
			}
		}

		/// <summary>Returns the remaining number of property columns that could be shown below the current scroll position.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RemainingEnabledPropertyColumns
		{
			get
			{
				return m_ShowColumnProperties ? Math.Max(0,this.m_NumberOfPropertyCols-VertScrollPos) : 0;
			}
		}

		/// <summary>Returns number of property columns that are enabled for been shown on the grid.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TotalEnabledPropertyColumns
		{
			get { return m_ShowColumnProperties ? this.m_NumberOfPropertyCols : 0; }
		}


		public int GetFirstVisiblePropertyColumn(int top)
		{
			int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height));
			return m_ShowColumnProperties ? firstTotRow+VertScrollPos : 0;
		}


		public int GetTopCoordinateOfPropertyColumn(int nCol)
		{
			return m_ColumnHeaderStyle.Height + (nCol-FirstVisiblePropertyColumn)*m_RowHeaderStyle.Height;
		}

		public int GetVisiblePropertyColumns(int top, int bottom)
		{
			if(this.m_ShowColumnProperties)
			{
				int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height));
				int lastTotRow  = (int)Math.Ceiling((bottom-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height)-1;
				int maxPossRows = Math.Max(0,RemainingEnabledPropertyColumns-firstTotRow);
				return Math.Min(maxPossRows,Math.Max(0,1 + lastTotRow - firstTotRow));
			}
			else
				return 0;
		}

		public int GetFullyVisiblePropertyColumns(int top, int bottom)
		{
			if(m_ShowColumnProperties)
			{
				int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height));
				int lastTotRow  = (int)Math.Floor((bottom-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height)-1;
				int maxPossRows = Math.Max(0,RemainingEnabledPropertyColumns-firstTotRow);
				return Math.Min(maxPossRows,Math.Max(0,1 + lastTotRow - firstTotRow));
			}
			else
				return 0;
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisiblePropertyColumns
		{
			get
			{
				return GetVisiblePropertyColumns(0,m_GridPanel.Height);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisiblePropertyColumns
		{
			get
			{
				return GetFullyVisiblePropertyColumns(0,m_GridPanel.Height);
			}
		}

		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisiblePropertyColumn
		{
			get
			{
				return (m_ShowColumnProperties && VertScrollPos<m_NumberOfPropertyCols) ? VertScrollPos : -1;
			}
		}



		public IndexSelection SelectedColumns
		{
			get	{	return m_SelectedColumns;	}
		}

		public IndexSelection SelectedRows
		{
			get { return m_SelectedRows; }
		}

		#endregion


		#region "public methods"


		public void RemoveSelected()
		{
			this.m_DataTable.SuspendDataChangedNotifications();


			// delete the selected columns
			if(this.m_SelectedColumns.Count>0)
			{

				int len = m_SelectedColumns.Count;
				int begin=-1;
				int end=-1; // note this _after_ the end of deleted columns
				int i;
				for(i=len-1;i>=0;i--)
				{
					int idx = m_SelectedColumns[i];
					if(begin<0)
					{
						begin=idx;
						end=idx+1;
					}
					else if(begin>=0 && idx==(begin-1))
					{
						begin=idx;
					}
					else
					{
						this.m_DataTable.RemoveColumns(begin,end-begin);
						begin=idx;
						end=idx+1;
					}
				} // end for
				// the last index must also be deleted, if not done already
				if(begin>=0 && end>=0)
					this.m_DataTable.RemoveColumns(begin,end-begin);


				this.m_SelectedColumns.Clear(); // now the columns are deleted, so they cannot be selected
			}


			// place here the code for selected rows
			if(this.m_SelectedRows.Count>0)
			{
				int begin=-1;
				int end=-1; // note this _after_ the end of deleted columns
				int i;
				for(i=m_SelectedRows.Count-1;i>=0;i--)
				{
					int idx = m_SelectedRows[i];
					if(begin<0)
					{
						begin=idx;
						end=idx+1;
					}
					else if(begin>=0 && idx==(begin-1))
					{
						begin=idx;
					}
					else
					{
						this.m_DataTable.DeleteRows(begin,end-begin);
						begin=idx;
						end=idx+1;
					}
				} // end for
				// the last index must also be deleted, if not done already
				if(begin>=0 && end>=0)
					this.m_DataTable.DeleteRows(begin,end-begin);


				this.m_SelectedRows.Clear(); // now the columns are deleted, so they cannot be selected
			}


			// end code for the selected rows
			this.Invalidate(); // necessary because we changed the selections
			this.m_DataTable.ResumeDataChangedNotifications();



		}


		public void SetSelectedColumnAsX()
		{
			if(SelectedColumns.Count>0)
			{
				m_DataTable[SelectedColumns[0]].XColumn=true;
				SelectedColumns.Clear();
				Invalidate(); // draw new because 
			}
		}

		public void SetSelectedColumnsGroup(int nGroup)
		{
			int len = SelectedColumns.Count;
			for(int i=0;i<len;i++)
			{
				m_DataTable[SelectedColumns[i]].Group = nGroup;
			}
			SelectedColumns.Clear();
			Invalidate();
		}


		public Altaxo.Worksheet.ColumnStyle GetColumnStyle(int i)
		{
			// zuerst in der ColumnStylesCollection nach dem passenden Namen
			// suchen, ansonsten default-Style zurückgeben
			Altaxo.Data.DataColumn dc = m_DataTable[i];
			Altaxo.Worksheet.ColumnStyle colstyle;

			// first look at the column styles hash table, column itself is the key
			colstyle = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[dc];
			if(null!=colstyle)
				return colstyle;
			
			// second look to the defaultcolumnstyles hash table, key is the type of the column style

			System.Type searchstyletype = dc.GetColumnStyleType();
			if(null==searchstyletype)
			{
				throw new ApplicationException("Error: Column of type +" + dc.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
			}
			else
			{
				if(null!=(colstyle = (Altaxo.Worksheet.ColumnStyle)m_DefaultColumnStyles[searchstyletype]))
					return colstyle;

				// if not successfull yet, we will create a new defaultColumnStyle
				colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
				m_DefaultColumnStyles.Add(searchstyletype,colstyle);
				return colstyle;
			}
		}



		public Altaxo.Worksheet.ColumnStyle GetPropertyColumnStyle(int i)
		{
			// zuerst in der ColumnStylesCollection nach dem passenden Namen
			// suchen, ansonsten default-Style zurückgeben
			Altaxo.Data.DataColumn dc = m_DataTable.PropCols[i];
			Altaxo.Worksheet.ColumnStyle colstyle;

			// first look at the column styles hash table, column itself is the key
			colstyle = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[dc];
			if(null!=colstyle)
				return colstyle;
			
			// second look to the defaultcolumnstyles hash table, key is the type of the column style

			System.Type searchstyletype = dc.GetColumnStyleType();
			if(null==searchstyletype)
			{
				throw new ApplicationException("Error: Column of type +" + dc.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
			}
			else
			{
				if(null!=(colstyle = (Altaxo.Worksheet.ColumnStyle)m_DefaultColumnStyles[searchstyletype]))
					return colstyle;

				// if not successfull yet, we will create a new defaultColumnStyle
				colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
				m_DefaultColumnStyles.Add(searchstyletype,colstyle);
				return colstyle;
			}
		}

		#endregion

		private void OnVertScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			VertScrollPos = e.NewValue;
		}

		private void OnHorzScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			HorzScrollPos = e.NewValue;
		}

		public void OnTableDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow)
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate

			int nOldRows = this.m_NumberOfTableRows;
			int nOldCols = this.m_NumberOfTableCols;

			m_NumberOfTableRows=m_DataTable.RowCount;
			m_NumberOfTableCols=m_DataTable.ColumnCount;

			if(nOldRows!=m_NumberOfTableRows)
			{
				if(this.VertScrollPos+1>m_NumberOfTableRows)
					VertScrollPos = m_NumberOfTableRows>0 ? m_NumberOfTableRows-1 : 0;

				m_VertScrollBar.Maximum = m_NumberOfTableRows>0 ? m_NumberOfTableRows-1	: 0;
				m_VertScrollBar.Refresh();
			}
			if(nOldCols!=m_NumberOfTableCols)
			{
				if(HorzScrollPos+1>m_NumberOfTableCols)
					HorzScrollPos = m_NumberOfTableCols>0 ? m_NumberOfTableCols-1 : 0;
	
				m_HorzScrollBar.Maximum = m_NumberOfTableCols>0 ? m_NumberOfTableCols-1 : 0;
				m_ColumnStyleCache.ForceUpdate(this);
				m_HorzScrollBar.Refresh();
			}

			m_GridPanel.Invalidate();

		}


		public void OnPropertyDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow)
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate
			int nOldPropCols = this.m_NumberOfPropertyCols;

			this.m_NumberOfPropertyCols=sender.ColumnCount;

			if(nOldPropCols!=this.m_NumberOfPropertyCols)
			{
				if(this.VertScrollPos+1>this.m_NumberOfTableRows+m_NumberOfPropertyCols)
					VertScrollPos = m_NumberOfTableRows+m_NumberOfPropertyCols>0 ? m_NumberOfTableRows+m_NumberOfPropertyCols-1 : 0;

				m_VertScrollBar.Maximum = m_NumberOfTableRows+m_NumberOfPropertyCols>0 ? m_NumberOfTableRows+m_NumberOfPropertyCols-1	: 0;
				m_VertScrollBar.Refresh();
			}

			m_GridPanel.Invalidate();

		}


		private int GetFirstAndNumberOfVisibleColumn(int left, int right, out int numVisibleColumns)
		{
			int nFirstCol = -1;
			int nLastCol = m_NumberOfTableCols;
			ColumnStyleCacheItem csci;
			
			for(int nCol=FirstVisibleColumn,i=0 ; i<m_ColumnStyleCache.Count ; nCol++,i++)
			{
				csci = ((ColumnStyleCacheItem)m_ColumnStyleCache[i]);
				if(csci.rightBorderPosition>left && nFirstCol<0)
					nFirstCol = nCol;
			
				if(csci.leftBorderPosition>=right)
				{
					nLastCol = nCol;
					break;
				}
			}

			numVisibleColumns = nFirstCol<0 ? 0 :  Math.Max(0,nLastCol-nFirstCol);
			return nFirstCol;
		}



		private Rectangle GetXCoordinatesOfColumn(int nCol, Rectangle cellRect)
		{
			int colOffs = nCol-FirstVisibleColumn;
			cellRect.X = ((ColumnStyleCacheItem)m_ColumnStyleCache[colOffs]).leftBorderPosition;
			cellRect.Width = ((ColumnStyleCacheItem)m_ColumnStyleCache[colOffs]).rightBorderPosition - cellRect.X;
			return cellRect;
		}

		private Rectangle GetXCoordinatesOfColumn(int nCol)
		{
			return GetXCoordinatesOfColumn(nCol,new Rectangle());
		}


		private Rectangle GetCoordinatesOfDataCell(int nCol, int nRow)
		{
			Rectangle cellRect = GetXCoordinatesOfColumn(nCol);

			cellRect.Y = this.GetTopCoordinateOfTableRow(nRow);
			cellRect.Height = this.m_RowHeaderStyle.Height;
			return cellRect;
		}



		private void OnTextBoxLostControl(object sender, System.EventArgs e)
		{
			this.ReadCellEditContent();
			m_CellEditControl.Hide();
		}

		private void OnCellEditControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)13) // Don't use the enter key, event is handled by KeyDown
			{
				e.Handled=true;
			}
			else if(e.KeyChar == (char)9) // Tab key pressed
			{
				if(m_CellEditControl.SelectionStart+m_CellEditControl.SelectionLength>=m_CellEditControl.TextLength)
				{
					e.Handled=true;
					// Navigate to the right
					NavigateCellEdit(1,0);
				}
			}

		}

		private void OnCellEditControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyData==System.Windows.Forms.Keys.Left)
			{
				// Navigate to the left if the cursor is already left
				if(m_CellEditControl.SelectionStart==0 && (m_CellEdit_nRow>0 || m_CellEdit_nCol>0) )
				{
					e.Handled=true;
					// Navigate to the left
					NavigateCellEdit(-1,0);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Right)
			{
				if(m_CellEditControl.SelectionStart+m_CellEditControl.SelectionLength>=m_CellEditControl.TextLength)
				{
					e.Handled=true;
					// Navigate to the right
					NavigateCellEdit(1,0);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Up && m_CellEdit_nRow>0)
			{
				e.Handled=true;
				// Navigate up
				NavigateCellEdit(0,-1);
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Down)
			{
				e.Handled=true;
				// Navigate down
				NavigateCellEdit(0,1);
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Enter)
			{
				// if some text is selected, deselect it and move the cursor to the end
				// else same action like keys.Down
				e.Handled=true;
				if(m_CellEditControl.SelectionLength>0)
				{
					m_CellEditControl.SelectionLength=0;
					m_CellEditControl.SelectionStart=m_CellEditControl.TextLength;
				}
				else
				{
					NavigateCellEdit(0,1);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Escape)
			{
				e.Handled=true;
				m_CellEdit_IsArmed=false;
				this.m_CellEditControl.Hide();
			}
		}



		/// <summary>
		/// retrieves, to which column should be scrolled in order to make
		/// the column nForLastCol the last visible column
		/// </summary>
		/// <param name="nForLastCol">the column number which should be the last visible column</param>
		/// <returns>the number of the first visible column</returns>
		public int GetFirstVisibleColumnForLastVisibleColumn(int nForLastCol)
		{
			
			int i = nForLastCol;
			int retv = nForLastCol;
			int horzSize = this.Right-m_RowHeaderStyle.Width-m_VertScrollBar.Width;
			while(i>=0)
			{
				horzSize -= GetColumnStyle(i).Width;
				if(horzSize>0 && i>0)
					i--;
				else
					break;
			}

			if(horzSize<0)
				i++; // increase one colum if size was bigger than available size

			return i<=nForLastCol ? i : nForLastCol;
		}

		/// <summary>
		/// SetScrollPositions only sets the scroll positions, and not Invalidates the 
		/// Area!
		/// </summary>
		/// <param name="nCol">first visible column (i.e. column at the left)</param>
		/// <param name="nRow">first visible row (i.e. row at the top)</param>
		protected void SetScrollPositionTo(int nCol, int nRow)
		{
			int oldCol = HorzScrollPos;
			if(m_HorzScrollBar.Maximum<nCol)
				m_HorzScrollBar.Maximum = nCol;
			HorzScrollPos=nCol;

			m_ColumnStyleCache.Update(this);

			if(m_VertScrollBar.Maximum<nRow)
				m_VertScrollBar.Maximum=nRow;
			VertScrollPos=nRow;
		}

		private void ReadCellEditContent()
		{
			if(this.m_CellEdit_IsArmed && this.m_CellEditControl.Modified)
			{
				GetColumnStyle(m_CellEdit_nCol).SetColumnValueAtRow(m_CellEditControl.Text,m_CellEdit_nRow,m_DataTable[m_CellEdit_nCol]);
				this.m_CellEdit_IsArmed=false;
			}
		}

		private void SetCellEditContent()
		{
			m_CellEditControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			m_CellEditControl.Text=GetColumnStyle(m_CellEdit_nCol).GetColumnValueAtRow(m_CellEdit_nRow,m_DataTable[m_CellEdit_nCol]);
			m_CellEditControl.SelectAll();
			m_CellEditControl.Modified=false;
			m_CellEditControl.Show();
			m_CellEditControl.Focus();
			this.m_CellEdit_IsArmed=true;
		}

		/// <summary>
		/// NavigateCellEdit moves the cell edit control to the next cell
		/// </summary>
		/// <param name="dx">move dx cells to the right</param>
		/// <param name="dy">move dy cells down</param>
		protected void NavigateCellEdit(int dx, int dy)
		{
			bool bScrolled = false;

			// 1. Read content of the cell edit, if neccessary write data back
			ReadCellEditContent();		
		
			// 2. look whether the new cell coordinates lie inside the client area, if
			// not scroll the worksheet appropriate
			int newCellCol = this.m_CellEdit_nCol + dx;
			if(newCellCol>=m_DataTable.ColumnCount)
			{
				newCellCol=0;
				dy+=1;
			}
			else if(newCellCol<0)
			{
				if(m_CellEdit_nRow>0) // move to the last cell only if not on cell 0
				{
					newCellCol=m_DataTable.ColumnCount-1;
					dy-=1;
				}
				else
				{
					newCellCol=0;
				}
			}

			int newCellRow = this.m_CellEdit_nRow + dy;
			if(newCellRow<0)
				newCellRow=0;
			// note: we do not catch the condition newCellRow>rowCount since we want to add new rows
			
			int navigateToCol;
			int navigateToRow;

			if(newCellCol<FirstVisibleColumn)
				navigateToCol = newCellCol;
			else if(newCellCol>LastFullyVisibleColumn)
				navigateToCol = GetFirstVisibleColumnForLastVisibleColumn(newCellCol);
			else
				navigateToCol = FirstVisibleColumn;

			if(newCellRow<FirstVisibleTableRow)
				navigateToRow = newCellRow;
			else if (newCellRow>LastFullyVisibleTableRow)
				navigateToRow = newCellRow + 1 - FullyVisibleTableRows;
			else
				navigateToRow = FirstVisibleTableRow;

			if(navigateToCol!=FirstVisibleColumn || navigateToRow!=FirstVisibleTableRow)
			{
				SetScrollPositionTo(navigateToCol,navigateToRow);
				bScrolled=true;
			}
			// 3. Fill the cell edit control with new content
			m_CellEdit_nCol=newCellCol;
			m_CellEdit_nRow=newCellRow;
			m_CellEditControl.Parent = m_GridPanel;
			Rectangle cellRect = this.GetCoordinatesOfDataCell(m_CellEdit_nCol,m_CellEdit_nRow);
			m_CellEditControl.Location = cellRect.Location;
			m_CellEditControl.Size = cellRect.Size;
			SetCellEditContent();

			// 4. Invalidate the client area if scrolled in step (2)
			if(bScrolled)
				m_GridPanel.Invalidate();

		}


		protected override void OnKeyDown(KeyEventArgs e)
		{
			e.Handled=true;
		}
	

		private void OnGridPanel_Click(object sender, System.EventArgs e)
		{
			ClickedCellInfo clickedCell = new ClickedCellInfo(this,this.m_MouseDownPosition);

			switch(clickedCell.ClickedArea)
			{
				case ClickedAreaType.DataCell:
				{
					//m_CellEditControl = new TextBox();
					m_CellEdit_nRow=clickedCell.Row;
					m_CellEdit_nCol=clickedCell.Column;
					m_CellEditControl.Parent = m_GridPanel;
					m_CellEditControl.Location = clickedCell.CellRectangle.Location;
					m_CellEditControl.Size = clickedCell.CellRectangle.Size;
					m_CellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
					this.SetCellEditContent();
				}
					break;
				case ClickedAreaType.DataColumnHeader:
				{
					if(!this.m_DragColumnWidth_InCapture)
					{
						bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
						bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
						if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
							m_SelectedRows.Clear(); // if we click a column, we remove row selections
						m_SelectedColumns.Select(clickedCell.Column,bShiftKey,bControlKey);
						m_LastSelectionType = SelectionType.DataColumnSelection;
						m_GridPanel.Invalidate();
					}
				}
					break;
				case ClickedAreaType.DataRowHeader:
				{
					bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
					bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
					if(m_LastSelectionType==SelectionType.DataColumnSelection && !bControlKey)
						m_SelectedColumns.Clear(); // if we click a column, we remove row selections
					m_SelectedRows.Select(clickedCell.Row,bShiftKey,bControlKey);
					m_LastSelectionType = SelectionType.DataRowSelection;
					m_GridPanel.Invalidate();
				}
					break;
			}
		}

		private void OnGridPanel_DoubleClick(object sender, System.EventArgs e)
		{
		
		}

		private void OnGridPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// base.OnMouseDown(e);
			this.m_MouseDownPosition = new Point(e.X, e.Y);
			m_CellEditControl.Hide();

			if(this.m_DragColumnWidth_ColumnNumber>=-1)
			{
				this.m_GridPanel.Capture=true;
				m_DragColumnWidth_OriginalPos = e.X;
				m_DragColumnWidth_InCapture=true;
			}

		}

		private void OnGridPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int Y = e.Y;
			int X = e.X;

			if(this.m_DragColumnWidth_InCapture)
			{
				int sizediff = X - this.m_DragColumnWidth_OriginalPos;
				
				Altaxo.Worksheet.ColumnStyle cs;
				if(-1==m_DragColumnWidth_ColumnNumber)
					cs = this.m_RowHeaderStyle;
				else
				{
					cs = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[m_DataTable[m_DragColumnWidth_ColumnNumber]];
				
					if(null==cs)
					{
						Altaxo.Worksheet.ColumnStyle template = GetColumnStyle(this.m_DragColumnWidth_ColumnNumber);
						cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
						m_ColumnStyles.Add(m_DataTable[m_DragColumnWidth_ColumnNumber],cs);
					}
				}

				int newWidth = this.m_DragColumnWidth_OriginalWidth + sizediff;
				if(newWidth<10)
					newWidth=10;
				cs.Width=newWidth;
				this.m_ColumnStyleCache.ForceUpdate(this);
				m_GridPanel.Invalidate();
			}
			else // not in Capture mode
			{
				if(Y<this.m_ColumnHeaderStyle.Height)
				{
					for(int i=this.m_ColumnStyleCache.Count-1;i>=0;i--)
					{
						ColumnStyleCacheItem csc = (ColumnStyleCacheItem)m_ColumnStyleCache[i];

						if(csc.rightBorderPosition-5 < X && X < csc.rightBorderPosition+5)
						{
							this.m_GridPanel.Cursor = System.Windows.Forms.Cursors.VSplit;
							this.m_DragColumnWidth_ColumnNumber = i+FirstVisibleColumn;
							this.m_DragColumnWidth_OriginalWidth = csc.columnStyle.Width;
							return;
						}
					} // end for

					if(this.m_RowHeaderStyle.Width -5 < X && X < m_RowHeaderStyle.Width+5)
					{
						this.m_GridPanel.Cursor = System.Windows.Forms.Cursors.VSplit;
						this.m_DragColumnWidth_ColumnNumber = -1;
						this.m_DragColumnWidth_OriginalWidth = this.m_RowHeaderStyle.Width;
						return;
					}
				}

				this.m_DragColumnWidth_ColumnNumber=int.MinValue;
				this.m_GridPanel.Cursor = System.Windows.Forms.Cursors.Default;
			} // end else
		
		}

		private void OnGridPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(this.m_DragColumnWidth_InCapture)
			{
				int sizediff = e.X - this.m_DragColumnWidth_OriginalPos;
				Altaxo.Worksheet.ColumnStyle cs;
				if(-1==m_DragColumnWidth_ColumnNumber)
				{
					cs = this.m_RowHeaderStyle;
				}
				else
				{
					cs = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[m_DataTable[m_DragColumnWidth_ColumnNumber]];
					if(null==cs)
					{
						Altaxo.Worksheet.ColumnStyle template = GetColumnStyle(this.m_DragColumnWidth_ColumnNumber);
						cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
						m_ColumnStyles.Add(m_DataTable[m_DragColumnWidth_ColumnNumber],cs);
					}
				}
				int newWidth = this.m_DragColumnWidth_OriginalWidth + sizediff;
				if(newWidth<10)
					newWidth=10;
				cs.Width=newWidth;
				this.m_ColumnStyleCache.ForceUpdate(this);

				this.m_DragColumnWidth_InCapture = false;
				this.m_DragColumnWidth_ColumnNumber = int.MinValue;
				this.Capture=false;
				this.Cursor = System.Windows.Forms.Cursors.Default;
				m_GridPanel.Invalidate();

			}
		}



		private void OnGridPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{

			Graphics dc=e.Graphics;
			Pen bluePen = new Pen(Color.Blue, 1);
			Brush brownBrush = new SolidBrush(Color.Aquamarine);

			bool bDrawColumnHeader = false;

			int firstTableRowToDraw     = this.GetFirstVisibleTableRow(e.ClipRectangle.Top);
			int numberOfTableRowsToDraw = this.GetVisibleTableRows(e.ClipRectangle.Top,e.ClipRectangle.Bottom);

			int firstPropertyColumnToDraw = this.GetFirstVisiblePropertyColumn(e.ClipRectangle.Top);
			int numberOfPropertyColumnsToDraw = this.GetVisiblePropertyColumns(e.ClipRectangle.Top,e.ClipRectangle.Bottom);

			bool bAreColumnsSelected = m_SelectedColumns.Count>0;
			bool bAreRowsSelected =    m_SelectedRows.Count>0;
			bool bAreCellsSelected =  bAreRowsSelected || bAreColumnsSelected;


			int yShift=0;



			dc.FillRectangle(brownBrush,e.ClipRectangle); // first set the background
			
			if(null==m_DataTable)
				return;

			Rectangle cellRectangle = new Rectangle();


			if(e.ClipRectangle.Top<m_ColumnHeaderStyle.Height)
			{
				bDrawColumnHeader = true;
			}

			// if neccessary, draw the row header (the most left column)
			if(e.ClipRectangle.Left<m_RowHeaderStyle.Width)
			{
				cellRectangle.Height = m_RowHeaderStyle.Height;
				cellRectangle.Width = m_RowHeaderStyle.Width;
				cellRectangle.X=0;
				

				// if visible, draw property column header items
				yShift=this.GetTopCoordinateOfPropertyColumn(firstPropertyColumnToDraw);
				for(int nCol=firstPropertyColumnToDraw, nInc=0;nInc<numberOfPropertyColumnsToDraw;nCol++,nInc++)
				{
					cellRectangle.Y = yShift+nInc*m_RowHeaderStyle.Height;
					this.m_PropertyColumnHeaderStyle.Paint(dc,cellRectangle,nCol,this.m_DataTable.PropCols[nCol],false);
				}
			}

			// draw the table row Header Items
			yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
			for(int nRow = firstTableRowToDraw,nInc=0; nInc<numberOfTableRowsToDraw; nRow++,nInc++)
			{
				cellRectangle.Y = yShift+nInc*m_RowHeaderStyle.Height;
				m_RowHeaderStyle.Paint(dc,cellRectangle,nRow,null, bAreRowsSelected && m_SelectedRows.ContainsKey(nRow));
			}
			

			if(e.ClipRectangle.Bottom>=m_ColumnHeaderStyle.Height || e.ClipRectangle.Right>=m_RowHeaderStyle.Width)		
			{
				int numberOfColumnsToDraw;
				int firstColToDraw =this.GetFirstAndNumberOfVisibleColumn(e.ClipRectangle.Left,e.ClipRectangle.Right, out numberOfColumnsToDraw);

				// draw the property columns
				for(int nPropCol=firstPropertyColumnToDraw, nIncPropCol=0; nIncPropCol<numberOfPropertyColumnsToDraw; nPropCol++, nIncPropCol++)
				{
					Altaxo.Worksheet.ColumnStyle cs = GetPropertyColumnStyle(nPropCol);
					cellRectangle.Y=this.GetTopCoordinateOfPropertyColumn(nPropCol);
					cellRectangle.Height = m_RowHeaderStyle.Height;
					
					for(int nCol=firstColToDraw, nIncCol=0; nIncCol<numberOfColumnsToDraw; nCol++,nIncCol++)
					{
						cellRectangle = this.GetXCoordinatesOfColumn(nCol,cellRectangle);
						cs.Paint(dc,cellRectangle,nCol,m_DataTable.PropCols[nPropCol],false);
					}
				}


				// draw the cells
				//int firstColToDraw = firstVisibleColumn+(e.ClipRectangle.Left-m_RowHeaderStyle.Width)/columnWidth;
				//int lastColToDraw  = firstVisibleColumn+(int)Math.Ceiling((e.ClipRectangle.Right-m_RowHeaderStyle.Width)/columnWidth);

				for(int nCol=firstColToDraw, nIncCol=0; nIncCol<numberOfColumnsToDraw; nCol++,nIncCol++)
				{
					Altaxo.Worksheet.ColumnStyle cs = GetColumnStyle(nCol);
					cellRectangle = this.GetXCoordinatesOfColumn(nCol,cellRectangle);

					bool bColumnSelected = bAreColumnsSelected && m_SelectedColumns.ContainsKey(nCol);
					bool bDataColumnIncluded = bAreColumnsSelected  ? bColumnSelected : true;


					if(bDrawColumnHeader) // must the column Header been drawn?
					{
						cellRectangle.Height = m_ColumnHeaderStyle.Height;
						cellRectangle.Y=0;
						m_ColumnHeaderStyle.Paint(dc,cellRectangle,0,m_DataTable[nCol],bColumnSelected);
					}

	
					yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
					cellRectangle.Height = m_RowHeaderStyle.Height;
					for(int nRow=firstTableRowToDraw, nIncRow=0;nIncRow<numberOfTableRowsToDraw;nRow++,nIncRow++)
					{
						bool bRowSelected = bAreRowsSelected && m_SelectedRows.ContainsKey(nRow);
						bool bDataRowIncluded = bAreRowsSelected ? bRowSelected : true;
						cellRectangle.Y= yShift+nIncRow*m_RowHeaderStyle.Height;
						cs.Paint(dc,cellRectangle,nRow,m_DataTable[nCol],bAreCellsSelected && bDataColumnIncluded && bDataRowIncluded);
					}
				}
			}
		
		}



		#region Column style cache

		public class ColumnStyleCacheItem
		{
			public Altaxo.Worksheet.ColumnStyle columnStyle;
			public int leftBorderPosition;
			public int rightBorderPosition;


			public ColumnStyleCacheItem(Altaxo.Worksheet.ColumnStyle cs, int leftBorderPosition, int rightBorderPosition)
			{
				this.columnStyle = cs;
				this.leftBorderPosition = leftBorderPosition;
				this.rightBorderPosition = rightBorderPosition;
			}

		}


		public class ColumnStyleCache : System.Collections.CollectionBase
		{
			protected int m_CachedFirstVisibleColumn=0; // the column number of the first cached item, i.e. for this[0]
			protected int m_CachedWidth=0; // cached width of painting area
 
			public ColumnStyleCacheItem this[int i]
			{
				get { return (ColumnStyleCacheItem)base.InnerList[i]; }
			}

			public void Add(ColumnStyleCacheItem item)
			{
				base.InnerList.Add(item);
			}

	

			public void Update(DataGrid dg)
			{
				if(	(this.Count==0)
					||(dg.m_GridPanel.Width!=this.m_CachedWidth)
					||(dg.FirstVisibleColumn != this.m_CachedFirstVisibleColumn) )
				{
					ForceUpdate(dg);
				}
			}

			public void ForceUpdate(DataGrid dg)
			{
				dg.m_LastVisibleColumn=0;
				dg.m_LastFullyVisibleColumn = 0;

				this.Clear(); // clear all items

				if(null==dg.m_DataTable)
					return;
		
				int actualColumnLeft = 0; 
				int actualColumnRight = dg.m_RowHeaderStyle.Width;
			
				this.m_CachedWidth = dg.m_GridPanel.Width;
				dg.m_LastFullyVisibleColumn = dg.FirstVisibleColumn;

				for(int i=dg.FirstVisibleColumn;i<dg.m_DataTable.ColumnCount && actualColumnLeft<this.m_CachedWidth;i++)
				{
					actualColumnLeft = actualColumnRight;
					Altaxo.Worksheet.ColumnStyle cs = dg.GetColumnStyle(i);
					actualColumnRight = actualColumnLeft+cs.Width;
					this.Add(new ColumnStyleCacheItem(cs,actualColumnLeft,actualColumnRight));

					if(actualColumnLeft<this.m_CachedWidth)
						dg.m_LastVisibleColumn = i;

					if(actualColumnRight<=this.m_CachedWidth)
						dg.m_LastFullyVisibleColumn = i;
				}
			}
		}

		#endregion

		#region Class ClickedCellInfo



		/// <summary>The type of area we have clicked into, used by ClickedCellInfo.</summary>
		public enum ClickedAreaType 
		{ 
			/// <summary>Outside of all relevant areas.</summary>
			OutsideAll,
			/// <summary>On the table header (top left corner of the data grid).</summary>
			TableHeader,
			/// <summary>Inside a data cell.</summary>
			DataCell,
			/// <summary>Inside a property cell.</summary>
			PropertyCell,
			/// <summary>On the column header.</summary>
			DataColumnHeader,
			/// <summary>On the row header.</summary>
			DataRowHeader,
			/// <summary>On the property column header.</summary>
			PropertyColumnHeader
		}


		/// <remarks>
		/// ClickedCellInfo retrieves (from mouse coordinates of a click), which cell has clicked onto. 
		/// </remarks>
		public class ClickedCellInfo
		{

			/// <summary>The enclosing Rectangle of the clicked cell</summary>
			protected Rectangle m_CellRectangle;

			/// <summary>The data row clicked onto.</summary>
			protected int m_Row;
			/// <summary>The data column number clicked onto.</summary>
			protected int m_Column;

			/// <summary>What have been clicked onto.</summary>
			protected ClickedAreaType m_ClickedArea;


			/// <value>The enclosing Rectangle of the clicked cell</value>
			public Rectangle CellRectangle { get { return m_CellRectangle; }}
			/// <value>The row number clicked onto.</value>
			public int Row { get { return m_Row; }}
			/// <value>The column number clicked onto.</value>
			public int Column { get { return m_Column; }}
			/// <value>The type of area clicked onto.</value>
			public ClickedAreaType ClickedArea { get { return m_ClickedArea; }}
 
			/// <summary>
			/// Retrieves the column number clicked onto 
			/// </summary>
			/// <param name="dg">The parent data grid</param>
			/// <param name="mouseCoord">The coordinates of the mouse click.</param>
			/// <param name="cellRect">The function sets the x-properties (X and Width) of the cell rectangle.</param>
			/// <returns>Either -1 when clicked on the row header area, column number when clicked in the column range, or int.MinValue when clicked outside of all.</returns>
			private int GetColumnNumber(DataGrid dg, Point mouseCoord, ref Rectangle cellRect)
			{
				int firstVisibleColumn = dg.FirstVisibleColumn;
				int actualColumnRight = dg.m_RowHeaderStyle.Width;
				int columnCount = dg.m_DataTable.ColumnCount;

				if(mouseCoord.X<actualColumnRight)
				{
					cellRect.X=0; cellRect.Width=actualColumnRight;
					return -1;
				}

				for(int i=firstVisibleColumn;i<columnCount;i++)
				{
					cellRect.X=actualColumnRight;
					Altaxo.Worksheet.ColumnStyle cs = dg.GetColumnStyle(i);
					actualColumnRight += cs.Width;
					if(actualColumnRight>mouseCoord.X)
					{
						cellRect.Width = cs.Width;
						return i;
					}
				} // end for
				return int.MinValue;
			}

			/// <summary>
			/// Returns the row number of the clicked cell.
			/// </summary>
			/// <param name="dg">The parent data grid.</param>
			/// <param name="mouseCoord">The mouse coordinates of the click.</param>
			/// <param name="cellRect">Returns the bounding rectangle of the clicked cell.</param>
			/// <param name="bPropertyCol">True if clicked on either the property column header or a property column, else false.</param>
			/// <returns>The row number of the clicked cell, or -1 if clicked on the column header.</returns>
			/// <remarks>If clicked onto a property cell, the function returns the property column number.</remarks>
			private int GetRowNumber(DataGrid dg, Point mouseCoord, ref Rectangle cellRect, ref bool bPropertyCol)
			{
				int firstVisibleColumn = dg.FirstVisibleColumn;
				int actualColumnRight = dg.m_RowHeaderStyle.Width;
				int columnCount = dg.m_DataTable.ColumnCount;

				if(mouseCoord.Y<dg.m_ColumnHeaderStyle.Height)
				{
					cellRect.Y=0; cellRect.Height=dg.m_ColumnHeaderStyle.Height;
					return -1;
				}

				// calculate the raw row number
				int rawrow = (int)Math.Floor((mouseCoord.Y-dg.m_ColumnHeaderStyle.Height)/(double)dg.m_RowHeaderStyle.Height);

				cellRect.Y= dg.m_ColumnHeaderStyle.Height + rawrow * dg.m_RowHeaderStyle.Height;
				cellRect.Height = dg.m_RowHeaderStyle.Height;

				if(rawrow < dg.VisiblePropertyColumns)
				{
					bPropertyCol=true;
					return dg.FirstVisiblePropertyColumn+rawrow;
				}
				else
				{
					bPropertyCol=false;
					return dg.FirstVisibleTableRow + rawrow - dg.VisiblePropertyColumns;
				}
			}




			/// <summary>
			/// Creates the ClickedCellInfo from the data grid and the mouse coordinates of the click.
			/// </summary>
			/// <param name="dg">The data grid.</param>
			/// <param name="mouseCoord">The mouse coordinates of the click.</param>
			public ClickedCellInfo(DataGrid dg, Point mouseCoord)
			{

				bool bIsPropertyColumn=false;
				m_Column = GetColumnNumber(dg,mouseCoord,ref m_CellRectangle);
				m_Row    = GetRowNumber(dg,mouseCoord,ref m_CellRectangle, ref bIsPropertyColumn);

				if(bIsPropertyColumn)
				{
					if(m_Column==-1)
						m_ClickedArea = ClickedAreaType.PropertyColumnHeader;
					else if(m_Column>=0)
						m_ClickedArea = ClickedAreaType.PropertyCell;
					else
						m_ClickedArea = ClickedAreaType.OutsideAll;

					int h=m_Column; m_Column = m_Row; m_Row = h; // Swap columns and rows since it is a property column
				}
				else // it is not a property related cell
				{
					if(m_Row==-1 && m_Column==-1)
						m_ClickedArea = ClickedAreaType.TableHeader;
					else if(m_Row==-1 && m_Column>=0)
						m_ClickedArea = ClickedAreaType.DataColumnHeader;
					else if(m_Row>=0 && m_Column==-1)
						m_ClickedArea = ClickedAreaType.DataRowHeader;
					else if(m_Row>=0 && m_Column>=0)
						m_ClickedArea = ClickedAreaType.DataCell;
					else
						m_ClickedArea = ClickedAreaType.OutsideAll;
				}
			}
		} // end of class ClickedCellInfo

		#endregion Class ClickedCellInfo
	}
}
