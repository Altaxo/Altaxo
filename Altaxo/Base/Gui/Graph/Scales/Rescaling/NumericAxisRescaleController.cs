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

using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Serialization;
using System;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
	#region Interfaces

	public interface IOrgEndSpanView
	{
		IOrgEndSpanViewEventReceiver Controller { get; set; }

		void SetLabel1(string txt);

		void SetLabel2(string txt);

		void SetLabel3(string txt);

		void SetChoice1(string[] choices, int selected);

		void SetChoice2(string[] choices, int selected);

		void SetChoice3(string[] choices, int selected);

		void SetValue1(string txt);

		void SetValue2(string txt);

		void SetValue3(string txt);

		void EnableChoice1(bool enable);

		void EnableChoice2(bool enable);

		void EnableChoice3(bool enable);

		void EnableValue1(bool enable);

		void EnableValue2(bool enable);

		void EnableValue3(bool enable);
	}

	public interface IOrgEndSpanViewEventReceiver
	{
		void EhChoice1Changed(string txt, int selected);

		void EhChoice2Changed(string txt, int selected);

		void EhChoice3Changed(string txt, int selected);

		bool EhValue1Changed(string txt);

		bool EhValue2Changed(string txt);

		bool EhValue3Changed(string txt);

		bool EhValue4Changed(string txt);

		bool EhValue5Changed(string txt);
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for NumericAxisRescaleController.
	/// </summary>
	[UserControllerForObject(typeof(NumericAxisRescaleConditions))]
	[ExpectedTypeOfView(typeof(IOrgEndSpanView))]
	public class NumericAxisRescaleController
		:
		MVCANControllerEditOriginalDocBase<NumericAxisRescaleConditions, IOrgEndSpanView>,
		IOrgEndSpanViewEventReceiver
	{
		protected NumericalScale _scale;

		protected double _org;
		protected double _end;
		protected double _span;

		protected int? _minorTicks;
		protected double? _majorTick;

		protected BoundaryRescaling _orgRescaling;
		protected BoundaryRescaling _endRescaling;
		protected BoundaryRescaling _spanRescaling;

		protected bool _orgChanged;
		protected bool _endChanged;
		protected bool _spanChanged;

		public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override bool InitializeDocument(params object[] args)
		{
			if (args != null && args.Length >= 2 && args[1] is NumericalScale)
				_scale = (NumericalScale)args[1];

			return base.InitializeDocument(args);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				if (null == _scale && _doc.ParentObject is NumericalScale)
				{
					_scale = (NumericalScale)_doc.ParentObject;
				}

				_orgRescaling = _doc.OrgRescaling;
				_endRescaling = _doc.EndRescaling;
				_spanRescaling = _doc.SpanRescaling;

				_org = _doc.Org;
				_end = _doc.End;
				_span = _doc.Span;

				if (_scale != null)
				{
					if (_orgRescaling == BoundaryRescaling.Auto)
						_org = _scale.Org;
					if (_endRescaling == BoundaryRescaling.Auto)
						_end = _scale.End;
					if (_spanRescaling == BoundaryRescaling.Auto)
						_span = _scale.End - _scale.Org;
				}
			}

			if (null != _view)
				InitView();
		}

		public override bool Apply(bool disposeController)
		{
			if (!(_end > _org))
			{
				Current.Gui.ErrorMessageBox("Please note: End must be greater than org.");
				return false;
			}

			_doc.SetOrgEndSpan(_orgRescaling, _org, _endRescaling, _end, _spanRescaling, _span, _minorTicks, _majorTick);

			if (null != _scale)
			{
				// if the user changed org or end, he maybe want to set the scale temporarily to the chosen values
				if (_orgRescaling == BoundaryRescaling.Auto && _endRescaling == BoundaryRescaling.Auto && (_orgChanged || _endChanged))
					_scale.SetScaleOrgEnd(_org, _end);
				else
					_scale.Rescale();
			}

			_orgChanged = _endChanged = false;

			Initialize(true);

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			_view.Controller = this;
		}

		protected override void DetachView()
		{
			_view.Controller = null;
		}

		/// <summary>
		/// Has to match the indices of BoundaryRescaling
		/// </summary>
		private static readonly string[] _choices = { "Auto", "Fixed", "<=", ">=" };

		protected virtual void InitView()
		{
			_view.SetLabel1("Org:");
			_view.SetLabel2("End:");
			_view.SetLabel3("Span:");

			_view.SetChoice1(_choices, (int)_orgRescaling);
			_view.SetChoice2(_choices, (int)_endRescaling);
			_view.SetChoice3(_choices, (int)_spanRescaling);

			_view.SetValue1(GUIConversion.ToString(_org));
			_view.SetValue2(GUIConversion.ToString(_end));
			_view.SetValue3(GUIConversion.ToString(_span));

			SetEnableState();
		}

		protected virtual void SetEnableState()
		{
			bool enableSpan = _spanRescaling != BoundaryRescaling.Auto;
			_view.EnableChoice1(!enableSpan);
			_view.EnableChoice2(!enableSpan);
			_view.EnableChoice3(true);

			_view.EnableValue1(!enableSpan);
			_view.EnableValue2(!enableSpan);
			_view.EnableValue3(enableSpan);
		}

		#region IOrgEndSpanControlEventReceiver Members

		public void EhChoice1Changed(string txt, int selected)
		{
			_orgRescaling = (BoundaryRescaling)selected;
		}

		public void EhChoice2Changed(string txt, int selected)
		{
			_endRescaling = (BoundaryRescaling)selected;
		}

		public void EhChoice3Changed(string txt, int selected)
		{
			_spanRescaling = (BoundaryRescaling)selected;
			SetEnableState();
		}

		public virtual bool EhValue1Changed(string txt)
		{
			if (!GUIConversion.IsDouble(txt))
				return true;

			GUIConversion.IsDouble(txt, out _org);
			_orgChanged = true;
			return false;
		}

		public virtual bool EhValue2Changed(string txt)
		{
			if (!GUIConversion.IsDouble(txt))
				return true;

			GUIConversion.IsDouble(txt, out _end);
			_endChanged = true;
			return false;
		}

		public virtual bool EhValue3Changed(string txt)
		{
			if (!GUIConversion.IsDouble(txt))
				return true;

			GUIConversion.IsDouble(txt, out _span);
			return false;
		}

		public virtual bool EhValue4Changed(string txt)
		{
			int val;

			if (string.IsNullOrEmpty(txt))
			{
				_minorTicks = null;
				return false;
			}
			else if (!GUIConversion.IsInteger(txt, out val))
				return true;

			_minorTicks = val;
			return false;
		}

		public virtual bool EhValue5Changed(string txt)
		{
			double val;

			if (string.IsNullOrEmpty(txt))
			{
				_majorTick = null;
				return false;
			}
			else if (!GUIConversion.IsDouble(txt, out val))
				return true;

			_majorTick = val;
			return false;
		}

		#endregion IOrgEndSpanControlEventReceiver Members
	}
}