#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  /// <summary>
  /// Provides thread safe and cached access to the availability of DirectX versions on the current system. 
  /// </summary>
  public static class DirectXVersionAvailability
  {
    /// <summary>
    /// Gets a cached value indicating whether DirectX 12 is available on the current system.
    /// </summary>
    /// <value>
    /// Returns <see langword="true"/> if a hardware adapter can create a D3D12 device; otherwise <see langword="false"/>.
    /// </value>
    public static bool IsDirectX12Available { get; }


    static DirectXVersionAvailability()
    {
      bool isDirectX12Available = false;
      try
      {
        using var factory = DXGI.CreateDXGIFactory2<IDXGIFactory4>(false);

        for (uint adapterIndex = 0; ; ++adapterIndex)
        {
          var result = factory.EnumAdapters1(adapterIndex, out var adapter);
          if (result.Failure || adapter is null)
            break;

          using (adapter)
          {
            var desc = adapter.Description1;
            if ((desc.Flags & AdapterFlags.Software) != 0)
              continue;

            var createResult = D3D12CreateDevice(adapter, FeatureLevel.Level_11_0, out ID3D12Device? testDevice);
            if (createResult.Success && testDevice is not null)
            {
              testDevice.Dispose();
              isDirectX12Available = true;
              break;
            }
          }
        }
      }
      catch
      {
      }

      IsDirectX12Available = isDirectX12Available;
    }
  }
}
