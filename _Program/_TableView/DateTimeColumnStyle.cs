using System;
using System.Drawing;
using Altaxo.Serialization;


namespace Altaxo.TableView
{
	
	[SerializationSurrogate(0,typeof(DateTimeColumnStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DateTimeColumnStyle : Altaxo.TableView.ColumnStyle
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






		public DateTimeColumnStyle()
		{
			m_TextFormat.Alignment=StringAlignment.Far;
			m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
		}

		public DateTimeColumnStyle(DateTimeColumnStyle dtcs)
			: base(dtcs)
		{
		}

		public override object Clone()
		{
			return new DateTimeColumnStyle(this);
		}

		public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
		{
			DateTime val = ((Altaxo.Data.DateTimeColumn)data)[nRow];
			return val==Altaxo.Data.DateTimeColumn.NullValue ? "" : val.ToString();
		}
	
		public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
		{
			DateTime newval;
			try
			{ 
				newval = s.Length==0 ? Altaxo.Data.DateTimeColumn.NullValue : System.Convert.ToDateTime(s);
				((Altaxo.Data.DateTimeColumn)data)[nRow] = newval;
			}
			catch(Exception) {}
		}

		public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			dc.DrawRectangle(m_CellPen.Pen,cellRectangle);
		
			if(bSelected)
				dc.FillRectangle(m_SelectedBackgroundBrush,cellRectangle);
		
			string myString = ((Altaxo.Data.DateTimeColumn)data)[nRow].ToString();
		
			if(bSelected)
				dc.DrawString(myString,m_TextFont,m_SelectedTextBrush,cellRectangle,m_TextFormat);
			else
				dc.DrawString(myString,m_TextFont,m_TextBrush,cellRectangle,m_TextFormat);
		}
	} // end of class Altaxo.TableView.DateTimeColumnStyle


}
