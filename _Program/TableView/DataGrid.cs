using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.Serialization;
using Altaxo.Serialization;

namespace Altaxo.TableView
{
	public class ColumnStyleCacheItem
	{
		public Altaxo.TableView.ColumnStyle columnStyle;
		public int leftBorderPosition;
		public int rightBorderPosition;


		public ColumnStyleCacheItem(Altaxo.TableView.ColumnStyle cs, int leftBorderPosition, int rightBorderPosition)
		{
			this.columnStyle = cs;
			this.leftBorderPosition = leftBorderPosition;
			this.rightBorderPosition = rightBorderPosition;
		}

	}



	/// <summary>
	/// Summary description for AltaxoDataGrid.
	/// </summary>
	[SerializationSurrogate(0,typeof(DataGrid.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataGrid : System.Windows.Forms.Control
	{
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
		
		// members, not neccessary to serialize

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		/// <summary>
		/// holds the positions (int) of the right boundarys of the __visible__ (!) columns
		/// i.e. columnBordersCache[0] is the with of the rowHeader plus the width of column[0]
		/// </summary>
		protected System.Collections.ArrayList columnStyleCache = new System.Collections.ArrayList();

		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private int  nLastVisibleColumn=0;
		private int  nLastFullyVisibleColumn=0;
		public IndexSelection m_SelectedColumns = new Altaxo.TableView.IndexSelection(); // holds the selected columns
		public IndexSelection m_SelectedRows    = new Altaxo.TableView.IndexSelection(); // holds the selected rows
		private int numberOfRows=0; // cached number of rows of the table
		private int numberOfCols=0;
		private System.Windows.Forms.TextBox cellEditControl; // cached number of cols of the table

		private Point mouseDownPosition; // holds the position of a double click
		private int  dragColumnWidth_ColumnNumber=int.MinValue; // stores the column number if mouse hovers over separator
		private int  dragColumnWidth_OriginalPos = 0;
		private int  dragColumnWidth_OriginalWidth=0;
		private bool dragColumnWidth_InCapture=false;
	


		private int                          cellEdit_nRow; // Row wich is edited by the control
		private int                          cellEdit_nCol; // Column which is edited by the control
		private bool                         cellEdit_IsArmed=false;

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
			
				dg.m_DataTable = m_DataTable;

				dg.m_ColumnStyles = m_ColumnStyles;
				dg.m_DefaultColumnStyles = this.m_DefaultColumnStyles;
				dg.m_RowHeaderStyle = this.m_RowHeaderStyle; this.m_RowHeaderStyle=null;
				dg.m_ColumnHeaderStyle = this.m_ColumnHeaderStyle; this.m_ColumnHeaderStyle=null;
				
				// now finish the dependent objects
				dg.m_DataTable.OnDeserialization(finisher);
				foreach(ColumnStyle cs in dg.m_ColumnStyles.Values) cs.OnDeserialization(finisher);
				foreach(ColumnStyle cs in dg.m_DefaultColumnStyles.Values) cs.OnDeserialization(finisher);
				dg.m_RowHeaderStyle.OnDeserialization(finisher);
				dg.m_ColumnHeaderStyle.OnDeserialization(finisher);
				
				return dg;
			}
		} // end SerializationSurrogate0

		#endregion


		#region "Constructor"
		public DataGrid()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			
			vScrollBar1.Maximum=numberOfRows;
			hScrollBar1.Maximum=numberOfCols;
			
			this.SetStyle(ControlStyles.DoubleBuffer,true); // to avoid flickering during redraw
			this.SetStyle(ControlStyles.ResizeRedraw,false); // redraw anything if resized
			this.SetStyle(ControlStyles.AllPaintingInWmPaint,true); // all work is done in OnPaint
			this.SetStyle(ControlStyles.UserPaint,true); // dito
			
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

		#endregion


		#region "public properties"
		public Altaxo.Data.DataTable DataTable
		{
			get
			{
				return m_DataTable;
			}
			set
			{
				if(null!=m_DataTable)
					m_DataTable.FireDataChanged -= new Altaxo.Data.DataTable.OnDataChanged(this.OnTableDataChanged);

				m_DataTable = value;
				
				if(null!=m_DataTable)
				{
					m_DataTable.FireDataChanged += new Altaxo.Data.DataTable.OnDataChanged(this.OnTableDataChanged);
					this.numberOfCols = m_DataTable.ColumnCount;
					this.numberOfRows = m_DataTable.RowCount;
					SetScrollPositionTo(0,0);
					SetColumnStyleCache();
					Invalidate();
				}
				else
				{
					this.numberOfCols = 0;
					this.numberOfRows = 0;
					columnStyleCache.Clear();
					SetScrollPositionTo(0,0);
					Invalidate();
				}
			}	
		}
		
		public int FirstVisibleColumn
		{
			get
			{
				return hScrollBar1.Value;
			}
			set
			{
				this.hScrollBar1.Value=value;
				this.SetColumnStyleCache();
				Invalidate();
			}
		}

		public int VisibleColumns
		{
			get
			{
				return nLastVisibleColumn>=FirstVisibleColumn ? 1+nLastVisibleColumn-FirstVisibleColumn : 0;
			}
		}

		public int FullyVisibleColumns
		{
			get
			{
				return nLastFullyVisibleColumn>=FirstVisibleColumn ? 1+nLastFullyVisibleColumn-FirstVisibleColumn : 0;
			}
		}
		public int LastVisibleColumn
		{
			get
			{
				return FirstVisibleColumn + VisibleColumns -1;
			}
		}
		public int LastFullyVisibleColumn
		{
			get
			{
				return FirstVisibleColumn + FullyVisibleColumns -1;
			}
		}
		
		public int FirstVisibleRow
		{
			get
			{
				return vScrollBar1.Value;
			}
			set
			{
				vScrollBar1.Value = value;
				Invalidate();
			}
		}

		public int VisibleRows
		{
			get
			{
				return (int)System.Math.Ceiling((this.Bottom-m_ColumnHeaderStyle.Height-hScrollBar1.Height)/m_RowHeaderStyle.Height);
			}
		}

		public int FullyVisibleRows
		{
			get
			{
				return (int)System.Math.Floor((this.Bottom-m_ColumnHeaderStyle.Height-hScrollBar1.Height)/m_RowHeaderStyle.Height);
			}
		}

		public int LastVisibleRow
		{
			get
			{
				return FirstVisibleRow + VisibleRows -1;
			}
		}

		public int LastFullyVisibleRow
		{
			get
			{
				return FirstVisibleRow + FullyVisibleRows - 1;
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


		public Altaxo.TableView.ColumnStyle GetColumnStyle(int i)
		{
			// zuerst in der ColumnStylesCollection nach dem passenden Namen
			// suchen, ansonsten default-Style zurückgeben
			Altaxo.Data.DataColumn dc = m_DataTable[i];
			Altaxo.TableView.ColumnStyle colstyle;

			// first look at the column styles hash table, column itself is the key
			colstyle = (Altaxo.TableView.ColumnStyle)m_ColumnStyles[dc];
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
				if(null!=(colstyle = (Altaxo.TableView.ColumnStyle)m_DefaultColumnStyles[searchstyletype]))
					return colstyle;

				// if not successfull yet, we will create a new defaultColumnStyle
				colstyle = (Altaxo.TableView.ColumnStyle)Activator.CreateInstance(searchstyletype);
				m_DefaultColumnStyles.Add(searchstyletype,colstyle);
				return colstyle;
			}
		}


		#endregion


		#region "internal stuff"
		protected override void OnPaint(	PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics dc=e.Graphics;
			Pen bluePen = new Pen(Color.Blue, 1);
			Brush brownBrush = new SolidBrush(Color.Aquamarine);
			int firstVisibleColumn = this.hScrollBar1.Value;
			int firstVisibleRow = this.vScrollBar1.Value;
			bool bDrawColumnHeader = false;

			dc.FillRectangle(brownBrush,e.ClipRectangle); // first set the background
			
			if(null==m_DataTable)
				return;

			Rectangle cellRectangle = new Rectangle();
			int firstRowToDraw = firstVisibleRow + (e.ClipRectangle.Top-m_ColumnHeaderStyle.Height)/m_RowHeaderStyle.Height;
			int lastRowToDraw = firstVisibleRow + (int)System.Math.Ceiling((e.ClipRectangle.Bottom-m_ColumnHeaderStyle.Height)/m_RowHeaderStyle.Height);
			if(firstRowToDraw<firstVisibleRow)
				firstRowToDraw=firstVisibleRow;

			
			if(e.ClipRectangle.Top<m_ColumnHeaderStyle.Height)
			{
				bDrawColumnHeader = true;
				// draw the Column Header Items
			}
			if(e.ClipRectangle.Left<m_RowHeaderStyle.Width)
			{
				// draw the Row Header Items
				cellRectangle.Height = m_RowHeaderStyle.Height;
				cellRectangle.Width = m_RowHeaderStyle.Width;
				cellRectangle.X=0;
				for(int nRow = firstRowToDraw;nRow<=lastRowToDraw;nRow++)
				{
					cellRectangle.Y = m_ColumnHeaderStyle.Height+(nRow-firstVisibleRow)*m_RowHeaderStyle.Height;
					m_RowHeaderStyle.Paint(dc,cellRectangle,nRow,null, false);
				}
			}
			if(e.ClipRectangle.Bottom>=m_ColumnHeaderStyle.Height || e.ClipRectangle.Right>=m_RowHeaderStyle.Width)		
			{
				// draw the cells
				//int firstColToDraw = firstVisibleColumn+(e.ClipRectangle.Left-m_RowHeaderStyle.Width)/columnWidth;
				//int lastColToDraw  = firstVisibleColumn+(int)Math.Ceiling((e.ClipRectangle.Right-m_RowHeaderStyle.Width)/columnWidth);

				int actualColumnLeft = m_RowHeaderStyle.Width; 
				int actualColumnRight;
				for(int i=firstVisibleColumn;i<m_DataTable.ColumnCount && actualColumnLeft<e.ClipRectangle.Right;i++)
				{
					Altaxo.TableView.ColumnStyle cs = GetColumnStyle(i);
					actualColumnRight = actualColumnLeft+cs.Width;
					cellRectangle.X = actualColumnLeft;
					cellRectangle.Width = actualColumnRight-actualColumnLeft;
					bool bColumnSelected = m_SelectedColumns.ContainsKey(i);
					bool bAreRowsSelected = m_SelectedRows.Count>0;
					bool bSelected;

					if(actualColumnRight>e.ClipRectangle.Left) // Column must be painted, so paint it!
					{
						if(bDrawColumnHeader) // must the column Header been drawn?
						{
							cellRectangle.Y=0;
							cellRectangle.Height = m_ColumnHeaderStyle.Height;
							m_ColumnHeaderStyle.Paint(dc,cellRectangle,0,m_DataTable[i],bColumnSelected);
						}
						
						cellRectangle.Height = m_RowHeaderStyle.Height;
						for(int nRow=firstRowToDraw;nRow<=lastRowToDraw;nRow++)
						{
							bSelected = bColumnSelected || (bAreRowsSelected && m_SelectedRows.ContainsKey(nRow));
							cellRectangle.Y= m_ColumnHeaderStyle.Height+(nRow-firstVisibleRow)*m_RowHeaderStyle.Height;
							cs.Paint(dc,cellRectangle,nRow,m_DataTable[i],bSelected);
						}
					}
					actualColumnLeft = actualColumnRight;
				}
			}
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.cellEditControl = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.vScrollBar1.LargeChange = 1;
			this.vScrollBar1.Location = new System.Drawing.Point(280, 0);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(16, 224);
			this.vScrollBar1.TabIndex = 0;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.hScrollBar1.LargeChange = 1;
			this.hScrollBar1.Location = new System.Drawing.Point(0, 208);
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(280, 16);
			this.hScrollBar1.TabIndex = 0;
			this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
			// 
			// cellEditControl
			// 
			this.cellEditControl.AcceptsTab = true;
			this.cellEditControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.cellEditControl.Location = new System.Drawing.Point(387, 0);
			this.cellEditControl.Multiline = true;
			this.cellEditControl.Name = "cellEditControl";
			this.cellEditControl.TabIndex = 0;
			this.cellEditControl.Text = "";
			this.cellEditControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnCellEditControl_KeyDown);
			this.cellEditControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnCellEditControl_KeyPress);
			// 
			// AltaxoDataGrid
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.hScrollBar1,
																																	this.vScrollBar1});
			this.Size = new System.Drawing.Size(296, 224);
			this.Resize += new System.EventHandler(this.OnResize);
			this.Click += new System.EventHandler(this.OnClick);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.DoubleClick += new System.EventHandler(this.OnDoubleClick);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
			this.ResumeLayout(false);

		}
		#endregion

		private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(cellEditControl.Visible)
			{
				this.ReadCellEditContent();
				cellEditControl.Hide();
			}
			this.Invalidate();
		}

		private void hScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(cellEditControl.Visible)
			{
				this.ReadCellEditContent();
				cellEditControl.Hide();
			}

			SetColumnStyleCache();
			this.Invalidate();
		}

		public void OnTableDataChanged(Altaxo.Data.DataTable sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow)
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate

			int nOldRows = this.numberOfRows;
			int nOldCols = this.numberOfCols;

			numberOfRows=m_DataTable.RowCount;
			numberOfCols=m_DataTable.ColumnCount;

			if(nOldRows!=numberOfRows)
			{
				if(this.vScrollBar1.Value+1>numberOfRows)
					vScrollBar1.Value = numberOfRows>0 ? numberOfRows-1 : 0;

				vScrollBar1.Maximum = numberOfRows>0 ? numberOfRows-1	: 0;
				vScrollBar1.Refresh();
			}
			if(nOldCols!=numberOfCols)
			{
				if(hScrollBar1.Value+1>numberOfCols)
					hScrollBar1.Value = numberOfCols>0 ? numberOfCols-1 : 0;
	
				hScrollBar1.Maximum = numberOfCols>0 ? numberOfCols-1 : 0;
				SetColumnStyleCache();
				hScrollBar1.Refresh();
			}

			Invalidate();

		}

		private Rectangle GetCoordinatesOfCell(int nCol, int nRow)
		{
			Rectangle cellRect = new Rectangle();

			cellRect.Y = this.m_ColumnHeaderStyle.Height + this.m_RowHeaderStyle.Height*(nRow-FirstVisibleRow);
			cellRect.Height = this.m_RowHeaderStyle.Height;

			int colOffs = nCol-FirstVisibleColumn;
			cellRect.X = ((ColumnStyleCacheItem)columnStyleCache[colOffs]).leftBorderPosition;
			cellRect.Width = ((ColumnStyleCacheItem)columnStyleCache[colOffs]).rightBorderPosition - cellRect.X;
			return cellRect;
		}


		private bool GetCellCoordinatesOfMouseDown(out int nCol, out int nRow, out Rectangle cellRect)
		{
			nCol = -2;
			nRow = -2;
			cellRect = new Rectangle();
			
			// -2 means outside a cell
			// -1   means the row or column header
			// >=0 is the index of a cell
			
			if(mouseDownPosition.Y<m_ColumnHeaderStyle.Height)
			{
				// clicked in the left upper edge, the worksheet properties / or select the hole worksheet
				nRow=-1;
				cellRect.Y=0;
				cellRect.Height = m_ColumnHeaderStyle.Height;
			}
			else
			{
				nRow = (mouseDownPosition.Y-m_ColumnHeaderStyle.Height)/this.m_RowHeaderStyle.Height;
				cellRect.Y=nRow*this.m_RowHeaderStyle.Height+m_ColumnHeaderStyle.Height;
				cellRect.Height = this.m_RowHeaderStyle.Height;
				nRow += this.vScrollBar1.Value;
			}

		
			if(mouseDownPosition.X<m_RowHeaderStyle.Width)
			{
				nCol = -1;
				cellRect.X=0;
				cellRect.Width=m_RowHeaderStyle.Width;
			}
			else // X is out of the row header -> 	
			{
				int firstVisibleColumn = this.hScrollBar1.Value;
				int actualColumnRight = m_RowHeaderStyle.Width;
				for(int i=firstVisibleColumn;i<m_DataTable.ColumnCount;i++)
				{
					cellRect.X=actualColumnRight;
					Altaxo.TableView.ColumnStyle cs = GetColumnStyle(i);
					actualColumnRight += cs.Width;
					if(mouseDownPosition.X<actualColumnRight)
					{
						nCol = i;
						cellRect.Width = cs.Width;
						break;
					}
				}
			}
			return nCol>=-1 && nRow>=-1;
		}

		private void OnDoubleClick(object sender, System.EventArgs e)
		{
		
		}

		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// base.OnMouseDown(e);
			this.mouseDownPosition = new Point(e.X, e.Y);
			cellEditControl.Hide();

			if(this.dragColumnWidth_ColumnNumber>=-1)
			{
				this.Capture=true;
				dragColumnWidth_OriginalPos = e.X;
				dragColumnWidth_InCapture=true;
			}
		}

		private void OnTextBoxLostControl(object sender, System.EventArgs e)
		{
			this.ReadCellEditContent();
			cellEditControl.Hide();
		}

		private void OnClick(object sender, System.EventArgs e)
		{
			Rectangle cellRect;
			int nRow; int nCol;
			
			if(GetCellCoordinatesOfMouseDown(out nCol, out nRow, out cellRect)) // we have clicked inside the valid area
			{
				if(nCol>=0 && nRow>=0) // we have clicked inside a cell
				{
					//cellEditControl = new TextBox();
					cellEdit_nRow=nRow;
					cellEdit_nCol=nCol;
					cellEditControl.Parent = this;
					cellEditControl.Location = cellRect.Location;
					cellEditControl.Size = cellRect.Size;
					cellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
					this.SetCellEditContent();
				}
				else if(nCol>=0 && nRow==-1) // clicked inside a column header
				{
					if(!this.dragColumnWidth_InCapture)
					{
						m_SelectedRows.Clear(); // if we click a column, we remove row selections
						bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
						bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
						m_SelectedColumns.Select(nCol,bShiftKey,bControlKey);
						this.Invalidate();
					}
				}
				else if(nCol==-1 && nRow>=0) // clicked inside a row header
				{
					m_SelectedColumns.Clear(); // if we click a row, we delete column selections
					bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
					bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
					m_SelectedRows.Select(nRow,bShiftKey,bControlKey);
					this.Invalidate();
				}
				else if(nCol==-1 && nRow==-1) // clicked inside the worksheet properties
				{
				
				}
				else // clicked outside any cell and any header
				{
				}

			}
		}

		private void OnCellEditControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)13) // Don't use the enter key, event is handled by KeyDown
			{
				e.Handled=true;
			}
			else if(e.KeyChar == (char)9) // Tab key pressed
			{
				if(cellEditControl.SelectionStart+cellEditControl.SelectionLength>=cellEditControl.TextLength)
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
				if(cellEditControl.SelectionStart==0 && (cellEdit_nRow>0 || cellEdit_nCol>0) )
				{
					e.Handled=true;
					// Navigate to the left
					NavigateCellEdit(-1,0);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Right)
			{
				if(cellEditControl.SelectionStart+cellEditControl.SelectionLength>=cellEditControl.TextLength)
				{
					e.Handled=true;
					// Navigate to the right
					NavigateCellEdit(1,0);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Up && cellEdit_nRow>0)
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
				if(cellEditControl.SelectionLength>0)
				{
					cellEditControl.SelectionLength=0;
					cellEditControl.SelectionStart=cellEditControl.TextLength;
				}
				else
				{
					NavigateCellEdit(0,1);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Escape)
			{
				e.Handled=true;
				cellEdit_IsArmed=false;
				this.cellEditControl.Hide();
			}
		}

		private void SetColumnStyleCache()
		{
			nLastVisibleColumn=0;
			nLastFullyVisibleColumn = 0;

			this.columnStyleCache.Clear();

			if(null==m_DataTable)
				return;
		
			int actualColumnLeft = 0; 
			int actualColumnRight = m_RowHeaderStyle.Width;
			
			int rightBorder = this.Right - vScrollBar1.Width;
			nLastFullyVisibleColumn = FirstVisibleColumn;
			for(int i=FirstVisibleColumn;i<m_DataTable.ColumnCount && actualColumnLeft<rightBorder;i++)
			{
				actualColumnLeft = actualColumnRight;
				Altaxo.TableView.ColumnStyle cs = GetColumnStyle(i);
				actualColumnRight = actualColumnLeft+cs.Width;
				columnStyleCache.Add(new ColumnStyleCacheItem(cs,actualColumnLeft,actualColumnRight));

				if(actualColumnLeft<rightBorder)
					nLastVisibleColumn = i;

				if(actualColumnRight<=rightBorder)
					nLastFullyVisibleColumn = i;
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
			int horzSize = this.Right-m_RowHeaderStyle.Width-vScrollBar1.Width;
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
			int oldCol = hScrollBar1.Value;
			if(hScrollBar1.Maximum<nCol)
				hScrollBar1.Maximum = nCol;
			hScrollBar1.Value=nCol;

			if(oldCol!=nCol || columnStyleCache.Count==0)
			{
				SetColumnStyleCache();
			}

			if(vScrollBar1.Maximum<nRow)
				vScrollBar1.Maximum=nRow;
			vScrollBar1.Value=nRow;
		}

		private void ReadCellEditContent()
		{
			if(this.cellEdit_IsArmed && this.cellEditControl.Modified)
			{
				GetColumnStyle(cellEdit_nCol).SetColumnValueAtRow(cellEditControl.Text,cellEdit_nRow,m_DataTable[cellEdit_nCol]);
				this.cellEdit_IsArmed=false;
			}
		}

		private void SetCellEditContent()
		{
			cellEditControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			cellEditControl.Text=GetColumnStyle(cellEdit_nCol).GetColumnValueAtRow(cellEdit_nRow,m_DataTable[cellEdit_nCol]);
			cellEditControl.SelectAll();
			cellEditControl.Modified=false;
			cellEditControl.Show();
			cellEditControl.Focus();
			this.cellEdit_IsArmed=true;
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
			int newCellCol = this.cellEdit_nCol + dx;
			if(newCellCol>=m_DataTable.ColumnCount)
			{
				newCellCol=0;
				dy+=1;
			}
			else if(newCellCol<0)
			{
				if(cellEdit_nRow>0) // move to the last cell only if not on cell 0
				{
					newCellCol=m_DataTable.ColumnCount-1;
					dy-=1;
				}
				else
				{
					newCellCol=0;
				}
			}

			int newCellRow = this.cellEdit_nRow + dy;
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

			if(newCellRow<FirstVisibleRow)
				navigateToRow = newCellRow;
			else if (newCellRow>LastFullyVisibleRow)
				navigateToRow = newCellRow + 1 - FullyVisibleRows;
			else
				navigateToRow = FirstVisibleRow;

			if(navigateToCol!=FirstVisibleColumn || navigateToRow!=FirstVisibleRow)
			{
				SetScrollPositionTo(navigateToCol,navigateToRow);
				bScrolled=true;
			}
			// 3. Fill the cell edit control with new content
			cellEdit_nCol=newCellCol;
			cellEdit_nRow=newCellRow;
			cellEditControl.Parent = this;
			Rectangle cellRect = this.GetCoordinatesOfCell(cellEdit_nCol,cellEdit_nRow);
			cellEditControl.Location = cellRect.Location;
			cellEditControl.Size = cellRect.Size;
			SetCellEditContent();

			// 4. Invalidate the client area if scrolled in step (2)
			if(bScrolled)
				Invalidate();

		}

		private void OnResize(object sender, System.EventArgs e)
		{
			SetColumnStyleCache();
		}

		private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled=true;
		}



		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int Y = e.Y;
			int X = e.X;

			if(this.dragColumnWidth_InCapture)
			{
				int sizediff = X - this.dragColumnWidth_OriginalPos;
				
				Altaxo.TableView.ColumnStyle cs;
				if(-1==dragColumnWidth_ColumnNumber)
					cs = this.m_RowHeaderStyle;
				else
				{
					cs = (Altaxo.TableView.ColumnStyle)m_ColumnStyles[m_DataTable[dragColumnWidth_ColumnNumber]];
				
					if(null==cs)
					{
						Altaxo.TableView.ColumnStyle template = GetColumnStyle(this.dragColumnWidth_ColumnNumber);
						cs = (Altaxo.TableView.ColumnStyle)template.Clone();
						m_ColumnStyles.Add(m_DataTable[dragColumnWidth_ColumnNumber],cs);
					}
				}

				int newWidth = this.dragColumnWidth_OriginalWidth + sizediff;
				if(newWidth<10)
					newWidth=10;
				cs.Width=newWidth;
				this.SetColumnStyleCache();
				Invalidate();
			}
			else // not in Capture mode
			{
				if(Y<this.m_ColumnHeaderStyle.Height)
				{
					for(int i=this.columnStyleCache.Count-1;i>=0;i--)
					{
						ColumnStyleCacheItem csc = (ColumnStyleCacheItem)columnStyleCache[i];

						if(csc.rightBorderPosition-5 < X && X < csc.rightBorderPosition+5)
						{
							this.Cursor = System.Windows.Forms.Cursors.VSplit;
							this.dragColumnWidth_ColumnNumber = i+FirstVisibleColumn;
							this.dragColumnWidth_OriginalWidth = csc.columnStyle.Width;
							return;
						}
					} // end for

					if(this.m_RowHeaderStyle.Width -5 < X && X < m_RowHeaderStyle.Width+5)
					{
						this.Cursor = System.Windows.Forms.Cursors.VSplit;
						this.dragColumnWidth_ColumnNumber = -1;
						this.dragColumnWidth_OriginalWidth = this.m_RowHeaderStyle.Width;
						return;
					}
				}

				this.dragColumnWidth_ColumnNumber=int.MinValue;
				this.Cursor = System.Windows.Forms.Cursors.Default;
			} // end else
		}

		private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(this.dragColumnWidth_InCapture)
			{
				int sizediff = e.X - this.dragColumnWidth_OriginalPos;
				Altaxo.TableView.ColumnStyle cs;
				if(-1==dragColumnWidth_ColumnNumber)
				{
					cs = this.m_RowHeaderStyle;
				}
				else
				{
					cs = (Altaxo.TableView.ColumnStyle)m_ColumnStyles[m_DataTable[dragColumnWidth_ColumnNumber]];
					if(null==cs)
					{
						Altaxo.TableView.ColumnStyle template = GetColumnStyle(this.dragColumnWidth_ColumnNumber);
						cs = (Altaxo.TableView.ColumnStyle)template.Clone();
						m_ColumnStyles.Add(m_DataTable[dragColumnWidth_ColumnNumber],cs);
					}
				}
				int newWidth = this.dragColumnWidth_OriginalWidth + sizediff;
				if(newWidth<10)
					newWidth=10;
				cs.Width=newWidth;
				this.SetColumnStyleCache();

				this.dragColumnWidth_InCapture = false;
				this.dragColumnWidth_ColumnNumber = int.MinValue;
				this.Capture=false;
				this.Cursor = System.Windows.Forms.Cursors.Default;
				Invalidate();

			}
		}
		#endregion
	} // end class
} // end namespace

