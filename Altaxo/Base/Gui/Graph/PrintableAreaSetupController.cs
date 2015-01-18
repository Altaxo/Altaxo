#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	public class PrintableAreaSetupOptions : ICloneable
	{
		public PointD2D AreaSize { get; set; }

		public bool Rescale { get; set; }

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion ICloneable Members
	}

	public interface IPrintableAreaSetupView
	{
		PointD2D AreaSize { get; set; }

		bool Rescale { get; set; }
	}

	[ExpectedTypeOfView(typeof(IPrintableAreaSetupView))]
	[UserControllerForObject(typeof(PrintableAreaSetupOptions))]
	public class PrintableAreaSetupController : IMVCANController
	{
		private PrintableAreaSetupOptions _doc;
		private IPrintableAreaSetupView _view;

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length >= 1 && args[0] is PrintableAreaSetupOptions)
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

		#endregion IMVCANController Members

		private void Initialize(bool initData)
		{
			if (_view != null)
			{
				_view.AreaSize = _doc.AreaSize;
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

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			_doc.AreaSize = _view.AreaSize;
			_doc.Rescale = _view.Rescale;
			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}