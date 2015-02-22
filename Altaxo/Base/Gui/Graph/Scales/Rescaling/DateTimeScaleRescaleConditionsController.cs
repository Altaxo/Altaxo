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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
	public interface IDateTimeScaleRescaleConditionsView
	{
		SelectableListNodeList OrgRescaling { set; }

		SelectableListNodeList EndRescaling { set; }

		SelectableListNodeList OrgRelativeTo { set; }

		SelectableListNodeList EndRelativeTo { set; }

		DateTime OrgValueDT { set; get; }

		TimeSpan OrgValueTS { set; get; }

		DateTime EndValueDT { set; get; }

		TimeSpan EndValueTS { set; get; }

		/// <summary>
		/// If true, the TimeSpan box is shown for org; otherwise the DateTime box is shown.
		/// </summary>
		bool ShowOrgTS { set; }

		/// <summary>
		/// If true, the TimeSpan box is shown for org; otherwise the DateTime box is shown.
		/// </summary>
		bool ShowEndTS { set; }

		event Action OrgValueChanged;

		event Action EndValueChanged;

		event Action OrgRelativeToChanged;

		event Action EndRelativeToChanged;
	}

	[ExpectedTypeOfView(typeof(IDateTimeScaleRescaleConditionsView))]
	[UserControllerForObject(typeof(DateTimeScaleRescaleConditions))]
	public class DateTimeScaleRescaleConditionsController : MVCANControllerEditOriginalDocBase<DateTimeScaleRescaleConditions, IDateTimeScaleRescaleConditionsView>
	{
		private SelectableListNodeList _orgRescalingChoices;
		private SelectableListNodeList _endRescalingChoices;

		private SelectableListNodeList _orgRelativeToChoices;
		private SelectableListNodeList _endRelativeToChoices;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_orgRescalingChoices = LinearScaleRescaleConditionsController.CreateListNodeList(_doc.OrgRescaling);
				_endRescalingChoices = LinearScaleRescaleConditionsController.CreateListNodeList(_doc.EndRescaling);

				_orgRelativeToChoices = new SelectableListNodeList(_doc.OrgRelativeTo);
				_endRelativeToChoices = new SelectableListNodeList(_doc.EndRelativeTo);
			}

			if (null != _view)
			{
				_view.OrgRescaling = _orgRescalingChoices;
				_view.EndRescaling = _endRescalingChoices;
				_view.OrgRelativeTo = _orgRelativeToChoices;
				_view.EndRelativeTo = _endRelativeToChoices;

				var org = GetOrgValueToShow();
				var end = GetEndValueToShow();

				if (org is DateTime)
				{
					_view.OrgValueDT = (DateTime)org;
					_view.ShowOrgTS = false;
				}
				else
				{
					_view.OrgValueTS = (TimeSpan)org;
					_view.ShowOrgTS = true;
				}

				if (end is DateTime)
				{
					_view.EndValueDT = (DateTime)end;
					_view.ShowEndTS = false;
				}
				else
				{
					_view.EndValueTS = (TimeSpan)end;
					_view.ShowEndTS = true;
				}
			}
		}

		public override bool Apply(bool disposeController)
		{
			var orgRescaling = (BoundaryRescaling)_orgRescalingChoices.FirstSelectedNode.Tag;
			var endRescaling = (BoundaryRescaling)_endRescalingChoices.FirstSelectedNode.Tag;

			var orgRelativeTo = (BoundariesRelativeTo)_orgRelativeToChoices.FirstSelectedNode.Tag;
			var endRelativeTo = (BoundariesRelativeTo)_endRelativeToChoices.FirstSelectedNode.Tag;

			long orgValue, endValue;
			DateTimeKind orgKind, endKind;
			if (orgRelativeTo == BoundariesRelativeTo.Absolute)
			{
				orgValue = _view.OrgValueDT.Ticks;
				orgKind = _view.OrgValueDT.Kind;
			}
			else
			{
				orgValue = _view.OrgValueTS.Ticks;
				orgKind = DateTimeKind.Utc;
			}
			if (endRelativeTo == BoundariesRelativeTo.Absolute)
			{
				endValue = _view.EndValueDT.Ticks;
				endKind = _view.EndValueDT.Kind;
			}
			else
			{
				endValue = _view.EndValueTS.Ticks;
				endKind = DateTimeKind.Utc;
			}

			_doc.SetUserParameters(orgRescaling, orgRelativeTo, orgValue, orgKind, endRescaling, endRelativeTo, endValue, endKind);

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.OrgValueChanged += EhOrgValueChanged;
			_view.EndValueChanged += EhEndValueChanged;

			_view.OrgRelativeToChanged += EhOrgRelativeToChanged;
			_view.EndRelativeToChanged += EhEndRelativeToChanged;
		}

		protected override void DetachView()
		{
			_view.OrgValueChanged -= EhOrgValueChanged;
			_view.EndValueChanged -= EhEndValueChanged;
			_view.OrgRelativeToChanged -= EhOrgRelativeToChanged;
			_view.EndRelativeToChanged -= EhEndRelativeToChanged;
			base.DetachView();
		}

		private object GetOrgValueToShow()
		{
			if (_doc.OrgRescaling == BoundaryRescaling.Auto)
			{
				DateTime orgValue = _doc.ResultingOrg;
				if (_doc.ParentObject is DateTimeScale)
					orgValue = ((DateTimeScale)_doc.ParentObject).Org;

				switch (_doc.EndRelativeTo)
				{
					case BoundariesRelativeTo.Absolute:
						return orgValue;

					case BoundariesRelativeTo.RelativeToDataBoundsOrg:
						return orgValue - _doc.DataBoundsOrg;

					case BoundariesRelativeTo.RelativeToDataBoundsEnd:
						return orgValue - _doc.DataBoundsEnd;

					case BoundariesRelativeTo.RelativeToDataBoundsMean:
						return orgValue - (_doc.DataBoundsOrg + TimeSpan.FromTicks((_doc.DataBoundsEnd - _doc.DataBoundsOrg).Ticks / 2));

					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				if (_doc.OrgRelativeTo == BoundariesRelativeTo.Absolute)

					return new DateTime(_doc.UserProvidedOrgValue, _doc.UserProvidedOrgKind);
				else
					return TimeSpan.FromTicks(_doc.UserProvidedOrgValue);
			}
		}

		private object GetEndValueToShow()
		{
			if (_doc.EndRescaling == BoundaryRescaling.Auto)
			{
				DateTime endValue = _doc.ResultingEnd;
				if (_doc.ParentObject is DateTimeScale)
					endValue = ((DateTimeScale)_doc.ParentObject).End;

				switch (_doc.EndRelativeTo)
				{
					case BoundariesRelativeTo.Absolute:
						return endValue;

					case BoundariesRelativeTo.RelativeToDataBoundsEnd:
						return TimeSpan.FromTicks(0);

					case BoundariesRelativeTo.RelativeToDataBoundsOrg:
						return (endValue - _doc.DataBoundsOrg);

					case BoundariesRelativeTo.RelativeToDataBoundsMean:
						return endValue - (_doc.DataBoundsOrg + TimeSpan.FromTicks((_doc.DataBoundsEnd - _doc.DataBoundsOrg).Ticks / 2));

					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				if (_doc.EndRelativeTo == BoundariesRelativeTo.Absolute)
					return new DateTime(_doc.UserProvidedEndValue, _doc.UserProvidedEndKind);
				else
					return TimeSpan.FromTicks(_doc.UserProvidedEndValue);
			}
		}

		private void EhOrgValueChanged()
		{
			var orgRescaling = (BoundaryRescaling)_orgRescalingChoices.FirstSelectedNode.Tag;
			if (orgRescaling == BoundaryRescaling.Auto)
			{
				_orgRescalingChoices.ClearSelectionsAll();

				foreach (var node in _orgRescalingChoices)
				{
					if (BoundaryRescaling.AutoTempFixed == (BoundaryRescaling)node.Tag)
					{
						node.IsSelected = true;
						break;
					}
				}

				if (null != _view)
					_view.OrgRescaling = _orgRescalingChoices;
			}
		}

		private void EhEndValueChanged()
		{
			var endRescaling = (BoundaryRescaling)_endRescalingChoices.FirstSelectedNode.Tag;
			if (endRescaling == BoundaryRescaling.Auto)
			{
				_endRescalingChoices.ClearSelectionsAll();

				foreach (var node in _endRescalingChoices)
				{
					if (BoundaryRescaling.AutoTempFixed == (BoundaryRescaling)node.Tag)
					{
						node.IsSelected = true;
						break;
					}
				}

				if (null != _view)
					_view.EndRescaling = _endRescalingChoices;
			}
		}

		private void EhOrgRelativeToChanged()
		{
			if (null == _orgRelativeToChoices.FirstSelectedNode)
				return;

			var orgRelativeTo = (BoundariesRelativeTo)_orgRelativeToChoices.FirstSelectedNode.Tag;

			if (orgRelativeTo == BoundariesRelativeTo.Absolute)
				_view.ShowOrgTS = false;
			else
				_view.ShowOrgTS = true;
		}

		private void EhEndRelativeToChanged()
		{
			if (null == _orgRelativeToChoices.FirstSelectedNode)
				return;

			var endRelativeTo = (BoundariesRelativeTo)_endRelativeToChoices.FirstSelectedNode.Tag;

			if (endRelativeTo == BoundariesRelativeTo.Absolute)
				_view.ShowEndTS = false;
			else
				_view.ShowEndTS = true;
		}
	}
}