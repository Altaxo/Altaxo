using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
namespace Altaxo.Gui.Graph
{
	public class PrintableAreaSetupOptions : ICloneable
	{
		public RectangleF Area { get; set; }
		public bool Rescale { get; set; }

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}


	public interface IPrintableAreaSetupView
	{
		RectangleF Area { get; set; }
		bool Rescale { get; set; }
	}

	[ExpectedTypeOfView(typeof(IPrintableAreaSetupView))]
	[UserControllerForObject(typeof(PrintableAreaSetupOptions))]
	public class PrintableAreaSetupController : IMVCANController
	{
		PrintableAreaSetupOptions _doc;
		IPrintableAreaSetupView _view;
		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length>=1 && args[0] is PrintableAreaSetupOptions)
			{
				_doc = args[0] as PrintableAreaSetupOptions;
				Initialize(true);
				return true;
			}

			return false;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion

		void Initialize(bool initData)
		{
			if (_view != null)
			{
				_view.Area = _doc.Area;
				_view.Rescale = _doc.Rescale;
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
				_view = value as IPrintableAreaSetupView;

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
			_doc.Area = _view.Area;
			_doc.Rescale = _view.Rescale;
			return true;
		}

		#endregion
	}
}
