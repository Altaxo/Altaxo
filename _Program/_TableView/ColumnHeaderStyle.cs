using System;
using System.Drawing;
using Altaxo.Serialization;


namespace Altaxo.TableView
{
	[SerializationSurrogate(0,typeof(ColumnHeaderStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class ColumnHeaderStyle : ColumnStyle
	{
		#region Serialization
			public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
			{
				public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
				{
					System.Runtime.Serialization.ISurrogateSelector ss;
					System.Runtime.Serialization.ISerializationSurrogate surr =
						App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
	
					surr.GetObjectData(obj,info,context); // stream the data of the base object
				}
				public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
				{
					System.Runtime.Serialization.ISurrogateSelector ss;
					System.Runtime.Serialization.ISerializationSurrogate surr =
						App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
					return surr.SetObjectData(obj,info,context,selector);
				}
			}

			public override void OnDeserialization(object obj)
			{
				base.OnDeserialization(obj);
			}	
			#endregion


		public int Height
		{
			get
			{
				return m_Size;
			}
			set
			{
				m_Size = value;
			}	
		}

		public ColumnHeaderStyle()
		{
			m_TextFormat.Alignment=StringAlignment.Center;
			m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
		}

		public ColumnHeaderStyle(ColumnHeaderStyle chs)
			: base(chs)
		{
		}

		public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			dc.DrawRectangle(m_CellPen.Pen,cellRectangle);
			
			string str = string.Format("{0} ({1}{2})",data.ColumnName, data.XColumn? "X":data.Kind.ToString(),data.Group); 
			dc.DrawString(str,m_TextFont,this.m_TextBrush,cellRectangle,m_TextFormat);
		}
		
		public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
		{
			return data.ColumnName;
		}

		public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
		{
		}

		public override object Clone()
		{
			return new ColumnHeaderStyle(this);
		}
	}
	
}
