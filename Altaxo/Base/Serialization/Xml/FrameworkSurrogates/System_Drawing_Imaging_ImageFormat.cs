using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Xml.FrameworkSurrogates
{
	using System.Drawing.Imaging;

	internal class System_Drawing_Imaging_ImageFormat
	{
		private static readonly ImageFormat[] ImageFormats = { ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Exif, ImageFormat.Gif, ImageFormat.Icon, ImageFormat.Jpeg, ImageFormat.MemoryBmp, ImageFormat.Png, ImageFormat.Tiff, ImageFormat.Wmf };

		#region Serialization

		/// <summary>
		/// Initial version (2014-01-18)
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Imaging.ImageFormat), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ImageFormat)obj;

				info.AddValue("Guid", System.Xml.XmlConvert.ToString(s.Guid));
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var guid = System.Xml.XmlConvert.ToGuid(info.GetString("Guid"));

				var s = ImageFormats.FirstOrDefault(x => x.Guid == guid);

				if (null == s)
					s = new ImageFormat(guid);

				return s;
			}
		}

		#endregion Serialization
	}
}