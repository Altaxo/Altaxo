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
	/// <summary>
	/// This view interface is for showing the options of the LineScatterPlotStyle
	/// </summary>
	public interface ILineScatterPlotStyleView
	{
		// Get / sets the controller of this view
		ILineScatterPlotStyleController Controller { get; set; }
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
	}

	/// <summary>
	/// This is the controller interface of the LineScatterPlotStyleView
	/// </summary>
	public interface ILineScatterPlotStyleController
	{
	}

	/// <summary>
	/// Summary description for LineScatterPlotStyleController.
	/// </summary>
	public class LineScatterPlotStyleController : ILineScatterPlotStyleController, Main.IApplyController
	{
		protected PlotStyle m_PlotStyle;
		protected PlotGroup m_PlotGroup;
		protected PlotGroupStyle m_PlotGroupStyle;
		ILineScatterPlotStyleView m_View;
		bool m_bEnableEvents=false;

		public LineScatterPlotStyleController(PlotStyle ps, PlotGroup plotGroup)
		{
			// if this plotstyle belongs to a plot group of this layer,
			// use the master plot style instead of the plotstyle itself
			m_PlotGroup=plotGroup;
			if(null!=m_PlotGroup)
			{
				m_PlotStyle = (PlotStyle)((PlotStyle)m_PlotGroup.MasterItem.Style).Clone();
				m_PlotGroupStyle = m_PlotGroup.Style;
			}
			else // not member of a plotgroup
			{
				m_PlotStyle = (PlotStyle)ps.Clone();
				m_PlotGroupStyle = 0;
			}
		}


		public ILineScatterPlotStyleView View
		{
			get { return m_View; }
			set
			{
				m_View = value;
				m_View.Controller = this;

				m_bEnableEvents=false; // disable events during fill in
				FillDialogElements();
				m_bEnableEvents=true; // just now enable the events
			}
		}


		void FillDialogElements()
		{
			/*
			FillColorComboBox(this.m_cbLineSymbolColor);
			FillLineConnectComboBox(this.m_cbLineConnect);
			FillLineStyleComboBox(this.m_cbLineType);
			FillLineWidthComboBox(this.m_cbLineWidth);
			FillColorComboBox(this.m_cbLineFillColor);
			FillFillDirectionComboBox(this.m_cbLineFillDirection);

			FillSymbolShapeComboBox(this.m_cbSymbolShape);
			FillSymbolStyleComboBox(this.m_cbSymbolStyle);
			FillSymbolSizeComboBox(this.m_cbSymbolSize);
*/

			// now we have to set all dialog elements to the right values
			SetPlotType(this.m_PlotStyle);
			SetPlotStyleColor(this.m_PlotStyle);
	
			/*
			SetLineSymbolGapCheckBox(this.m_chkLineSymbolGap, this.m_PlotStyle);

			SetDropLineLeftCheckBox(this.m_chkSymbolDropLineLeft, this.m_PlotStyle);
			SetDropLineRightCheckBox(this.m_chkSymbolDropLineRight, this.m_PlotStyle);
			SetDropLineTopCheckBox(this.m_chkSymbolDropLineTop, this.m_PlotStyle);
			SetDropLineBottomCheckBox(this.m_chkSymbolDropLineBottom, this.m_PlotStyle);


			SetLineConnectComboBox(this.m_cbLineConnect,this.m_PlotStyle);
			SetLineStyleComboBox(this.m_cbLineType,this.m_PlotStyle);
			SetLineWidthComboBox(this.m_cbLineWidth, m_PlotStyle);
			SetFillCheckBox(this.m_chkLineFillArea, m_PlotStyle);
			SetFillDirectionComboBox(this.m_cbLineFillDirection,m_PlotStyle);
			SetFillColorComboBox(this.m_cbLineFillColor, m_PlotStyle);
			EnableDisableFillElements(this,m_PlotStyle);
		
			SetSymbolShapeComboBox(this.m_cbSymbolShape,m_PlotStyle);
			SetSymbolStyleComboBox(this.m_cbSymbolStyle,m_PlotStyle);
			SetSymbolSizeComboBox(this.m_cbSymbolSize,m_PlotStyle);

			SetPlotGroupElements(m_PlotGroup);
			
			*/
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
		#region IApplyController Members

		public bool Apply()
		{
			return true;
		}

		#endregion
	} // end of class LineScatterPlotStyleController
} // end of namespace
