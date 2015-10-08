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
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Graph3D;
	using Altaxo.Graph3D.GraphicsContext.D3D;
	using Altaxo.Gui.Graph3D.Common;
	using SharpDX;
	using SharpDX.D3DCompiler;
	using SharpDX.Direct3D10;
	using SharpDX.DXGI;
	using System;
	using System.Linq;
	using Buffer = SharpDX.Direct3D10.Buffer;
	using Device = SharpDX.Direct3D10.Device;

	public class Scene : IScene
	{
		private ISceneHost Host;

		private InputLayout VertextLayout_Position;
		private InputLayout VertextLayout_PositionNormal;

		private InputLayout VertextLayout_PositionColor;
		private InputLayout VertextLayout_PositionNormalColor;

		private InputLayout VertextLayout_PositionUV;
		private InputLayout VertextLayout_PositionNormalUV;

		private DataStream VertexStream;
		private Buffer _nonindexedTriangleVerticesPositionColor;
		private int _nonindexedTriangleVerticesPositionColor_Count;

		private Effect SimpleEffect;
		private Color4 OverlayColor = new Color4(1.0f);

		private Buffer IndexedVertices;
		private Buffer IndexedVerticesIndexes;
		private int IndexedVerticesIndexes_Count;

		private D3D10GraphicContext _drawing;

		private Altaxo.Graph3D.SceneSettings _sceneSettings;

		protected Buffer _constantBuffer;

		protected Buffer _constantBufferForColor;

		private int _renderCounter;

		private VertexShader vertexShader_PC;

		private PixelShader pixelShader_PC;

		private VertexShader vertexShader_PN;

		private PixelShader pixelShader_PN;

		private VertexShader vertexShader_PNC;

		private PixelShader pixelShader_PNC;

		void IScene.Attach(ISceneHost host)
		{
			this.Host = host;

			Device device = host.Device;
			if (device == null)
				throw new Exception("Scene host device is null");

			var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.VS.MiniCube.cso");
			ShaderBytecode vertexShaderByteCode = ShaderBytecode.FromStream(stream);
			vertexShader_PC = new VertexShader(device, vertexShaderByteCode);

			stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.PS.MiniCube.cso");
			ShaderBytecode pixelShaderByteCode = ShaderBytecode.FromStream(stream);
			pixelShader_PC = new PixelShader(device, pixelShaderByteCode);

			stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.VS.PN_DULL.cso");
			ShaderBytecode vertexShaderByteCode_PN = ShaderBytecode.FromStream(stream);
			vertexShader_PN = new VertexShader(device, vertexShaderByteCode_PN);

			stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.PS.PN_DULL.cso");
			ShaderBytecode pixelShaderByteCode_PN = ShaderBytecode.FromStream(stream);
			pixelShader_PN = new PixelShader(device, pixelShaderByteCode_PN);

			stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.VS.PNC_DULL.cso");
			ShaderBytecode vertexShaderByteCode_PNC = ShaderBytecode.FromStream(stream);
			vertexShader_PNC = new VertexShader(device, vertexShaderByteCode_PNC);

			stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.PS.PNC_DULL.cso");
			ShaderBytecode pixelShaderByteCode_PNC = ShaderBytecode.FromStream(stream);
			pixelShader_PNC = new PixelShader(device, pixelShaderByteCode_PNC);

			/*
			this.VertextLayout_Position = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[] {
								new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0)
						});

				*/

			this.VertextLayout_PositionNormal = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode_PN), new[] {
								new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
								new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0)
						});

			this.VertextLayout_PositionColor = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[] {
								new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
								new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
						});

			this.VertextLayout_PositionNormalColor = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode_PNC), new[] {
								new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
								new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
								new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0)
						});
			/*
			this.VertextLayout_PositionUV = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[] {
								new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
								new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
						});

			this.VertextLayout_PositionNormalUV = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[] {
								new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
								new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
								new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0)
						});

			*/

			// Create Constant Buffer
			_constantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);
			_constantBufferForColor = new Buffer(device, Utilities.SizeOf<Vector4>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);
			/*
			// Prepare All the stages
			device.InputAssembler.InputLayout = this.VertextLayout_PositionColor;
			//			device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			//			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
			device.VertexShader.SetConstantBuffer(0, _constantBuffer);
			device.VertexShader.Set(vertexShader_PC);
			//			device.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
			device.PixelShader.Set(pixelShader_PC);
			*/

			if (_drawing != null)
			{
				BringDrawingIntoBuffers(_drawing);
			}
			else
			{
				this.VertexStream = new DataStream(3 * 32, true, true);
				this.VertexStream.WriteRange(new[] {
								new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1f),
								new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1f),
								new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1f)
						});
				this.VertexStream.Position = 0;

				this._nonindexedTriangleVerticesPositionColor = new Buffer(device, this.VertexStream, new BufferDescription()
				{
					BindFlags = BindFlags.VertexBuffer,
					CpuAccessFlags = CpuAccessFlags.None,
					OptionFlags = ResourceOptionFlags.None,
					SizeInBytes = 3 * 32,
					Usage = ResourceUsage.Default
				}
				);
				device.Flush();
			}
		}

		public void SetDrawing(D3D10GraphicContext drawing)
		{
			var olddrawing = _drawing;
			_drawing = drawing;

			if (olddrawing != null)
				olddrawing.Dispose();

			BringDrawingIntoBuffers(drawing);
		}

		public void SetSceneSettings(Altaxo.Graph3D.SceneSettings sceneSettings)
		{
			_sceneSettings = sceneSettings;
		}

		private void BringDrawingIntoBuffers(D3D10GraphicContext drawing)
		{
			Device device = this.Host?.Device;
			if (device == null)
				return;
			/*

			// IndexedTriangles Position Color

			{
				Disposer.RemoveAndDispose(ref this.IndexedVertices);
				Disposer.RemoveAndDispose(ref this.IndexedVerticesIndexes);
				var buf = drawing.BuffersIndexTrianglesPositionColor;
				if (null != buf && buf.TriangleCount > 0)
				{
					//buf.VertexStream.Position = 0;
					this.IndexedVertices = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
					{
						BindFlags = BindFlags.VertexBuffer,
						CpuAccessFlags = CpuAccessFlags.None,
						OptionFlags = ResourceOptionFlags.None,
						SizeInBytes = buf.VertexStreamLength,
						Usage = ResourceUsage.Default
					});

					//buf.IndexStream.Position = 0;
					this.IndexedVerticesIndexes = Buffer.Create<int>(device, buf.IndexStream, new BufferDescription()
					{
						BindFlags = BindFlags.IndexBuffer,
						CpuAccessFlags = CpuAccessFlags.None,
						OptionFlags = ResourceOptionFlags.None,
						SizeInBytes = buf.IndexStreamLength,
						Usage = ResourceUsage.Default
					});
					this.IndexedVerticesIndexes_Count = buf.TriangleCount * 3;
				}
			}

			device.Flush();

	*/
		}

		void IScene.Detach()
		{
			Disposer.RemoveAndDispose(ref this._nonindexedTriangleVerticesPositionColor);
			Disposer.RemoveAndDispose(ref this.VertextLayout_PositionColor);
			Disposer.RemoveAndDispose(ref this.SimpleEffect);
			Disposer.RemoveAndDispose(ref this.VertexStream);
		}

		void IScene.Update(TimeSpan sceneTime)
		{
			float t = (float)(0.5 * (1 + Math.Sin(sceneTime.TotalSeconds)));
			this.OverlayColor = new Color4(t, t, t, t);
		}

		void IScene.Render()
		{
			Device device = this.Host.Device;
			if (device == null)
				return;

			float time = _renderCounter / 100f;
			++_renderCounter;

			// Prepare matrices
			//var view = Matrix.LookAtLH(new Vector3(0, 0, -1500), new Vector3(0, 0, 0), Vector3.UnitY);
			//var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)(Host.HostSize.X / Host.HostSize.Y), 0.1f, float.MaxValue);

			Matrix view, proj;

			if (null != _sceneSettings)
			{
				var cam = _sceneSettings.Camera;
				var eye = cam.EyePosition;
				var target = cam.TargetPosition;
				var up = cam.UpVector;
				view = Matrix.LookAtRH(new Vector3((float)eye.X, (float)eye.Y, (float)eye.Z), new Vector3((float)target.X, (float)target.Y, (float)target.Z), new Vector3((float)up.X, (float)up.Y, (float)up.Z));
				if (cam is Altaxo.Graph3D.Camera.PerspectiveCamera)
				{
					var angle = (cam as Altaxo.Graph3D.Camera.PerspectiveCamera).Angle;
					proj = Matrix.PerspectiveFovRH((float)angle, (float)(Host.HostSize.X / Host.HostSize.Y), 0.1f, float.MaxValue);
				}
				else if (cam is Altaxo.Graph3D.Camera.OrthographicCamera)
				{
					var scale = (cam as Altaxo.Graph3D.Camera.OrthographicCamera).Scale;
					proj = Matrix.OrthoRH((float)scale, (float)(scale * Host.HostSize.Y / Host.HostSize.X), 1.0f, 2000.0f);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			else
			{
				view = Matrix.LookAtRH(new Vector3(0, 0, -1500), new Vector3(0, 0, 0), Vector3.UnitY);
				proj = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)(Host.HostSize.X / Host.HostSize.Y), 0.1f, float.MaxValue);
			}
			var viewProj = Matrix.Multiply(view, proj);

			// Update WorldViewProj Matrix
			//var worldViewProj = Matrix.Translation(-300.0f, -300.0f, -300.0f) * Matrix.RotationX(time) * Matrix.RotationY(time * 2) * Matrix.RotationZ(time * .7f) * viewProj;
			var worldViewProj = viewProj;
			worldViewProj.Transpose();
			//device.UpdateSubresource(ref worldViewProj, _constantBuffer);

			foreach (var entry in _drawing.PositionColorIndexedTriangleBuffers)
			{
				DrawPositionColorIndexedTriangleBuffer(device, entry.Value, entry.Key, worldViewProj);
			}

			foreach (var entry in _drawing.PositionNormalIndexedTriangleBuffers)
			{
				DrawPositionNormalIndexedTriangleBuffer(device, entry.Value, entry.Key, worldViewProj);
			}

			foreach (var entry in _drawing.PositionNormalColorIndexedTriangleBuffers)
			{
				DrawPositionNormalColorIndexedTriangleBuffer(device, entry.Value, entry.Key, worldViewProj);
			}
		}

		private void DrawPositionColorIndexedTriangleBuffer(Device device, PositionColorIndexedTriangleBuffer buf, IMaterial3D material, Matrix worldViewProj)
		{
			// Create Constant Buffer
			//_constantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);

			// Prepare All the stages
			device.InputAssembler.InputLayout = this.VertextLayout_PositionNormal;
			//			device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			//			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
			device.VertexShader.SetConstantBuffer(0, _constantBuffer);
			device.VertexShader.Set(vertexShader_PC);
			//			device.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
			device.PixelShader.Set(pixelShader_PC);

			device.UpdateSubresource(ref worldViewProj, _constantBuffer);

			// hier

			Disposer.RemoveAndDispose(ref this.IndexedVertices);
			Disposer.RemoveAndDispose(ref this.IndexedVerticesIndexes);

			if (null == buf || buf.TriangleCount == 0)
				return;

			//buf.VertexStream.Position = 0;
			this.IndexedVertices = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
			{
				BindFlags = BindFlags.VertexBuffer,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = buf.VertexStreamLength,
				Usage = ResourceUsage.Default
			});

			//buf.IndexStream.Position = 0;
			this.IndexedVerticesIndexes = Buffer.Create<int>(device, buf.IndexStream, new BufferDescription()
			{
				BindFlags = BindFlags.IndexBuffer,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = buf.IndexStreamLength,
				Usage = ResourceUsage.Default
			});
			this.IndexedVerticesIndexes_Count = buf.TriangleCount * 3;

			device.InputAssembler.InputLayout = this.VertextLayout_PositionColor;
			device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.IndexedVertices, 32, 0));
			device.InputAssembler.SetIndexBuffer(this.IndexedVerticesIndexes, Format.R32_UInt, 0);
			device.DrawIndexed(IndexedVerticesIndexes_Count, 0, 0);
		}

		private void DrawPositionNormalIndexedTriangleBuffer(Device device, PositionNormalIndexedTriangleBuffer buf, IMaterial3D material, Matrix worldViewProj)
		{
			// Create Constant Buffer
			//_constantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);

			// Prepare All the stages
			device.InputAssembler.InputLayout = this.VertextLayout_PositionNormal;
			//			device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			//			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
			device.VertexShader.SetConstantBuffer(0, _constantBuffer);
			device.VertexShader.SetConstantBuffer(1, _constantBufferForColor);

			device.VertexShader.Set(vertexShader_PN);
			//			device.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
			device.PixelShader.Set(pixelShader_PN);

			device.UpdateSubresource(ref worldViewProj, _constantBuffer);

			var color = material.Color.Color;
			Vector4 colorVec = new Vector4(color.ScR, color.ScG, color.ScB, color.ScA);
			device.UpdateSubresource(ref colorVec, _constantBufferForColor);

			Disposer.RemoveAndDispose(ref this.IndexedVertices);
			Disposer.RemoveAndDispose(ref this.IndexedVerticesIndexes);

			if (null == buf || buf.TriangleCount == 0)
				return;

			//buf.VertexStream.Position = 0;
			this.IndexedVertices = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
			{
				BindFlags = BindFlags.VertexBuffer,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = buf.VertexStreamLength,
				Usage = ResourceUsage.Default
			});

			//buf.IndexStream.Position = 0;
			this.IndexedVerticesIndexes = Buffer.Create<int>(device, buf.IndexStream, new BufferDescription()
			{
				BindFlags = BindFlags.IndexBuffer,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = buf.IndexStreamLength,
				Usage = ResourceUsage.Default
			});
			this.IndexedVerticesIndexes_Count = buf.TriangleCount * 3;

			device.InputAssembler.InputLayout = this.VertextLayout_PositionNormal;
			device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.IndexedVertices, 32, 0));
			device.InputAssembler.SetIndexBuffer(this.IndexedVerticesIndexes, Format.R32_UInt, 0);
			device.DrawIndexed(IndexedVerticesIndexes_Count, 0, 0);
		}

		private void DrawPositionNormalColorIndexedTriangleBuffer(Device device, PositionNormalColorIndexedTriangleBuffer buf, IMaterial3D material, Matrix worldViewProj)
		{
			// Create Constant Buffer
			//		_constantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);

			// Prepare All the stages
			device.InputAssembler.InputLayout = this.VertextLayout_PositionNormalColor;
			device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			//			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
			device.VertexShader.SetConstantBuffer(0, _constantBuffer);
			device.VertexShader.Set(vertexShader_PNC);
			//			device.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
			device.PixelShader.Set(pixelShader_PNC);

			device.UpdateSubresource(ref worldViewProj, _constantBuffer);

			Disposer.RemoveAndDispose(ref this.IndexedVertices);
			Disposer.RemoveAndDispose(ref this.IndexedVerticesIndexes);

			if (null == buf || buf.TriangleCount == 0)
				return;

			//buf.VertexStream.Position = 0;
			this.IndexedVertices = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
			{
				BindFlags = BindFlags.VertexBuffer,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = buf.VertexStreamLength,
				Usage = ResourceUsage.Default
			});

			//buf.IndexStream.Position = 0;
			this.IndexedVerticesIndexes = Buffer.Create<int>(device, buf.IndexStream, new BufferDescription()
			{
				BindFlags = BindFlags.IndexBuffer,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = buf.IndexStreamLength,
				Usage = ResourceUsage.Default
			});
			this.IndexedVerticesIndexes_Count = buf.TriangleCount * 3;

			device.InputAssembler.InputLayout = this.VertextLayout_PositionNormalColor;
			device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.IndexedVertices, 48, 0));
			device.InputAssembler.SetIndexBuffer(this.IndexedVerticesIndexes, Format.R32_UInt, 0);
			device.DrawIndexed(IndexedVerticesIndexes_Count, 0, 0);
		}
	}
}