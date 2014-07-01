using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface IAltaxoOleDbDataSourceView
	{
		void SetQueryView(object viewObject);

		void SetImportOptionsView(object viewObject);
	}

	[ExpectedTypeOfView(typeof(IAltaxoOleDbDataSourceView))]
	[UserControllerForObject(typeof(AltaxoOleDbDataSource))]
	public class AltaxoOleDbDataSourceController : MVCANControllerBase<AltaxoOleDbDataSource, IAltaxoOleDbDataSourceView>
	{
		private OleDbDataQueryController _connectionMainController;
		private Altaxo.Gui.Data.DataSourceImportOptionsController _importOptionsController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_importOptionsController = new Data.DataSourceImportOptionsController() { UseDocumentCopy = UseDocument.Directly };
				_importOptionsController.InitializeDocument(_doc.ImportOptions);

				_connectionMainController = new OleDbDataQueryController() { UseDocumentCopy = UseDocument.Directly };
				_connectionMainController.InitializeDocument(_doc.DataQuery);
			}

			if (null != _view)
			{
				if (null == _importOptionsController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_importOptionsController);

				if (null == _connectionMainController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_connectionMainController);

				_view.SetImportOptionsView(_importOptionsController.ViewObject);
				_view.SetQueryView(_connectionMainController.ViewObject);
			}
		}

		public override bool Apply()
		{
			if (!_importOptionsController.Apply())
				return false;
			if (!_connectionMainController.Apply())
				return false;

			_doc.ImportOptions = (Altaxo.Data.DataSourceImportOptions)_importOptionsController.ModelObject;
			_doc.DataQuery = (OleDbDataQuery)_connectionMainController.ModelObject;

			if (!object.ReferenceEquals(_doc, _originalDoc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}