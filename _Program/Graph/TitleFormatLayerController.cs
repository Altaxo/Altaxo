using System;
using System.Drawing;

namespace Altaxo.Graph
{
	#region Interfaces
	public interface ITitleFormatLayerController : Main.IApplyController
	{
		ITitleFormatLayerView View { get; set; }
		void EhView_ShowAxisChanged(bool bShow);
		void EhView_TitleChanged(string text);
		void EhView_ColorChanged(string text);
		void EhView_ThicknessChanged(string text);
		void EhView_MajorTickLengthChanged(string text);
		void EhView_MajorTicksChanged(int sel);
		void EhView_MinorTicksChanged(int sel);
		void EhView_AxisPositionChanged(int sel);
		void EhView_AxisPositionValueChanged( string text);

	}

	public interface ITitleFormatLayerView
	{

		ITitleFormatLayerController Controller { get; set; }

		System.Windows.Forms.Form Form	{	get; }

		void InitializeShowAxis(bool bShow);

		void InitializeTitle(string org);

		void InitializeColor(string[] arr, string sel);

		void InitializeThickness(string[] arr, string sel);

		void InitializeMajorTickLength(string[] arr, string sel);

		void InitializeMajorTicks(string[] arr, int sel);

		void InitializeMinorTicks(string[] arr, int sel);

		void InitializeAxisPosition(string[] arr, int sel);

		void InitializeAxisPositionValue(string end);

		void InitializeAxisPositionValueEnabled(bool bEnab);


	}
	#endregion


	/// <summary>
	/// Summary description for TitleFormatLayerController.
	/// </summary>
	public class TitleFormatLayerController : ITitleFormatLayerController
	{
		protected ITitleFormatLayerView m_View;
		protected Layer                 m_Layer;
		protected EdgeType              m_Edge;
 
		protected bool		m_bShowAxis;
		protected string	m_Title;
		protected string	m_Color;
		protected string	m_Thickness;
		protected string	m_MajorTickLength;
		protected int			m_MajorTicks;
		protected int			m_MinorTicks;
		protected int			m_AxisPosition;
		protected string	m_AxisPositionValue;
		protected bool		m_AxisPositionValueEnabled = true;

		protected bool   m_bElementsInitialized = false;


		public TitleFormatLayerController(Layer layer, EdgeType edge)
		{
			m_Layer = layer;
			m_Edge  = edge;
			this.InitializeElements();
		}

		public void InitializeElements()
		{

			XYLayerAxisStyle axstyle=null;
			string title=null;
			bool bAxisEnabled=false;
			switch(m_Edge)
			{
				case EdgeType.Left:
					axstyle = m_Layer.LeftAxisStyle;
					title   = m_Layer.LeftAxisTitleString;
					bAxisEnabled = m_Layer.LeftAxisEnabled;
					break;
				case EdgeType.Right:
					axstyle = m_Layer.RightAxisStyle;
					title   = m_Layer.RightAxisTitleString;
					bAxisEnabled = m_Layer.RightAxisEnabled;
					break;
				case EdgeType.Bottom:
					axstyle = m_Layer.BottomAxisStyle;
					title   = m_Layer.BottomAxisTitleString;
					bAxisEnabled = m_Layer.BottomAxisEnabled;
					break;
				case EdgeType.Top:
					axstyle = m_Layer.TopAxisStyle;
					title   = m_Layer.TopAxisTitleString;
					bAxisEnabled = m_Layer.TopAxisEnabled;
					break;
			}

			this.m_bShowAxis = bAxisEnabled ;

			// fill axis title box
			this.m_Title = null!=title ? title : "";

			this.m_Color = PlotStyle.GetPlotColorName(axstyle.Color);
			if(null==this.m_Color)
				this.m_Color = "Custom";

			this.m_Thickness = axstyle.Thickness.ToString();

			this.m_MajorTickLength = axstyle.MajorTickLength.ToString();

			this.m_MajorTicks = (axstyle.InnerMajorTicks?1:0) + (axstyle.OuterMajorTicks?2:0); 

			this.m_MinorTicks = (axstyle.InnerMinorTicks?1:0) + (axstyle.OuterMinorTicks?2:0); 


			if(axstyle.Position.Value==0)
			{
				this.m_AxisPosition=0;
				this.m_AxisPositionValue = "";
				this.m_AxisPositionValueEnabled = false;
			}
			else if(axstyle.Position.IsRelative)
			{
				this.m_AxisPosition=1;
				this.m_AxisPositionValue= (100.0*axstyle.Position.Value).ToString();
				this.m_AxisPositionValueEnabled = true;
			}
			else
			{
				this.m_AxisPosition=2;
				this.m_AxisPositionValue = axstyle.Position.Value.ToString();
				this.m_AxisPositionValueEnabled = true;
			}
		
			m_bElementsInitialized = true;
		}

		void SetViewElements()
		{
			if(null==View)
				return;

			int i;
			System.Collections.ArrayList arr = new System.Collections.ArrayList();

			View.InitializeShowAxis(m_bShowAxis);

			// fill the color dialog box
			arr.Clear();
			arr.Add("Custom");
			foreach(Color c in PlotStyle.PlotColors)
			{
				string name = c.ToString();
				arr.Add(name.Substring(7,name.Length-8));
			}
			object sarr = arr.ToArray(typeof(string));
			View.InitializeColor((string[])sarr,this.m_Color);

			// fill axis thickness combo box
			double[] thicknesses = new double[]{0.0,0.2,0.5,1.0,1.5,2.0,3.0,4.0,5.0};
			string[] s_thicknesses = new string[thicknesses.Length];
			for(i=0;i<s_thicknesses.Length;i++)
				s_thicknesses[i] = thicknesses[i].ToString();
			View.InitializeThickness(s_thicknesses,this.m_Thickness);
			
			// fill major tick lenght combo box
			double[] ticklengths = new double[]{3,4,5,6,8,10,12,15,18,24,32};
			string[] s_ticklengths = new string[ticklengths.Length];
			for(i=0;i<s_ticklengths.Length;i++)
				s_ticklengths[i] = ticklengths[i].ToString();
			View.InitializeMajorTickLength(s_ticklengths,this.m_MajorTickLength);

			// fill the Major Ticks combo box
			View.InitializeMajorTicks(new string[]{"None","In","Out","In&Out"},this.m_MajorTicks);

			// fill the Minor Ticks combo box
			View.InitializeMinorTicks(new string[]{"None","In","Out","In&Out"},this.m_MinorTicks);
		

			// fill the position combo box
			View.InitializeAxisPosition(new string[] {
																								 m_Edge.ToString(),
																								 "% from " + m_Edge.ToString(),
																								 "At position ="
																							 }, this.m_AxisPosition);

			View.InitializeAxisPositionValue(this.m_AxisPositionValue);

			View.InitializeAxisPositionValueEnabled(this.m_AxisPositionValueEnabled);
		}

		#region ITitleFormatLayerController Members

		public ITitleFormatLayerView View 
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
				m_View.Controller = this;

				SetViewElements();
			}
		}

		public void EhView_ShowAxisChanged(bool bShow)
		{
			m_bShowAxis = bShow;
		}

		public void EhView_TitleChanged(string text)
		{
			m_Title = text;
		}

		public void EhView_ColorChanged(string text)
		{
			m_Color = text;
		}

		public void EhView_ThicknessChanged(string text)
		{
			m_Thickness = text;
		}

		public void EhView_MajorTickLengthChanged(string text)
		{
			m_MajorTickLength = text;
		}

		public void EhView_MajorTicksChanged(int sel)
		{
			m_MajorTicks = sel;
		}

		public void EhView_MinorTicksChanged(int sel)
		{
			m_MinorTicks = sel;
		}

		public void EhView_AxisPositionChanged(int sel)
		{
			m_AxisPosition = sel;
		}

		public void EhView_AxisPositionValueChanged(string text)
		{
			m_AxisPositionValue = text;
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			XYLayerAxisStyle axstyle=null;
			switch(m_Edge)
			{
				case EdgeType.Left:
					axstyle = m_Layer.LeftAxisStyle;
					break;
				case EdgeType.Right:
					axstyle = m_Layer.RightAxisStyle;
					break;
				case EdgeType.Bottom:
					axstyle = m_Layer.BottomAxisStyle;
					break;
				case EdgeType.Top:
					axstyle = m_Layer.TopAxisStyle;
					break;
			}

			try
			{

				// read axis title
				if(m_Title.Length==0)
					m_Title=null;


				// read axis thickness
				axstyle.Thickness = System.Convert.ToSingle(m_Thickness);

				// read major thick length
				axstyle.MajorTickLength = System.Convert.ToSingle(m_MajorTickLength);

				// read major ticks
				axstyle.InnerMajorTicks = 0!=(1&m_MajorTicks);
				axstyle.OuterMajorTicks = 0!=(2&m_MajorTicks);
 
				// read minor ticks
				axstyle.InnerMinorTicks = 0!=(1&m_MinorTicks);
				axstyle.OuterMinorTicks = 0!=(2&m_MinorTicks);
			
				// read axis position
				double posval;
				switch(m_AxisPosition)
				{
					case 0:
						axstyle.Position = new Calc.RelativeOrAbsoluteValue(0,false);
						break;
					case 1:
						posval = System.Convert.ToDouble(m_AxisPositionValue);
						axstyle.Position = new Calc.RelativeOrAbsoluteValue(posval/100,true);
						break;
					case 2:
						posval = System.Convert.ToDouble(this.m_AxisPositionValue);
						axstyle.Position = new Calc.RelativeOrAbsoluteValue(posval,false);
						break;
				}

				// set axis enabled on the layer
				switch(m_Edge)
				{
					case EdgeType.Left:
						m_Layer.LeftAxisEnabled = m_bShowAxis;
						m_Layer.LeftAxisTitleString = m_Title;
						break;
					case EdgeType.Right:
						m_Layer.RightAxisEnabled = m_bShowAxis;
						m_Layer.RightAxisTitleString = m_Title;
						break;
					case EdgeType.Bottom:
						m_Layer.BottomAxisEnabled = m_bShowAxis;
						m_Layer.BottomAxisTitleString = m_Title;
						break;
					case EdgeType.Top:
						m_Layer.TopAxisEnabled = m_bShowAxis;
						m_Layer.TopAxisTitleString = m_Title;
						break;
				}

			}
			catch(Exception)
			{
				return false; // failed
			}


			return true; // all ok
		}

		#endregion
	}
}
