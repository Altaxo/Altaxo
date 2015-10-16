using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Common
{
	using Altaxo.Graph;
	using SharpDX;
	using SharpDX.Direct3D;
	using SharpDX.Direct3D11;
	using SharpDX.DXGI;
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Media;
	using Device = SharpDX.Direct3D11.Device;
	using STG = SharpDX.Toolkit.Graphics;

	public class D3D11BitmapExporter
	{
		public Color4 ClearColor = SharpDX.Color.LightBlue;

		public void Export(int sizeX, int sizeY, ID3D11Scene scene)
		{
			var device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
			var devContext = device.ImmediateContext;

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

			devContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
			devContext.Rasterizer.SetViewports(new ViewportF[] { new ViewportF(0, 0, sizeX, sizeY, 0.0f, 1.0f) }, 1);

			devContext.ClearRenderTargetView(renderTargetView, this.ClearColor);
			devContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

			scene.Attach(device, new PointD2D(sizeX, sizeY));
			scene.Render();

			devContext.Flush();

			STG.GraphicsDevice stgDevice = STG.GraphicsDevice.New(device);
			var stgTexture = SharpDX.Toolkit.Graphics.Texture2D.New(stgDevice, renderTarget);

			stgTexture.Save(@"C:\temp\d3dimage.png", SharpDX.Toolkit.Graphics.ImageFileType.Png);

			Disposer.RemoveAndDispose(ref stgTexture);
			Disposer.RemoveAndDispose(ref stgDevice);

			scene.Detach();

			Disposer.RemoveAndDispose(ref depthStencilView);
			Disposer.RemoveAndDispose(ref depthStencilView);
			Disposer.RemoveAndDispose(ref depthStencil);
			Disposer.RemoveAndDispose(ref renderTarget);
			Disposer.RemoveAndDispose(ref devContext);
			Disposer.RemoveAndDispose(ref device);
		}
	}
}