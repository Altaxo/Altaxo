#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for XYFunctionPlotData.
	/// </summary>
	public class XYFunctionPlotData : ICloneable, Calc.IScalarFunctionDD, Main.IChangedEventSource
	{
		public XYFunctionPlotData()
		{
		}
		public XYFunctionPlotData(XYFunctionPlotData from)
		{
		}

		#region ICloneable Members

		public object Clone()
		{
			return new XYFunctionPlotData(this);
		}

		#endregion

		#region IScalarFunctionDD Members

		public double Function(double x)
		{
			return x*x;
		}

		#endregion

		#region IChangedEventSource Members

		public event System.EventHandler Changed;

		#endregion
	}
}
