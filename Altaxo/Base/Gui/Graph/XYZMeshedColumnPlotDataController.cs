using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	public interface IXYZMeshedColumnPlotDataView
	{
		void SetDataView(object viewObject);
	}

	[ExpectedTypeOfView(typeof(IXYZMeshedColumnPlotDataView))]
	[UserControllerForObject(typeof(XYZMeshedColumnPlotData))]
	public class XYZMeshedColumnPlotDataController : MVCANControllerBase<XYZMeshedColumnPlotData, IXYZMeshedColumnPlotDataView>
	{
		private IMVCANController _dataProxyController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_dataProxyController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.DataTableMatrix }, typeof(IMVCANController), UseDocument.Directly);
			}
			if (null != _view)
			{
				_view.SetDataView(_dataProxyController.ViewObject);
			}
		}

		public override bool Apply()
		{
			bool result;

			result = _dataProxyController.Apply();
			if (!result) return result;

			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}