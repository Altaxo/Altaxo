#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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


namespace Altaxo.Gui.Graph.Graph3D.Common
{
  using System;
  using Altaxo.Shaders;
  using Vortice.Direct3D11;
  using Buffer = Vortice.Direct3D11.ID3D11Buffer;
  using Device = Vortice.Direct3D11.ID3D11Device;
  using Format = Vortice.DXGI.Format;

  public class D3D11GammaCorrector : IDisposable
  {
    private bool _isDisposed;
    private ID3D11InputLayout _vertexLayout;
    private Buffer _vertices;
    private ID3D11VertexShader _vertexShader;
    private ID3D11PixelShader _pixelShader;

    private ID3D11VertexShader CreateVertexShader(ID3D11Device device, string entryPoint, out byte[] vertexShaderBytes)
    {
      var resourceName = $"Altaxo.CompiledShaders.GammaCorrector_{entryPoint}.cso";
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {
        if (stream is null)
          throw new InvalidOperationException($"Compiled shader resource not found: {resourceName}");

        vertexShaderBytes = new byte[stream.Length];
        stream.Read(vertexShaderBytes, 0, vertexShaderBytes.Length);
        return device.CreateVertexShader(vertexShaderBytes);
      }
    }

    private ID3D11PixelShader CreatePixelShader(ID3D11Device device, string entryPoint, out byte[] pixelShaderBytes)
    {
      var resourceName = $"Altaxo.CompiledShaders.GammaCorrector_{entryPoint}.cso";
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {
        if (stream is null)
          throw new InvalidOperationException($"Compiled shader resource not found: {resourceName}");

        pixelShaderBytes = new byte[stream.Length];
        stream.Read(pixelShaderBytes, 0, pixelShaderBytes.Length);
        return device.CreatePixelShader(pixelShaderBytes);
      }
    }


    public void Attach(Device device)
    {
      if (device is null)
        throw new ArgumentNullException(nameof(device));

      _vertexShader = CreateVertexShader(device, "VS", out var vertexShaderBytes);
      _pixelShader = CreatePixelShader(device, "PS", out var _);



      this._vertexLayout = device.CreateInputLayout(
                   new[] {
                                new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                    },
                   vertexShaderBytes);

      this._vertices = device.CreateBuffer(
        new ReadOnlySpan<float>(
        new[]
          {
           // 3D coordinates              UV Texture coordinates
           -1.0f,  1.0f, 0.5f, 1.0f,      0.0f, 0.0f,
            1.0f, -1.0f, 0.5f, 1.0f,      1.0f, 1.0f,
           -1.0f, -1.0f, 0.5f, 1.0f,      0.0f, 1.0f,
           -1.0f,  1.0f, 0.5f, 1.0f,      0.0f, 0.0f,
            1.0f,  1.0f, 0.5f, 1.0f,      1.0f, 0.0f,
            1.0f, -1.0f, 0.5f, 1.0f,      1.0f, 1.0f,
           }),

        new BufferDescription()
        {
          BindFlags = BindFlags.VertexBuffer,
          CPUAccessFlags = CpuAccessFlags.None,
          MiscFlags = ResourceOptionFlags.None,
          ByteWidth = 8 * 6 * 4,
          Usage = ResourceUsage.Default
        }
        );
    }

    public void Detach(Device device)
    {
      if (device is null)
        throw new ArgumentNullException(nameof(device));

      Disposer.RemoveAndDispose(ref this._vertexLayout);
      Disposer.RemoveAndDispose(ref this._vertices);
      Disposer.RemoveAndDispose(ref this._vertexShader);
      Disposer.RemoveAndDispose(ref this._pixelShader);
    }
    #region IDisposable Support

    ~D3D11GammaCorrector()
    {
      Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        Disposer.RemoveAndDispose(ref _vertexLayout);
        Disposer.RemoveAndDispose(ref _vertices);
        Disposer.RemoveAndDispose(ref _vertexShader);
        Disposer.RemoveAndDispose(ref _pixelShader);

        _isDisposed = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support
    public void Render(Device device, ID3D11ShaderResourceView textureView)
    {
      if (device?.ImmediateContext is { } context)
      {
        context.VSSetShader(_vertexShader);
        context.PSSetShader(_pixelShader);
        context.IASetInputLayout(this._vertexLayout);
        context.IASetPrimitiveTopology(Vortice.Direct3D.PrimitiveTopology.TriangleList);
        context.IASetVertexBuffer(0, this._vertices, 24, 0);
        context.PSSetShaderResource(GammaCorrectorHlsl.ShaderTexture_RegisterNumber, textureView); // use register 7 for the texture
        context.Draw(6, 0);
        context.Flush();
      }
    }
  }
}
