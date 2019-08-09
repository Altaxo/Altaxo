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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D10.Device1;

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  /// <summary>
  /// Despite all efforts, resources allocated by creating a new D3D10 device could not be released properly (2019-04-08). Because of this,
  /// created devices are cached, instead of tried to release. This class manages the already created devices.
  /// </summary>
  public class D3D10DeviceFactory
  {
    /// <summary>
    /// Gets the only instance of this class.
    /// </summary>
    public static D3D10DeviceFactory Instance { get; } = new D3D10DeviceFactory();


    /// <summary>
    /// Bag that stores devices. Note that we do not use WeakReference here, because it seems that a WeakReference
    /// becomes released, despite the fact that the device is not released!
    /// </summary>
    ConcurrentBag<Device> _devices = new ConcurrentBag<Device>();

    private D3D10DeviceFactory() { }


    /// <summary>
    /// Borrows a D3D10 device. After the device has been used, it must be given back by calling <see cref="PassbackDevice(ref Device)"/>.
    /// </summary>
    /// <returns>A D3D10 device.</returns>
    public Device BorrowDevice()
    {
      Device dev;
      while (_devices.TryTake(out dev) && dev.IsDisposed)
      {
      }

      if (dev is null)
      {
        dev = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
      }

      return dev;
    }

    /// <summary>
    /// Gives back a D3D10 device.
    /// </summary>
    /// <param name="device">Reference to the device to give back. After calling the function, the reference is set to null to signal that the device
    /// was given back to the device factory.</param>
    public void PassbackDevice(ref Device device)
    {
      if (device is null)
        return;

      // Do not dispose the device here
      // Disposing the device will set its flag to disposed,
      // but it still has Unmanaged memory allocated


      if (!device.IsDisposed)
      {
        device.ClearState();
        _devices.Add(device);
        device = null;
      }
    }
  }
}
