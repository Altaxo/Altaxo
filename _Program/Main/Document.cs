using System;
using System.Runtime.Serialization;
using Altaxo.Serialization;

namespace Altaxo
{
	//[Serializable()]
	[SerializationSurrogate(0,typeof(AltaxoTestObject02.AltaxoTestSurrogateV1))]
	[SerializationVersion(1,"Initial version")]
	public class AltaxoTestObject02 : IDeserializationCallback
	{
		private DateTime a1;
		public double b1=33;
		public float c1=34.6f;

		public AltaxoTestObject02()
		{
			b1 = 66;
		}

		public AltaxoTestObject02(DateTime a, double b)
		{
			a1 = a;
			b1 = b;
		}


		public class AltaxoTestSurrogateV1 : System.Runtime.Serialization.ISerializationSurrogate , IDeserializationCallback
		{
			public void GetObjectData(	object obj,	
				System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context	)
			{
				AltaxoTestObject02 s = (AltaxoTestObject02)obj;
				info.AddValue("Date",s.a1);
				info.AddValue("b1",s.b1);
				//System.Runtime.Remoting.ObjRef or = s.br.CreateObjRef(s.br.GetType());
				//info.AddValue("Brush", or);
			}
			public object SetObjectData(
				object obj,
				System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context,
				System.Runtime.Serialization.ISurrogateSelector selector
				)
			{
				AltaxoTestObject02 s = (AltaxoTestObject02)obj;
				s.a1 = info.GetDateTime("Date");
				s.b1 = info.GetDouble("b1");
				return s;
			}
			public void OnDeserialization(object obj)
			{
			}
		}

		
		public void OnDeserialization(object obj)
		{
		}

	} // end class
	/// <summary>
	/// Summary description for AltaxoDocument.
	/// </summary>
	[SerializationSurrogate(0,typeof(AltaxoDocument.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version of the main document only contains m_DataSet")]
	public class AltaxoDocument : IDeserializationCallback
	{
		protected Altaxo.Data.DataSet m_DataSet = null; // The root of all the data
		protected System.Collections.ArrayList m_Worksheets;
	
		[NonSerialized]
		protected bool m_IsDirty=false;
		[NonSerialized]
		private bool m_DeserializationFinished=false;

		public AltaxoDocument()
		{
			m_DataSet = new Altaxo.Data.DataSet(this);
			m_Worksheets = new System.Collections.ArrayList();
		}

		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				info.AddValue("DataSet",s.m_DataSet);
				info.AddValue("Worksheets",s.m_Worksheets);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				s.m_DataSet = (Altaxo.Data.DataSet)info.GetValue("DataSet",typeof(Altaxo.Data.DataSet));
				// s.tstObj    = (AltaxoTestObject02)info.GetValue("TstObj",typeof(AltaxoTestObject02));
				s.m_Worksheets = (System.Collections.ArrayList)info.GetValue("Worksheets",typeof(System.Collections.ArrayList));
				s.m_IsDirty = false;
				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
			if(!m_DeserializationFinished && obj is DeserializationFinisher)
			{
				m_DeserializationFinished=true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);
			
				m_DataSet.ParentDocument = this;
				m_DataSet.OnDeserialization(finisher);

			
				int len = m_Worksheets.Count;
				for(int i=0;i<len;i++)
				{
					m_Worksheets[i] = ((IDeserializationSubstitute)m_Worksheets[i]).GetRealObject(App.CurrentApplication);
					((System.Windows.Forms.Form)m_Worksheets[i]).Show();
				}
			}
		}

		public void RestoreWindowsAfterDeserialization()
		{
		}

		public AltaxoDocument(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			m_DataSet = (Altaxo.Data.DataSet)(info.GetValue("DataSet",typeof(Altaxo.Data.DataSet)));
		}

		public void GetObjectData(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context
			)
		{
			info.AddValue("DataSet",m_DataSet);
		}
		
		public Altaxo.Data.DataSet DataSet
		{
			get { return m_DataSet; }
		}

		public void OnDirtySet(object sender)
		{
			m_IsDirty=true;
		}

		public Altaxo.Worksheet.Worksheet CreateNewWorksheet(string worksheetName, System.Windows.Forms.Form parent, bool bCreateDefaultColumns)
		{
			Altaxo.Data.DataTable dt1 = new Altaxo.Data.DataTable(worksheetName);


			if(bCreateDefaultColumns)
			{
				Altaxo.Data.DoubleColumn colA = new Altaxo.Data.DoubleColumn("A");
				colA.Kind = Data.ColumnKind.X;

				Altaxo.Data.DoubleColumn colB = new Altaxo.Data.DoubleColumn("B");

				dt1.Add(colA);
				dt1.Add(colB);
			}

			DataSet.Add(dt1);

			Altaxo.Worksheet.Worksheet form1= new Altaxo.Worksheet.Worksheet(parent,this,dt1);
			form1.Text = worksheetName;
			m_Worksheets.Add(form1);
			return form1;
		}

		public Altaxo.Worksheet.Worksheet CreateNewWorksheet(System.Windows.Forms.Form parent, bool bCreateDefaultColumns)
		{
			return CreateNewWorksheet(this.DataSet.FindNewTableName(),parent,bCreateDefaultColumns);
		}

		public Altaxo.Worksheet.Worksheet CreateNewWorksheet(System.Windows.Forms.Form parent)
		{
			return CreateNewWorksheet(this.DataSet.FindNewTableName(),parent,true);
		}

	}
}
