using System;
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI
{

	#region Interfaces
	public interface ILayerPositionController : Main.GUI.IApplyController, Main.GUI.IMVCController
	{
		/// <summary>
		/// Get/sets the view this controller controls.
		/// </summary>
		ILayerPositionView View { get; set; }

		void EhView_LinkedLayerChanged(string txt);
		void EhView_CommonTypeChanged(string txt);
		void EhView_LeftTypeChanged(string txt);
		void EhView_TopTypeChanged(string txt);
		void EhView_WidthTypeChanged(string txt);
		void EhView_HeightTypeChanged(string txt);

		void EhView_LeftChanged(string txt, ref bool bCancel);
		void EhView_TopChanged(string txt, ref bool bCancel);
		void EhView_WidthChanged(string txt, ref bool bCancel);
		void EhView_HeightChanged(string txt, ref bool bCancel);
		void EhView_RotationChanged(string txt, ref bool bCancel);
		void EhView_ScaleChanged(string txt, ref bool bCancel);
	}

	public interface ILayerPositionView : Main.GUI.IMVCView
	{

		/// <summary>
		/// Get/sets the controller of this view.
		/// </summary>
		ILayerPositionController Controller { get; set; }

		/// <summary>
		/// Gets the hosting parent form of this view.
		/// </summary>
		System.Windows.Forms.Form Form	{	get; }

		void InitializeLeft(string txt);
		void InitializeTop(string txt);
		void InitializeHeight(string txt);
		void InitializeWidth(string txt);
		void InitializeRotation(string txt);
		void InitializeScale(string txt);

		void InitializeLeftType(string[] names, string txt);
		void InitializeTopType(string[] names, string txt);
		void InitializeHeightType(string[] names, string txt);
		void InitializeWidthType(string[] names, string txt);
		void InitializeLinkedLayer(string[] names, string name);

		IAxisLinkView GetXAxisLink();
		IAxisLinkView GetYAxisLink();

	}
	#endregion

	/// <summary>
	/// Summary description for LayerPositionController.
	/// </summary>
	public class LayerPositionController : ILayerPositionController
	{
		// the view
		ILayerPositionView m_View;

		// the document
		XYPlotLayer m_Layer;

		// Shadow variables
		double m_Left, m_Top, m_Width, m_Height, m_Rotation, m_Scale;
		XYPlotLayer.PositionType m_LeftType, m_TopType;
		XYPlotLayer.SizeType			m_HeightType, m_WidthType;
		XYPlotLayer m_LinkedLayer;
		IAxisLinkController m_XAxisLink, m_YAxisLink;

		public LayerPositionController(XYPlotLayer layer)
		{
			m_Layer = layer;
			SetElements(true);
		}


		public void SetElements(bool bInit)
		{


			if(bInit)
			{
				m_Height		= m_Layer.UserHeight;
				m_Width			= m_Layer.UserWidth;
				m_Left			= m_Layer.UserXPosition;
				m_Top				= m_Layer.UserYPosition;
				m_Rotation	= m_Layer.Rotation;
				m_Scale			= m_Layer.Scale;

				m_LeftType = m_Layer.UserXPositionType;
				m_TopType  = m_Layer.UserYPositionType;
				m_HeightType = m_Layer.UserHeightType;
				m_WidthType = m_Layer.UserWidthType;
				m_LinkedLayer = m_Layer.LinkedLayer;

				m_XAxisLink = new AxisLinkController(m_Layer,true);
				m_YAxisLink = new AxisLinkController(m_Layer,false);
			}

			if(View!=null)
			{
			
				View.InitializeHeight(Serialization.NumberConversion.ToString(m_Height));
				View.InitializeWidth(Serialization.NumberConversion.ToString(m_Width));

				View.InitializeLeft(Serialization.NumberConversion.ToString(m_Left));
				View.InitializeTop(Serialization.NumberConversion.ToString(m_Top));

				View.InitializeRotation(Serialization.NumberConversion.ToString(m_Rotation));
				View.InitializeScale(Serialization.NumberConversion.ToString(m_Scale));


				// Fill the comboboxes of the x and y position with possible values
				string [] names = Enum.GetNames(typeof(XYPlotLayer.PositionType));
				
				string nameLeft = Enum.GetName(typeof(XYPlotLayer.PositionType),m_LeftType);
				string nameTop = Enum.GetName(typeof(XYPlotLayer.PositionType),m_TopType);

				View.InitializeLeftType(names,nameLeft);
				View.InitializeTopType(names,nameTop);

				// Fill the comboboxes of the width  and height with possible values
				names = Enum.GetNames(typeof(XYPlotLayer.SizeType));
				string nameWidth  = Enum.GetName(typeof(XYPlotLayer.SizeType),m_WidthType);
				string nameHeigth = Enum.GetName(typeof(XYPlotLayer.SizeType),m_HeightType);

				View.InitializeWidthType(names,nameWidth);
				View.InitializeHeightType(names,nameHeigth);

				// Fill the combobox of linked layer with possible values
				System.Collections.ArrayList arr = new System.Collections.ArrayList();
				arr.Add("None");
				if(null!=m_Layer.ParentLayerList)
				{
					for(int i=0;i<m_Layer.ParentLayerList.Count;i++)
					{
						if(!m_Layer.IsLayerDependentOnMe(m_Layer.ParentLayerList[i]))
							arr.Add("XYPlotLayer " + i.ToString());
					}
				}

				// now if we have a linked layer, set the selected item to the right value
				string nameLL= null==m_LinkedLayer ? "None" : "XYPlotLayer " + m_LinkedLayer.Number;

				View.InitializeLinkedLayer((string[])arr.ToArray(typeof(string)),nameLL);

				// initialize the axis link properties
				m_XAxisLink.View = View.GetXAxisLink();
				m_YAxisLink.View = View.GetYAxisLink();

			}

		}

	

		#region IApplyController Members

		public bool Apply()
		{
			bool bSuccess = true;

			try
			{
				m_Layer.LinkedLayer = m_LinkedLayer;

			
				// now update the layer
				m_Layer.Rotation = (float)m_Rotation;
				m_Layer.Scale    = (float)m_Scale;

				m_Layer.SetSize(m_Width,m_WidthType,m_Height,m_HeightType);
				m_Layer.SetPosition(m_Left,m_LeftType,m_Top,m_TopType);

				if(!this.m_XAxisLink.Apply())
					bSuccess = false;
				if(!this.m_YAxisLink.Apply())
					bSuccess = false;
			}
			catch(Exception)
			{
				return false; // indicate that something failed
			}
			return bSuccess;
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
				View = value as ILayerPositionView;
			}
		}

		#endregion

		#region ILayerPositionController Members

		public ILayerPositionView View
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

		public void EhView_LinkedLayerChanged(string txt)
		{
			int linkedlayernumber = -1;

			if(txt.StartsWith("XYPlotLayer "))
				linkedlayernumber= System.Convert.ToInt32(txt.Substring(6));

			m_LinkedLayer = linkedlayernumber<0 ? null : m_Layer.ParentLayerList[linkedlayernumber];
		}

		public void EhView_CommonTypeChanged(string txt)
		{
			// TODO:  Add LayerPositionController.EhView_CommonTypeChanged implementation
		}

		public void EhView_LeftTypeChanged(string txt)
		{
			this.m_LeftType = (XYPlotLayer.PositionType)Enum.Parse(typeof(XYPlotLayer.PositionType),txt);
		}

		public void EhView_TopTypeChanged(string txt)
		{
			this.m_TopType = (XYPlotLayer.PositionType)Enum.Parse(typeof(XYPlotLayer.PositionType),txt);
		}

		public void EhView_WidthTypeChanged(string txt)
		{
			this.m_WidthType = (XYPlotLayer.SizeType)Enum.Parse(typeof(XYPlotLayer.SizeType),txt);
		}

		public void EhView_HeightTypeChanged(string txt)
		{
			this.m_HeightType = (XYPlotLayer.SizeType)Enum.Parse(typeof(XYPlotLayer.SizeType),txt);
		}

		public void EhView_LeftChanged(string txt, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(txt, out m_Left);
		}

		void ILayerPositionController.EhView_TopChanged(string txt, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(txt, out m_Top);
		}

		public void EhView_WidthChanged(string txt, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(txt, out m_Width);
		}

		public void EhView_HeightChanged(string txt, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(txt, out m_Height);
		}

		public void EhView_RotationChanged(string txt, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(txt, out m_Rotation);
		}

		public void EhView_ScaleChanged(string txt, ref bool bCancel)
		{
			bCancel = !NumberConversion.IsDouble(txt, out m_Scale);
		}

		#endregion
	}
}
