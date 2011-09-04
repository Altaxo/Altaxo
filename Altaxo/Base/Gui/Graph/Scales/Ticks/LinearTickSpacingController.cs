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
#endregion

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using Altaxo.Data;
using Altaxo.Collections;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Serialization;
using Altaxo.Calc;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	#region Interfaces

	public interface ILinearTickSpacingView
	{
		string MajorTicks { set; }
		string MinorTicks { set; }

		string ZeroLever { set; }
		string MinGrace { set; }
		string MaxGrace { set; }
    string TargetNumberMajorTicks { get; set; }
    string TargetNumberMinorTicks { get; set; }

    SelectableListNodeList SnapTicksToOrg { set; }
    SelectableListNodeList SnapTicksToEnd { set; }

		string DivideBy { set; }
		string TransfoOffset { set; }
		bool TransfoOperationIsMultiply { set; }

		string SuppressMajorTickValues { get; set; }
    string SuppressMinorTickValues { get; set; }
    string SuppressMajorTicksByNumber { get; set; }
		string SuppressMinorTicksByNumber { get; set; }
		string AddMajorTickValues { get; set; }
		string AddMinorTickValues { get; set; }



		event Action<string,CancelEventArgs> MajorTicksValidating;
		event Action<string, CancelEventArgs> MinorTicksValidating;
		event Action<string, CancelEventArgs> ZeroLeverValidating;
		event Action<string, CancelEventArgs> MinGraceValidating;
		event Action<string, CancelEventArgs> MaxGraceValidating;
		event Action<string, CancelEventArgs> DivideByValidating;
		event Action<string, CancelEventArgs> TransfoOffsetValidating;
		event Action<bool> TransfoOperationChanged;
	}

	#endregion

	[UserControllerForObject(typeof(LinearTickSpacing))]
	[ExpectedTypeOfView(typeof(ILinearTickSpacingView))]
	public class LinearTickSpacingController : IMVCANController
	{
		ILinearTickSpacingView _view;
		LinearTickSpacing _originalDoc;
		LinearTickSpacing _doc;

    SelectableListNodeList _snapTicksToOrg = new SelectableListNodeList();
    SelectableListNodeList _snapTicksToEnd = new SelectableListNodeList();

		void Initialize(bool initData)
		{
			if (_view != null)
			{
				_view.MajorTicks = GUIConversion.ToString(_doc.MajorTick);
				_view.MinorTicks = GUIConversion.ToString(_doc.MinorTicks);
				_view.ZeroLever = GUIConversion.ToString(_doc.ZeroLever);
				_view.MinGrace = GUIConversion.ToString(_doc.MinGrace);
				_view.MaxGrace = GUIConversion.ToString(_doc.MaxGrace);

        _snapTicksToOrg.Clear();
        _snapTicksToEnd.Clear();
        foreach (BoundaryTickSnapping s in Enum.GetValues(typeof(BoundaryTickSnapping)))
        {
          _snapTicksToOrg.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s),s,s==_doc.SnapOrgToTick));
          _snapTicksToEnd.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s), s, s == _doc.SnapEndToTick));
        }
        _view.SnapTicksToOrg = _snapTicksToOrg;
        _view.SnapTicksToEnd = _snapTicksToEnd;

        _view.TargetNumberMajorTicks = Serialization.GUIConversion.ToString(_doc.TargetNumberOfMajorTicks);
        _view.TargetNumberMinorTicks = Serialization.GUIConversion.ToString(_doc.TargetNumberOfMinorTicks);

				_view.TransfoOffset = GUIConversion.ToString(_doc.TransformationOffset);
				_view.DivideBy = GUIConversion.ToString(_doc.TransformationDivider);
				_view.TransfoOperationIsMultiply = _doc.TransformationOperationIsMultiply;

        _view.SuppressMajorTickValues = Serialization.GUIConversion.ToString(_doc.SuppressedMajorTicks.ByValues);
        _view.SuppressMinorTickValues = Serialization.GUIConversion.ToString(_doc.SuppressedMinorTicks.ByValues);
        _view.SuppressMajorTicksByNumber = Serialization.GUIConversion.ToString(_doc.SuppressedMajorTicks.ByNumbers);
        _view.SuppressMinorTicksByNumber = Serialization.GUIConversion.ToString(_doc.SuppressedMinorTicks.ByNumbers);

        _view.AddMajorTickValues = Serialization.GUIConversion.ToString(_doc.AdditionalMajorTicks.ByValues);
        _view.AddMinorTickValues = Serialization.GUIConversion.ToString(_doc.AdditionalMinorTicks.ByValues);
      }
		}

		void EhMajorSpanValidating(string txt, CancelEventArgs e)
		{
			double? val;
			if (GUIConversion.IsDoubleOrNull(txt, out val))
				_doc.MajorTick = val;
			else
				e.Cancel = true;
		}

		void EhMinorTicksValidating(string txt, CancelEventArgs e)
		{
			int? val;
			if (GUIConversion.IsInt32OrNull(txt, out val))
				_doc.MinorTicks = val;
			else
				e.Cancel = true;
		}


		void EhZeroLeverValidating(string txt, CancelEventArgs e)
		{
			double val;
			if (GUIConversion.IsDouble(txt, out val))
				_doc.ZeroLever = val;
			else
				e.Cancel = true;
		}

		void EhMinGraceValidating(string txt, CancelEventArgs e)
		{
			double val;
			if (GUIConversion.IsDouble(txt, out val))
				_doc.MinGrace = val;
			else
				e.Cancel = true;
		}

		void EhMaxGraceValidating(string txt, CancelEventArgs e)
		{
			double val;
			if (GUIConversion.IsDouble(txt, out val))
				_doc.MaxGrace = val;
			else
				e.Cancel = true;
		}

		void EhDivideByValidating(string txt, CancelEventArgs e)
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

		void EhTransformationOffsetValidating(string txt, CancelEventArgs e)
		{
			double val;
			if (GUIConversion.IsDouble(txt, out val))
			{
				if (!val.IsFinite())
					e.Cancel = true;
				else
					_doc.TransformationOffset = val;
			}
			else
			{
				e.Cancel = true;
			}
		}

		void EhTransformationOperationChanged(bool transfoOpIsMultiply)
		{
			_doc.TransformationOperationIsMultiply = transfoOpIsMultiply;
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0)
				return false;
			_originalDoc = args[0] as LinearTickSpacing;
			if (null == _originalDoc)
				return false;
			_doc = (LinearTickSpacing)_originalDoc.Clone();

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
				if (null != _view)
				{
					_view.MajorTicksValidating -= EhMajorSpanValidating;
					_view.MinorTicksValidating -= EhMinorTicksValidating;
					_view.ZeroLeverValidating -= EhZeroLeverValidating;
					_view.MinGraceValidating -= EhMinGraceValidating;
					_view.MaxGraceValidating -= EhMaxGraceValidating;
					_view.DivideByValidating -= EhDivideByValidating;
					_view.TransfoOffsetValidating -= EhTransformationOffsetValidating;
					_view.TransfoOperationChanged -= EhTransformationOperationChanged;
				}
				_view = value as ILinearTickSpacingView;

				if (null != _view)
				{
					_view.MajorTicksValidating += EhMajorSpanValidating;
					_view.MinorTicksValidating += EhMinorTicksValidating;
					_view.ZeroLeverValidating += EhZeroLeverValidating;
					_view.MinGraceValidating += EhMinGraceValidating;
					_view.MaxGraceValidating += EhMaxGraceValidating;
					_view.DivideByValidating += EhDivideByValidating;
					_view.TransfoOffsetValidating += EhTransformationOffsetValidating;
					_view.TransfoOperationChanged += EhTransformationOperationChanged;

					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _originalDoc; }
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
      AltaxoVariant[] varVals;
      int[] intVals;
      int intVal;

      if (Serialization.GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMajorTickValues, out varVals))
      {
        _doc.SuppressedMajorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMajorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (Serialization.GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMinorTickValues, out varVals))
      {
        _doc.SuppressedMinorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMinorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (Serialization.GUIConversion.TryParseMultipleInt32(_view.SuppressMajorTicksByNumber, out intVals))
      {
        _doc.SuppressedMajorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMajorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (Serialization.GUIConversion.TryParseMultipleInt32(_view.SuppressMinorTicksByNumber, out intVals))
      {
        _doc.SuppressedMinorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMinorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (Serialization.GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMajorTickValues, out varVals))
      {
        _doc.AdditionalMajorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMajorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (Serialization.GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMinorTickValues, out varVals))
      {
        _doc.AdditionalMinorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMinorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if(Serialization.GUIConversion.IsInteger(_view.TargetNumberMajorTicks, out intVal))
        _doc.TargetNumberOfMajorTicks = intVal;
      else
        return false;

      if(Serialization.GUIConversion.IsInteger(_view.TargetNumberMinorTicks, out intVal))
        _doc.TargetNumberOfMinorTicks = intVal;
      else
        return false;

      _doc.SnapOrgToTick = (BoundaryTickSnapping)_snapTicksToOrg.FirstSelectedNode.Tag;
      _doc.SnapEndToTick = (BoundaryTickSnapping)_snapTicksToEnd.FirstSelectedNode.Tag;

			_originalDoc.CopyFrom(_doc);
			return true;
		}

		#endregion
	}
}