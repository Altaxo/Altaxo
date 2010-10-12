using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Data;

namespace Altaxo.Gui.Worksheet
{
	public interface IMasterCurveCreationMainView
	{
		void InitializeDataTab(object guiControl);
		void InitializeEditTab(object guiControl);
	}

	[UserControllerForObject(typeof(MasterCurveCreation.Options))]
	[ExpectedTypeOfView(typeof(IMasterCurveCreationMainView))]
	class MasterCurveCreationMainController : IMVCANController
	{
		MasterCurveCreation.Options _doc;
		IMasterCurveCreationMainView _view;

		IMVCANController _dataController;
		IMVCANController _editController;

		void Initialize(bool initData)
		{
			if (initData)
			{
				_dataController = new MasterCurveCreationDataController();
				_dataController.InitializeDocument(_doc.ColumnGroups);
				Current.Gui.FindAndAttachControlTo(_dataController);
			}

			if (null != _view)
			{
				_view.InitializeDataTab(_dataController.ViewObject);
			}
		}


		public bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is MasterCurveCreation.Options))
				return false;

			_doc = args[0] as MasterCurveCreation.Options;
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IMasterCurveCreationMainView;

				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public bool Apply()
		{
			return true;
		}
	}
}
