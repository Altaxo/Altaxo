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

	public partial class D3D10Canvas : Image
	{
		private Device _device;

		private Texture2D _depthStencil;
		private DepthStencilView _depthStencilView;

		private Texture2D _renderTarget;
		private RenderTargetView _renderTargetView;

		private Texture2D _renderTargetIntermediate;
		private RenderTargetView _renderTargetIntermediateView;
		private ShaderResourceView _renderTargetIntermediateShaderResourceView;

		private DX10ImageSource _d3DSurface;

		private IScene _renderScene;
		private bool _isRenderSceneAttached;
		private D3D10GammaCorrector _gammaCorrector;

		public Color4 _renderTargetClearColor = SharpDX.Color.White;

		/// <summary>
		/// Occurs when 3D rendering is ready.
		/// </summary>
		public event EventHandler D3DStarted;

		public D3D10Canvas()
		{
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
			if (D3D10Canvas.IsInDesignMode)
				return;

			this.StartD3D();
			this.Focusable = true;

			D3DStarted?.Invoke(this, EventArgs.Empty);
		}

		private void EhUnloaded(object sender, RoutedEventArgs e)
		{
			if (D3D10Canvas.IsInDesignMode)
				return;

			this.EndD3D();
		}

		private void StartD3D()
		{
			this._device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);

			this._d3DSurface = new DX10ImageSource();
			this._d3DSurface.IsFrontBufferAvailableChanged += EhIsFrontBufferAvailableChanged;

			this.CreateAndBindTargets();

			this.Source = this._d3DSurface;
		}

		private void EndD3D()
		{
			if (this._renderScene != null)
			{
				this._renderScene.Detach();
				this._isRenderSceneAttached = false;
			}

			if (null != this._d3DSurface)
				this._d3DSurface.IsFrontBufferAvailableChanged -= EhIsFrontBufferAvailableChanged;
			this.Source = null;

			Disposer.RemoveAndDispose(ref this._d3DSurface);
			Disposer.RemoveAndDispose(ref this._renderTargetView);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediateView);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediateShaderResourceView);
			Disposer.RemoveAndDispose(ref this._depthStencilView);
			Disposer.RemoveAndDispose(ref this._renderTarget);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediate);
			Disposer.RemoveAndDispose(ref this._depthStencil);
			Disposer.RemoveAndDispose(ref this._gammaCorrector);
			Disposer.RemoveAndDispose(ref this._device);
		}

		private void CreateAndBindTargets()
		{
			if (null != this._d3DSurface)
				this._d3DSurface.SetRenderTargetDX10(null);

			Disposer.RemoveAndDispose(ref this._renderTargetView);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediateView);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediateShaderResourceView);
			Disposer.RemoveAndDispose(ref this._depthStencilView);
			Disposer.RemoveAndDispose(ref this._renderTarget);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediate);
			Disposer.RemoveAndDispose(ref this._depthStencil);
			Disposer.RemoveAndDispose(ref this._gammaCorrector);

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

			Texture2DDescription renderTextureDescriptionForD3D9 = new Texture2DDescription
			{
				BindFlags = BindFlags.None,
				Format = Format.B8G8R8A8_UNorm,
				Width = width,
				Height = height,
				MipLevels = 1,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Staging,
				OptionFlags = ResourceOptionFlags.Shared,
				CpuAccessFlags = CpuAccessFlags.Read,
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

			this._renderTarget = new Texture2D(this._device, colordesc);
			this._renderTargetIntermediate = new Texture2D(this._device, colordesc);
			this._depthStencil = new Texture2D(this._device, depthdesc);
			this._renderTargetIntermediateView = new RenderTargetView(this._device, this._renderTargetIntermediate);
			this._renderTargetIntermediateShaderResourceView = new ShaderResourceView(this._device, this._renderTargetIntermediate);
			this._renderTargetView = new RenderTargetView(this._device, this._renderTarget);
			this._depthStencilView = new DepthStencilView(this._device, this._depthStencil);
			this._gammaCorrector = new D3D10GammaCorrector(_device, "Altaxo.CompiledShaders.Effects.GammaCorrector.cso");

			this._d3DSurface.SetRenderTargetDX10(this._renderTarget);
		}

		/// <summary>
		/// Triggers a rendering, and invalidates the bitmap afterwards
		/// </summary>
		public void TriggerRendering()
		{
			this.Render();
			this._d3DSurface?.InvalidateD3DImage();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			if (null != this._device)
			{
				this.CreateAndBindTargets();
				Scene?.SetHostSize(HostSize);

				base.OnRenderSizeChanged(sizeInfo);

				TriggerRendering();
			}
		}

		private void Render()
		{
			SharpDX.Direct3D10.Device device = this._device;
			if (device == null)
				return;

			Texture2D renderTarget = this._renderTargetIntermediate;
			if (renderTarget == null)
				return;

			int targetWidth = renderTarget.Description.Width;
			int targetHeight = renderTarget.Description.Height;

			device.OutputMerger.SetTargets(this._depthStencilView, this._renderTargetIntermediateView);
			device.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));

			device.ClearRenderTargetView(this._renderTargetIntermediateView, this._renderTargetClearColor);
			device.ClearDepthStencilView(this._depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

			if (this.Scene != null)
			{
				if (!this._isRenderSceneAttached)
				{
					this._isRenderSceneAttached = true;
					this._renderScene.Attach(_device, HostSize);
				}

				this.Scene.Render();
			}

			device.Flush(); // make intermediate render target valid

			// now start a 2nd stage of rendering, in order to gamma-correct the image
			// we use the RenderTextureIntermediate that was the target in the first stage now as a ShaderResource in this 2nd stage
			device.OutputMerger.SetTargets(this._renderTargetView);
			device.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));
			device.ClearRenderTargetView(this._renderTargetView, SharpDX.Color.Black);
			_gammaCorrector.Render(this.Device, this._renderTargetIntermediateShaderResourceView);

			device.Flush(); // make final render target valid
		}

		private void EhIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			// this fires when the screensaver kicks in, the machine goes into sleep or hibernate
			// and any other catastrophic losses of the d3d device from WPF's point of view
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
			get { return this._renderScene; }
			set
			{
				if (ReferenceEquals(this._renderScene, value))
					return;

				if (this._renderScene != null)
					this._renderScene.Detach();

				this._isRenderSceneAttached = false;
				this._renderScene = value;
			}
		}

		public SharpDX.Direct3D10.Device Device
		{
			get { return this._device; }
		}

		public PointD2D HostSize
		{
			get
			{
				Texture2D renderTarget = this._renderTarget;
				if (renderTarget == null)
					return new PointD2D(0, 0);

				return new PointD2D(renderTarget.Description.Width, renderTarget.Description.Height);
			}
		}
	}
}