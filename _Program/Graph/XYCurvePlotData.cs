using System;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for XYCurvePlotData.
	/// </summary>
	public class XYCurvePlotData : ICloneable, Calc.IScalarFunctionDD, IChangedEventSource
	{
		public XYCurvePlotData()
		{
		}
		public XYCurvePlotData(XYCurvePlotData from)
		{
		}

		#region ICloneable Members

		public object Clone()
		{
			return new XYCurvePlotData(this);
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
