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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Pads.LightingPad
{
  /// <summary>
  /// Interaction logic for LightingControl.xaml
  /// </summary>
  public partial class LightingControl : UserControl, ILightingView
  {
    public event EventHandler? LightingChanged;

    private Altaxo.Graph.Graph3D.LightSettings _lighting = new Altaxo.Graph.Graph3D.LightSettings();

    public Altaxo.Graph.Graph3D.LightSettings Lighting
    {
      get
      {
        return _lighting;
      }
      set
      {
        if (value is not null)
        {
          _lighting = value;
          _guiAmbientControl.SelectedValue = _lighting.AmbientLight;

          _guiDiscreteLight0.SelectedValue = _lighting.GetDiscreteLight(0);
          _guiDiscreteLight1.SelectedValue = _lighting.GetDiscreteLight(1);
          _guiDiscreteLight2.SelectedValue = _lighting.GetDiscreteLight(2);
          _guiDiscreteLight3.SelectedValue = _lighting.GetDiscreteLight(3);
        }
      }
    }

    public LightingControl()
    {
      InitializeComponent();
    }

    private void EhValueChanged(object sender, EventArgs e)
    {
      if (object.ReferenceEquals(sender, _guiAmbientControl))
        _lighting = _lighting.WithAmbientLight(_guiAmbientControl.SelectedValue);

      LightingChanged?.Invoke(this, e);
    }

    private void EhDiscreteLightChanged(object sender, EventArgs e)
    {
      if (object.ReferenceEquals(sender, _guiDiscreteLight0))
      {
        _lighting = _lighting.WithDiscreteLight(0, _guiDiscreteLight0.SelectedValue);
        LightingChanged?.Invoke(this, e);
      }
      else if (object.ReferenceEquals(sender, _guiDiscreteLight1))
      {
        _lighting = _lighting.WithDiscreteLight(1, _guiDiscreteLight1.SelectedValue);
        LightingChanged?.Invoke(this, e);
      }
      else if (object.ReferenceEquals(sender, _guiDiscreteLight2))
      {
        _lighting = _lighting.WithDiscreteLight(2, _guiDiscreteLight2.SelectedValue);
        LightingChanged?.Invoke(this, e);
      }
      else if (object.ReferenceEquals(sender, _guiDiscreteLight3))
      {
        _lighting = _lighting.WithDiscreteLight(3, _guiDiscreteLight3.SelectedValue);
        LightingChanged?.Invoke(this, e);
      }
    }
  }
}
