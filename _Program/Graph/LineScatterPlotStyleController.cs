#region Disclaimer
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002..2003 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Graph
{
	#region Interfaces
	/// <summary>
	/// This view interface is for showing the options of the LineScatterPlotStyle
	/// </summary>
	public interface ILineScatterPlotStyleView
	{
		// Get / sets the controller of this view
		ILineScatterPlotStyleController Controller { get; set; }
	
		
		/// <summary>
		/// Returns the form this view is shown in.
		/// </summary>
		System.Windows.Forms.Form Form { get; }
		
		/// <summary>
		/// Initialized the plot style combobox.
		/// </summary>
		/// <param name="arr">Array of possible selections.</param>
		/// <param name="sel">Current selection.</param>
		void InitializePlotType(string[] arr, string sel);
		
		/// <summary>
		/// Initializes the plot style color combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializePlotStyleColor(string[] arr , string sel);

		/// <summary>
		/// Initializes the symbol size combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializeSymbolSize(string[] arr , string sel);

		/// <summary>
		/// Initializes the symbol style combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializeSymbolStyle(string[] arr , string sel);

		/// <summary>
		/// Initializes the symbol shape combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializeSymbolShape(string[] arr , string sel);


		/// <summary>
		/// Intitalizes the drop line checkboxes.
		/// </summary>
		/// <param name="bLeft">True when a line should be drawn to the left layer edge.</param>
		/// <param name="bBottom">True when a line should be drawn to the bottom layer edge.</param>
		/// <param name="bRight">True when a line should be drawn to the right layer edge.</param>
		/// <param name="bTop">True when a line should be drawn to the top layer edge.</param>
		void InitializeDropLineConditions(bool bLeft, bool bBottom, bool bRight, bool bTop);

		/// <summary>
		/// Initializes the LineSymbolGap check box.
		/// </summary>
		/// <param name="bGap">True if a gap between symbols and line should be shown.</param>
		void InitializeLineSymbolGapCondition(bool bGap);


		/// <summary>
		/// Initializes the Line connection combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections.</param>
		/// <param name="sel">Current selection.</param>
		void InitializeLineConnect(string[] arr , string sel);

		/// <summary>
		/// Initializes the Line Style combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializeLineStyle(string[] arr , string sel);

		/// <summary>
		/// Initializes the Line Width combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializeLineWidth(string[] arr , string sel);

		/// <summary>
		/// Initializes the fill check box.
		/// </summary>
		/// <param name="bFill">True if the plot should be filled.</param>
		void InitializeFillCondition(bool bFill);

		/// <summary>
		/// Initializes the fill direction combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializeFillDirection(string[] arr , string sel);

		/// <summary>
		/// Initializes the fill color combobox.
		/// </summary>
		/// <param name="arr">String array of possible selections</param>
		/// <param name="sel">Current selection.</param>
		void InitializeFillColor(string[] arr , string sel);


		/// <summary>
		/// Initializes the plot group conditions.
		/// </summary>
		/// <param name="bMemberOfPlotGroup">True if this PlotItem is member of a plot group.</param>
		/// <param name="bIndependent">True if all plots independent from each other.</param>
		/// <param name="bColor">True if the color is changed.</param>
		/// <param name="bLineType">True if the line type is changed.</param>
		/// <param name="bSymbol">True if the symbol shape is changed.</param>
		void InitializePlotGroupConditions(bool bMemberOfPlotGroup, bool bIndependent, bool bColor, bool bLineType, bool bSymbol);


		#region Getter

		bool LineSymbolGap { get; }
		string SymbolColor { get; }
		string LineConnect { get; }
		string LineType    { get; }
		string LineWidth   { get; }
		bool   LineFillArea { get; }
		string LineFillDirection { get; }
		string LineFillColor {get; }

		string SymbolShape {get; }
		string SymbolStyle {get; }
		string SymbolSize  {get; }

		bool DropLineLeft  {get; }
		bool DropLineBottom {get; }
		bool DropLineRight {get; }
		bool DropLineTop   {get; }

		bool PlotGroupIncremental { get; }
		bool PlotGroupColor { get; }
		bool PlotGroupLineType { get; }
		bool PlotGroupSymbol { get; }





		#endregion // Getter
	}

	/// <summary>
	/// This is the controller interface of the LineScatterPlotStyleView
	/// </summary>
	public interface ILineScatterPlotStyleController
	{
		void EhView_PlotGroupIndependent_Changed(bool bPlotGroupIsIndependent);
	}

	#endregion

	/// <summary>
	/// Summary description for LineScatterPlotStyleController.
	/// </summary>
	public class LineScatterPlotStyleController : ILineScatterPlotStyleController, Main.IApplyController
	{
		protected PlotStyle m_MasterItemPlotStyle;
		protected PlotStyle m_PlotItemPlotStyle;
		protected PlotStyle m_PlotStyle;
		protected PlotGroup m_PlotGroup;
		protected PlotGroupStyle m_PlotGroupStyle;
		ILineScatterPlotStyleView m_View;

		public LineScatterPlotStyleController(PlotStyle ps, PlotGroup plotGroup)
		{
			// if this plotstyle belongs to a plot group of this layer,
			// use the master plot style instead of the plotstyle itself
			m_PlotGroup=plotGroup;
			m_PlotItemPlotStyle = ps;
			m_MasterItemPlotStyle = null!=plotGroup ? (PlotStyle)plotGroup.MasterItem.Style : null;
	
			if(null!=m_PlotGroup)
			{
				m_PlotStyle = (PlotStyle)m_PlotGroup.MasterItem.Style;
				m_PlotGroupStyle = m_PlotGroup.Style;
			}
			else // not member of a plotgroup
			{
				m_PlotStyle = ps;
				m_PlotGroupStyle = 0;
			}
		}


		public static bool ShowPlotStyleDialog(System.Windows.Forms.Form parentWindow, PlotItem pa, PlotGroup plotGroup)
		{
			Graph.LineScatterPlotStyleController	ctrl = new Graph.LineScatterPlotStyleController((PlotStyle)pa.Style,plotGroup);
			Graph.LineScatterPlotStyleControl view = new Graph.LineScatterPlotStyleControl();
			ctrl.View = view;

			Main.DialogShellController dsc = new Main.DialogShellController(
				new Main.DialogShellView(view), ctrl);

			return dsc.ShowDialog(parentWindow);
		}

		public ILineScatterPlotStyleView View
		{
			get { return m_View; }
			set
			{
				m_View = value;
				m_View.Controller = this;

				SetPlotGroupElements();
				SetPlotStyleElements();
			}
		}

		public static string [] GetPlotColorNames()
		{
			string[] arr = new string[1+PlotStyle.PlotColors.Length];

			arr[0] = "Custom";

			int i=1;
			foreach(Color c in PlotStyle.PlotColors)
			{
				string name = c.ToString();
				arr[i++] = name.Substring(7,name.Length-8);
			}

			return arr;
		}


		void SetPlotGroupElements()
		{
			SetPlotGroupConditions(this.m_PlotGroup);
		}

		void SetPlotStyleElements()
		{
			// now we have to set all dialog elements to the right values
			SetPlotType(this.m_PlotStyle);
			SetPlotStyleColor(this.m_PlotStyle);
			SetLineSymbolGapCondition(this.m_PlotStyle);


			// Scatter properties
			SetSymbolShape(m_PlotStyle);
			SetSymbolStyle(m_PlotStyle);
			SetSymbolSize(m_PlotStyle);
			SetDropLineConditions(m_PlotStyle);


			// Line properties
			SetLineConnect(m_PlotStyle);
			SetLineStyle(m_PlotStyle);
			SetLineWidth(m_PlotStyle);
			SetFillCondition(m_PlotStyle);
			SetFillDirection(m_PlotStyle);
			SetFillColor(m_PlotStyle);
		}


		public void SetSymbolSize(PlotStyle ps)
		{
			string[] SymbolSizes = 
			{	"0","1","3","5","8","12","15","18","24","30"};

			float symbolsize = 1;
			if(null!=ps && null!=ps.ScatterStyle && null!=ps.ScatterStyle)
				symbolsize = ps.ScatterStyle.SymbolSize;

			string name = symbolsize.ToString();


			View.InitializeSymbolSize(SymbolSizes, name);
		}

	
		public void SetSymbolStyle(PlotStyle ps)
		{
			string [] names = System.Enum.GetNames(typeof(ScatterStyles.Style));

			ScatterStyles.Style sh = ScatterStyles.Style.Solid;
			if(null!=ps && null!=ps.ScatterStyle)
				sh = ps.ScatterStyle.Style;

			string name = sh.ToString();
		
			View.InitializeSymbolStyle(names,name);
		}

	
		public void SetSymbolShape(PlotStyle ps)
		{
			string [] names = System.Enum.GetNames(typeof(ScatterStyles.Shape));

			ScatterStyles.Shape sh = ScatterStyles.Shape.NoSymbol;
			if(null!=ps && null!=ps.ScatterStyle)
				sh = ps.ScatterStyle.Shape;

			string name = sh.ToString();
			
			View.InitializeSymbolShape(names,name);
		}

		public void SetLineSymbolGapCondition(PlotStyle ps)
		{
			bool bGap = ps.LineSymbolGap; // default
			View.InitializeLineSymbolGapCondition( bGap );
		}

		public void SetDropLineConditions(PlotStyle ps)
		{
			bool bLeft = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Left);
			bool bRight = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Right);
			bool bTop = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Top);
			bool bBottom = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Bottom);
		
			View.InitializeDropLineConditions(bLeft,bBottom,bRight,bTop);
		}

		public void SetLineConnect(PlotStyle ps)
		{

			string [] names = System.Enum.GetNames(typeof(LineStyles.ConnectionStyle));
		
			LineStyles.ConnectionStyle cn = LineStyles.ConnectionStyle.NoLine; // default

			if(ps!=null && ps.LineStyle!=null)
				cn = ps.LineStyle.Connection;

			string name = cn.ToString();

			View.InitializeLineConnect(names,name);
		}

		public void SetLineStyle(PlotStyle ps)
		{
			string [] names = System.Enum.GetNames(typeof(DashStyle));

			DashStyle ds = DashStyle.Solid; // default
			if(ps!=null && ps.LineStyle!=null && ps.LineStyle.PenHolder!=null)
				ds = ps.LineStyle.PenHolder.DashStyle;

			string name = ds.ToString();

			View.InitializeLineStyle(names,name);
		}


	
		public void SetLineWidth(PlotStyle ps)
		{
			float[] LineWidths = 
			{	0.2f,0.5f,1,1.5f,2,3,4,5 };
			string[] names = new string[LineWidths.Length];
			for(int i=0;i<names.Length;i++)
				names[i] = LineWidths[i].ToString();

			float linewidth = 1; // default value
			if(null!=ps && null!=ps.LineStyle && null!=ps.LineStyle.PenHolder)
				linewidth = ps.LineStyle.PenHolder.Width;

			string name = linewidth.ToString();

			View.InitializeLineWidth(names,name);
		}

		public void SetFillCondition(PlotStyle ps)
		{
			bool bFill = false; // default
			if(null!=ps && null!=ps.LineStyle)
				bFill = ps.LineStyle.FillArea;

			View.InitializeFillCondition( bFill );

		}

		public void SetFillDirection(PlotStyle ps)
		{
			string [] names = System.Enum.GetNames(typeof(LineStyles.FillDirection));
			

			LineStyles.FillDirection dir = LineStyles.FillDirection.Bottom; // default
			if(null!=ps && null!=ps.LineStyle)
				dir = ps.LineStyle.FillDirection;

			string name = dir.ToString();
			View.InitializeFillDirection(names,name);
		}

		public void SetFillColor(PlotStyle ps)
		{
			string name = "Custom"; // default

			if(null!=ps && null!=ps.LineStyle && null!=ps.LineStyle.FillBrush)
			{
				name = "Custom";
				if(ps.LineStyle.FillBrush.BrushType==BrushType.SolidBrush) 
				{
					name = PlotStyle.GetPlotColorName(ps.LineStyle.FillBrush.Color);
					if(null==name) name = "Custom";
				}
			}
			View.InitializeFillColor(GetPlotColorNames(), name);
		}



		public void SetPlotType(PlotStyle ps)
		{

			string[] arr = { "Nothing", "Line", "Symbol", "Line_Symbol" };
			
			string sel = "Nothing";
			if(null!=ps.LineStyle && null!=ps.ScatterStyle)
				sel = "Line_Symbol";
			else if(null!=ps.LineStyle && null==ps.ScatterStyle)
				sel = "Line";
			else if(null==ps.LineStyle && null!=ps.ScatterStyle)
				sel = "Symbol";
			else
				sel = "Nothing";

			View.InitializePlotType(arr,sel);
		}


		public void SetPlotStyleColor(PlotStyle ps)
		{
			string name = "Custom"; // default

			if(null!=ps && null!=ps.LineStyle && null!=ps.LineStyle.PenHolder)
			{
				name = "Custom";
				if(ps.LineStyle.PenHolder.PenType == PenType.SolidColor)
				{
					name = PlotStyle.GetPlotColorName(ps.LineStyle.PenHolder.Color);
					if(null==name) 
						name = "Custom";
				}
			}
			else if(null!=ps && null!=ps.ScatterStyle && null!=ps.ScatterStyle.Pen)
			{
				name = "Custom";
				if(ps.ScatterStyle.Pen.PenType == PenType.SolidColor)
				{
					name = PlotStyle.GetPlotColorName(ps.ScatterStyle.Pen.Color);
					if(null==name) name = "Custom";
				}
			}
			
			View.InitializePlotStyleColor(GetPlotColorNames(),name);
		}


		private void SetPlotGroupConditions(PlotGroup grp)
		{
			PlotGroupStyle Style = null!=grp ? grp.Style : 0;
	
			View.InitializePlotGroupConditions(
				null!=grp,
				null!=grp && grp.IsIndependent,
				(0!=(Style&PlotGroupStyle.Color)),
				(0!=(Style & PlotGroupStyle.Line)),
				(0!=(Style & PlotGroupStyle.Symbol))
				);

		}
		#region IApplyController Members

		public bool Apply()
		{

			// don't trust user input, so all into a try statement
			try
			{
				// Plot group options first, since they determine what PlotStyle needs to be changed
				m_PlotGroupStyle = 0;
				if(View.PlotGroupIncremental)
				{
					if(View.PlotGroupColor)    m_PlotGroupStyle |= PlotGroupStyle.Color;
					if(View.PlotGroupLineType) m_PlotGroupStyle |= PlotGroupStyle.Line;
					if(View.PlotGroupSymbol)   m_PlotGroupStyle |= PlotGroupStyle.Symbol;
	
					m_PlotStyle = m_MasterItemPlotStyle!=null ? m_MasterItemPlotStyle : m_PlotStyle;
				}
				else // independent
				{
					m_PlotStyle = m_PlotItemPlotStyle;
				}



				// Symbol Gap
				m_PlotStyle.LineSymbolGap = View.LineSymbolGap;

				// Symbol Color
				string str = View.SymbolColor;
				if(str!="Custom")
				{
					m_PlotStyle.Color = Color.FromName(str);
				}


				// Line Connect
				m_PlotStyle.LineStyle.Connection = (LineStyles.ConnectionStyle)Enum.Parse(typeof(LineStyles.ConnectionStyle),View.LineConnect);
				// Line Type
				m_PlotStyle.LineStyle.PenHolder.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),View.LineType);
				// Line Width
				float width = System.Convert.ToSingle(View.LineWidth);
				m_PlotStyle.LineStyle.PenHolder.Width = width;


				// Fill Area
				m_PlotStyle.LineStyle.FillArea = View.LineFillArea;
				// Line fill direction
				m_PlotStyle.LineStyle.FillDirection = (LineStyles.FillDirection)Enum.Parse(typeof(LineStyles.FillDirection),View.LineFillDirection);
				// Line fill color
				str = View.LineFillColor;
				if(str!="Custom")
					m_PlotStyle.LineStyle.FillBrush = new BrushHolder(Color.FromName(str));


				// Symbol Shape
				str = View.SymbolShape;
				m_PlotStyle.ScatterStyle.Shape = (ScatterStyles.Shape)Enum.Parse(typeof(ScatterStyles.Shape),str);

				// Symbol Style
				str = View.SymbolStyle;
				m_PlotStyle.ScatterStyle.Style = (ScatterStyles.Style)Enum.Parse(typeof(ScatterStyles.Style),str);

				// Symbol Size
				str = View.SymbolSize;
				m_PlotStyle.SymbolSize = System.Convert.ToSingle(str);

				// Drop line left
				if(View.DropLineLeft) 
					m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Left;
				else
					m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Left);


				// Drop line bottom
				if(View.DropLineBottom) 
					m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Bottom;
				else
					m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Bottom);

				// Drop line right
				if(View.DropLineRight) 
					m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Right;
				else
					m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Right);

				// Drop line top
				if(View.DropLineTop) 
					m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Top;
				else
					m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Top);


				if(null!=m_PlotGroup)
				{
					m_PlotGroup.Style = m_PlotGroupStyle;
					if(!m_PlotGroup.IsIndependent)
						m_PlotGroup.UpdateMembers();
				}

			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this.View.Form,ex.ToString(),"A problem occured");
				return false;
			}

			return true;
		}

		#endregion

		#region ILineScatterPlotStyleController Members
		
		public void EhView_PlotGroupIndependent_Changed(bool bPlotGroupIsIndependent)
		{
			if(bPlotGroupIsIndependent)
			{
				m_PlotStyle = this.m_PlotItemPlotStyle;
				this.SetPlotStyleElements();
			}
			else // Plot Group is dependent
			{
				m_PlotStyle = this.m_MasterItemPlotStyle;
				this.SetPlotStyleElements();
			}

		}

		#endregion
	} // end of class LineScatterPlotStyleController
} // end of namespace
