using System;
using Altaxo.Serialization;

namespace Altaxo.Data
{
	

	public enum ColumnKind
	{
		Y,
		X,			
		Z,								
		Err,		
		pErr,					
		mErr,
		Label,
		Condition
	}




	public interface IReadableColumn
	{

		/// <summary>
		/// the indexer property returns the element at index i as an AltaxoVariant
		/// </summary>
		AltaxoVariant this[int i] 
		{
			get;
		}

		/// <summary>
		/// IsElementEmpty returns true, if the value at index i of the column
		/// is null or invalid or in another state comparable to null or empty
		/// </summary>
		/// <param name="i"></param>
		/// <returns>true if element is null/empty, false if element is valid</returns>
		bool IsElementEmpty(int i);

		/// <summary>
		/// FullName returns a descriptive name for a column
		/// for columns which belongs to a table, the table name and the column
		/// name, separated by a backslash, should be returned
		/// for other columns, a descriptive name should be returned so that the
		/// user knows the location of this column
		/// </summary>
		string FullName
		{
			get;
		}
	}

	public interface IWriteableColumn
	{
		AltaxoVariant this[int i] 
		{
			set;
		}
	}


	public interface INumericColumn 
	{
		double GetDoubleAt(int i);
	}

	public class IndexerColumn : INumericColumn, IReadableColumn
	{
		public double GetDoubleAt(int i)
		{
			return i;
		}

		public bool IsElementEmpty(int i)
		{
			return false;
		}

		public AltaxoVariant this[int i] 
		{
			get 
			{
				return new AltaxoVariant((double)i);
			}
		}

		public string FullName
		{
			get { return "IndexerColumn"; }
		}

	}


	public interface IDefinedCount
	{
		int Count
		{
			get;
		}
	}



	/// <summary>
	/// Summary description for Altaxo.Data.DataColumn.
	/// </summary>
	[SerializationSurrogate(0,typeof(Altaxo.Data.DataColumn.SerializationSurrogate0))]
	[SerializationVersion(0)]
	[Serializable()]
	public abstract class DataColumn : IDisposable, System.Runtime.Serialization.IDeserializationCallback, IReadableColumn, IWriteableColumn, IDefinedCount
	{
		// Data Members
		protected string m_ColumnName=null; // the name of the column in the data m_Table
		protected int m_ColumnNumber=0; // number of the column in the data m_Table
		protected Altaxo.Data.DataTable m_Table=null;
		protected int m_Group=0; // number of group this column is belonging to
		protected ColumnKind m_Kind; // kind of column
		protected bool       m_bXColumn; // is this a X-Column

		protected int m_Count=0; // Index of last valid data -1
		protected static double increaseFactor=2; // array space is increased by this factor plus addSpace
		protected static int    addSpace=32; // array space is increased by multiplying with increasefactor + addspase

		protected int m_MinRowChanged=int.MaxValue; // area of rows, which changed during event off period
		protected int m_MaxRowChanged=int.MinValue;
		protected bool m_bRowCountDecreased=false; // true if during event switch of period, the row m_Count  of this column decreases 
		protected int  m_DataEventsSuspendCount=0;

		public delegate void DataChangedHandler(Altaxo.Data.DataColumn sender, int nMinRow, int nMaxRow, bool m_bRowCountDecreased );   // delegate declaration
		public delegate void DirtySetHandler(Altaxo.Data.DataColumn sender);
		public delegate void DisposedHandler(Altaxo.Data.DataColumn sender);

		public event DataChangedHandler						DataChanged;
		public event DisposedHandler							ColumnDisposed;
		protected internal event DirtySetHandler	DirtySet;
 

		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)obj;
				// I decided _not_ to serialize the parent object, since if we only want
				// to serialize this column, we would otherwise serialize the entire object
				// graph
				// info.AddValue("Parent",s.m_Table); // 
				info.AddValue("Name",s.m_ColumnName);
				info.AddValue("Number",s.m_ColumnNumber);
				info.AddValue("Count",s.m_Count);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)obj;
				// s.m_Table = (Altaxo.Data.DataTable)(info.GetValue("Parent",typeof(Altaxo.Data.DataTable)));
				s.m_Table = null;
				s.m_ColumnName = info.GetString("Name");
				s.m_ColumnNumber = info.GetInt32("Number");
				s.m_Count = info.GetInt32("Count");

				// set the helper data
				s.m_MinRowChanged=int.MaxValue; // area of rows, which changed during event off period
				s.m_MaxRowChanged=int.MinValue;
				return s;
			}
		}

		public virtual void OnDeserialization(object obj)
		{
		}
		#endregion


		public void SuspendDataChangedNotifications()
		{
			m_DataEventsSuspendCount++;
		}

		public void ResumeDataChangedNotifications()
		{
			m_DataEventsSuspendCount--;
			if(m_DataEventsSuspendCount<0) m_DataEventsSuspendCount=0;

			if(0==m_DataEventsSuspendCount && this.IsDirty)
				NotifyDataChanged(m_MinRowChanged,m_MaxRowChanged,m_bRowCountDecreased);
		}

		/// <summary>
		/// Copy the head of the column, i.e. Column comment, label unit and
		/// scripts from another column
		/// number is not copied, since this is set by the m_Table
		/// </summary>
		/// <param name="ano"></param>
		public void CopyHeaderFrom(DataColumn ano)
		{
			// throw an exception, if the destination column has a parent, that is not supported!
			// because the m_Table has to set up the names and so on

			if(this.m_Table!=null)
				throw new ApplicationException("The column " + this.ColumnName + " has the parent m_Table " + m_Table.TableName);
			
			this.m_ColumnName = ano.m_ColumnName;
		}

		public int Group
		{
			get { return m_Group; }
			set { m_Group = value; }
		}

		public ColumnKind Kind
		{
			get { return m_bXColumn? ColumnKind.X : m_Kind; }
			set 
			{
				if(ColumnKind.X==value)
					this.XColumn = true;
				else
					m_Kind = value;
			}
		}

		public bool XColumn
		{
			get { return m_bXColumn; }
			set
			{
				if(null!=this.m_Table && true==value)
				{
					m_Table.DeleteXProperty(m_Group);
				}
				m_bXColumn = value;
			}
		}

		/// <summary>
		/// copies the header information, like label and so on
		/// from another column to this column
		/// ColumnName and ColumnNumber is not copied!
		/// </summary>
		/// <param name="ano"></param>
		public void CopyHeaderInformationFrom(DataColumn ano)
		{
		}

		protected void NotifyDataChanged(int minRow, int maxRow, bool rowCountDecreased)
		{
			if(null!=m_Table)
			{
				bool bWasDirtyBefore = this.IsDirty;

				if(minRow < m_MinRowChanged) m_MinRowChanged = minRow;
				if(maxRow > m_MaxRowChanged) m_MaxRowChanged = maxRow;
				m_bRowCountDecreased |= rowCountDecreased;

				if(m_DataEventsSuspendCount==0)
				{
					// always inform the parent first,
					// because the parent can change our bDataEventEnabled to false)
					this.m_Table.OnColumnDataChanged(this,m_MinRowChanged,m_MaxRowChanged,m_bRowCountDecreased);
				}
				if(m_DataEventsSuspendCount==0) // look again for this variable, because the parent can change it during OnDataChanged
				{	
					if(null!=DataChanged)
						DataChanged(this, m_MinRowChanged,m_MaxRowChanged,m_bRowCountDecreased);
			
					ResetDirty();
				}
				else // Data events disabled
				{
					// if the row is not dirty up to now, add it to the DataTables dirty column collection
					if(!bWasDirtyBefore && null!=DirtySet)
						DirtySet(this);
				}

			}
		}
/// <summary>
/// returns the type of the associated ColumnStyle
/// if the data column is not used in a datagrid, 
/// you can return null for this
/// </summary>
/// <returns>the type of the associated ColumnStyle</returns>
		public abstract System.Type GetColumnStyleType();

		/// <summary>
		/// Column is dirty if either there are new data, and no DataChange event fired up,
		/// and/or the column m_Count was decreased
		/// </summary>
		public bool IsDirty
		{
			get
			{
				return (m_MinRowChanged<=m_MaxRowChanged);
			}
		}

		protected void ResetDirty()
		{
			m_MinRowChanged=int.MaxValue;
			m_MaxRowChanged=int.MinValue;
			m_bRowCountDecreased=false;
		}

		/// <summary>
		/// the position of the column in the m_Table, has to be syncronized by the
		/// parent m_Table
		/// </summary>
		public int ColumnNumber
		{
			get
			{
				return m_ColumnNumber;
			}
		}

		/// <summary>
		/// Sets the column number, only the data m_Table should do that!
		/// because this column number must be synchronized with the
		/// position of the column in the data m_Table 
		/// </summary>
		/// <param name="n">hte position of the column in the parent data m_Table</param>
		protected internal void SetColumnNumber(int n)
		{
			if(null!=m_Table && m_Table[n]!=this) // test if the column is really there
				throw new ApplicationException("Try to set wrong column number to column " + this.ColumnName + ", m_Table " + m_Table.TableName);

			m_ColumnNumber=n;
		}

		protected DataColumn()
		{
		}
		protected DataColumn(string name)
		{
			m_ColumnName = name;
		}
		protected DataColumn(Altaxo.Data.DataTable parenttable, string name)
		{
			this.m_Table = parenttable;
			this.m_ColumnName = name;
		}

		public string ColumnName
		{
			get
			{
				return m_ColumnName;
			}
			set
			{
				m_ColumnName = value;
			}
		}
		
		public string FullName
		{
			get 
			{
				return null==m_Table ? m_ColumnName : String.Format("{0}\\{1}",m_Table.TableName,m_ColumnName);
			}
			}


		public int Count
		{
			get
			{
				return m_Count;
			}
		}
		
		public string TypeAndName
		{
			get
			{
				return null==m_ColumnName ? this.GetType().ToString(): this.GetType().ToString() + "(\"" + m_ColumnName + "\")";
			}
		}

		public Altaxo.Data.DataTable ParentTable
		{
			get
			{
				return m_Table;
			}
			set
			{
				m_Table = value;

			}
		}
		// hashcode
		// public override int GetHashCode() { return guid.GetHashCode(); }
		
		
		// indexers
		public abstract void SetValueAt(int i, AltaxoVariant val);
		public abstract AltaxoVariant GetVariantAt(int i);
		public abstract bool IsElementEmpty(int i);


		public AltaxoVariant this[int i] 
		{
			get
			{
				return GetVariantAt(i);
			}
			set
			{
				SetValueAt(i,value);
			}
		}

		public void Dispose()
		{
			this.m_Table=null;
			this.m_ColumnNumber = int.MinValue;
			this.Clear();
			if(null!=ColumnDisposed)
				ColumnDisposed(this);
		}

		public void Clear()
		{
			RemoveRows(0,this.Count);
		}

		public DataColumn Data
		{
			set { CopyDataFrom(value); }
		}

		// CopyData
		public abstract void CopyDataFrom(Altaxo.Data.DataColumn v);
		public abstract void RemoveRows(int nFirstRow, int nCount); // removes nCount rows starting from nFirstRow 
		public abstract void InsertRows(int nBeforeRow, int nCount); // inserts additional empty rows
		// -----------------------------------------------------------------------------
		// 
		//                      Operators
		//
		// -----------------------------------------------------------------------------

		// Note: unfortunately (and maybe also undocumented) we can not use
		// the names op_Addition, op_Subtraction and so one, because these
		// names seems to be used by the compiler for the operators itself
		// so we use here vopAddition and so on


		
		
		
		
		
		
		
		
		
		
		
		
		
		
		// Note: unfortunately (and maybe also undocumented) we can not use
		// the names op_Addition, op_Subtraction and so one, because these
		// names seems to be used by the compiler for the operators itself
		// so we use here vopAddition and so on (the v from virtual)

		public virtual bool vop_Addition(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Addition_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Addition(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Addition_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Subtraction(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Subtraction_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Subtraction(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Subtraction_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Multiplication(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Multiplication_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Multiplication(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Multiplication_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Division(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Division_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Division(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Division_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Modulo(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Modulo_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Modulo(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Modulo_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_And(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_And_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_And(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_And_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Or(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Or_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Or(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Or_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Xor(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Xor_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Xor(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Xor_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_ShiftLeft(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftLeft_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftLeft(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftLeft_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_ShiftRight(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftRight_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftRight(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftRight_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Lesser(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Lesser_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Lesser(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Lesser_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Greater(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Greater_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Greater(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Greater_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_LesserOrEqual(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_LesserOrEqual_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_LesserOrEqual(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_LesserOrEqual_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_GreaterOrEqual(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_GreaterOrEqual_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_GreaterOrEqual(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_GreaterOrEqual_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		// Unary operators
		public virtual bool vop_Plus(out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Minus(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Not(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Complement(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Increment(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Decrement(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool        vop_True(out bool b)
		{	b=false; return false; }
		
		public virtual bool        vop_False(out bool b)
		{	b=false; return false; }

	
		
		public static DataColumn operator +(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Addition(c2, out c3))
				return c3;
			if(c2.vop_Addition_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator +(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Addition(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator +(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Addition_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator -(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Subtraction(c2, out c3))
				return c3;
			if(c2.vop_Subtraction_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator -(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Subtraction(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator -(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Subtraction_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}


		
		
		public static DataColumn operator *(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Multiplication(c2, out c3))
				return c3;
			if(c2.vop_Multiplication_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator *(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Multiplication(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator *(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Multiplication_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}




		public static DataColumn operator /(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Division(c2, out c3))
				return c3;
			if(c2.vop_Division_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator /(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Division(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator /(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Division_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}



		public static DataColumn operator %(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Modulo(c2, out c3))
				return c3;
			if(c2.vop_Modulo_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator %(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Modulo(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator %(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Modulo_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}


		public static DataColumn operator &(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_And(c2, out c3))
				return c3;
			if(c2.vop_And_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply and operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator &(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_And(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator AND to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator &(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_And_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator AND to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator |(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Or(c2, out c3))
				return c3;
			if(c2.vop_Or_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply or operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator |(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Or(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator OR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator |(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Or_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator OR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator ^(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Xor(c2, out c3))
				return c3;
			if(c2.vop_Xor_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply xor operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator ^(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Xor(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator XOR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator ^(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Xor_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator XOR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator <<(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_ShiftLeft(c2, out c3))
				return c3;
			if(c2.vop_ShiftLeft_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to shift left " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <<(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_ShiftLeft(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator << to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <<(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_ShiftLeft_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator << to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator >>(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_ShiftRight(c2, out c3))
				return c3;
			if(c2.vop_ShiftRight_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to shift right " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >>(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_ShiftRight(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator >> to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >>(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_ShiftRight_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator >> to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}



		public static DataColumn operator <(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Lesser(c2, out c3))
				return c3;
			if(c2.vop_Lesser_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator lesser to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Lesser(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator < to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Lesser_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator < to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator >(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Greater(c2, out c3))
				return c3;
			if(c2.vop_Greater_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator greater to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Greater(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator > to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Greater_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator > to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator <=(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_LesserOrEqual(c2, out c3))
				return c3;
			if(c2.vop_LesserOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator LesserOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <=(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_LesserOrEqual(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator <= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <=(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_LesserOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator <= " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator >=(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_GreaterOrEqual(c2, out c3))
				return c3;
			if(c2.vop_GreaterOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator GreaterOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >=(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_GreaterOrEqual(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator >= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >=(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_GreaterOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator >= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator +(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Plus(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator plus to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator -(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Minus(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator minus to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator !(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Not(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator not to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator ~(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Complement(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator complement to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator ++(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Increment(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator increment to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator --(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Decrement(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator decrement to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static bool operator true (DataColumn c1)
		{
			bool c3;

			if(c1.vop_True(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator TRUE to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static bool operator false (DataColumn c1)
		{
			bool c3;

			if(c1.vop_False(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator FALSE to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

	} // end of class Altaxo.Data.DataColumn
	

	

}
