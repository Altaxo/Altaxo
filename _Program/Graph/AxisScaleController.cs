using System;

namespace Altaxo.Graph
{
	#region Interfaces
	public interface IAxisScaleController
	{
		void EhView_AxisOrgChanged(string text);
		void EhView_AxisEndChanged(string text);
		void EhView_AxisTypeChanged(string text);
		void EhView_AxisRescaleChanged(string text);
	}

	public interface IAxisScaleView
	{

		IAxisScaleController Controller { get; set; }

		System.Windows.Forms.Form Form	{	get; }

		void InitializeAxisOrg(string org);

		void InitializeAxisEnd(string end);

		void InitializeAxisType(string[] arr, string sel);

		void InitializeAxisRescale(string[] arr, string sel);
	}
	#endregion

	/// <summary>
	/// Summary description for AxisScaleController.
	/// </summary>
	public class AxisScaleController : IAxisScaleController
	{
		public enum AxisDirection { Horizontal=0, Vertical=1 }
		protected IAxisScaleView m_View;
		protected Layer m_Layer;
		protected AxisDirection m_Direction;
		
		// Cached values
		protected Axis m_Axis;

		protected string m_AxisOrg;
		protected string m_AxisEnd;
		protected string m_AxisType;
		protected string m_AxisRescale;




		public AxisScaleController(Layer layer, AxisDirection dir)
		{
			m_Layer = layer;
			m_Direction = dir;
			m_Axis = dir==AxisDirection.Horizontal ? m_Layer.XAxis : m_Layer.YAxis;
		}

		public IAxisScaleView View
		{
			get { return m_View; }
			set
			{
				m_View = value;
				m_View.Controller = this;
				SetElements();
			}
		}

		public void SetElements()
		{
			SetAxisOrg();
			SetAxisEnd();
			SetAxisType();
			SetAxisRescale();
		}


		public void SetAxisOrg()
		{
			string name = null!=m_AxisOrg ? m_AxisOrg : m_Axis.Org.ToString();
			if(null!=View)
				View.InitializeAxisOrg(name);
		}
		public void SetAxisEnd()
		{
			string name = null!=m_AxisEnd ? m_AxisEnd : m_Axis.End.ToString();
			if(null!=View)
				View.InitializeAxisEnd(name);
		}

		public void SetAxisType()
		{
			string[] names = new string[Axis.AvailableAxes.Keys.Count];
			string name;
			if(null!=m_AxisType)
				name = m_AxisType;
			else
				name = m_Axis.GetType().ToString();

			int i=0;
			foreach(string axs in Axis.AvailableAxes.Keys)
			{
				names[i++] = axs;
			}

			if(null!=View)
				View.InitializeAxisType(names,name);
		}

		public void SetAxisRescale()
		{
			string[] names = {"automatic", "org fixed", "end fixed", "both fixed" };
			string name;
			if(null!=m_AxisRescale)
			{
				name = m_AxisRescale;
			}
			else
			{
				if(!currAxis.OrgFixed && !currAxis.EndFixed)
					name = names[0];
				else if(currAxis.OrgFixed && !currAxis.EndFixed)
					name = names[1];
				else if(!currAxis.OrgFixed && currAxis.EndFixed)
					name = names[2];
				else 
					name = names[3];
			}
			if(null!=View)
				View.InitializeAxisRescale(names,name);
		}


		protected int GetElements()
		{
			// retrieve the axis type from the dialog box and compare it
			// with the current type
			string axisname = this.m_Scale_cbType.SelectedItem.ToString();
			System.Type axistype = (System.Type)Axis.AvailableAxes[axisname];
			if(null!=axistype)
			{
				if(axistype!=currAxis.GetType())
				{
					// replace the current axis by a new axis of the type axistype
					currAxis = (Axis)System.Activator.CreateInstance(axistype);

					if((m_CurrentEdge==EdgeType.Bottom || m_CurrentEdge==EdgeType.Top))
						m_Layer.XAxis = currAxis;
					else
						m_Layer.YAxis = currAxis;
				}
			}



			switch(this.m_Scale_cbRescale.SelectedIndex)
			{
				default:
				case 0:
					m_Axis.OrgFixed = false; m_Axis.EndFixed=false;
					break;
				case 1:
					m_Axis.OrgFixed = true; m_Axis.EndFixed=false;
					break;
				case 2:
					m_Axis.OrgFixed = false; m_Axis.EndFixed=true;
					break;
				case 3:
					m_Axis.OrgFixed = true; m_Axis.EndFixed=true;
					break;
			} // end switch

			double org = System.Convert.ToDouble(m_Scale_edFrom.Text);
			double end = System.Convert.ToDouble(m_Scale_edTo.Text);




			if(this.m_Scale_FromOrToChanged)
				currAxis.ProcessDataBounds(org,true,end,true);
			return 0; // all ok
		}

		public void EhView_AxisOrgChanged(string text)
		{
			m_AxisOrg = text;
		}
		public void EhView_AxisEndChanged(string text)
		{
			m_AxisEnd = text;
		}
		public void EhView_AxisTypeChanged(string text)
		{
			m_AxisType = text;
		}
		public void EhView_AxisRescaleChanged(string text)
		{
			m_AxisRescale = text;
		}

	}

}
