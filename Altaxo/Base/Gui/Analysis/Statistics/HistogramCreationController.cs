using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Analysis.Statistics
{
	public interface IHistogramCreationView
	{
	}

	public class HistogramCreationController : MVCANControllerEditOriginalDocBase<HistogramCreationInformation, IHistogramCreationView>
	{
	}
}