#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Common
{
	using Altaxo.Graph;
	using Geometry;
	using SharpDX;
	using SharpDX.Direct3D10;
	using SharpDX.DXGI;
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Media;
	using Device = SharpDX.Direct3D10.Device1;
	using STG = SharpDX.Toolkit.Graphics;

	public class D3D10BitmapExporter
	{
		public Color4 ClearColor = SharpDX.Color.Beige;

		public void Export(int sizeX, int sizeY, ID3D10Scene scene)
		{
			var device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug, FeatureLevel.Level_10_0);

			// try to get the highest MSAA level with the highest quality
			int sampleCount = 32;
			int qlevel_sampleCount = 0;

			for (; sampleCount >= 0; sampleCount /= 2)
			{
				if (0 != (qlevel_sampleCount = device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, sampleCount))) // quality level for sample count
					break;
			}

			Texture2DDescription colordesc = new Texture2DDescription
			{
				BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
				Format = Format.B8G8R8A8_UNorm,
				Width = sizeX,
				Height = sizeY,
				MipLevels = 1,
				SampleDescription = new SampleDescription(sampleCount, qlevel_sampleCount - 1),
				Usage = ResourceUsage.Default,
				OptionFlags = ResourceOptionFlags.Shared,
				CpuAccessFlags = CpuAccessFlags.None,
				ArraySize = 1
			};

			Texture2DDescription depthdesc = new Texture2DDescription
			{
				BindFlags = BindFlags.DepthStencil,
				Format = Format.D32_Float_S8X24_UInt,
				Width = sizeX,
				Height = sizeY,
				MipLevels = 1,
				SampleDescription = new SampleDescription(sampleCount, qlevel_sampleCount - 1),
				Usage = ResourceUsage.Default,
				OptionFlags = ResourceOptionFlags.None,
				CpuAccessFlags = CpuAccessFlags.None,
				ArraySize = 1,
			};

			var renderTarget = new Texture2D(device, colordesc);
			var depthStencil = new Texture2D(device, depthdesc);
			var renderTargetView = new RenderTargetView(device, renderTarget);
			var depthStencilView = new DepthStencilView(device, depthStencil);

			// Rendering

			device.OutputMerger.SetTargets(depthStencilView, renderTargetView);
			device.Rasterizer.SetViewports(new Viewport(0, 0, sizeX, sizeY, 0.0f, 1.0f));

			device.ClearRenderTargetView(renderTargetView, this.ClearColor);
			device.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

			scene.Attach(device, new PointD2D(sizeX, sizeY));
			scene.Render();

			device.Flush();

			Texture2D renderTarget2 = null;

			if (sampleCount > 1) // if renderTarget is an MSAA render target, we first have to copy it into a non-MSAA render target before we can copy it to a CPU texture and then hope to save it
			{
				// create a non-MSAA render target with the same size
				Texture2DDescription renderTarget2Description = new Texture2DDescription
				{
					BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
					Format = Format.B8G8R8A8_UNorm,
					Width = sizeX,
					Height = sizeY,
					MipLevels = 1,
					SampleDescription = new SampleDescription(1, 0), // non MSAA
					Usage = ResourceUsage.Default,
					OptionFlags = ResourceOptionFlags.Shared,
					CpuAccessFlags = CpuAccessFlags.None,
					ArraySize = 1
				};

				renderTarget2 = new Texture2D(device, renderTarget2Description); // create non-MSAA render target
				device.ResolveSubresource(renderTarget, 0, renderTarget2, 0, renderTarget.Description.Format); // copy from MSAA render target to the non-MSAA render target

				var h = renderTarget; // exchange renderTarget with renderTarget2
				renderTarget = renderTarget2;
				renderTarget2 = h;
			}

			// renderTarget is now a non-MSAA renderTarget
			Texture2DExtensions.Save(renderTarget);

			scene.Detach();

			Disposer.RemoveAndDispose(ref depthStencilView);
			Disposer.RemoveAndDispose(ref renderTargetView);
			Disposer.RemoveAndDispose(ref renderTarget2);
			Disposer.RemoveAndDispose(ref renderTarget);
			Disposer.RemoveAndDispose(ref depthStencil);
			Disposer.RemoveAndDispose(ref device);
		}
	}
}