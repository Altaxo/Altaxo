/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph
{

	/// <summary>
	/// PlotItem holds the pair of data and style neccessary to plot a curve, function,
	/// surface and so on.  
	/// </summary>
	public abstract class  PlotItem
	{
		/// <summary>
		/// Get/sets the data object of this plot.
		/// </summary>
		public abstract object Data { get; set; }
		/// <summary>
		/// Get/sets the style object of this plot.
		/// </summary>
		public abstract object Style { get; set; }
		/// <summary>
		/// The name of the plot. It can be of different length. An argument of zero or less
		/// returns the shortest possible name, higher values return more verbose names.
		/// </summary>
		/// <param name="level">The naming level, 0 returns the shortest possible name, 1 or more returns more
		/// verbose names.</param>
		/// <returns>The name of the plot.</returns>
		public abstract string GetName(int level);

		/// <summary>
		/// This paints the plot to the layer.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="layer">The plot layer.</param>
		public abstract void Paint(Graphics g, Graph.Layer layer);

	}

	/// <summary>
	/// Summary description for DataPlotCurve.
	/// </summary>
	public class XYDataPlot : PlotItem
	{
		protected PlotAssociation m_PlotAssociation;
		protected PlotStyle       m_PlotStyle;


		public XYDataPlot(PlotAssociation pa, PlotStyle ps)
		{
			m_PlotAssociation = pa;
			m_PlotStyle       = ps;
		}



		public override object Data
		{
			get { return m_PlotAssociation; }
			set
			{
				if(null==value)
					throw new System.ArgumentNullException();
				else if(value is PlotAssociation)
					m_PlotAssociation = (PlotAssociation)value;
				else
					throw new System.ArgumentException("The provided data object is not of the type " + m_PlotAssociation.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
			}
		}
		public override object Style
		{
			get { return m_PlotStyle; }
			set
			{
				if(null==value)
					throw new System.ArgumentNullException();
				else if(value is PlotStyle)
					m_PlotStyle = (PlotStyle)value;
				else
					throw new System.ArgumentException("The provided data object is not of the type " + m_PlotAssociation.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
			}
		}


		public override string GetName(int level)
		{
			return m_PlotAssociation.ToString();
		}

		public override void Paint(Graphics g, Graph.Layer layer)
		{
			if(null!=this.m_PlotStyle)
			{
				m_PlotStyle.Paint(g,layer,m_PlotAssociation);
			}
		}

	}
}
