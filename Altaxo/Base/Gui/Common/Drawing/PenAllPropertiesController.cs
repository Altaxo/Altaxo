using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common.Drawing
{
	using Altaxo.Graph.Gdi;

	#region Interfaces

	public interface IPenAllPropertiesView
	{
		PenX Pen { get; set; }
	}


	#endregion

	[ExpectedTypeOfView(typeof(IPenAllPropertiesView))]
	public class PenAllPropertiesController : IMVCAController
	{
		IPenAllPropertiesView _view;
		PenX _doc;

		public PenAllPropertiesController(PenX doc)
		{
			_doc = doc;
			Initialize(true);
		}

		void Initialize(bool initData)
		{
			if (_view != null)
			{
				_view.Pen = _doc;
			}
		}

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IPenAllPropertiesView;

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
			_doc = _view.Pen;
			return true;
		}

		#endregion
	}
}
