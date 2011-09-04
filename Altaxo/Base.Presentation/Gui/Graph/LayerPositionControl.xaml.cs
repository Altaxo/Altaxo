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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for LayerPositionControl.xaml
	/// </summary>
	public partial class LayerPositionControl : UserControl, ILayerPositionView
	{
		private ILayerPositionViewEventSink m_Controller;

		public LayerPositionControl()
		{
			InitializeComponent();
		}

		private void EhLinkedLayer_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller)
				Controller.EhView_LinkedLayerChanged((SelectableListNode)this.m_Layer_cbLinkedLayer.SelectedItem);
		}

		private void EhLeft_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_LeftChanged(((TextBox)sender).Text, ref bCancel);
				if (bCancel)
					e.AddError("The provided string can not be converted to a valid number");
			}

		}

		private void EhTop_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_TopChanged(((TextBox)sender).Text, ref bCancel);
				if (bCancel)
					e.AddError("The provided string can not be converted to a valid number");
			}
		}

		private void EhWidth_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_WidthChanged(((TextBox)sender).Text, ref bCancel);
				if (bCancel)
					e.AddError("The provided string can not be converted to a valid number");
			}
		}

		private void EhHeight_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_HeightChanged(((TextBox)sender).Text, ref bCancel);
				if (bCancel)
					e.AddError("The provided string can not be converted to a valid number");
			}
		}

		private void EhLeftType_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller && null != m_Layer_cbLeftType.SelectedItem)
				Controller.EhView_LeftTypeChanged((SelectableListNode)this.m_Layer_cbLeftType.SelectedItem);
		}

		private void EhTopType_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller && null != m_Layer_cbTopType.SelectedItem)
				Controller.EhView_TopTypeChanged((SelectableListNode)this.m_Layer_cbTopType.SelectedItem);
		}

		private void EhWidthType_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller && null != m_Layer_cbWidthType.SelectedItem)
				Controller.EhView_WidthTypeChanged((SelectableListNode)this.m_Layer_cbWidthType.SelectedItem);
		}

		private void EhHeightType_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller && null != m_Layer_cbHeightType.SelectedItem)
				Controller.EhView_HeightTypeChanged((SelectableListNode)this.m_Layer_cbHeightType.SelectedItem);
		}

		private void EhRotation_Changed(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_RotationChanged(m_Layer_edRotation.SelectedRotation);
			}

		}

		private void EhScale_Validating(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_ScaleChanged(m_Layer_edScale.SelectedScale);
			}
		}

		private void EhClipDataToFrame_Validating(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_ClipDataToFrameChanged(this.m_Layer_ClipDataToFrame.IsChecked == true);
			}
		}
		#region ILayerPositionView

		public ILayerPositionViewEventSink Controller
		{
			get
			{
				return m_Controller;
			}
			set
			{
				m_Controller = value;
			}
		}

		public void InitializeLeft(string txt)
		{
			this.m_Layer_edLeftPosition.Text = txt;
		}

		public void InitializeTop(string txt)
		{
			this.m_Layer_edTopPosition.Text = txt;
		}

		public void InitializeHeight(string txt)
		{
			this.m_Layer_edHeight.Text = txt;
		}

		public void InitializeWidth(string txt)
		{
			this.m_Layer_edWidth.Text = txt;
		}

		public void InitializeRotation(float val)
		{
			this.m_Layer_edRotation.SelectedRotation = val;
		}

		public void InitializeScale(string txt)
		{
			this.m_Layer_edScale.Text = txt;
		}

		public void InitializeClipDataToFrame(bool value)
		{
			this.m_Layer_ClipDataToFrame.IsChecked = value;
		}

		public void InitializeLeftType(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(m_Layer_cbLeftType, names);
		}

		public void InitializeTopType(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(this.m_Layer_cbTopType, names);
		}

		public void InitializeHeightType(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(this.m_Layer_cbHeightType, names);
		}

		public void InitializeWidthType(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(this.m_Layer_cbWidthType, names);
		}

		public void InitializeLinkedLayer(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(this.m_Layer_cbLinkedLayer, names);
		}

		#endregion ILayerPositionView


	}
}
