using System;
using System.Windows.Forms;
using Altaxo.Serialization;

namespace Altaxo.Gui
{
	/// <summary>
	/// Summary description for WorkbenchForm.
	/// </summary>
	[SerializationSurrogate(0,typeof(WorkbenchForm.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version.")]
	public class WorkbenchForm : System.Windows.Forms.Form, IMdiActivationEventSource
	{

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
				WorkbenchForm frm = new WorkbenchForm((System.Windows.Forms.Form)parent);
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


		public WorkbenchForm(System.Windows.Forms.Form parent)
		{

			if(null!=parent)
				this.MdiParent = parent;


			// register event so to be informed when activated
			if(parent is IMdiActivationEventSource)
			{
				((IMdiActivationEventSource)parent).MdiChildDeactivateBefore += new EventHandler(this.EhMdiChildDeactivate);
				((IMdiActivationEventSource)parent).MdiChildActivateAfter += new EventHandler(this.EhMdiChildActivate);
			}
			else if(parent!=null)
			{
				parent.MdiChildActivate += new EventHandler(this.EhMdiChildActivate);
				parent.MdiChildActivate += new EventHandler(this.EhMdiChildDeactivate);
			}
		}


		protected void EhMdiChildActivate(object sender, EventArgs e)
		{
		if(null!=this.MdiChildActivateAfter)
			this.MdiChildActivateAfter(sender,e);
		}

		protected void EhMdiChildDeactivate(object sender, EventArgs e)
		{
		if(null!=this.MdiChildDeactivateBefore)
			this.MdiChildDeactivateBefore(sender,e);
		}

		#region IMdiActivationEventSource Members

		public event System.EventHandler MdiChildDeactivateBefore;

		public event System.EventHandler MdiChildActivateBefore;

		public event System.EventHandler MdiChildDeactivateAfter;

		public event System.EventHandler MdiChildActivateAfter;

		#endregion
	}
}
