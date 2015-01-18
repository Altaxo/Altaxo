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

using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	#region Interfaces

	public interface ILog10TickSpacingView
	{
		int? DecadesPerMajorTick { get; set; }

		int? MinorTicks { get; set; }

		double OneLever { get; set; }

		double MinGrace { get; set; }

		double MaxGrace { get; set; }

		int TargetNumberMajorTicks { get; set; }

		int TargetNumberMinorTicks { get; set; }

		SelectableListNodeList SnapTicksToOrg { set; }

		SelectableListNodeList SnapTicksToEnd { set; }

		string DivideBy { set; }

		bool TransfoOperationIsMultiply { set; }

		string SuppressMajorTickValues { get; set; }

		string SuppressMinorTickValues { get; set; }

		string SuppressMajorTicksByNumber { get; set; }

		string SuppressMinorTicksByNumber { get; set; }

		string AddMajorTickValues { get; set; }

		string AddMinorTickValues { get; set; }

		event Action<string, CancelEventArgs> DivideByValidating;

		event Action<bool> TransfoOperationChanged;
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(Log10TickSpacing), 200)]
	[ExpectedTypeOfView(typeof(ILog10TickSpacingView))]
	public class Log10TickSpacingController : IMVCANController
	{
		private ILog10TickSpacingView _view;
		private Log10TickSpacing _originalDoc;
		private Log10TickSpacing _doc;

		private SelectableListNodeList _snapTicksToOrg = new SelectableListNodeList();
		private SelectableListNodeList _snapTicksToEnd = new SelectableListNodeList();

		private void Initialize(bool initData)
		{
			if (_view != null)
			{
				_view.DecadesPerMajorTick = _doc.DecadesPerMajorTick;
				_view.MinorTicks = _doc.MinorTicks;
				_view.OneLever = _doc.OneLever;
				_view.MinGrace = _doc.MinGrace;
				_view.MaxGrace = _doc.MaxGrace;

				_snapTicksToOrg.Clear();
				_snapTicksToEnd.Clear();
				foreach (BoundaryTickSnapping s in Enum.GetValues(typeof(BoundaryTickSnapping)))
				{
					_snapTicksToOrg.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s), s, s == _doc.SnapOrgToTick));
					_snapTicksToEnd.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s), s, s == _doc.SnapEndToTick));
				}
				_view.SnapTicksToOrg = _snapTicksToOrg;
				_view.SnapTicksToEnd = _snapTicksToEnd;

				_view.TargetNumberMajorTicks = _doc.TargetNumberOfMajorTicks;
				_view.TargetNumberMinorTicks = _doc.TargetNumberOfMinorTicks;

				_view.DivideBy = GUIConversion.ToString(_doc.TransformationDivider);
				_view.TransfoOperationIsMultiply = _doc.TransformationOperationIsMultiply;

				_view.SuppressMajorTickValues = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByValues);
				_view.SuppressMinorTickValues = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByValues);
				_view.SuppressMajorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByNumbers);
				_view.SuppressMinorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByNumbers);

				_view.AddMajorTickValues = GUIConversion.ToString(_doc.AdditionalMajorTicks.ByValues);
				_view.AddMinorTickValues = GUIConversion.ToString(_doc.AdditionalMinorTicks.ByValues);
			}
		}

		private void EhMajorSpanValidating(string txt, CancelEventArgs e)
		{
			int? val;
			if (GUIConversion.IsInt32OrNull(txt, out val))
				_doc.DecadesPerMajorTick = val;
			else
				e.Cancel = true;
		}

		private void EhMinorTicksValidating(string txt, CancelEventArgs e)
		{
			int? val;
			if (GUIConversion.IsInt32OrNull(txt, out val))
				_doc.MinorTicks = val;
			else
				e.Cancel = true;
		}

		private void EhOneLeverValidating(CancelEventArgs e)
		{
			_doc.OneLever = _view.OneLever;
		}

		private void EhMinGraceValidating(CancelEventArgs e)
		{
			_doc.MinGrace = _view.MinGrace;
		}

		private void EhMaxGraceValidating(CancelEventArgs e)
		{
			_doc.MaxGrace = _view.MaxGrace;
		}

		private void EhDivideByValidating(string txt, CancelEventArgs e)
		{
			double val;
			if (GUIConversion.IsDouble(txt, out val))
			{
				double val1 = (double)val;
				if (val == 0 || !val1.IsFinite())
					e.Cancel = true;
				else
					_doc.TransformationDivider = val1;
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void EhTransformationOperationChanged(bool transfoOpIsMultiply)
		{
			_doc.TransformationOperationIsMultiply = transfoOpIsMultiply;
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0)
				return false;
			_originalDoc = args[0] as Log10TickSpacing;
			if (null == _originalDoc)
				return false;
			_doc = (Log10TickSpacing)_originalDoc.Clone();

			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion IMVCANController Members

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.DivideByValidating -= EhDivideByValidating;
					_view.TransfoOperationChanged -= EhTransformationOperationChanged;
				}
				_view = value as ILog10TickSpacingView;

				if (null != _view)
				{
					_view.DivideByValidating += EhDivideByValidating;
					_view.TransfoOperationChanged += EhTransformationOperationChanged;

					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _originalDoc; }
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			AltaxoVariant[] varVals;
			int[] intVals;

			if (GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMajorTickValues, out varVals))
			{
				_doc.SuppressedMajorTicks.ByValues.Clear();
				foreach (AltaxoVariant v in varVals)
					_doc.SuppressedMajorTicks.ByValues.Add(v);
			}
			else
			{
				return false;
			}

			if (GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMinorTickValues, out varVals))
			{
				_doc.SuppressedMinorTicks.ByValues.Clear();
				foreach (AltaxoVariant v in varVals)
					_doc.SuppressedMinorTicks.ByValues.Add(v);
			}
			else
			{
				return false;
			}

			if (GUIConversion.TryParseMultipleInt32(_view.SuppressMajorTicksByNumber, out intVals))
			{
				_doc.SuppressedMajorTicks.ByNumbers.Clear();
				foreach (int v in intVals)
					_doc.SuppressedMajorTicks.ByNumbers.Add(v);
			}
			else
			{
				return false;
			}

			if (GUIConversion.TryParseMultipleInt32(_view.SuppressMinorTicksByNumber, out intVals))
			{
				_doc.SuppressedMinorTicks.ByNumbers.Clear();
				foreach (int v in intVals)
					_doc.SuppressedMinorTicks.ByNumbers.Add(v);
			}
			else
			{
				return false;
			}

			if (GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMajorTickValues, out varVals))
			{
				_doc.AdditionalMajorTicks.ByValues.Clear();
				foreach (AltaxoVariant v in varVals)
					_doc.AdditionalMajorTicks.ByValues.Add(v);
			}
			else
			{
				return false;
			}

			if (GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMinorTickValues, out varVals))
			{
				_doc.AdditionalMinorTicks.ByValues.Clear();
				foreach (AltaxoVariant v in varVals)
					_doc.AdditionalMinorTicks.ByValues.Add(v);
			}
			else
			{
				return false;
			}

			_doc.DecadesPerMajorTick = _view.DecadesPerMajorTick;
			_doc.MinorTicks = _view.MinorTicks;

			_doc.TargetNumberOfMajorTicks = _view.TargetNumberMajorTicks;
			_doc.TargetNumberOfMinorTicks = _view.TargetNumberMinorTicks;

			_doc.OneLever = _view.OneLever;
			_doc.MinGrace = _view.MinGrace;
			_doc.MaxGrace = _view.MaxGrace;

			_doc.SnapOrgToTick = (BoundaryTickSnapping)_snapTicksToOrg.FirstSelectedNode.Tag;
			_doc.SnapEndToTick = (BoundaryTickSnapping)_snapTicksToEnd.FirstSelectedNode.Tag;

			_originalDoc.CopyFrom(_doc);
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