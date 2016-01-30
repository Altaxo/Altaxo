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

using Altaxo.Graph.Graph3D.Lighting;
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

namespace Altaxo.Gui.Graph3D.Lighting
{
	/// <summary>
	/// Interaction logic for DiscreteLightControl.xaml
	/// </summary>
	public partial class DiscreteLightControl : UserControl
	{
		private IDiscreteLightControl _control;

		public event EventHandler SelectedValueChanged;

		private GuiChangeLocker _lock;

		public DiscreteLightControl()
		{
			InitializeComponent();
		}

		public IDiscreteLight SelectedValue
		{
			get
			{
				return _control == null ? null : _control.SelectedValueAsIDiscreteLight;
			}
			set
			{
				_lock.ExecuteLocked(
					() =>
					{
						if (null == value)
						{
							ChangeHostControl(null);
						}
						else if (value is DirectionalLight)
						{
							var ctrl = new DirectionalLightControl();
							ctrl.SelectedValue = value as DirectionalLight;
							ChangeHostControl(ctrl);
						}
						else if (value is PointLight)
						{
							var ctrl = new PointLightControl();
							ctrl.SelectedValue = value as PointLight;
							ChangeHostControl(ctrl);
						}
						else if (value is SpotLight)
						{
							var ctrl = new SpotLightControl();
							ctrl.SelectedValue = value as SpotLight;
							ChangeHostControl(ctrl);
						}
						else
						{
							throw new NotImplementedException();
						}
					}
					);
			}
		}

		private void EhLightTypeChanged(object sender, RoutedEventArgs e)
		{
			string tag = (string)((RadioButton)sender).Tag;

			IDiscreteLightControl ctrl;

			switch (tag)
			{
				case "NotUsed":
					ctrl = null;
					break;

				case "Directional":
					{
						var control = new DirectionalLightControl();
						control.SelectedValue = new Altaxo.Graph.Graph3D.Lighting.DirectionalLight();
						ctrl = control;
					}
					break;

				case "Point":
					{
						var control = new PointLightControl();
						control.SelectedValue = new Altaxo.Graph.Graph3D.Lighting.PointLight();
						ctrl = control;
					}
					break;

				case "Spot":
					{
						var control = new SpotLightControl();
						control.SelectedValue = new Altaxo.Graph.Graph3D.Lighting.SpotLight();
						ctrl = control;
					}
					break;

				default:
					throw new NotImplementedException();
			}

			if (ctrl?.GetType() != _control?.GetType())
			{
				ChangeHostControl(ctrl);

				if (_lock.IsNotLocked)
					SelectedValueChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private void ChangeHostControl(IDiscreteLightControl newControl)
		{
			if (null != _control)
				_control.SelectedValueChanged -= EhSelectedValueChanged;

			_control = newControl;
			_guiControlHost.Child = (UIElement)_control;

			if (null != _control)
				_control.SelectedValueChanged += EhSelectedValueChanged;

			/*

			if (newControl == null)
			{
				_guiNotUsed.IsChecked = true;
			}
			else if (newControl is DirectionalLightControl)
			{
				_guiDirectional.IsChecked = true;
			}
			else if (newControl is PointLightControl)
			{
				_guiPoint.IsChecked = true;
			}
			else if (newControl is SpotLightControl)
			{
				_guiSpot.IsChecked = true;
			}
			else
			{
				throw new NotImplementedException();
			}
			*/
		}

		private void EhSelectedValueChanged(object sender, EventArgs e)
		{
			if (_lock.IsNotLocked)
				SelectedValueChanged?.Invoke(this, e);
		}
	}
}