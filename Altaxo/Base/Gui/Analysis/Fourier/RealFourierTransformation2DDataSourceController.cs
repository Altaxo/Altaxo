using Altaxo.Worksheet.Commands.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Analysis.Fourier
{
	public interface IRealFourierTransformation2DDataSourceView
	{
		void SetFourierTransformation2DOptionsControl(object p);

		void SetImportOptionsControl(object p);

		void SetInputDataControl(object p);
	}

	[ExpectedTypeOfView(typeof(IRealFourierTransformation2DDataSourceView))]
	[UserControllerForObject(typeof(FourierTransformation2DDataSource))]
	public class RealFourierTransformation2DDataSourceController : MVCANControllerBase<FourierTransformation2DDataSource, IRealFourierTransformation2DDataSourceView>
	{
		private IMVCANController _dataSourceOptionsController;
		private IMVCANController _fourierTransformationOptionsController;
		private IMVCANController _inputDataController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_dataSourceOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
				_fourierTransformationOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.FourierTransformation2DOptions }, typeof(IMVCANController), UseDocument.Directly);
				_inputDataController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.InputData }, typeof(IMVCANController), UseDocument.Directly);
			}

			if (null != _view)
			{
				_view.SetImportOptionsControl(_dataSourceOptionsController.ViewObject);
				_view.SetFourierTransformation2DOptionsControl(_fourierTransformationOptionsController.ViewObject);
				if (null != _inputDataController)
				{
					_view.SetInputDataControl(_inputDataController.ViewObject);
				}
			}
		}

		public override bool Apply()
		{
			bool result;

			result = _dataSourceOptionsController.Apply();
			if (!result) return result;

			result = _fourierTransformationOptionsController.Apply();
			if (!result) return result;

			if (null != _inputDataController)
			{
				result = _inputDataController.Apply();
				if (!result) return result;
			}

			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}