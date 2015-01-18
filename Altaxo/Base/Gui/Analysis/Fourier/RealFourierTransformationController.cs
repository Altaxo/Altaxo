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

using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Analysis.Fourier
{
	public interface IRealFourierTransformationView
	{
		void SetColumnToTransform(string val);

		void SetXIncrement(string val, bool bMarkAsWarning);

		void SetOutputQuantities(SelectableListNodeList list);

		void SetCreationOptions(SelectableListNodeList list);
	}

	[ExpectedTypeOfView(typeof(IRealFourierTransformationView))]
	[UserControllerForObject(typeof(AnalysisRealFourierTransformationCommands.RealFourierTransformOptions))]
	public class RealFourierTransformationController : IMVCANController
	{
		private IRealFourierTransformationView _view;
		private AnalysisRealFourierTransformationCommands.RealFourierTransformOptions _doc;

		private SelectableListNodeList _outputQuantities;
		private SelectableListNodeList _creationOptions;

		private void Initialize(bool bInitData)
		{
			if (bInitData)
			{
				_outputQuantities = new SelectableListNodeList();
				_creationOptions = new SelectableListNodeList();
			}

			if (_view != null)
			{
				var yColName = AbsoluteDocumentPath.GetPathString(_doc.ColumnToTransform, int.MaxValue);
				_view.SetColumnToTransform(yColName);

				string xInc = _doc.XIncrementValue.ToString();
				if (_doc.XIncrementMessage != null)
					xInc += string.Format(" ({0})", _doc.XIncrementMessage);
				_view.SetXIncrement(xInc, _doc.XIncrementMessage != null);

				_outputQuantities.FillWithFlagEnumeration(_doc.Output);
				_view.SetOutputQuantities(_outputQuantities);

				_creationOptions.FillWithEnumeration(_doc.OutputPlacement);
				_view.SetCreationOptions(_creationOptions);
			}
		}

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0 || !(args[0] is AnalysisRealFourierTransformationCommands.RealFourierTransformOptions))
				return false;
			_doc = args[0] as AnalysisRealFourierTransformationCommands.RealFourierTransformOptions;
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
				_view = value as IRealFourierTransformationView;
				if (_view != null)
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

		public bool Apply(bool disposeController)
		{
			_doc.Output = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutput)_outputQuantities.GetFlagEnumValueAsInt32();
			_doc.OutputPlacement = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutputPlacement)_creationOptions.FirstSelectedNode.Tag;
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
	}
}