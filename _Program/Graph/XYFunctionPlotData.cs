using System;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for XYFunctionPlotData.
	/// </summary>
	public class XYFunctionPlotData : ICloneable, Calc.IScalarFunctionDD, IChangedEventSource
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
