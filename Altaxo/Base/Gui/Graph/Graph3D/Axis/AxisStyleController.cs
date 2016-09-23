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

using Altaxo.Graph.Graph3D.Axis;
using Altaxo.Gui.Graph.Scales.Ticks;
using System;

namespace Altaxo.Gui.Graph.Graph3D.Axis
{
	#region Interfaces

	public interface IAxisStyleView
	{
		bool ShowAxisLine { get; set; }

		bool ShowMajorLabels { get; set; }

		bool ShowMinorLabels { get; set; }

		bool ShowCustomTickSpacing { get; set; }

		event Action ShowAxisLineChanged;

		event Action ShowMajorLabelsChanged;

		event Action ShowMinorLabelsChanged;

		event Action ShowCustomTickSpacingChanged;

		event Action EditTitle;

		object LineStyleView { set; }

		object TickSpacingView { set; }

		string AxisTitle { get; set; }
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for TitleFormatLayerController.
	/// </summary>
	[UserControllerForObject(typeof(AxisStyle), 90)]
	[ExpectedTypeOfView(typeof(IAxisStyleView))]
	public class AxisStyleController : MVCANDControllerEditOriginalDocBase<AxisStyle, IAxisStyleView>
	{
		protected IMVCAController _axisLineStyleController;

		protected TickSpacingController _tickSpacingController;

		private Altaxo.Main.Properties.IReadOnlyPropertyBag _context;

		public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_axisLineStyleController, () => _axisLineStyleController = null);
			yield return new ControllerAndSetNullMethod(_tickSpacingController, () => _tickSpacingController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_context = _doc.GetPropertyContext();

				if (_doc.AxisLineStyle != null)
				{
					_axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisLineStyle }, typeof(IMVCAController), UseDocument.Directly);
				}
				else
				{
					_axisLineStyleController = null;
				}

				if (_doc.TickSpacing != null)
				{
					_tickSpacingController = new TickSpacingController() { UseDocumentCopy = UseDocument.Directly };
					_tickSpacingController.InitializeDocument(_doc.TickSpacing);
					Current.Gui.FindAndAttachControlTo(_tickSpacingController);
				}
			}

			if (_view != null)
			{
				_view.AxisTitle = _doc.TitleText;
				_view.ShowAxisLine = _doc.IsAxisLineEnabled;
				_view.ShowMajorLabels = _doc.AreMajorLabelsEnabled;
				_view.ShowMinorLabels = _doc.AreMinorLabelsEnabled;
				_view.LineStyleView = _axisLineStyleController == null ? null : _axisLineStyleController.ViewObject;
				_view.ShowCustomTickSpacing = _doc.TickSpacing != null;
				_view.TickSpacingView = _tickSpacingController != null ? _tickSpacingController.ViewObject : null;
			}
		}

		public override bool Apply(bool disposeController)
		{
			// read axis title
			_doc.TitleText = _view.AxisTitle;

			if (null != _axisLineStyleController)
			{
				if (!_axisLineStyleController.Apply(disposeController))
					return false;
				else
					_doc.AxisLineStyle = (AxisLineStyle)_axisLineStyleController.ModelObject;
			}

			if (_view.ShowMajorLabels)
				_doc.ShowMajorLabels(_context);
			else
				_doc.HideMajorLabels();

			if (_view.ShowMinorLabels)
				_doc.ShowMinorLabels(_context);
			else
				_doc.HideMinorLabels();

			if (_tickSpacingController != null && !_tickSpacingController.Apply(disposeController))
				return false;
			if (_view.ShowCustomTickSpacing && null != _tickSpacingController)
				_doc.TickSpacing = (Altaxo.Graph.Scales.Ticks.TickSpacing)_tickSpacingController.ModelObject;

			return ApplyEnd(true, disposeController); // all ok
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.ShowAxisLineChanged += EhShowAxisLineChanged;
			_view.ShowMajorLabelsChanged += EhShowMajorLabelsChanged;
			_view.ShowMinorLabelsChanged += EhShowMinorLabelsChanged;
			_view.ShowCustomTickSpacingChanged += EhShowCustomTickSpacingChanged;
			_view.EditTitle += EhEditAxisTitle;
		}

		protected override void DetachView()
		{
			_view.ShowAxisLineChanged -= EhShowAxisLineChanged;
			_view.ShowMajorLabelsChanged -= EhShowMajorLabelsChanged;
			_view.ShowMinorLabelsChanged -= EhShowMinorLabelsChanged;
			_view.ShowCustomTickSpacingChanged -= EhShowCustomTickSpacingChanged;
			_view.EditTitle -= EhEditAxisTitle;

			base.DetachView();
		}

		/// <summary>Can be called by an external controller if the state of either the major or the minor label has been changed by an external controller. This will update
		/// the state of the checkboxes for major and minor labels in the view that is controlled by this controller.</summary>
		public void AnnounceExternalChangeOfMajorOrMinorLabelState()
		{
			if (null != _view)
			{
				_view.ShowMajorLabels = _doc.AreMajorLabelsEnabled;
				_view.ShowMinorLabels = _doc.AreMinorLabelsEnabled;
			}
		}

		private void EhShowCustomTickSpacingChanged()
		{
			var isShown = _view.ShowCustomTickSpacing;

			if (isShown)
			{
				if (_tickSpacingController == null)
				{
					if (_doc.TickSpacing == null)
					{
						_doc.TickSpacing = new Altaxo.Graph.Scales.Ticks.LinearTickSpacing();
					}
					_tickSpacingController = new TickSpacingController() { UseDocumentCopy = UseDocument.Directly };
					_tickSpacingController.InitializeDocument(_doc.TickSpacing);
					Current.Gui.FindAndAttachControlTo(_tickSpacingController);
					if (null != _view)
						_view.TickSpacingView = _tickSpacingController.ViewObject;
				}
			}
			else
			{
				_doc.TickSpacing = null;
				_view.TickSpacingView = null;
				_tickSpacingController = null;
			}
		}

		private void EhShowAxisLineChanged()
		{
			var oldValue = _doc.IsAxisLineEnabled;
			if (_view.ShowAxisLine && null == _doc.AxisLineStyle)
			{
				_doc.ShowAxisLine(_context);
				this._axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisLineStyle }, typeof(IMVCAController), UseDocument.Directly);
				_view.LineStyleView = _axisLineStyleController.ViewObject;
			}
			if (oldValue != _doc.IsAxisLineEnabled)
				OnMadeDirty();
		}

		private void EhShowMajorLabelsChanged()
		{
			var oldValue = _doc.AreMajorLabelsEnabled;
			var newValue = _view.ShowMajorLabels;

			if (oldValue != newValue)
			{
				if (newValue)
					_doc.ShowMajorLabels(_context);
				else
					_doc.HideMajorLabels();
				OnMadeDirty();
			}
		}

		private void EhShowMinorLabelsChanged()
		{
			var oldValue = _doc.AreMinorLabelsEnabled;
			var newValue = _view.ShowMinorLabels;

			if (oldValue != newValue)
			{
				if (newValue)
					_doc.ShowMinorLabels(_context);
				else
					_doc.HideMinorLabels();

				OnMadeDirty();
			}
		}

		private void EhEditAxisTitle()
		{
			var title = _doc.Title;
			if (Current.Gui.ShowDialog(ref title, "Edit title", true))
			{
				_doc.Title = title;
				_view.AxisTitle = _doc.TitleText;
			}
		}
	}
}