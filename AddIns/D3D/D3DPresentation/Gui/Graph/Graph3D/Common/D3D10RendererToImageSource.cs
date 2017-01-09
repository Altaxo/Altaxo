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

namespace Altaxo.Gui.Graph.Graph3D.Common
{
	using Altaxo.Geometry;
	using Altaxo.Graph;
	using SharpDX;
	using SharpDX.Direct3D10;
	using SharpDX.DXGI;
	using System;
	using System.ComponentModel;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Media;
	using Device = SharpDX.Direct3D10.Device1;

	/// <summary>
	/// Supports rendering of a scene (<see cref="IScene"/>) to an <see cref="D3D10ImageSource"/>.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public class D3D10RendererToImageSource : IDisposable
	{
		private Device _device;

		private Texture2D _depthStencil;
		private DepthStencilView _depthStencilView;

		private Texture2D _renderTarget;
		private RenderTargetView _renderTargetView;

		private Texture2D _renderTargetIntermediate;
		private RenderTargetView _renderTargetIntermediateView;
		private ShaderResourceView _renderTargetIntermediateShaderResourceView;

		private D3D10ImageSource _d3dImageSource;

		private IScene _renderScene;
		private bool _isRenderSceneAttached;
		private D3D10GammaCorrector _gammaCorrector;

		public Color4 _renderTargetClearColor = SharpDX.Color.White;

		private bool _isDisposed;

		public int InstanceID { get; private set; }
		private static int _instanceCounter;

		/// <summary>
		/// Initializes a new instance of the <see cref="D3D10RendererToImageSource"/> class.
		/// </summary>
		/// <param name="scene">The scene to render.</param>
		/// <param name="d3dImageSource">The D3D image source, which is the target of the rendering.</param>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		public D3D10RendererToImageSource(IScene scene, D3D10ImageSource d3dImageSource)
		{
			InstanceID = ++_instanceCounter;

			if (null == scene)
				throw new ArgumentNullException(nameof(scene));

			if (null == d3dImageSource)
				throw new ArgumentNullException(nameof(d3dImageSource));

			this.Scene = scene;
			this._d3dImageSource = d3dImageSource;
			this._d3dImageSource.IsFrontBufferAvailableChanged += EhIsFrontBufferAvailableChanged;
		}

		~D3D10RendererToImageSource()
		{
			if (!_isDisposed)
				throw new InvalidProgramException("Object was not disposed!");
		}

		public void Dispose()
		{
			_isDisposed = true;
			EndD3D();

			_d3dImageSource.IsFrontBufferAvailableChanged -= EhIsFrontBufferAvailableChanged;
			_d3dImageSource = null;
		}

		/// <summary>
		/// Must be called when the render size has changed. Adjusts the render size, but does not trigger a new rendering. Call <see cref="Render"/> afterwards.
		/// If you provide zero for both <paramref name="sizeX"/> and <paramref name="sizeY"/>, you can free resources in case the rendering is not used currently.
		/// </summary>
		/// <param name="sizeX">The render size x component in pixels.</param>
		/// <param name="sizeY">The render size y component in pixels.</param>
		public void SetRenderSize(int sizeX, int sizeY)
		{
			if (_isDisposed)
				throw new ObjectDisposedException("this");

			if (null == _device)
			{
				this._device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
			}

			Scene?.SetHostSize(new PointD2D(sizeX, sizeY));
			CreateAndBindTargets(sizeX, sizeY);
		}

		/// <summary>
		/// Triggers a rendering, and invalidates the bitmap afterwards
		/// </summary>
		public void TriggerRendering()
		{
			this.Render();
			this._d3dImageSource?.InvalidateD3DImage();
		}

		private void EndD3D()
		{
			if (this._renderScene != null)
			{
				this._renderScene.Detach();
				this._isRenderSceneAttached = false;
			}

			CreateAndBindTargets(0, 0);

			Disposer.RemoveAndDispose(ref this._device);
		}

		private void CreateAndBindTargets(int sizeX, int sizeY)
		{
			_d3dImageSource.SetRenderTargetDX10(null);

			Disposer.RemoveAndDispose(ref this._renderTargetView);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediateView);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediateShaderResourceView);
			Disposer.RemoveAndDispose(ref this._depthStencilView);
			Disposer.RemoveAndDispose(ref this._renderTarget);
			Disposer.RemoveAndDispose(ref this._renderTargetIntermediate);
			Disposer.RemoveAndDispose(ref this._depthStencil);
			Disposer.RemoveAndDispose(ref this._gammaCorrector);

			if (sizeX >= 2 && sizeY >= 2)
			{
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

				Texture2DDescription renderTextureDescriptionForD3D9 = new Texture2DDescription
				{
					BindFlags = BindFlags.None,
					Format = Format.B8G8R8A8_UNorm,
					Width = sizeX,
					Height = sizeY,
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
					Width = sizeX,
					Height = sizeY,
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

				this._d3dImageSource.SetRenderTargetDX10(this._renderTarget);
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

			device.ClearDepthStencilView(this._depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

			if (this.Scene == null)
			{
				device.ClearRenderTargetView(this._renderTargetIntermediateView, this._renderTargetClearColor);
			}
			else // if (this.Scene != null)
			{
				if (!this._isRenderSceneAttached)
				{
					this._isRenderSceneAttached = true;
					this._renderScene.Attach(_device, new PointD2D(renderTarget.Description.Width, renderTarget.Description.Height));
				}

				// Attention: it is now the Render function of the scene that is responsible for clearing the render target

				var renderTargetClearColor = _renderTargetClearColor;
				var sceneBack = this.Scene.SceneBackgroundColor;
				if (sceneBack.HasValue)
					renderTargetClearColor = new Color4((float)sceneBack.Value.ScR, (float)sceneBack.Value.ScG, (float)sceneBack.Value.ScB, (float)sceneBack.Value.ScA);
				device.ClearRenderTargetView(this._renderTargetIntermediateView, renderTargetClearColor);

				this.Scene.Render();
			}

			device.Flush(); // make intermediate render target valid

			// now start a 2nd stage of rendering, in order to gamma-correct the image
			// we use the RenderTextureIntermediate that was the target in the first stage now as a ShaderResource in this 2nd stage
			device.OutputMerger.SetTargets(this._renderTargetView);
			device.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));
			device.ClearRenderTargetView(this._renderTargetView, SharpDX.Color.Black);
			_gammaCorrector.Render(this._device, this._renderTargetIntermediateShaderResourceView);

			device.Flush(); // make final render target valid
		}

		private void EhIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			// this fires when the screensaver kicks in, the machine goes into sleep or hibernate
			// and any other catastrophic losses of the d3d device from WPF's point of view

			if (true == (bool)e.NewValue)
			{
				_d3dImageSource.SetRenderTargetDX10(_renderTarget);
				TriggerRendering();
			}
		}

		/// <summary>
		/// Gets the rendering scene.
		/// </summary>
		/// <value>
		/// The rendering scene.
		/// </value>
		public IScene Scene
		{
			get { return this._renderScene; }
			private set
			{
				if (ReferenceEquals(this._renderScene, value))
					return;

				if (this._renderScene != null)
					this._renderScene.Detach();

				this._isRenderSceneAttached = false;
				this._renderScene = value;
			}
		}
	}
}