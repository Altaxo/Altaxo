using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Gui.Graph
{
	public interface IFillToCurvePlotStyleView
	{
		bool FillToPreviousItem { get; set; }
		bool FillToNextItem { get; set; }
		BrushX FillColor { get; set; }
	}

	[UserControllerForObject(typeof(FillToCurvePlotStyle))]
	[ExpectedTypeOfView(typeof(IFillToCurvePlotStyleView))]
	public class FillToCurvePlotStyleController : IMVCANController
	{
		FillToCurvePlotStyle _doc;
		IFillToCurvePlotStyleView _view;

		void Initialize(bool initData)
		{
			if (_view != null)
			{
				_view.FillToPreviousItem = _doc.FillToPreviousItem;
				_view.FillToNextItem = _doc.FillToNextItem;
				_view.FillColor = _doc.FillBrush;
			}
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is FillToCurvePlotStyle))
				return false;

			_doc = (FillToCurvePlotStyle)args[0];
			Initialize(true);

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set {  }
		}

		#endregion

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IFillToCurvePlotStyleView;

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

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			_doc.FillToPreviousItem = _view.FillToPreviousItem;
			_doc.FillToNextItem = _view.FillToNextItem;
			_doc.FillBrush = _view.FillColor;

			return true;
		}

		#endregion
	}
}
