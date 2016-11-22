using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Gui.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	/// <summary>
	/// Controls the data of a single <see cref="FitElement"/>
	/// </summary>
	
	[ExpectedTypeOfView(typeof(IColumnPlotDataView))]
	public class FitElementDataController
		:
		ColumnPlotDataControllerBase<FitElement>
	{
	}
}
