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
	/// <summary>
	/// Summary description for DateTimeAxisRescaleController.
	/// </summary>
	[UserControllerForObject(typeof(DateTimeAxisRescaleConditions))]
	[ExpectedTypeOfView(typeof(IOrgEndSpanView))]
	public class DateTimeAxisRescaleController
		:
		MVCANControllerEditOriginalDocBase<DateTimeAxisRescaleConditions, IOrgEndSpanView>,
		IOrgEndSpanViewEventReceiver
	{
		protected DateTimeScale _scale;

		protected DateTime _org;
		protected DateTime _end;
		protected TimeSpan _span;

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
			if (!base.InitializeDocument(args))
				return false;

			if (args != null && args.Length >= 2 && args[1] is DateTimeScale)
				_scale = (DateTimeScale)args[1];

			return true;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				if (null == _scale && _doc.ParentObject is DateTimeScale)
				{
					_scale = (DateTimeScale)_doc.ParentObject;
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
			{
				InitView();
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.SetOrgEndSpan(_orgRescaling, _org, _endRescaling, _end, _spanRescaling, _span);

			if (null != _scale)
			{
				// if the user changed org or end, he maybe want to set the scale temporarily to the chosen values
				if (_orgRescaling == BoundaryRescaling.Auto && _endRescaling == BoundaryRescaling.Auto && (_orgChanged || _endChanged))
					_scale.SetScaleOrgEnd(_org, _end);
				else
					_scale.Rescale();
			}

			_orgChanged = _endChanged = false;

			if (!disposeController)
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
		private static readonly string[] _choices = { "Auto", "Fixed", "<=", ">=", "Use Span" };

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
			if (!GUIConversion.IsDateTime(txt))
				return true;

			GUIConversion.IsDateTime(txt, out _org);
			_orgChanged = true;
			return false;
		}

		public virtual bool EhValue2Changed(string txt)
		{
			if (!GUIConversion.IsDateTime(txt))
				return true;

			GUIConversion.IsDateTime(txt, out _end);
			_endChanged = true;
			return false;
		}

		public virtual bool EhValue3Changed(string txt)
		{
			if (!GUIConversion.IsTimeSpan(txt))
				return true;

			GUIConversion.IsTimeSpan(txt, out _span);
			return false;
		}

		public virtual bool EhValue4Changed(string txt)
		{
			return false;
		}

		public virtual bool EhValue5Changed(string txt)
		{
			return false;
		}

		#endregion IOrgEndSpanControlEventReceiver Members
	}
}