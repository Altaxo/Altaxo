using System;

namespace System.Drawing
{
	
	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Color),0)]
	public class ColorXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		static System.Drawing.ColorConverter sm_Converter = new System.Drawing.ColorConverter();

		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			System.Drawing.Color s = (System.Drawing.Color)obj;
			/*
			info.AddValue("A",(int)s.A);
			info.AddValue("R",(int)s.R);
			info.AddValue("G",(int)s.G);
			info.AddValue("B",(int)s.B);
			*/

			info.AddValue("Value",sm_Converter.ConvertToInvariantString(s));
		}
		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
		{
			/*
			int a = info.GetInt32("A");
			int r = info.GetInt32("R");
			int g = info.GetInt32("G");
			int b = info.GetInt32("B");
			return System.Drawing.Color.FromArgb(a,r,g,b);
			*/

			string val = info.GetString("Value");
			return (Color)sm_Converter.ConvertFromInvariantString(val);
		}
	}


	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Font),0)]
	public class FontXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		System.Drawing.FontConverter sm_Converter = new System.Drawing.FontConverter();

		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			System.Drawing.Font s = (System.Drawing.Font)obj;
			
			info.AddValue("Value",sm_Converter.ConvertToInvariantString(s));
			

		}
		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
		{
			string val = info.GetString("Value");
			return (Font)sm_Converter.ConvertFromInvariantString(val);
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.SizeF),0)]
	public class SizeFXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{

		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			System.Drawing.SizeF s = (System.Drawing.SizeF)obj;
			
			info.AddValue("Width",s.Width);
			info.AddValue("Height",s.Height);
		}
		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
		{
			float w = info.GetSingle("Width");
			float h = info.GetSingle("Height");
			return new SizeF(w,h);
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.PointF),0)]
	public class PointFXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{

		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			System.Drawing.PointF s = (System.Drawing.PointF)obj;
			
			info.AddValue("X",s.X);
			info.AddValue("Y",s.Y);
		}
		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
		{
			float x = info.GetSingle("X");
			float y = info.GetSingle("Y");
			return new PointF(x,y);
		}
	}

	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.RectangleF),0)]
	public class RectangleFXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{

		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			System.Drawing.RectangleF s = (System.Drawing.RectangleF)obj;
					
			info.AddValue("X",s.X);
			info.AddValue("Y",s.Y);
			info.AddValue("Width",s.Width);
			info.AddValue("Height",s.Height);
		}
		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
		{
			float x = info.GetSingle("X");
			float y = info.GetSingle("Y");
			float w = info.GetSingle("Width");
			float h = info.GetSingle("Height");
			return new System.Drawing.RectangleF(x,y,w,h);
		}
	}



} // end namespace System.Drawing
