// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Altaxo.Gui.Graph3D.Common
{
	using Altaxo.Geometry;
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

	public partial class D3D11Canvas : Image
	{
		private Device _device;
		private Texture2D RenderTarget;
		private Texture2D DepthStencil;
		private RenderTargetView RenderTargetView;
		private DepthStencilView DepthStencilView;
		private DX11ImageSource D3DSurface;
		private Stopwatch RenderTimer;
		private IScene RenderScene;
		private bool SceneAttached;

		public Color4 ClearColor = SharpDX.Color.White;

		/// <summary>
		/// Occurs when 3D rendering is ready.
		/// </summary>
		public event EventHandler D3DStarted;

		public D3D11Canvas()
		{
			this.RenderTimer = new Stopwatch();
			this.Loaded += this.EhLoaded;
			this.Unloaded += this.EhUnloaded;
			this.GotFocus += EhFocused;
			this.GotKeyboardFocus += EhGotKeyboardFocus;
			this.MouseDown += EhMouseDown;
		}

		private void EhMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.Focus();
		}

		private void EhGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
		}

		private void EhFocused(object sender, RoutedEventArgs e)
		{
		}

		private void EhLoaded(object sender, RoutedEventArgs e)
		{
			if (D3D11Canvas.IsInDesignMode)
				return;

			this.StartD3D();
			this.StartRendering();
			this.Focusable = true;

			D3DStarted?.Invoke(this, EventArgs.Empty);
		}

		private void EhUnloaded(object sender, RoutedEventArgs e)
		{
			if (D3D10Canvas.IsInDesignMode)
				return;

			this.StopRendering();
			this.EndD3D();
		}

		private void StartD3D()
		{
			this._device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);

			this.D3DSurface = new DX11ImageSource();
			this.D3DSurface.IsFrontBufferAvailableChanged += EhIsFrontBufferAvailableChanged;

			this.CreateAndBindTargets();

			this.Source = this.D3DSurface;
		}

		private void EndD3D()
		{
			if (this.RenderScene != null)
			{
				this.RenderScene.Detach();
				this.SceneAttached = false;
			}

			this.D3DSurface.IsFrontBufferAvailableChanged -= EhIsFrontBufferAvailableChanged;
			this.Source = null;

			Disposer.RemoveAndDispose(ref this.D3DSurface);
			Disposer.RemoveAndDispose(ref this.RenderTargetView);
			Disposer.RemoveAndDispose(ref this.DepthStencilView);
			Disposer.RemoveAndDispose(ref this.RenderTarget);
			Disposer.RemoveAndDispose(ref this.DepthStencil);
			Disposer.RemoveAndDispose(ref this._device);
		}

		private void CreateAndBindTargets()
		{
			if (null != this.D3DSurface)
				this.D3DSurface.SetRenderTargetDX11(null);

			Disposer.RemoveAndDispose(ref this.RenderTargetView);
			Disposer.RemoveAndDispose(ref this.DepthStencilView);
			Disposer.RemoveAndDispose(ref this.RenderTarget);
			Disposer.RemoveAndDispose(ref this.DepthStencil);

			int width = Math.Max((int)base.ActualWidth, 100);
			int height = Math.Max((int)base.ActualHeight, 100);

			Texture2DDescription colordesc = new Texture2DDescription
			{
				BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
				Format = Format.B8G8R8A8_UNorm,
				Width = width,
				Height = height,
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
				Width = width,
				Height = height,
				MipLevels = 1,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				OptionFlags = ResourceOptionFlags.None,
				CpuAccessFlags = CpuAccessFlags.None,
				ArraySize = 1,
			};

			this.RenderTarget = new Texture2D(this._device, colordesc);
			this.DepthStencil = new Texture2D(this._device, depthdesc);
			this.RenderTargetView = new RenderTargetView(this._device, this.RenderTarget);
			this.DepthStencilView = new DepthStencilView(this._device, this.DepthStencil);

			this.D3DSurface.SetRenderTargetDX11(this.RenderTarget);
		}

		private void StartRendering()
		{
			if (this.RenderTimer.IsRunning)
				return;

			CompositionTarget.Rendering += OnRendering;
			this.RenderTimer.Start();
		}

		private void StopRendering()
		{
			if (!this.RenderTimer.IsRunning)
				return;

			CompositionTarget.Rendering -= OnRendering;
			this.RenderTimer.Stop();
		}

		private void OnRendering(object sender, EventArgs e)
		{
			if (!this.RenderTimer.IsRunning)
				return;

			this.Render(this.RenderTimer.Elapsed);
			this.D3DSurface.InvalidateD3DImage();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			if (null != this._device)
			{
				this.CreateAndBindTargets();
				base.OnRenderSizeChanged(sizeInfo);
			}
		}

		private void Render(TimeSpan sceneTime)
		{
			var dev = this._device;
			if (dev == null)
				return;

			var device = new DeviceContext(dev);

			Texture2D renderTarget = this.RenderTarget;
			if (renderTarget == null)
				return;

			int targetWidth = renderTarget.Description.Width;
			int targetHeight = renderTarget.Description.Height;

			device.OutputMerger.SetTargets(this.DepthStencilView, this.RenderTargetView);
			device.Rasterizer.SetViewports(new ViewportF[] { new ViewportF(0, 0, targetWidth, targetHeight, 0.0f, 1.0f) });

			device.ClearRenderTargetView(this.RenderTargetView, this.ClearColor);
			device.ClearDepthStencilView(this.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

			if (this.Scene != null)
			{
				if (!this.SceneAttached)
				{
					this.SceneAttached = true;
					this.RenderScene.Attach(_device, HostSize);
				}

				this.Scene.Update(this.RenderTimer.Elapsed);
				this.Scene.Render();
			}

			device.Flush();
		}

		private void EhIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			// this fires when the screensaver kicks in, the machine goes into sleep or hibernate
			// and any other catastrophic losses of the d3d device from WPF's point of view
			if (this.D3DSurface.IsFrontBufferAvailable)
				this.StartRendering();
			else
				this.StopRendering();
		}

		/// <summary>
		/// Gets a value indicating whether the control is in design mode
		/// (running in Blend or Visual Studio).
		/// </summary>
		public static bool IsInDesignMode
		{
			get
			{
				DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
				bool isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
				return isDesignMode;
			}
		}

		public IScene Scene
		{
			get { return this.RenderScene; }
			set
			{
				if (ReferenceEquals(this.RenderScene, value))
					return;

				if (this.RenderScene != null)
					this.RenderScene.Detach();

				this.SceneAttached = false;
				this.RenderScene = value;
			}
		}

		public SharpDX.Direct3D11.Device Device
		{
			get { return this._device; }
		}

		public PointD2D HostSize
		{
			get
			{
				Texture2D renderTarget = this.RenderTarget;
				if (renderTarget == null)
					return new PointD2D(0, 0);

				return new PointD2D(renderTarget.Description.Width, renderTarget.Description.Height);
			}
		}
	}
}