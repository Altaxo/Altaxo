using System;
using System.Drawing;
using Altaxo.Serialization;


namespace Altaxo.Worksheet
{
	[SerializationSurrogate(0,typeof(TextColumnStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class TextColumnStyle : Altaxo.Worksheet.ColumnStyle
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


		public TextColumnStyle()
		{
			m_TextFormat.Alignment=StringAlignment.Near;
			m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
		}

		public TextColumnStyle(TextColumnStyle tcs)
			: base(tcs)
		{
		}

		public override object Clone()
		{
			return new TextColumnStyle(this);
		}

		public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
		{
			string val = ((Altaxo.Data.TextColumn)data)[nRow];
			return val==Altaxo.Data.TextColumn.NullValue ? "" : val;
		}
	
		public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
		{
			try
			{ 
				((Altaxo.Data.TextColumn)data)[nRow] = s;
			}
			catch(Exception) {}
		}

		public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			m_CellPen.Cached = true;

			dc.DrawRectangle(m_CellPen.Pen,cellRectangle);
		
			if(bSelected)
				dc.FillRectangle(m_SelectedBackgroundBrush,cellRectangle);
		
			string myString = ((Altaxo.Data.TextColumn)data)[nRow];
		
			if(bSelected)
				dc.DrawString(myString,m_TextFont,m_SelectedTextBrush,cellRectangle,m_TextFormat);
			else
				dc.DrawString(myString,m_TextFont,m_TextBrush,cellRectangle,m_TextFormat);
		}
	} // end of class Altaxo.Worksheet.DateTimeColumnStyle



}
