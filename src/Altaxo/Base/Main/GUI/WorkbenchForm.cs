using System;
using System.Windows.Forms;
using Altaxo.Serialization;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Summary description for WorkbenchForm.
	/// </summary>
	[SerializationSurrogate(0,typeof(WorkbenchForm.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version.")]
	public class WorkbenchForm 
		:
		System.Windows.Forms.Form,
		Main.GUI.IWorkbenchWindowView
	{
		IWorkbenchWindowController m_Controller;

		#region Serialization
		public class SerializationSurrogate0 : IDeserializationSubstitute, System.Runtime.Serialization.ISerializationSurrogate, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
		{
			protected System.Drawing.Point		m_Location;
			protected System.Drawing.Size		m_Size;
			protected object[]	m_Childs=null;

			// we need a empty constructor
			public SerializationSurrogate0() {}

			// not used for deserialization, since the ISerializable constructor is used for that
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector){return obj;}
			// not used for serialization, instead the ISerializationSurrogate is used for that
			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)	{}

			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				info.SetType(this.GetType());
				WorkbenchForm s = (WorkbenchForm)obj;
				info.AddValue("Location",s.Location);
				info.AddValue("Size",s.Size);
				info.AddValue("NumberOfChilds",s.Controls.Count);
				for(int i=0;i<s.Controls.Count;i++)
					info.AddValue("Child"+i.ToString(),s.Controls[i]);
			}

			public SerializationSurrogate0(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
			{
				m_Location = (System.Drawing.Point)info.GetValue("Location",typeof(System.Drawing.Point));
				m_Size     = (System.Drawing.Size)info.GetValue("Size",typeof(System.Drawing.Size));
				int numberOfChilds = info.GetInt32("NumberOfChilds");
				m_Childs = new object[numberOfChilds];
				for(int i=0;i<numberOfChilds;i++)
					m_Childs[i] = info.GetValue("Child"+i.ToString(),typeof(object));
			}

			public void OnDeserialization(object o)
			{
			}

			public object GetRealObject(object parent)
			{
				// We create the view firstly without controller to have the creation finished
				// before the controler is set
				// otherwise we will have callbacks to not initialized variables
				WorkbenchForm frm = new WorkbenchForm();				
				frm.Location = m_Location;
				frm.Size = m_Size;
			

				for(int i=0;i<m_Childs.Length;i++)
				{
					if(m_Childs[i] is System.Runtime.Serialization.IDeserializationCallback)
					{
						DeserializationFinisher finisher = new DeserializationFinisher(frm);
						((System.Runtime.Serialization.IDeserializationCallback)m_Childs[i]).OnDeserialization(finisher);
					}

					if(m_Childs[i] is IDeserializationSubstitute)
					{
						m_Childs[i] = ((IDeserializationSubstitute)m_Childs[i]).GetRealObject(frm);
					}

					frm.Controls.Add((System.Windows.Forms.Control)m_Childs[i]);
				}
				return frm;
			}
		}
		#endregion


		public WorkbenchForm()
		{
		}
		

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed (e);
			if(m_Controller!=null)
				m_Controller.EhView_OnClosed();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if(m_Controller!=null)
				e.Cancel = m_Controller.EhView_OnClosing(e.Cancel);

			base.OnClosing (e);
		}


		#region IWorkbenchView Members

		public Form Form
		{
			get
			{
				return this;
			}
		}

		public IWorkbenchWindowController Controller
		{
			get
			{
				return m_Controller;
			}
			set
			{
				m_Controller = value;
			}
		}

		public void SetChild(IWorkbenchContentView child)
		{
			System.Windows.Forms.Control fc = (System.Windows.Forms.Control)child;
			if(this.Controls.Count>0)
				this.Controls.Clear();

			this.Controls.Add(fc);
			fc.Dock = System.Windows.Forms.DockStyle.Fill;
		}
	
		public void SetTitle(string title)
		{
			this.Text = title;
		}

		#endregion
	}
}
