using System;
using System.Drawing;
using Altaxo.Serialization;


namespace Altaxo.TableView
{
	[SerializationSurrogate(0,typeof(RowHeaderStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class RowHeaderStyle : Altaxo.TableView.ColumnStyle
	{
		protected int m_RowHeight=20;

		#region Serialization
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				System.Runtime.Serialization.ISurrogateSelector ss;
				System.Runtime.Serialization.ISerializationSurrogate surr =
					App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
	
				surr.GetObjectData(obj,info,context); // stream the data of the base object
				RowHeaderStyle s = (RowHeaderStyle)obj;
				info.AddValue("Height",(float)s.m_RowHeight);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				// first the base class
				System.Runtime.Serialization.ISurrogateSelector ss;
				System.Runtime.Serialization.ISerializationSurrogate surr =
					App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
				surr.SetObjectData(obj,info,context,selector);
				// now the class itself
				RowHeaderStyle s = (RowHeaderStyle)obj;
				s.m_RowHeight = (int)info.GetSingle("Height");
				return obj;
			}
		}

		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}

		#endregion

		public RowHeaderStyle()
		{
			m_TextFormat.Alignment=StringAlignment.Center;
			m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
		}

		public RowHeaderStyle(RowHeaderStyle rhs)
			: base(rhs)
		{
			m_RowHeight = rhs.m_RowHeight;
		}


		public int Height
		{
			get
			{
				return m_RowHeight;
			}
			set
			{
				m_RowHeight = value;
			}	
		}


		public override object Clone()
		{
			return new RowHeaderStyle(this);
		}

		public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
		{
			return nRow.ToString();
		}
		public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
		{
		}

		public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			dc.DrawRectangle(m_CellPen.Pen,cellRectangle);
			dc.DrawString("["+nRow+"]",m_TextFont,m_TextBrush,cellRectangle,m_TextFormat);
		}
		
	}

}
