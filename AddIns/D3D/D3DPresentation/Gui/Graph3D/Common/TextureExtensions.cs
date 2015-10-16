using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Common
{
	public static class Texture2DExtensions
	{
		public static void Save(this Texture2D texture)
		{
			var textureCopy = new Texture2D(texture.Device, new Texture2DDescription
			{
				Width = (int)texture.Description.Width,
				Height = (int)texture.Description.Height,
				MipLevels = 1,
				ArraySize = 1,
				Format = texture.Description.Format,
				Usage = ResourceUsage.Staging,
				SampleDescription = new SampleDescription(1, 0),
				BindFlags = BindFlags.None,
				CpuAccessFlags = CpuAccessFlags.Read,
				OptionFlags = ResourceOptionFlags.None
			});

			texture.Device.CopyResource(texture, textureCopy);

			DataStream dataStream;

			DataRectangle dataRectangle = textureCopy.Map(0, MapMode.Read, SharpDX.Direct3D10.MapFlags.None);

			var imagingFactory = new ImagingFactory();
			var bitmap = new Bitmap(
					imagingFactory,
					textureCopy.Description.Width,
					textureCopy.Description.Height,
					PixelFormat.Format32bppBGRA,
					dataRectangle);

			using (var s = new System.IO.FileStream(@"C:\temp\d3dimage.png", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
			{
				s.Position = 0;
				using (var bitmapEncoder = new PngBitmapEncoder(imagingFactory, s))
				{
					using (var bitmapFrameEncode = new BitmapFrameEncode(bitmapEncoder))
					{
						bitmapFrameEncode.Initialize();
						bitmapFrameEncode.SetSize(bitmap.Size.Width, bitmap.Size.Height);
						var pixelFormat = PixelFormat.FormatDontCare;
						bitmapFrameEncode.SetPixelFormat(ref pixelFormat);
						bitmapFrameEncode.WriteSource(bitmap);
						bitmapFrameEncode.Commit();
						bitmapEncoder.Commit();
					}
				}
			}

			textureCopy.Unmap(0);
			textureCopy.Dispose();
			bitmap.Dispose();
		}
	}
}