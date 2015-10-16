using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Common
{
	using Altaxo.Graph;
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
		public Color4 ClearColor = SharpDX.Color.White;

		public void Export(int sizeX, int sizeY, ID3D10Scene scene)
		{
			var device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);

			Texture2DDescription colordesc = new Texture2DDescription
			{
				BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
				Format = Format.B8G8R8A8_UNorm,
				Width = sizeX,
				Height = sizeY,
				MipLevels = 1,
				SampleDescription = new SampleDescription(1, 0),
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
				SampleDescription = new SampleDescription(1, 0),
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

			Texture2DExtensions.Save(renderTarget);

			scene.Detach();

			Disposer.RemoveAndDispose(ref depthStencilView);
			Disposer.RemoveAndDispose(ref depthStencilView);
			Disposer.RemoveAndDispose(ref depthStencil);
			Disposer.RemoveAndDispose(ref renderTarget);
			Disposer.RemoveAndDispose(ref device);
		}
	}
}