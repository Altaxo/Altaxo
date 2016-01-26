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

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Geometry;
	using Altaxo.Graph.Graph3D;
	using Altaxo.Graph.Graph3D.Camera;
	using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
	using Altaxo.Graph.Graph3D.Lighting;
	using Altaxo.Gui.Graph3D.Common;
	using Drawing.D3D;
	using SharpDX;
	using SharpDX.D3DCompiler;
	using SharpDX.Direct3D10;
	using SharpDX.DXGI;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Buffer = SharpDX.Direct3D10.Buffer;
	using Device = SharpDX.Direct3D10.Device;

	public class D3D10Scene : ID3D10Scene
	{
		#region Internal structs

		internal struct RenderLayout
		{
			public InputLayout VertexLayout;
			public EffectTechnique technique;
			public EffectPass pass;

			public void Dispose()
			{
				Disposer.RemoveAndDispose(ref VertexLayout);
				Disposer.RemoveAndDispose(ref technique);
				Disposer.RemoveAndDispose(ref pass);
			}
		}

		internal struct VertexAndIndexDeviceBuffer
		{
			public IMaterial Material;
			public Buffer VertexBuffer;
			public Buffer IndexBuffer;
			public int VertexCount;
			public int IndexCount;
			public Plane[] ClipPlanes;

			public void RemoveAndDispose()
			{
				Disposer.RemoveAndDispose(ref VertexBuffer);
				Disposer.RemoveAndDispose(ref IndexBuffer);
				VertexCount = 0;
				IndexCount = 0;
				Material = null;
			}
		}

		#endregion Internal structs

		private Device _hostDevice;

		private PointD2D _hostSize;

		private D3D10GraphicContext _drawing;

		private CameraBase _camera;

		/// <summary>The light settings from AltaxoBase</summary>
		private LightSettings _lightSettings;

		protected Buffer _constantBuffer;

		protected Buffer _constantBufferForColor;

		protected Buffer _constantBufferForSixPlanes;

		private int _renderCounter;

		private string[] _layoutNames = new string[6] { "P", "PC", "PT", "PN", "PNC", "PNT" };

		private RenderLayout[] _renderLayouts = new RenderLayout[6];

		private Effect _lightingEffect;

		/// <summary>
		/// The _this triangle buffers. These buffers are used for current rendering
		/// 0: Position, 1: PositionColor, 2: PositionUV, 3: PositionNormal, 4: PositionNormalColor, 5: PositionNormalUV
		/// </summary>
		private List<VertexAndIndexDeviceBuffer>[] _thisTriangleDeviceBuffers = new List<VertexAndIndexDeviceBuffer>[6];

		private List<VertexAndIndexDeviceBuffer>[] _nextTriangleDeviceBuffers = new List<VertexAndIndexDeviceBuffer>[6];

		// Effect variables

		// Transformation variables
		private EffectConstantBuffer _cbViewTransformation;

		private EffectMatrixVariable _evWorldViewProj;
		private EffectVectorVariable _evEyePosition;

		// Materials
		private EffectConstantBuffer _cbMaterial;

		private EffectVectorVariable _evMaterialDiffuseColor;
		private EffectScalarVariable _evMaterialSpecularExponent;
		private EffectScalarVariable _evMaterialSpecularIntensity;

		// Clip planes
		private EffectConstantBuffer _cbClipPlanes;

		private EffectVectorVariable[] _evClipPlanes = new EffectVectorVariable[6];

		// Lighting
		private Lighting _lighting;

		public void Attach(SharpDX.ComObject hostDevice, PointD2D hostSize)
		{
			Attach((Device)hostDevice, hostSize);
		}

		public void Attach(Device hostDevice, PointD2D hostSize)
		{
			if (hostDevice == null)
				throw new ArgumentNullException(nameof(hostDevice));

			_hostDevice = hostDevice;
			_hostSize = hostSize;

			Device device = _hostDevice;

			using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.Effects.Lighting.cso"))
			{
				if (null == stream)
					throw new InvalidOperationException(string.Format("Compiled shader resource not found: {0}", "Altaxo.CompiledShaders.Effects.Lighting.cso"));

				using (var shaderBytes = ShaderBytecode.FromStream(stream))
				{
					_lightingEffect = new Effect(device, shaderBytes);
				}
			}

			int i;

			for (i = 0; i < _layoutNames.Length; ++i)
			{
				string techniqueName = "Shade_" + _layoutNames[i];
				_renderLayouts[i].technique = this._lightingEffect.GetTechniqueByName(techniqueName);
				_renderLayouts[i].pass = _renderLayouts[i].technique.GetPassByIndex(0);

				if (null == _renderLayouts[i].technique || !_renderLayouts[i].technique.IsValid)
					throw new InvalidProgramException(string.Format("Technique {0} was not found or is invalid", techniqueName));
				if (null == _renderLayouts[i].pass || !_renderLayouts[i].pass.IsValid)
					throw new InvalidProgramException(string.Format("Pass[0] of technique {0} was not found or is invalid", techniqueName));
			}

			i = 0;
			_renderLayouts[i].VertexLayout = new InputLayout(device, _renderLayouts[i].pass.Description.Signature, new[] {
																																new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0)
																								});

			i = 1;
			_renderLayouts[i].VertexLayout = new InputLayout(device, _renderLayouts[i].pass.Description.Signature, new[] {
																																new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
																																new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
																								});

			i = 2;
			_renderLayouts[i].VertexLayout = new InputLayout(device, _renderLayouts[i].pass.Description.Signature, new[] {
																																new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
																																new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
																								});

			i = 3;
			_renderLayouts[i].VertexLayout = new InputLayout(device, _renderLayouts[i].pass.Description.Signature, new[] {
																																new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
																																new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0)
																								});

			i = 4;
			_renderLayouts[i].VertexLayout = new InputLayout(device, _renderLayouts[i].pass.Description.Signature, new[] {
																																new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
																																new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
																																new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0)
																								});

			i = 5;
			_renderLayouts[i].VertexLayout = new InputLayout(device, _renderLayouts[i].pass.Description.Signature, new[] {
																																new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
																																new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
																																new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0)
																								});

			// Create Constant Buffers
			//_constantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);
			//_constantBufferForColor = new Buffer(device, Utilities.SizeOf<Vector4>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);
			//_constantBufferForSixPlanes = new Buffer(device, Utilities.SizeOf<Vector4>() * 6, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);

			// View transformation variables
			_cbViewTransformation = this._lightingEffect.GetConstantBufferByName("cbViewTransformation");
			_evWorldViewProj = _cbViewTransformation.GetMemberByName("WorldViewProj").AsMatrix();
			_evEyePosition = _cbViewTransformation.GetMemberByName("EyePosition").AsVector();

			_cbMaterial = this._lightingEffect.GetConstantBufferByName("cbMaterial");
			_evMaterialDiffuseColor = _cbMaterial.GetMemberByName("MaterialDiffuseColor").AsVector();
			_evMaterialSpecularExponent = _cbMaterial.GetMemberByName("MaterialSpecularExponent").AsScalar();
			_evMaterialSpecularExponent.Set(4.0f);
			_evMaterialSpecularIntensity = _cbMaterial.GetMemberByName("MaterialSpecularIntensity").AsScalar();
			_evMaterialSpecularIntensity.Set(1.0f);

			// Clip plane variables
			_cbClipPlanes = this._lightingEffect.GetConstantBufferByName("cbClipPlanes");
			for (i = 0; i < 6; ++i)
			{
				_evClipPlanes[i] = _cbClipPlanes.GetMemberByName("ClipPlane" + i.ToString(System.Globalization.CultureInfo.InvariantCulture)).AsVector();
			}

			// Lighting variables

			_lighting.Initialize(_lightingEffect);
			_lighting.SetDefaultLighting();

			// --------------------
			if (_drawing != null)
			{
				BringDrawingIntoBuffers(_drawing);
			}
		}

		public void SetHostSize(PointD2D hostSize)
		{
			_hostSize = hostSize;
		}

		public void SetDrawing(D3D10GraphicContext drawing)
		{
			_drawing = drawing;
			BringDrawingIntoBuffers(drawing);
		}

		public void SetCamera(CameraBase camera)
		{
			if (null == camera)
				throw new ArgumentNullException(nameof(camera));

			_camera = camera;
		}

		public void SetLighting(LightSettings lightSettings)
		{
			if (null == lightSettings)
				throw new ArgumentNullException(nameof(lightSettings));

			_lightSettings = lightSettings;
		}

		private void UseNextTriangleDeviceBuffers()
		{
			for (int i = 0; i < _nextTriangleDeviceBuffers.Length; ++i)
			{
				if (null != _nextTriangleDeviceBuffers[i])
				{
					var oldBuffers = _thisTriangleDeviceBuffers[i];
					_thisTriangleDeviceBuffers[i] = System.Threading.Interlocked.Exchange(ref _nextTriangleDeviceBuffers[i], null);

					if (null != oldBuffers)
					{
						foreach (var entry in oldBuffers)
							entry.RemoveAndDispose();
					}
				}
			}
		}

		private void BringDrawingIntoBuffers(D3D10GraphicContext drawing)
		{
			Device device = _hostDevice;
			if (device == null)
				return;

			var buffersOfType =
							new IEnumerable<KeyValuePair<IMaterial, IndexedTriangleBuffer>>[]
							{
																drawing.PositionIndexedTriangleBuffersAsIndexedTriangleBuffers,
																drawing.PositionColorIndexedTriangleBuffersAsIndexedTriangleBuffers,
																drawing.PositionUVIndexedTriangleBuffersAsIndexedTriangleBuffers,
																drawing.PositionNormalIndexedTriangleBuffersAsIndexedTriangleBuffers,
																drawing.PositionNormalColorIndexedTriangleBuffersAsIndexedTriangleBuffers,
																drawing.PositionNormalUVIndexedTriangleBuffersAsIndexedTriangleBuffers
							};

			for (int i = 0; i < buffersOfType.Length; ++i)
			{
				var newDeviceBuffers = new List<VertexAndIndexDeviceBuffer>();
				foreach (var entry in buffersOfType[i])
				{
					var buf = entry.Value;
					if (buf.TriangleCount == 0)
						continue;

					var vertexBuffer = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
					{
						BindFlags = BindFlags.VertexBuffer,
						CpuAccessFlags = CpuAccessFlags.None,
						OptionFlags = ResourceOptionFlags.None,
						SizeInBytes = buf.VertexStreamLength,
						Usage = ResourceUsage.Default
					});

					var indexBuffer = Buffer.Create<int>(device, buf.IndexStream, new BufferDescription()
					{
						BindFlags = BindFlags.IndexBuffer,
						CpuAccessFlags = CpuAccessFlags.None,
						OptionFlags = ResourceOptionFlags.None,
						SizeInBytes = buf.IndexStreamLength,
						Usage = ResourceUsage.Default
					});
					var indexCount = buf.TriangleCount * 3;

					Plane[] clipPlanes = null;
					if (buf is PositionNormalColorIndexedTriangleBufferWithClipping)
					{
						var axoClipPlanes = (buf as PositionNormalColorIndexedTriangleBufferWithClipping).ClipPlanes;
						if (null != axoClipPlanes)
							clipPlanes = axoClipPlanes.Select(axoPlane => new Plane((float)axoPlane.X, (float)axoPlane.Y, (float)axoPlane.Z, (float)-axoPlane.W)).ToArray();
					}

					newDeviceBuffers.Add(new VertexAndIndexDeviceBuffer { Material = entry.Key, VertexBuffer = vertexBuffer, VertexCount = buf.VertexCount, IndexBuffer = indexBuffer, IndexCount = indexCount, ClipPlanes = clipPlanes });
				}

				System.Threading.Interlocked.Exchange(ref _nextTriangleDeviceBuffers[i], newDeviceBuffers);
			}

			device.Flush();
		}

		void IScene.Detach()
		{
			if (null != _nextTriangleDeviceBuffers)
				foreach (var bufType in _nextTriangleDeviceBuffers)
					if (null != bufType)
						foreach (var ele in bufType)
							ele.RemoveAndDispose();

			if (null != _thisTriangleDeviceBuffers)
				foreach (var bufType in _thisTriangleDeviceBuffers)
					if (null != bufType)
						foreach (var ele in bufType)
							ele.RemoveAndDispose();

			foreach (var entry in _renderLayouts)
				entry.Dispose();
		}

		void IScene.Update(TimeSpan sceneTime)
		{
			// use sceneTime.TotalSeconds to update the scene here in dependence on the scene time
		}

		void IScene.Render()
		{
			Device device = _hostDevice;
			if (device == null)
				return;
			if (_camera == null)
				return;
			if (_drawing == null)
				return;

			UseNextTriangleDeviceBuffers();

			float time = _renderCounter / 100f;
			++_renderCounter;

			Matrix worldViewProjTr; // world-view matrix, transposed

			if (null != _camera)
			{
				var cam = _camera;
				var eye = cam.EyePosition;
				var target = cam.TargetPosition;
				var up = cam.UpVector;
				//view = Matrix.LookAtRH(new Vector3((float)eye.X, (float)eye.Y, (float)eye.Z), new Vector3((float)target.X, (float)target.Y, (float)target.Z), new Vector3((float)up.X, (float)up.Y, (float)up.Z));

				//var viewProjD3D = (cam as PerspectiveCamera).GetLookAtRHTimesPerspectiveRHMatrix(_hostSize.Y / _hostSize.X);
				var viewProjD3D = cam.GetViewProjectionMatrix(_hostSize.Y / _hostSize.X);
				worldViewProjTr = new Matrix(
								(float)viewProjD3D.M11, (float)viewProjD3D.M21, (float)viewProjD3D.M31, (float)viewProjD3D.M41,
								(float)viewProjD3D.M12, (float)viewProjD3D.M22, (float)viewProjD3D.M32, (float)viewProjD3D.M42,
								(float)viewProjD3D.M13, (float)viewProjD3D.M23, (float)viewProjD3D.M33, (float)viewProjD3D.M43,
								(float)viewProjD3D.M14, (float)viewProjD3D.M24, (float)viewProjD3D.M34, (float)viewProjD3D.M44
								);
			}
			else
			{
				var view = Matrix.LookAtRH(new Vector3(0, 0, -1500), new Vector3(0, 0, 0), Vector3.UnitY);
				var proj = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)(_hostSize.X / _hostSize.Y), 0.1f, float.MaxValue);
				var viewProj = Matrix.Multiply(view, proj);

				// Update WorldViewProj Matrix
				worldViewProjTr = viewProj;
				worldViewProjTr.Transpose();
			}

			_evWorldViewProj.SetMatrixTranspose(ref worldViewProjTr);
			_evEyePosition.Set(ToVector3(_camera.EyePosition));

			// lighting
			_lighting.SetLighting(_lightSettings, _camera);

			foreach (var entry in _thisTriangleDeviceBuffers[1]) // Position-Color
			{
				DrawPositionColorIndexedTriangleBuffer(device, entry, worldViewProjTr);
			}

			foreach (var entry in _thisTriangleDeviceBuffers[3]) // Position-Normal
			{
				DrawPositionNormalIndexedTriangleBuffer(device, entry, worldViewProjTr);
			}

			foreach (var entry in _thisTriangleDeviceBuffers[4]) // Position-Normal-Color
			{
				DrawPositionNormalColorIndexedTriangleBuffer(device, entry, worldViewProjTr);
			}
		}

		private struct SixPlanes
		{
			public Plane v0, v1, v2, v3, v4, v5;

			public Plane this[int i]
			{
				get
				{
					switch (i)
					{
						case 0:
							return v0;

						case 1:
							return v1;

						case 2:
							return v2;

						case 3:
							return v3;

						case 4:
							return v4;

						case 5:
							return v5;

						default:
							throw new IndexOutOfRangeException();
					}
				}
				set
				{
					switch (i)
					{
						case 0:
							v0 = value;
							break;

						case 1:
							v1 = value;
							break;

						case 2:
							v2 = value;
							break;

						case 3:
							v3 = value;
							break;

						case 4:
							v4 = value;
							break;

						case 5:
							v5 = value;
							break;

						default:
							throw new IndexOutOfRangeException();
					}
				}
			}
		}

		private void DrawPositionColorIndexedTriangleBuffer(Device device, VertexAndIndexDeviceBuffer deviceBuffers, Matrix worldViewProj)
		{
			int layoutNumber = 1;
			device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
			device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

			_evWorldViewProj.SetMatrixTranspose(ref worldViewProj);

			//device.VertexShader.SetConstantBuffer(0, _constantBuffer);
			//device.VertexShader.SetConstantBuffer(1, _constantBufferForSixPlanes);

			//device.VertexShader.Set(_renderLayouts[layoutNumber].VertexShader);
			//device.PixelShader.Set(_renderLayouts[layoutNumber].PixelShader);

			//device.UpdateSubresource(ref worldViewProj, _constantBuffer);

			var planes = new SixPlanes();
			if (null != deviceBuffers.ClipPlanes)
			{
				for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
				{
					planes[i] = deviceBuffers.ClipPlanes[i];
				}
			}

			//device.UpdateSubresource(ref planes, _constantBufferForSixPlanes);

			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 32, 0));
			device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

			_renderLayouts[layoutNumber].pass.Apply();
			device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
		}

		private void DrawPositionNormalColorIndexedTriangleBuffer(Device device, VertexAndIndexDeviceBuffer deviceBuffers, Matrix worldViewProj)
		{
			int layoutNumber = 4;
			device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
			device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

			if (null != deviceBuffers.ClipPlanes)
			{
				for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
				{
					_evClipPlanes[i].Set(deviceBuffers.ClipPlanes[i]);
				}
			}

			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 48, 0));
			device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

			_renderLayouts[layoutNumber].pass.Apply();
			device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);

			if (null != deviceBuffers.ClipPlanes)
			{
				var emptyPlane = new Plane();
				for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
				{
					_evClipPlanes[i].Set(emptyPlane);
				}
			}
		}

		private void DrawPositionNormalIndexedTriangleBuffer(Device device, VertexAndIndexDeviceBuffer deviceBuffers, Matrix worldViewProj)
		{
			int layoutNumber = 3;
			device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
			device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

			Vector4 colorVec = ToVector4(deviceBuffers.Material.Color.Color);
			_evMaterialDiffuseColor.Set(ref colorVec);

			device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 32, 0));
			device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

			_renderLayouts[layoutNumber].pass.Apply();
			device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
		}

		// helper

		private static Vector3 ToVector3(PointD3D a)
		{
			return new Vector3((float)a.X, (float)a.Y, (float)a.Z);
		}

		private static Vector3 ToVector3(VectorD3D a)
		{
			return new Vector3((float)a.X, (float)a.Y, (float)a.Z);
		}

		private static Vector4 ToVector4(PointD3D a)
		{
			return new Vector4((float)a.X, (float)a.Y, (float)a.Z, 1.0f);
		}

		private static Vector3 ToVector3(Altaxo.Drawing.AxoColor color)
		{
			return new Vector3(color.ScR, color.ScG, color.ScB);
		}

		private static Vector3 ToVector3(Altaxo.Drawing.AxoColor color, double amplitude)
		{
			float amp = (float)amplitude;
			return new Vector3(color.ScR * amp, color.ScG * amp, color.ScB * amp);
		}

		private static Vector4 ToVector4(Altaxo.Drawing.AxoColor color)
		{
			return new Vector4(color.ScR, color.ScG, color.ScB, color.ScA);
		}

		private struct Lighting
		{
			public EffectConstantBuffer _cbLighting;

			public EffectVectorVariable HemisphericLightColorBelow;
			public EffectVectorVariable HemisphericLightColorAbove;
			public EffectVectorVariable HemisphericLightBelowToAboveVector;

			public EffectVectorVariable LightPosX;
			public EffectVectorVariable LightPosY;
			public EffectVectorVariable LightPosZ;

			// Light directions
			public EffectVectorVariable LightDirX;

			public EffectVectorVariable LightDirY;
			public EffectVectorVariable LightDirZ;

			public EffectVectorVariable LightColorR;
			public EffectVectorVariable LightColorG;
			public EffectVectorVariable LightColorB;

			public EffectVectorVariable LightRangeRcp; // reciprocal of light range

			public EffectVectorVariable CapsuleLen;

			public EffectVectorVariable SpotCosOuterCone;

			public EffectVectorVariable SpotCosInnerConeRcp;

			private struct SingleLight
			{
				public Vector3 Color; // unused if Color == 0
				public Vector3 Position;
				public Vector3 Direction;
				public float LightRangeRcp;
				public float CapsuleLength;
				public float SpotCosOuterCone;
				public float SpotCosInnerConeRcp;
			}

			private SingleLight[] _singleLights;

			public void Initialize(Effect effect)
			{
				_singleLights = new SingleLight[4];

				_cbLighting = effect.GetConstantBufferByName("cbLights");
				HemisphericLightColorBelow = _cbLighting.GetMemberByName("HemisphericLightColorBelow").AsVector();
				HemisphericLightColorAbove = _cbLighting.GetMemberByName("HemisphericLightColorAbove").AsVector();
				HemisphericLightBelowToAboveVector = _cbLighting.GetMemberByName("HemisphericLightBelowToAboveVector").AsVector();

				LightPosX = _cbLighting.GetMemberByName("LightPosX").AsVector();
				LightPosY = _cbLighting.GetMemberByName("LightPosY").AsVector();
				LightPosZ = _cbLighting.GetMemberByName("LightPosZ").AsVector();

				LightDirX = _cbLighting.GetMemberByName("LightDirX").AsVector();
				LightDirY = _cbLighting.GetMemberByName("LightDirY").AsVector();
				LightDirZ = _cbLighting.GetMemberByName("LightDirZ").AsVector();

				LightColorR = _cbLighting.GetMemberByName("LightColorR").AsVector();
				LightColorG = _cbLighting.GetMemberByName("LightColorG").AsVector();
				LightColorB = _cbLighting.GetMemberByName("LightColorB").AsVector();

				LightRangeRcp = _cbLighting.GetMemberByName("LightRangeRcp").AsVector();
				CapsuleLen = _cbLighting.GetMemberByName("CapsuleLen").AsVector();
				SpotCosOuterCone = _cbLighting.GetMemberByName("SpotCosOuterCone").AsVector();
				SpotCosInnerConeRcp = _cbLighting.GetMemberByName("SpotCosInnerConeRcp").AsVector();
			}

			public void SetDefaultLighting()
			{
				HemisphericLightBelowToAboveVector.Set(new Vector3(0, 0, 1));
				HemisphericLightColorBelow.Set(0.1f * new Vector3(0.55f, 0.5f, 0.5f)); // slightly red
				HemisphericLightColorAbove.Set(new Vector3(0.5f, 0.5f, 0.55f)); // slightly blue

				ClearSingleLight(0);
				ClearSingleLight(1);
				ClearSingleLight(2);
				ClearSingleLight(3);

				SetDirectionalLight(0, Altaxo.Drawing.NamedColors.White.Color, 0.5, new VectorD3D(-2, -1, 1));
				SetPointLight(1, Altaxo.Drawing.NamedColors.White.Color, 0.5, new PointD3D(200, 200, 200), 400);
				SetCapsuleLight(2, Altaxo.Drawing.NamedColors.Red, 1, new PointD3D(400, 200, 200), 500, new VectorD3D(0, 1, 0), 200);

				AssembleLights();
			}

			public void SetLighting(LightSettings lightSettings, CameraBase camera)
			{
				Matrix4x3 cameraM = Matrix4x3.Identity;
				if (lightSettings.IsAnyLightAffixedToCamera)
				{
					// if a light is affixed to the camera, its position is considered to be in camera coordinates
					// but here we need the light in world coordinates
					// cameraM transforms from camera coordinates to world coordinates
					cameraM = camera.InverseLookAtRHMatrix;
				}

				// first ambient light
				var al = lightSettings.AmbientLight;
				SetAmbientLight(al.ColorBelow.Color, al.ColorAbove.Color, al.LightAmplitude, al.IsAffixedToCamera ? cameraM.Transform(al.DirectionBelowToAbove) : al.DirectionBelowToAbove);

				for (int idx = 0; idx < 4; ++idx)
				{
					var l = lightSettings.GetDiscreteLight(idx);
					if (null == l)
					{
						ClearSingleLight(idx);
					}
					else if (l is DirectionalLight)
					{
						var dl = (DirectionalLight)l;
						SetDirectionalLight(
							idx,
							dl.Color.Color,
							dl.LightAmplitude,
							dl.IsAffixedToCamera ? cameraM.Transform(dl.DirectionFromLight) : dl.DirectionFromLight
							);
					}
					else if (l is PointLight)
					{
						var pl = (PointLight)l;
						SetPointLight(
							idx,
							pl.Color.Color,
							pl.LightAmplitude,
							pl.IsAffixedToCamera ? cameraM.Transform(pl.Position) : pl.Position,
							pl.Range
							);
					}
					else if (l is SpotLight)
					{
						var sl = (SpotLight)l;
						SetSpotLight(
							idx,
							sl.Color.Color,
							sl.LightAmplitude,
							sl.IsAffixedToCamera ? cameraM.Transform(sl.Position) : sl.Position,
							sl.IsAffixedToCamera ? cameraM.Transform(sl.DirectionFromLight) : sl.DirectionFromLight,
							sl.Range,
							Math.Cos(sl.OuterConeAngle),
							1 / Math.Cos(sl.InnerConeAngle)
							);
					}
					else
					{
						throw new NotImplementedException(string.Format("The type of lighting ({0}) is not implemented here."));
					}
				}

				AssembleLights();
			}

			public void SetAmbientLight(Altaxo.Drawing.AxoColor colorBelow, Altaxo.Drawing.AxoColor colorAbove, double lightAmplitude, VectorD3D directionBelowToAbove)
			{
				directionBelowToAbove.Normalize();
				HemisphericLightBelowToAboveVector.Set(new Vector3((float)directionBelowToAbove.X, (float)directionBelowToAbove.Y, (float)directionBelowToAbove.Z));
				HemisphericLightColorBelow.Set(ToVector3(colorBelow, lightAmplitude));
				HemisphericLightColorAbove.Set(ToVector3(colorAbove, lightAmplitude));
			}

			public void SetDirectionalLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, VectorD3D directionFromLight)
			{
				directionFromLight.Normalize();
				SetSingleLight(idx, color, colorAmplitude, (PointD3D)(-directionFromLight * 1E7), -directionFromLight, 0, 0, 0, 1);
			}

			public void SetPointLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, double range)
			{
				if (range <= 0)
					throw new ArgumentOutOfRangeException(nameof(range));

				SetSingleLight(idx, color, colorAmplitude, position, new VectorD3D(1, 0, 0), 1 / range, 0, 0, 1);
			}

			public void SetSpotLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, VectorD3D directionFromLight, double range, double cosOuterCone, double cosInnerConeRcp)
			{
				if (range <= 0)
					throw new ArgumentOutOfRangeException(nameof(range));

				SetSingleLight(idx, color, colorAmplitude, position, -directionFromLight, 1 / range, 0, cosOuterCone, cosInnerConeRcp);
			}

			public void SetCapsuleLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, double range, VectorD3D capsuleDirection, double capsuleLength)
			{
				if (range <= 0)
					throw new ArgumentOutOfRangeException(nameof(range));

				SetSingleLight(idx, color, colorAmplitude, position, capsuleDirection, 1 / range, capsuleLength, 0, 1);
			}

			private void SetSingleLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, VectorD3D direction, double lightRangeRcp, double capsuleLength, double spotCosOuterCone, double spotCosInnerConeRcp)
			{
				var sl = new SingleLight()
				{
					Color = ToVector3(color, colorAmplitude),
					Position = ToVector3(position),
					Direction = ToVector3(direction),
					LightRangeRcp = (float)lightRangeRcp,
					CapsuleLength = (float)capsuleLength,
					SpotCosOuterCone = (float)spotCosOuterCone,
					SpotCosInnerConeRcp = (float)spotCosInnerConeRcp
				};

				_singleLights[idx] = sl;
			}

			public void ClearSingleLight(int idx)
			{
				_singleLights[idx] = new SingleLight();
			}

			private void AssembleLights()
			{
				LightPosX.Set(new Vector4(_singleLights[0].Position.X, _singleLights[1].Position.X, _singleLights[2].Position.X, _singleLights[3].Position.X));
				LightPosY.Set(new Vector4(_singleLights[0].Position.Y, _singleLights[1].Position.Y, _singleLights[2].Position.Y, _singleLights[3].Position.Y));
				LightPosZ.Set(new Vector4(_singleLights[0].Position.Z, _singleLights[1].Position.Z, _singleLights[2].Position.Z, _singleLights[3].Position.Z));

				LightDirX.Set(new Vector4(_singleLights[0].Direction.X, _singleLights[1].Direction.X, _singleLights[2].Direction.X, _singleLights[3].Direction.X));
				LightDirY.Set(new Vector4(_singleLights[0].Direction.Y, _singleLights[1].Direction.Y, _singleLights[2].Direction.Y, _singleLights[3].Direction.Y));
				LightDirZ.Set(new Vector4(_singleLights[0].Direction.Z, _singleLights[1].Direction.Z, _singleLights[2].Direction.Z, _singleLights[3].Direction.Z));

				LightColorR.Set(new Vector4(_singleLights[0].Color.X, _singleLights[1].Color.X, _singleLights[2].Color.X, _singleLights[3].Color.X));
				LightColorG.Set(new Vector4(_singleLights[0].Color.Y, _singleLights[1].Color.Y, _singleLights[2].Color.Y, _singleLights[3].Color.Y));
				LightColorB.Set(new Vector4(_singleLights[0].Color.Z, _singleLights[1].Color.Z, _singleLights[2].Color.Z, _singleLights[3].Color.Z));

				LightRangeRcp.Set(new Vector4(_singleLights[0].LightRangeRcp, _singleLights[1].LightRangeRcp, _singleLights[2].LightRangeRcp, _singleLights[3].LightRangeRcp));
				CapsuleLen.Set(new Vector4(_singleLights[0].CapsuleLength, _singleLights[1].CapsuleLength, _singleLights[2].CapsuleLength, _singleLights[3].CapsuleLength));
				SpotCosInnerConeRcp.Set(new Vector4(_singleLights[0].SpotCosInnerConeRcp, _singleLights[1].SpotCosInnerConeRcp, _singleLights[2].SpotCosInnerConeRcp, _singleLights[3].SpotCosInnerConeRcp));
				SpotCosOuterCone.Set(new Vector4(_singleLights[0].SpotCosOuterCone, _singleLights[1].SpotCosOuterCone, _singleLights[2].SpotCosOuterCone, _singleLights[3].SpotCosOuterCone));
			}
		}
	}
}