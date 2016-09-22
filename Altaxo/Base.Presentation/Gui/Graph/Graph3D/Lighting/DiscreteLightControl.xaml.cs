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

namespace Altaxo.Gui.Graph.Graph3D.Lighting
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
						ChangeHostControlAccordingToNewLight(value);
						SetRadioButtonAccordingToNewLight(value);
					}
					);
			}
		}

		/// <summary>
		/// Called when the radio button signals that the user want to change the light type.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
		private void EhLightTypeChanged(object sender, RoutedEventArgs e)
		{
			if (_lock.IsNotLocked)
			{
				IDiscreteLight newLight = null;
				if (object.ReferenceEquals(sender, _guiNotUsed))
				{
					newLight = null;
				}
				else if (object.ReferenceEquals(sender, _guiDirectional))
				{
					newLight = new Altaxo.Graph.Graph3D.Lighting.DirectionalLight();
				}
				else if (object.ReferenceEquals(sender, _guiPoint))
				{
					newLight = new Altaxo.Graph.Graph3D.Lighting.PointLight();
				}
				else if (object.ReferenceEquals(sender, _guiSpot))
				{
					newLight = new Altaxo.Graph.Graph3D.Lighting.SpotLight();
				}
				else
				{
					throw new NotImplementedException();
				}

				if (newLight?.GetType() != SelectedValue?.GetType())
				{
					ChangeHostControlAccordingToNewLight(newLight);
					SelectedValueChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		private void ChangeHostControlAccordingToNewLight(IDiscreteLight newLightValue)
		{
			if (null == newLightValue)
			{
				ChangeHostControl(null);
			}
			else if (newLightValue is DirectionalLight)
			{
				var ctrl = new DirectionalLightControl();
				ctrl.SelectedValue = newLightValue as DirectionalLight;
				ChangeHostControl(ctrl);
			}
			else if (newLightValue is PointLight)
			{
				var ctrl = new PointLightControl();
				ctrl.SelectedValue = newLightValue as PointLight;
				ChangeHostControl(ctrl);
			}
			else if (newLightValue is SpotLight)
			{
				var ctrl = new SpotLightControl();
				ctrl.SelectedValue = newLightValue as SpotLight;
				ChangeHostControl(ctrl);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private void SetRadioButtonAccordingToNewLight(IDiscreteLight newLightValue)
		{
			if (newLightValue == null)
			{
				_guiNotUsed.IsChecked = true;
			}
			else if (newLightValue is DirectionalLight)
			{
				_guiDirectional.IsChecked = true;
			}
			else if (newLightValue is PointLight)
			{
				_guiPoint.IsChecked = true;
			}
			else if (newLightValue is SpotLight)
			{
				_guiSpot.IsChecked = true;
			}
			else
			{
				throw new NotImplementedException();
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
		}

		private void EhSelectedValueChanged(object sender, EventArgs e)
		{
			if (_lock.IsNotLocked)
				SelectedValueChanged?.Invoke(this, e);
		}
	}
}