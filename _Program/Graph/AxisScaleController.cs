using System;

namespace Altaxo.Graph
{
	#region Interfaces
	public interface IAxisScaleController : Main.IApplyController, Main.IMVCController
	{
		void EhView_AxisOrgChanged(string text);
		void EhView_AxisEndChanged(string text);
		void EhView_AxisTypeChanged(string text);
		void EhView_AxisRescaleChanged(string text);
	}

	public interface IAxisScaleView : Main.IMVCView
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

		protected string	m_AxisOrg;
		protected string	m_Original_AxisOrg;

		protected string	m_AxisEnd;
		protected string	m_Original_AxisEnd;

		protected string	m_AxisType;
		protected string	m_Original_AxisType;

		protected string	m_AxisRescale;
		protected string	m_Original_AxisRescale;




		public AxisScaleController(Layer layer, AxisDirection dir)
		{
			m_Layer = layer;
			m_Direction = dir;
			m_Axis = dir==AxisDirection.Horizontal ? m_Layer.XAxis : m_Layer.YAxis;

			SetElements();
		}

		public IAxisScaleView View
		{
			get { return m_View; }
			set
			{
				if(null!=m_View)
					m_View.Controller = null;

				m_View = value;

				if(null!=m_View)
				{
					m_View.Controller = this;
					SetViewElements();
				}
			}
		}

		public object ViewObject
		{
			get { return View; }
			set { View = value as IAxisScaleView; }
		}

		public void SetElements()
		{
			SetAxisOrg(true);
			SetAxisEnd(true);
			SetAxisType(true);
			SetAxisRescale(true);
		}

		public void SetViewElements()
		{
			SetAxisOrg(false);
			SetAxisEnd(false);
			SetAxisType(false);
			SetAxisRescale(false);
		}


		public void SetAxisOrg(bool bInit)
		{
			if(bInit) 
				m_AxisOrg = m_Original_AxisOrg = m_Axis.Org.ToString();

			if(null!=View)
				View.InitializeAxisOrg(m_AxisOrg);
		}
		public void SetAxisEnd(bool bInit)
		{
			if(bInit)
				m_AxisEnd = m_Original_AxisEnd = m_Axis.End.ToString();

			if(null!=View)
				View.InitializeAxisEnd(m_AxisEnd);
		}

		public void SetAxisType(bool bInit)
		{
			string[] names = new string[Axis.AvailableAxes.Keys.Count];
			

			int i=0;
			string curraxisname=null;
			foreach(string axs in Axis.AvailableAxes.Keys)
			{
				names[i++] = axs;
				if(m_Axis.GetType()==Axis.AvailableAxes[axs])
					curraxisname = axs;
			}

			if(bInit)
				m_AxisType = m_Original_AxisType = curraxisname;


			if(null!=View)
				View.InitializeAxisType(names,m_AxisType);
		}

		public void SetAxisRescale(bool bInit)
		{
			string[] names = {"automatic", "org fixed", "end fixed", "both fixed" };
			if(bInit)
			{
				if(!m_Axis.OrgFixed && !m_Axis.EndFixed)
					m_Original_AxisRescale = names[0];
				else if(m_Axis.OrgFixed && !m_Axis.EndFixed)
					m_Original_AxisRescale = names[1];
				else if(!m_Axis.OrgFixed && m_Axis.EndFixed)
					m_Original_AxisRescale = names[2];
				else 
					m_Original_AxisRescale = names[3];

				m_AxisRescale = m_Original_AxisRescale;
			}

			if(null!=View)
				View.InitializeAxisRescale(names,m_AxisRescale);
		}


		public bool Apply()
		{
			if(
				(m_AxisOrg == m_Original_AxisOrg) &&
				(m_AxisEnd == m_Original_AxisEnd) &&
				(m_AxisType == m_Original_AxisType) &&
				(m_AxisRescale == m_Original_AxisRescale))
				return true; // all ok, we don't need to apply since nothing is changed

			try
			{
				// retrieve the axis type from the dialog box and compare it
				// with the current type
				System.Type axistype = (System.Type)Axis.AvailableAxes[m_AxisType];
				if(null!=axistype)
				{
					if(axistype!=m_Axis.GetType())
					{
						// replace the current axis by a new axis of the type axistype
						m_Axis = (Axis)System.Activator.CreateInstance(axistype);

						if((m_Direction==AxisDirection.Horizontal))
							m_Layer.XAxis = m_Axis;
						else
							m_Layer.YAxis = m_Axis;
					}
				}

				switch(m_AxisRescale)
				{
					default:
					case "automatic":
						m_Axis.OrgFixed = false; m_Axis.EndFixed=false;
						break;
					case "org fixed":
						m_Axis.OrgFixed = true; m_Axis.EndFixed=false;
						break;
					case "end fixed":
						m_Axis.OrgFixed = false; m_Axis.EndFixed=true;
						break;
					case "both fixed":
						m_Axis.OrgFixed = true; m_Axis.EndFixed=true;
						break;
				} // end switch


				double org = System.Convert.ToDouble(m_AxisOrg);
				double end = System.Convert.ToDouble(m_AxisEnd);

				if((m_AxisOrg!=m_Original_AxisOrg)  || (m_AxisEnd!=m_Original_AxisEnd))
					m_Axis.ProcessDataBounds(org,true,end,true);

				SetElements();
			}
			catch(Exception )
			{
				return false; // failure
			}
			
			return true; // all ok
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
