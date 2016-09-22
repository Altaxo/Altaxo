#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing.ColorManagement;
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing.D3D;
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Gui.Drawing.D3D.LineCaps;
using Altaxo.Gui.Drawing.DashPatternManagement;
using Altaxo.Gui.Graph.Graph3D.Material;
using Altaxo.Gui.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Graph3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Drawing.D3D
{
	public class PenControlsGlue : FrameworkElement
	{
		private bool _userChangedAbsLineStartCapSize;
		private bool _userChangedAbsLineEndCapSize;

		private bool _userChangedRelLineStartCapSize;
		private bool _userChangedRelLineEndCapSize;

		private bool _userChangedAbsDashStartCapSize;
		private bool _userChangedAbsDashEndCapSize;

		private bool _userChangedRelDashStartCapSize;
		private bool _userChangedRelDashEndCapSize;

		private bool _isAllPropertiesGlue;

		public PenControlsGlue()
			: this(false)
		{
		}

		public PenControlsGlue(bool isAllPropertiesGlue)
		{
			this.InternalSelectedPen = new PenX3D(ColorSetManager.Instance.BuiltinDarkPlotColors[0], 1);
			_isAllPropertiesGlue = isAllPropertiesGlue;
		}

		#region Pen

		private PenX3D _pen;

		/// <summary>
		/// Gets or sets the pen. The pen you get is a clone of the pen that is used internally. Similarly, when setting the pen, a clone is created, so that the pen
		/// can be used internally, without interfering with external functions that changes the pen.
		/// </summary>
		/// <value>
		/// The pen.
		/// </value>
		public PenX3D Pen
		{
			get
			{
				return _pen; // Pen is not immutable. Before giving it out, make a copy, so an external program can meddle with this without disturbing us
			}
			set
			{
				if (null == value)
					throw new NotImplementedException("Pen is null");
				InternalSelectedPen = value; // Pen is not immutable. Before changing it here in this control, make a copy, so an external program can change the old pen without interference
			}
		}

		/// <summary>
		/// Gets or sets the selected pen internally, <b>but without cloning it. Use this function only internally.</b>
		/// </summary>
		/// <value>
		/// The selected pen.
		/// </value>
		protected PenX3D InternalSelectedPen
		{
			get
			{
				return _pen;
			}
			set
			{
				if (null == value)
					throw new NotImplementedException("Pen is null");

				_pen = value;

				InitControlProperties();
			}
		}

		private void InitControlProperties()
		{
			if (null != CbBrush) CbBrush.SelectedMaterial = _pen.Material;
			if (null != CbLineThickness1) CbLineThickness1.SelectedQuantityAsValueInPoints = _pen.Thickness1;
			if (null != CbLineThickness2) CbLineThickness2.SelectedQuantityAsValueInPoints = _pen.Thickness2;

			if (null != CbCrossSection) InitializeCrossSectionCombobox();

			if (null != CbDashStyle) CbDashStyle.SelectedDashStyle = _pen.DashPattern;
			if (null != CbDashPattern) CbDashPattern.SelectedItem = _pen.DashPattern;

			if (null != CbLineStartCap) CbLineStartCap.SelectedLineCap = _pen.LineStartCap;
			if (null != CbLineStartCapAbsSize) CbLineStartCapAbsSize.SelectedQuantityAsValueInPoints = null != _pen.LineStartCap ? _pen.LineStartCap.MinimumAbsoluteSizePt : 0;
			if (null != CbLineStartCapRelSize) CbLineStartCapRelSize.SelectedQuantityAsValueInSIUnits = null != _pen.LineStartCap ? _pen.LineStartCap.MinimumRelativeSize : 0;

			if (null != CbLineEndCap) CbLineEndCap.SelectedLineCap = _pen.LineEndCap;
			if (null != CbLineEndCapAbsSize) CbLineEndCapAbsSize.SelectedQuantityAsValueInPoints = null != _pen.LineEndCap ? _pen.LineEndCap.MinimumAbsoluteSizePt : 0;
			if (null != CbLineEndCapRelSize) CbLineEndCapRelSize.SelectedQuantityAsValueInSIUnits = null != _pen.LineEndCap ? _pen.LineEndCap.MinimumRelativeSize : 0;

			if (null != CbDashStartCap) CbDashStartCap.SelectedLineCap = _pen.DashStartCap;
			if (null != CbDashStartCapAbsSize) CbDashStartCapAbsSize.SelectedQuantityAsValueInPoints = null != _pen.DashStartCap ? _pen.DashStartCap.MinimumAbsoluteSizePt : 0;
			if (null != CbDashStartCapRelSize) CbDashStartCapRelSize.SelectedQuantityAsValueInSIUnits = null != _pen.DashStartCap ? _pen.DashStartCap.MinimumRelativeSize : 0;

			if (null != CbDashEndCap) CbDashEndCap.SelectedLineCap = _pen.DashEndCap;
			if (null != CbDashEndCapAbsSize) CbDashEndCapAbsSize.SelectedQuantityAsValueInPoints = null != _pen.DashEndCap ? _pen.DashEndCap.MinimumAbsoluteSizePt : 0;
			if (null != CbDashEndCapRelSize) CbDashEndCapRelSize.SelectedQuantityAsValueInSIUnits = null != _pen.DashEndCap ? _pen.DashEndCap.MinimumRelativeSize : 0;

			if (null != CbLineJoin) CbLineJoin.SelectedLineJoin = _pen.LineJoin;
			if (null != CbMiterLimit) CbMiterLimit.SelectedQuantityInSIUnits = _pen.MiterLimit;

			_userChangedAbsLineStartCapSize = false;
			_userChangedAbsLineEndCapSize = false;

			_userChangedRelLineStartCapSize = false;
			_userChangedRelLineEndCapSize = false;
		}

		public event EventHandler PenChanged;

		protected virtual void OnPenChanged()
		{
			if (PenChanged != null)
				PenChanged(this, EventArgs.Empty);

			UpdatePreviewPanel();
		}

		private WeakEventHandler _weakPenChangedHandler;

		private void EhPenChanged(object sender, EventArgs e)
		{
			OnPenChanged();
		}

		#endregion Pen

		#region Brush

		private bool _showPlotColorsOnly;
		private MaterialComboBox _cbBrush;

		public MaterialComboBox CbBrush
		{
			get { return _cbBrush; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MaterialComboBox.SelectedMaterialProperty, typeof(MaterialComboBox));

				if (_cbBrush != null)
					dpd.RemoveValueChanged(_cbBrush, EhBrush_SelectionChangeCommitted);

				_cbBrush = value;
				if (_cbBrush != null && null != _pen)
				{
					_cbBrush.ShowPlotColorsOnly = _showPlotColorsOnly;
					_cbBrush.SelectedMaterial = _pen.Material;
				}

				if (_cbBrush != null)
				{
					dpd.AddValueChanged(_cbBrush, EhBrush_SelectionChangeCommitted);
					if (!_isAllPropertiesGlue)
					{
						var menuItem = new MenuItem();
						menuItem.Header = "Custom Pen ...";
						menuItem.Click += EhShowCustomPenDialog;
						_cbBrush.ContextMenu.Items.Insert(0, menuItem);
					}
				}
			}
		}

		public bool ShowPlotColorsOnly
		{
			get
			{
				return _showPlotColorsOnly;
			}
			set
			{
				_showPlotColorsOnly = value;
				if (null != _cbBrush)
					_cbBrush.ShowPlotColorsOnly = _showPlotColorsOnly;
			}
		}

		private void EhBrush_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var oldPen = _pen;
				_pen = _pen.WithMaterial(_cbBrush.SelectedMaterial);
				if (!object.ReferenceEquals(_pen, oldPen))
					OnPenChanged();
			}
		}

		#endregion Brush

		#region Cross section

		private ComboBox _cbCrossSection;
		private SelectableListNodeList _crossSectionChoices;

		public ComboBox CbCrossSection
		{
			get { return _cbCrossSection; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ComboBox.SelectedValueProperty, typeof(ComboBox));

				if (_cbCrossSection != null)
					dpd.RemoveValueChanged(_cbCrossSection, EhCrossSection_SelectionChangeCommitted);

				_cbCrossSection = value;
				if (_pen != null && _cbCrossSection != null)
					InitializeCrossSectionCombobox();

				if (_cbCrossSection != null)
					dpd.AddValueChanged(_cbCrossSection, EhCrossSection_SelectionChangeCommitted);
			}
		}

		public void InitializeCrossSectionCombobox()
		{
			_crossSectionChoices = new SelectableListNodeList();
			var selectableTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ICrossSectionOfLine));

			foreach (var t in selectableTypes)
			{
				_crossSectionChoices.Add(new SelectableListNode(t.Name, t, t == _pen.CrossSection.GetType()));
			}

			GuiHelper.Initialize(_cbCrossSection, _crossSectionChoices);
		}

		private void EhCrossSection_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var node = (SelectableListNode)_cbCrossSection.SelectedValue;
				if (null != node)
				{
					var type = (Type)node.Tag;
					var crossSection = (ICrossSectionOfLine)Activator.CreateInstance(type);
					crossSection = crossSection.WithSize(_pen.Thickness1, _pen.Thickness2);
					var oldPen = _pen;
					_pen = _pen.WithCrossSection(crossSection);
					if (!object.ReferenceEquals(_pen, oldPen))
						OnPenChanged();
				}
			}
		}

		#endregion Cross section

		#region Dash pattern

		private DashPatternComboBox _cbDashPattern;

		public DashPatternComboBox CbDashPattern
		{
			get { return _cbDashPattern; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DashPatternComboBox.SelectedItemProperty, typeof(DashPatternComboBox));

				if (_cbDashPattern != null)
					dpd.RemoveValueChanged(_cbDashPattern, EhDashPattern_SelectionChangeCommitted);

				_cbDashPattern = value;
				if (_pen != null && _cbDashPattern != null)
					_cbDashPattern.SelectedItem = _pen.DashPattern;

				if (_cbDashPattern != null)
					dpd.AddValueChanged(_cbDashPattern, EhDashPattern_SelectionChangeCommitted);
			}
		}

		private void EhDashPattern_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var oldPen = _pen;
				_pen = _pen.WithDashPattern(_cbDashPattern.SelectedItem);
				if (!object.ReferenceEquals(_pen, oldPen))
					OnPenChanged();
			}
		}

		#endregion Dash pattern

		#region Dash style

		private DashStyleComboBox _cbDashStyle;

		public DashStyleComboBox CbDashStyle
		{
			get { return _cbDashStyle; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DashStyleComboBox.SelectedDashStyleProperty, typeof(DashStyleComboBox));

				if (_cbDashStyle != null)
					dpd.RemoveValueChanged(_cbDashStyle, EhDashStyle_SelectionChangeCommitted);

				_cbDashStyle = value;
				if (_pen != null && _cbDashStyle != null)
					_cbDashStyle.SelectedDashStyle = _pen.DashPattern;

				if (_cbDashStyle != null)
					dpd.AddValueChanged(_cbDashStyle, EhDashStyle_SelectionChangeCommitted);
			}
		}

		private void EhDashStyle_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var oldPen = _pen;
				_pen = _pen.WithDashPattern(_cbDashStyle.SelectedDashStyle);
				if (!object.ReferenceEquals(_pen, oldPen))
					OnPenChanged();
			}
		}

		#endregion Dash style

		/*

		private DashCapComboBox _cbDashCap;

		public DashCapComboBox CbDashCap
		{
			get { return _cbDashCap; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DashCapComboBox.SelectedDashCapProperty, typeof(DashCapComboBox));

				if (_cbDashCap != null)
					dpd.RemoveValueChanged(_cbDashCap, EhDashCap_SelectionChangeCommitted);

				_cbDashCap = value;
				if (_pen != null && _cbDashCap != null)
					_cbDashCap.SelectedDashCap = _pen.DashCap;

				if (_cbDashCap != null)
					dpd.AddValueChanged(_cbDashCap, EhDashCap_SelectionChangeCommitted);
			}
		}

		private void EhDashCap_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				_pen.DashCap = _cbDashCap.SelectedDashCap;
				OnPenChanged();
			}
		}

		#endregion Dash

	*/

		#region Thickness1

		private Common.Drawing.LineThicknessComboBox _cbThickness1;

		public Common.Drawing.LineThicknessComboBox CbLineThickness1
		{
			get { return _cbThickness1; }
			set
			{
				if (_cbThickness1 != null)
					_cbThickness1.SelectedQuantityChanged -= EhThickness1_ChoiceChanged;

				_cbThickness1 = value;
				if (_pen != null && _cbThickness1 != null)
					_cbThickness1.SelectedQuantityAsValueInPoints = _pen.Thickness1;

				if (_cbThickness1 != null)
					_cbThickness1.SelectedQuantityChanged += EhThickness1_ChoiceChanged;
			}
		}

		private void EhThickness1_ChoiceChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_pen != null)
			{
				var oldPen = _pen;

				if (null != _cbThickness2)
					_pen = _pen.WithThickness1(_cbThickness1.SelectedQuantityAsValueInPoints);
				else
					_pen = _pen.WithUniformThickness(_cbThickness1.SelectedQuantityAsValueInPoints);

				if (!object.ReferenceEquals(_pen, oldPen))
					OnPenChanged();
			}
		}

		#endregion Thickness1

		#region Thickness2

		private Common.Drawing.LineThicknessComboBox _cbThickness2;

		public Common.Drawing.LineThicknessComboBox CbLineThickness2
		{
			get { return _cbThickness2; }
			set
			{
				if (_cbThickness2 != null)
					_cbThickness2.SelectedQuantityChanged -= EhThickness2_ChoiceChanged;

				_cbThickness2 = value;
				if (_pen != null && _cbThickness2 != null)
					_cbThickness2.SelectedQuantityAsValueInPoints = _pen.Thickness1;

				if (_cbThickness2 != null)
					_cbThickness2.SelectedQuantityChanged += EhThickness2_ChoiceChanged;
			}
		}

		private void EhThickness2_ChoiceChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_pen != null)
			{
				var oldPen = _pen;
				_pen = _pen.WithThickness2(_cbThickness2.SelectedQuantityAsValueInPoints);

				if (!object.ReferenceEquals(_pen, oldPen))
					OnPenChanged();
			}
		}

		#endregion Thickness2

		#region LineStartCap

		private LineCapComboBox _cbLineStartCap;

		public LineCapComboBox CbLineStartCap
		{
			get { return _cbLineStartCap; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineCapComboBox.SelectedLineCapProperty, typeof(LineCapComboBox));

				if (_cbLineStartCap != null)
					dpd.RemoveValueChanged(_cbLineStartCap, EhLineStartCap_SelectionChangeCommitted);

				_cbLineStartCap = value;
				if (_pen != null && _cbLineStartCap != null)
					_cbLineStartCap.SelectedLineCap = _pen.LineStartCap;

				if (_cbLineStartCap != null)
					dpd.AddValueChanged(_cbLineStartCap, EhLineStartCap_SelectionChangeCommitted);
			}
		}

		private void EhLineStartCap_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var cap = _cbLineStartCap.SelectedLineCap;
				if (null != cap)
				{
					if (_userChangedAbsLineStartCapSize && _cbLineStartCapAbsSize != null)
						cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbLineStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
					if (_userChangedRelLineStartCapSize && _cbLineStartCapRelSize != null)
						cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbLineStartCapRelSize.SelectedQuantityAsValueInSIUnits);
				}
				_pen = _pen.WithLineStartCap(cap);

				if (_cbLineStartCapAbsSize != null && cap != null)
				{
					var oldValue = _userChangedAbsLineStartCapSize;
					_cbLineStartCapAbsSize.SelectedQuantityAsValueInPoints = cap.MinimumAbsoluteSizePt;
					_userChangedAbsLineStartCapSize = oldValue;
				}
				if (_cbLineStartCapRelSize != null && cap != null)
				{
					var oldValue = _userChangedRelLineStartCapSize;
					_cbLineStartCapRelSize.SelectedQuantityAsValueInSIUnits = cap.MinimumRelativeSize;
					_userChangedRelLineStartCapSize = oldValue;
				}

				OnPenChanged();
			}
		}

		private Common.Drawing.LineCapSizeComboBox _cbLineStartCapAbsSize;

		public Common.Drawing.LineCapSizeComboBox CbLineStartCapAbsSize
		{
			get { return _cbLineStartCapAbsSize; }
			set
			{
				if (_cbLineStartCapAbsSize != null)
					_cbLineStartCapAbsSize.SelectedQuantityChanged -= EhLineStartCapAbsSize_SelectionChangeCommitted;

				_cbLineStartCapAbsSize = value;
				if (_pen != null && _cbLineStartCapAbsSize != null)
					_cbLineStartCapAbsSize.SelectedQuantityAsValueInPoints = null == _pen.LineStartCap ? 0 : _pen.LineStartCap.MinimumAbsoluteSizePt;

				if (_cbLineStartCapAbsSize != null)
					_cbLineStartCapAbsSize.SelectedQuantityChanged += EhLineStartCapAbsSize_SelectionChangeCommitted;
			}
		}

		private void EhLineStartCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedAbsLineStartCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.LineStartCap;

				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbLineStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				}

				_pen = _pen.WithLineStartCap(cap);

				OnPenChanged();
			}
		}

		private QuantityWithUnitTextBox _cbLineStartCapRelSize;

		public QuantityWithUnitTextBox CbLineStartCapRelSize
		{
			get { return _cbLineStartCapRelSize; }
			set
			{
				if (_cbLineStartCapRelSize != null)
					_cbLineStartCapRelSize.SelectedQuantityChanged -= EhLineStartCapRelSize_SelectionChangeCommitted;

				_cbLineStartCapRelSize = value;
				if (_pen != null && _cbLineStartCapRelSize != null)
					_cbLineStartCapRelSize.SelectedQuantityAsValueInSIUnits = null == _pen.LineStartCap ? 0 : _pen.LineStartCap.MinimumRelativeSize;

				if (_cbLineStartCapRelSize != null)
					_cbLineStartCapRelSize.SelectedQuantityChanged += EhLineStartCapRelSize_SelectionChangeCommitted;
			}
		}

		private void EhLineStartCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedRelLineStartCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.LineStartCap;
				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbLineStartCapRelSize.SelectedQuantityAsValueInSIUnits);
				}
				_pen = _pen.WithLineStartCap(cap);

				OnPenChanged();
			}
		}

		#endregion StartCap

		#region LineEndCap

		private LineCapComboBox _cbLineEndCap;

		public LineCapComboBox CbLineEndCap
		{
			get { return _cbLineEndCap; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineCapComboBox.SelectedLineCapProperty, typeof(LineCapComboBox));

				if (_cbLineEndCap != null)
					dpd.RemoveValueChanged(_cbLineEndCap, EhLineEndCap_SelectionChangeCommitted);

				_cbLineEndCap = value;
				if (_pen != null && _cbLineEndCap != null)
					_cbLineEndCap.SelectedLineCap = _pen.LineEndCap;

				if (_cbLineEndCap != null)
					dpd.AddValueChanged(_cbLineEndCap, EhLineEndCap_SelectionChangeCommitted);
			}
		}

		private void EhLineEndCap_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var cap = _cbLineEndCap.SelectedLineCap;
				if (_userChangedAbsLineEndCapSize && _cbLineEndCapAbsSize != null)
					cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbLineEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				if (_userChangedRelLineEndCapSize && _cbLineEndCapRelSize != null)
					cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbLineEndCapRelSize.SelectedQuantityAsValueInSIUnits);

				_pen = _pen.WithLineEndCap(cap);

				if (_cbLineEndCapAbsSize != null)
				{
					var oldValue = _userChangedAbsLineEndCapSize;
					_cbLineEndCapAbsSize.SelectedQuantityAsValueInPoints = cap.MinimumAbsoluteSizePt;
					_userChangedAbsLineEndCapSize = oldValue;
				}
				if (_cbLineEndCapRelSize != null)
				{
					var oldValue = _userChangedRelLineEndCapSize;
					_cbLineEndCapRelSize.SelectedQuantityAsValueInSIUnits = cap.MinimumRelativeSize;
					_userChangedRelLineEndCapSize = oldValue;
				}

				OnPenChanged();
			}
		}

		private Common.Drawing.LineCapSizeComboBox _cbLineEndCapAbsSize;

		public Common.Drawing.LineCapSizeComboBox CbLineEndCapAbsSize
		{
			get { return _cbLineEndCapAbsSize; }
			set
			{
				if (_cbLineEndCapAbsSize != null)
					_cbLineEndCapAbsSize.SelectedQuantityChanged -= EhLineEndCapAbsSize_SelectionChangeCommitted;

				_cbLineEndCapAbsSize = value;
				if (_pen != null && _cbLineEndCapAbsSize != null)
					_cbLineEndCapAbsSize.SelectedQuantityAsValueInPoints = null == _pen.LineEndCap ? 0 : _pen.LineEndCap.MinimumAbsoluteSizePt;

				if (_cbLineEndCapAbsSize != null)
					_cbLineEndCapAbsSize.SelectedQuantityChanged += EhLineEndCapAbsSize_SelectionChangeCommitted;
			}
		}

		private void EhLineEndCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedAbsLineEndCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.LineEndCap;
				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbLineEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				}
				_pen = _pen.WithLineEndCap(cap);

				OnPenChanged();
			}
		}

		private QuantityWithUnitTextBox _cbLineEndCapRelSize;

		public QuantityWithUnitTextBox CbLineEndCapRelSize
		{
			get { return _cbLineEndCapRelSize; }
			set
			{
				if (_cbLineEndCapRelSize != null)
					_cbLineEndCapRelSize.SelectedQuantityChanged -= EhLineEndCapRelSize_SelectionChangeCommitted;

				_cbLineEndCapRelSize = value;
				if (_pen != null && _cbLineEndCapRelSize != null)
					_cbLineEndCapRelSize.SelectedQuantityAsValueInSIUnits = null == _pen.LineEndCap ? 0 : _pen.LineEndCap.MinimumRelativeSize;

				if (_cbLineEndCapRelSize != null)
					_cbLineEndCapRelSize.SelectedQuantityChanged += EhLineEndCapRelSize_SelectionChangeCommitted;
			}
		}

		private void EhLineEndCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedRelLineEndCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.LineEndCap;
				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbLineEndCapRelSize.SelectedQuantityAsValueInSIUnits);
				}
				_pen = _pen.WithLineEndCap(cap);

				OnPenChanged();
			}
		}

		#endregion EndCap

		#region DashStartCap

		private LineCapComboBox _cbDashStartCap;

		public LineCapComboBox CbDashStartCap
		{
			get { return _cbDashStartCap; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineCapComboBox.SelectedLineCapProperty, typeof(LineCapComboBox));

				if (_cbDashStartCap != null)
					dpd.RemoveValueChanged(_cbDashStartCap, EhDashStartCap_SelectionChangeCommitted);

				_cbDashStartCap = value;
				if (_pen != null && _cbDashStartCap != null)
					_cbDashStartCap.SelectedLineCap = _pen.DashStartCap;

				if (_cbDashStartCap != null)
					dpd.AddValueChanged(_cbDashStartCap, EhDashStartCap_SelectionChangeCommitted);
			}
		}

		private void EhDashStartCap_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var cap = _cbDashStartCap.SelectedLineCap;
				if (null != cap)
				{
					if (_userChangedAbsDashStartCapSize && _cbDashStartCapAbsSize != null)
						cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbDashStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
					if (_userChangedRelDashStartCapSize && _cbDashStartCapRelSize != null)
						cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbDashStartCapRelSize.SelectedQuantityAsValueInSIUnits);
				}
				_pen = _pen.WithDashStartCap(cap);

				if (_cbDashStartCapAbsSize != null && cap != null)
				{
					var oldValue = _userChangedAbsDashStartCapSize;
					_cbDashStartCapAbsSize.SelectedQuantityAsValueInPoints = cap.MinimumAbsoluteSizePt;
					_userChangedAbsDashStartCapSize = oldValue;
				}
				if (_cbDashStartCapRelSize != null && cap != null)
				{
					var oldValue = _userChangedRelDashStartCapSize;
					_cbDashStartCapRelSize.SelectedQuantityAsValueInSIUnits = cap.MinimumRelativeSize;
					_userChangedRelDashStartCapSize = oldValue;
				}

				OnPenChanged();
			}
		}

		private Common.Drawing.LineCapSizeComboBox _cbDashStartCapAbsSize;

		public Common.Drawing.LineCapSizeComboBox CbDashStartCapAbsSize
		{
			get { return _cbDashStartCapAbsSize; }
			set
			{
				if (_cbDashStartCapAbsSize != null)
					_cbDashStartCapAbsSize.SelectedQuantityChanged -= EhDashStartCapAbsSize_SelectionChangeCommitted;

				_cbDashStartCapAbsSize = value;
				if (_pen != null && _cbDashStartCapAbsSize != null)
					_cbDashStartCapAbsSize.SelectedQuantityAsValueInPoints = null == _pen.DashStartCap ? 0 : _pen.DashStartCap.MinimumAbsoluteSizePt;

				if (_cbDashStartCapAbsSize != null)
					_cbDashStartCapAbsSize.SelectedQuantityChanged += EhDashStartCapAbsSize_SelectionChangeCommitted;
			}
		}

		private void EhDashStartCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedAbsDashStartCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.DashStartCap;

				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbDashStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				}

				_pen = _pen.WithDashStartCap(cap);

				OnPenChanged();
			}
		}

		private QuantityWithUnitTextBox _cbDashStartCapRelSize;

		public QuantityWithUnitTextBox CbDashStartCapRelSize
		{
			get { return _cbDashStartCapRelSize; }
			set
			{
				if (_cbDashStartCapRelSize != null)
					_cbDashStartCapRelSize.SelectedQuantityChanged -= EhDashStartCapRelSize_SelectionChangeCommitted;

				_cbDashStartCapRelSize = value;
				if (_pen != null && _cbDashStartCapRelSize != null)
					_cbDashStartCapRelSize.SelectedQuantityAsValueInSIUnits = null == _pen.DashStartCap ? 0 : _pen.DashStartCap.MinimumRelativeSize;

				if (_cbDashStartCapRelSize != null)
					_cbDashStartCapRelSize.SelectedQuantityChanged += EhDashStartCapRelSize_SelectionChangeCommitted;
			}
		}

		private void EhDashStartCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedRelDashStartCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.DashStartCap;
				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbDashStartCapRelSize.SelectedQuantityAsValueInSIUnits);
				}
				_pen = _pen.WithDashStartCap(cap);

				OnPenChanged();
			}
		}

		#endregion DashStartCap

		#region DashEndCap

		private LineCapComboBox _cbDashEndCap;

		public LineCapComboBox CbDashEndCap
		{
			get { return _cbDashEndCap; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineCapComboBox.SelectedLineCapProperty, typeof(LineCapComboBox));

				if (_cbDashEndCap != null)
					dpd.RemoveValueChanged(_cbDashEndCap, EhDashEndCap_SelectionChangeCommitted);

				_cbDashEndCap = value;
				if (_pen != null && _cbDashEndCap != null)
					_cbDashEndCap.SelectedLineCap = _pen.DashEndCap;

				if (_cbDashEndCap != null)
					dpd.AddValueChanged(_cbDashEndCap, EhDashEndCap_SelectionChangeCommitted);
			}
		}

		private void EhDashEndCap_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				var cap = _cbDashEndCap.SelectedLineCap;
				if (_userChangedAbsDashEndCapSize && _cbDashEndCapAbsSize != null)
					cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbDashEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				if (_userChangedRelDashEndCapSize && _cbDashEndCapRelSize != null)
					cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbDashEndCapRelSize.SelectedQuantityAsValueInSIUnits);

				_pen = _pen.WithDashEndCap(cap);

				if (_cbDashEndCapAbsSize != null)
				{
					var oldValue = _userChangedAbsDashEndCapSize;
					_cbDashEndCapAbsSize.SelectedQuantityAsValueInPoints = cap.MinimumAbsoluteSizePt;
					_userChangedAbsDashEndCapSize = oldValue;
				}
				if (_cbDashEndCapRelSize != null)
				{
					var oldValue = _userChangedRelDashEndCapSize;
					_cbDashEndCapRelSize.SelectedQuantityAsValueInSIUnits = cap.MinimumRelativeSize;
					_userChangedRelDashEndCapSize = oldValue;
				}

				OnPenChanged();
			}
		}

		private Common.Drawing.LineCapSizeComboBox _cbDashEndCapAbsSize;

		public Common.Drawing.LineCapSizeComboBox CbDashEndCapAbsSize
		{
			get { return _cbDashEndCapAbsSize; }
			set
			{
				if (_cbDashEndCapAbsSize != null)
					_cbDashEndCapAbsSize.SelectedQuantityChanged -= EhDashEndCapAbsSize_SelectionChangeCommitted;

				_cbDashEndCapAbsSize = value;
				if (_pen != null && _cbDashEndCapAbsSize != null)
					_cbDashEndCapAbsSize.SelectedQuantityAsValueInPoints = null == _pen.DashEndCap ? 0 : _pen.DashEndCap.MinimumAbsoluteSizePt;

				if (_cbDashEndCapAbsSize != null)
					_cbDashEndCapAbsSize.SelectedQuantityChanged += EhDashEndCapAbsSize_SelectionChangeCommitted;
			}
		}

		private void EhDashEndCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedAbsDashEndCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.DashEndCap;
				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(_cbDashEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				}
				_pen = _pen.WithDashEndCap(cap);

				OnPenChanged();
			}
		}

		private QuantityWithUnitTextBox _cbDashEndCapRelSize;

		public QuantityWithUnitTextBox CbDashEndCapRelSize
		{
			get { return _cbDashEndCapRelSize; }
			set
			{
				if (_cbDashEndCapRelSize != null)
					_cbDashEndCapRelSize.SelectedQuantityChanged -= EhDashEndCapRelSize_SelectionChangeCommitted;

				_cbDashEndCapRelSize = value;
				if (_pen != null && _cbDashEndCapRelSize != null)
					_cbDashEndCapRelSize.SelectedQuantityAsValueInSIUnits = null == _pen.DashEndCap ? 0 : _pen.DashEndCap.MinimumRelativeSize;

				if (_cbDashEndCapRelSize != null)
					_cbDashEndCapRelSize.SelectedQuantityChanged += EhDashEndCapRelSize_SelectionChangeCommitted;
			}
		}

		private void EhDashEndCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedRelDashEndCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.DashEndCap;
				if (null != cap)
				{
					cap = cap.WithMinimumAbsoluteAndRelativeSize(cap.MinimumAbsoluteSizePt, _cbDashEndCapRelSize.SelectedQuantityAsValueInSIUnits);
				}
				_pen = _pen.WithDashEndCap(cap);

				OnPenChanged();
			}
		}

		#endregion EndCap

		#region LineJoin

		private LineJoinComboBox _cbLineJoin;

		public LineJoinComboBox CbLineJoin
		{
			get { return _cbLineJoin; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineJoinComboBox.SelectedLineJoinProperty, typeof(LineJoinComboBox));

				if (_cbLineJoin != null)
					dpd.RemoveValueChanged(_cbLineJoin, EhLineJoin_SelectionChangeCommitted);

				_cbLineJoin = value;
				if (_pen != null && _cbLineJoin != null)
					_cbLineJoin.SelectedLineJoin = _pen.LineJoin;

				if (_cbLineJoin != null)
					dpd.AddValueChanged(_cbLineJoin, EhLineJoin_SelectionChangeCommitted);
			}
		}

		private void EhLineJoin_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				_pen = _pen.WithLineJoin(_cbLineJoin.SelectedLineJoin);

				OnPenChanged();
			}
		}

		#endregion LineJoin

		#region Miter

		private Common.Drawing.MiterLimitComboBox _cbMiterLimit;

		public Common.Drawing.MiterLimitComboBox CbMiterLimit
		{
			get { return _cbMiterLimit; }
			set
			{
				if (_cbMiterLimit != null)
					_cbMiterLimit.SelectedQuantityChanged -= EhMiterLimit_SelectionChangeCommitted;

				_cbMiterLimit = value;
				if (_pen != null && _cbMiterLimit != null)
					_cbMiterLimit.SelectedQuantityInSIUnits = _pen.MiterLimit;

				if (_cbLineJoin != null)
					_cbMiterLimit.SelectedQuantityChanged += EhMiterLimit_SelectionChangeCommitted;
			}
		}

		private void EhMiterLimit_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_pen != null)
			{
				_pen = _pen.WithMiterLimit(_cbMiterLimit.SelectedQuantityInSIUnits);

				OnPenChanged();
			}
		}

		#endregion Miter

		#region Dialog

		private void EhShowCustomPenDialog(object sender, EventArgs e)
		{
			PenAllPropertiesController ctrler = new PenAllPropertiesController(this.Pen);
			ctrler.ShowPlotColorsOnly = this._showPlotColorsOnly;
			ctrler.ViewObject = new PenAllPropertiesControl();
			if (Current.Gui.ShowDialog(ctrler, "Edit pen properties"))
			{
				this.Pen = (PenX3D)ctrler.ModelObject;
			}
		}

		#endregion Dialog

		#region Preview

		private Image _previewPanel;
		private GdiToWpfBitmap _previewBitmap;

		public Image PreviewPanel
		{
			get
			{
				return _previewPanel;
			}
			set
			{
				if (null != _previewPanel)
				{
					_previewPanel.SizeChanged -= EhPreviewPanel_SizeChanged;
				}

				_previewPanel = value;

				if (null != _previewPanel)
				{
					_previewPanel.SizeChanged += EhPreviewPanel_SizeChanged;
					UpdatePreviewPanel();
				}
			}
		}

		private void EhPreviewPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdatePreviewPanel();
		}

		private void UpdatePreviewPanel()
		{
		}

		#endregion Preview
	}
}