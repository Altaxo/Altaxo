using System;
using Altaxo.Serialization;

namespace Altaxo.Graph
{
	#region Interfaces
	public interface IAxisLinkController : Gui.IApplyController, Main.IMVCController
	{
		/// <summary>
		/// Get/sets the view this controller controls.
		/// </summary>
		IAxisLinkView View { get; set; }

		/// <summary>
		/// Called if the type of the link is changed.
		/// </summary>
		/// <param name="linktype">The linktype. Valid arguments are "None", "Straight" and "Custom".</param>
		void EhView_LinkTypeChanged(Layer.AxisLinkType linktype);

		/// <summary>
		/// Called when the contents of OrgA is changed.
		/// </summary>
		/// <param name="orgA">Contents of OrgA.</param>
		/// <param name="bCancel">Normally false, this can be set to true if OrgA is not a valid entry.</param>
		void EhView_OrgAValidating(string orgA, ref bool bCancel);
		/// <summary>
		/// Called when the contents of OrgB is changed.
		/// </summary>
		/// <param name="orgB">Contents of OrgB.</param>
		/// <param name="bCancel">Normally false, this can be set to true if OrgB is not a valid entry.</param>
		void EhView_OrgBValidating(string orgB, ref bool bCancel);
		/// <summary>
		/// Called when the contents of EndA is changed.
		/// </summary>
		/// <param name="endA">Contents of EndA.</param>
		/// <param name="bCancel">Normally false, this can be set to true if EndA is not a valid entry.</param>
		void EhView_EndAValidating(string endA, ref bool bCancel);
		/// <summary>
		/// Called when the contents of EndB is changed.
		/// </summary>
		/// <param name="endB">Contents of EndB.</param>
		/// <param name="bCancel">Normally false, this can be set to true if EndB is not a valid entry.</param>
		void EhView_EndBValidating(string endB, ref bool bCancel);


	}

	public interface IAxisLinkView : Main.IMVCView
	{

		/// <summary>
		/// Get/sets the controller of this view.
		/// </summary>
		IAxisLinkController Controller { get; set; }

		/// <summary>
		/// Gets the hosting parent form of this view.
		/// </summary>
		System.Windows.Forms.Form Form	{	get; }

		/// <summary>
		/// Initializes the type of the link.
		/// </summary>
		/// <param name="linktype"></param>
		void LinkType_Initialize(Layer.AxisLinkType linktype);

		/// <summary>
		/// Initializes the content of the OrgA edit box.
		/// </summary>
		void OrgA_Initialize(string text);

		/// <summary>
		/// Initializes the content of the OrgB edit box.
		/// </summary>
		void OrgB_Initialize(string text);

		/// <summary>
		/// Initializes the content of the EndA edit box.
		/// </summary>
		void EndA_Initialize(string text);

		/// <summary>
		/// Initializes the content of the EndB edit box.
		/// </summary>
		void EndB_Initialize(string text);


		/// <summary>
		/// Enables / Disables the edit boxes for the org and end values
		/// </summary>
		/// <param name="bEnable">True if the boxes are enabled for editing.</param>
		void Enable_OrgAndEnd_Boxes(bool bEnable);
	
	}
	#endregion

	/// <summary>
	/// Summary description for LinkAxisController.
	/// </summary>
	public class AxisLinkController : IAxisLinkController
	{
		IAxisLinkView m_View;
		Layer m_Layer;
		bool  m_bXAxis;

		Layer.AxisLinkType m_LinkType;
		double m_OrgA;
		double m_OrgB;
		double m_EndA;
		double m_EndB;


		public AxisLinkController(Layer layer, bool bXAxis)
		{
			m_Layer = layer;
			m_bXAxis = bXAxis;
			SetElements(true);
		}


		void SetElements(bool bInit)
		{
			if(bInit)
			{
				if(m_bXAxis)
				{
					m_LinkType	= m_Layer.XAxisLinkType;
					m_OrgA			= m_Layer.LinkXAxisOrgA;
					m_OrgB			= m_Layer.LinkXAxisOrgB;
					m_EndA			= m_Layer.LinkXAxisEndA;
					m_EndB			= m_Layer.LinkXAxisEndB;
				}
				else
				{
					m_LinkType	= m_Layer.YAxisLinkType;
					m_OrgA			= m_Layer.LinkYAxisOrgA;
					m_OrgB			= m_Layer.LinkYAxisOrgB;
					m_EndA			= m_Layer.LinkYAxisEndA;
					m_EndB			= m_Layer.LinkYAxisEndB;
				}
			}

			if(null!=View)
			{
				View.LinkType_Initialize(m_LinkType);
				View.OrgA_Initialize(m_OrgA.ToString());
				View.OrgB_Initialize(m_OrgB.ToString());
				View.EndA_Initialize(m_EndA.ToString());
				View.EndB_Initialize(m_EndB.ToString());
			}
		}
		#region ILinkAxisController Members

		public IAxisLinkView View
		{
			get
			{
				return m_View;
			}
			set
			{
				if(null!=m_View)
					m_View.Controller = null;
				
				m_View = value;

				if(null!=m_View)
				{
					m_View.Controller = this;
					SetElements(false); // set only the view elements, dont't initialize the variables
				}
			}
		}

		public void EhView_LinkTypeChanged(Layer.AxisLinkType linktype)
		{
			m_LinkType = linktype;

			if(null!=View)
				View.Enable_OrgAndEnd_Boxes(linktype == Layer.AxisLinkType.Custom);
		}

		public void EhView_OrgAValidating(string orgA, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(orgA, out m_OrgA);
		}

		public void EhView_OrgBValidating(string orgB, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(orgB, out m_OrgB);
		}

		public void EhView_EndAValidating(string endA, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(endA, out m_EndA);
		}

		public void EhView_EndBValidating(string endB, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(endB, out m_EndA);
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			if(this.m_bXAxis)
			{
				m_Layer.XAxisLinkType = m_LinkType;
				m_Layer.LinkXAxisOrgA = m_OrgA;
				m_Layer.LinkXAxisEndA = m_OrgB;
				m_Layer.LinkXAxisOrgB = m_EndA;
				m_Layer.LinkXAxisEndB = m_EndB;
			}
			else
			{
				m_Layer.YAxisLinkType = m_LinkType;
				m_Layer.LinkYAxisOrgA = m_OrgA;
				m_Layer.LinkYAxisEndA = m_OrgB;
				m_Layer.LinkYAxisOrgB = m_EndA;
				m_Layer.LinkYAxisEndB = m_EndB;
				}
			return true;
		}

		#endregion

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return View;
			}
			set
			{
				View = value as IAxisLinkView;
			}
		}

		#endregion
	}
}
