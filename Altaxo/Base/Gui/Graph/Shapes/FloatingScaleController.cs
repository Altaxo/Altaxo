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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Graph.Gdi;
using Altaxo.Graph;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IFloatingScaleView
	{
		PointD2D DocPosition { get; set; }
		int ScaleNumber { get; set; }
		FloatingScale.ScaleSegmentType ScaleSegmentType { get; set; }
		void InitializeTickSpacingTypes(SelectableListNodeList names);
		double ScaleSpanValue { get; set; }
		FloatingScaleSpanType ScaleSpanType { get; set; }
		object TickSpacingView { set; }
		object TitleFormatView { set; }
		object MajorLabelView { set; }
		object MinorLabelView { set; }

		Altaxo.Graph.Margin2D BackgroundPadding { get; set; }
		Altaxo.Graph.Gdi.Background.IBackgroundStyle SelectedBackground { get; set; }

		event Action TickSpacingTypeChanged;

	}

	[UserControllerForObject(typeof(FloatingScale), 110)]
	[ExpectedTypeOfView(typeof(IFloatingScaleView))]
	public class FloatingScaleController : MVCANControllerBase<FloatingScale, IFloatingScaleView>
	{
		IMVCANController _titleFormatController;
		IMVCANController _majorLabelController;
		IMVCANController _minorLabelController;

		protected TickSpacing _tempTickSpacing;
		protected SelectableListNodeList _tickSpacingTypes;
		protected IMVCAController _tickSpacingController;


		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_tempTickSpacing = (TickSpacing)_doc.TickSpacing.Clone();

				// Tick spacing types
				_tickSpacingTypes = new SelectableListNodeList();
				Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(TickSpacing));
				for (int i = 0; i < classes.Length; i++)
				{
					SelectableListNode node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _tempTickSpacing.GetType() == classes[i]);
					_tickSpacingTypes.Add(node);
				}

				_titleFormatController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisStyle }, typeof(IMVCANController), UseDocument.Directly);
				if (_titleFormatController is IMVCANDController)
					((IMVCANDController)_titleFormatController).MadeDirty += EhTitleFormatControllerMadeDirty;


				if(_doc.AxisStyle.ShowMajorLabels)
					_majorLabelController = (IXYAxisLabelStyleController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisStyle.MajorLabelStyle }, typeof(IXYAxisLabelStyleController), UseDocument.Directly);

				if (_doc.AxisStyle.ShowMinorLabels)
					_minorLabelController = (IXYAxisLabelStyleController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisStyle.MinorLabelStyle }, typeof(IXYAxisLabelStyleController), UseDocument.Directly);

			}
			if (null != _view)
			{
				_view.DocPosition = _doc.Position;
				_view.ScaleNumber = _doc.ScaleNumber;
				_view.ScaleSpanType = _doc.ScaleSpanType;
				_view.ScaleSpanValue = _doc.ScaleSpanValue;

				_view.ScaleSegmentType = _doc.ScaleType;
				_view.InitializeTickSpacingTypes(_tickSpacingTypes);

				_view.TitleFormatView = _titleFormatController.ViewObject;

				if (null != _majorLabelController)
					_view.MajorLabelView = _majorLabelController.ViewObject;

				if (null != _minorLabelController)
					_view.MinorLabelView = _minorLabelController.ViewObject;

				_view.BackgroundPadding = _doc.BackgroundPadding;
				_view.SelectedBackground = _doc.Background;

			}

			InitTickSpacingController(initData);

		}

		public override bool Apply()
		{
			_doc.Position = _view.DocPosition;
			_doc.ScaleNumber = _view.ScaleNumber;
			_doc.ScaleSpanType = _view.ScaleSpanType;
			_doc.ScaleSpanValue = _view.ScaleSpanValue;

			// Scale/ticks
			_doc.ScaleType = _view.ScaleSegmentType;
			if (null != _tickSpacingController && false == _tickSpacingController.Apply())
				return false;
			_doc.TickSpacing = _tempTickSpacing;

			// Title/format
			if (false == _titleFormatController.Apply())
				return false;

			// Major ticks
			if (null != _majorLabelController && false == _majorLabelController.Apply())
				return false;

			// Minor ticks
			if (null != _minorLabelController && false == _minorLabelController.Apply())
				return false;

			_doc.BackgroundPadding = _view.BackgroundPadding;
			_doc.Background = _view.SelectedBackground;

			if (!object.ReferenceEquals(_doc, _originalDoc))
				_originalDoc.CopyFrom(_doc);

			return true;
		}


		protected override void AttachView()
		{
			_view.TickSpacingTypeChanged += new Action(EhTickSpacingTypeChanged);
		}

		protected override void DetachView()
		{
			_view.TickSpacingTypeChanged -= new Action(EhTickSpacingTypeChanged);
		}

		public void InitTickSpacingController(bool bInit)
		{
			if (bInit)
			{
				if (_tempTickSpacing != null)
					_tickSpacingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempTickSpacing }, typeof(IMVCAController), UseDocument.Directly);
				else
					_tickSpacingController = null;
			}
			if (null != _view)
			{
				_view.TickSpacingView = null != _tickSpacingController ? _tickSpacingController.ViewObject : null;
			}
		}


		void EhTitleFormatControllerMadeDirty(IMVCANDController ctrl)
		{
			// Major labels
			if (_doc.AxisStyle.ShowMajorLabels && null == _majorLabelController)
			{
				_majorLabelController = (IXYAxisLabelStyleController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisStyle.MajorLabelStyle }, typeof(IXYAxisLabelStyleController), UseDocument.Directly);
				_view.MajorLabelView = _majorLabelController.ViewObject;
			}
			else if (!_doc.AxisStyle.ShowMajorLabels && null != _majorLabelController)
			{
				_majorLabelController = null;
				_view.MajorLabelView = null;
			}

			// Minor labels
			if (_doc.AxisStyle.ShowMinorLabels && null == _minorLabelController)
			{
				_minorLabelController = (IXYAxisLabelStyleController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisStyle.MinorLabelStyle }, typeof(IXYAxisLabelStyleController), UseDocument.Directly);
				_view.MinorLabelView = _minorLabelController.ViewObject;
			}
			else if (!_doc.AxisStyle.ShowMinorLabels && null != _minorLabelController)
			{
				_minorLabelController = null;
				_view.MinorLabelView = null;
			}
		}

		void EhTickSpacingTypeChanged()
		{
			var selNode = _tickSpacingTypes.FirstSelectedNode; // FirstSelectedNode can be null when the content of the box changes
			if (null == selNode)
				return;

			Type spaceType = (Type)_tickSpacingTypes.FirstSelectedNode.Tag;

			if (spaceType == _tempTickSpacing.GetType())
				return;

			_tempTickSpacing = (TickSpacing)Activator.CreateInstance(spaceType);
			InitTickSpacingController(true);
		}

	}
}
