#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Altaxo.Gui.Common.MultiRename;
using Altaxo.Main.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Stores options used to display graphs embedded in other applications.
	/// </summary>
	public class EmbeddedObjectRenderingOptions : ICloneable
	{
		private double _sourceDpiResolution;
		private double _outputScalingFactor;

		private bool _renderEnhancedMetafile; // can be rendered as true metafile or as enhanced metafile with included bitmap
		private bool _renderEnhancedMetafileAsVectorFormat; // if true, use a true enhanced metafile
		private bool _renderWindowsMetafile; // has to be rendered as Metafile with included bitmap
		private bool _renderBitmap; // rendered as bitmap plus DIB bitmap
		private NamedColor _backgroundColorForFormatsWithoutAlphaChannel;
		private BrushX _backgroundBrush;

		#region Serialization

		/// <summary>
		/// Initial version (2014-09-19)
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.EmbeddedObjectRenderingOptions", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (EmbeddedObjectRenderingOptions)obj;

				info.AddValue("SourceResolution", s._sourceDpiResolution);
				info.AddValue("BackgroundForFormatsWithoutAlphaChannel", s._backgroundColorForFormatsWithoutAlphaChannel);
				info.AddValue("BackgroundBrush", s._backgroundBrush);
				info.AddValue("RenderEnhancedMetafile", s._renderEnhancedMetafile);
				info.AddValue("RenderEnhancedMetafileAsVectorFormat", s._renderEnhancedMetafileAsVectorFormat);
				info.AddValue("RenderWindowsMetafile", s._renderWindowsMetafile);
				info.AddValue("RenderBitmap", s._renderBitmap);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (EmbeddedObjectRenderingOptions)o : new EmbeddedObjectRenderingOptions();

				s._sourceDpiResolution = info.GetDouble("SourceResolution");
				s._backgroundColorForFormatsWithoutAlphaChannel = (Altaxo.Graph.NamedColor)info.GetValue("BackgroundForFormatsWithoutAlphaChannel");
				s.BackgroundBrush = (BrushX)info.GetValue("Background");
				s._renderEnhancedMetafile = info.GetBoolean("RenderEnhancedMetafile");
				s._renderEnhancedMetafileAsVectorFormat = info.GetBoolean("RenderEnhancedMetafileAsVectorFormat");
				s._renderWindowsMetafile = info.GetBoolean("RenderWindowsMetafile");
				s._renderBitmap = info.GetBoolean("RenderBitmap");

				return s;
			}
		}

		/// <summary>
		/// 2014-09-24 added OutputScalingFactor
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EmbeddedObjectRenderingOptions), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (EmbeddedObjectRenderingOptions)obj;

				info.AddValue("SourceResolution", s._sourceDpiResolution);
				info.AddValue("OutputScaling", s._outputScalingFactor);
				info.AddValue("BackgroundForFormatsWithoutAlphaChannel", s._backgroundColorForFormatsWithoutAlphaChannel);
				info.AddValue("BackgroundBrush", s._backgroundBrush);
				info.AddValue("RenderEnhancedMetafile", s._renderEnhancedMetafile);
				info.AddValue("RenderEnhancedMetafileAsVectorFormat", s._renderEnhancedMetafileAsVectorFormat);
				info.AddValue("RenderWindowsMetafile", s._renderWindowsMetafile);
				info.AddValue("RenderBitmap", s._renderBitmap);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (EmbeddedObjectRenderingOptions)o : new EmbeddedObjectRenderingOptions();

				s._sourceDpiResolution = info.GetDouble("SourceResolution");
				s._outputScalingFactor = info.GetDouble("OutputScaling");
				s._backgroundColorForFormatsWithoutAlphaChannel = (Altaxo.Graph.NamedColor)info.GetValue("BackgroundForFormatsWithoutAlphaChannel");
				s.BackgroundBrush = (BrushX)info.GetValue("Background");
				s._renderEnhancedMetafile = info.GetBoolean("RenderEnhancedMetafile");
				s._renderEnhancedMetafileAsVectorFormat = info.GetBoolean("RenderEnhancedMetafileAsVectorFormat");
				s._renderWindowsMetafile = info.GetBoolean("RenderWindowsMetafile");
				s._renderBitmap = info.GetBoolean("RenderBitmap");

				return s;
			}
		}

		#endregion Serialization

		#region Construction

		public EmbeddedObjectRenderingOptions()
		{
			_sourceDpiResolution = 300;
			_renderEnhancedMetafile = false;
			_renderEnhancedMetafileAsVectorFormat = false;
			_renderWindowsMetafile = true;
			_renderBitmap = true;
			_backgroundColorForFormatsWithoutAlphaChannel = Altaxo.Graph.NamedColors.White;
			_backgroundBrush = null;
			_outputScalingFactor = 1;
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as EmbeddedObjectRenderingOptions;
			if (null != from)
			{
				_sourceDpiResolution = from._sourceDpiResolution;
				_outputScalingFactor = from._outputScalingFactor;

				_renderEnhancedMetafile = from._renderEnhancedMetafile; // can be rendered as true metafile or as enhanced metafile with included bitmap
				_renderEnhancedMetafileAsVectorFormat = from._renderEnhancedMetafileAsVectorFormat; // if true, use a true enhanced metafile
				_renderWindowsMetafile = from._renderWindowsMetafile; // has to be rendered as Metafile with included bitmap
				_renderBitmap = from._renderBitmap; // rendered as bitmap plus DIB bitmap
				_backgroundColorForFormatsWithoutAlphaChannel = from._backgroundColorForFormatsWithoutAlphaChannel;
				_backgroundBrush = null == from._backgroundBrush ? null : from._backgroundBrush.Clone();

				return true;
			}
			return false;
		}

		public EmbeddedObjectRenderingOptions(EmbeddedObjectRenderingOptions from)
		{
			if (null == from)
				throw new ArgumentNullException();
			CopyFrom(from);
		}

		public EmbeddedObjectRenderingOptions Clone()
		{
			return new EmbeddedObjectRenderingOptions(this);
		}

		object ICloneable.Clone()
		{
			return new EmbeddedObjectRenderingOptions(this);
		}

		#endregion Construction

		#region Property management

		public static readonly Altaxo.Main.Properties.PropertyKey<EmbeddedObjectRenderingOptions> PropertyKeyEmbeddedObjectRenderingOptions =
			new Altaxo.Main.Properties.PropertyKey<EmbeddedObjectRenderingOptions>(
				"1030D700-CB3B-445B-95D8-88E1ECFE78C0",
				"Graph\\EmbeddedRenderingOptions",
				Altaxo.Main.Properties.PropertyLevel.Project | Main.Properties.PropertyLevel.ProjectFolder | Main.Properties.PropertyLevel.Document,
				typeof(Altaxo.Graph.Gdi.GraphDocument),
				null);

		#endregion Property management

		/// <summary>
		/// Gets or sets the dpi resolution of the bitmap or bitmap embedded in a metafile that is rendered.
		/// </summary>
		/// <value>
		/// The source dpi resolution.
		/// </value>
		/// <exception cref="System.ArgumentException">SourceDpiResolution has to be >0</exception>
		public double SourceDpiResolution
		{
			get
			{
				return _sourceDpiResolution;
			}
			set
			{
				if (!(value > 0))
					throw new ArgumentException("SourceDpiResolution has to be >0");

				_sourceDpiResolution = value;
			}
		}

		public double OutputScalingFactor
		{
			get
			{
				return _outputScalingFactor;
			}
			set
			{
				if (value > 0 && value <= double.MaxValue)
					_outputScalingFactor = value;
				else
					throw new ArgumentOutOfRangeException(string.Format("OutputScalingFactor is {0} and therefore outside valid range.", value));
			}
		}

		public BrushX BackgroundBrush
		{
			get
			{
				return _backgroundBrush;
			}
			set
			{
				_backgroundBrush = value;
			}
		}

		public Altaxo.Graph.NamedColor BackgroundColorForFormatsWithoutAlphaChannel
		{
			get
			{
				return _backgroundColorForFormatsWithoutAlphaChannel;
			}
			set
			{
				if (value.Color.A != 255)
					throw new ArgumentException("Provided color has to be opaque!");
				_backgroundColorForFormatsWithoutAlphaChannel = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether an enhanced metafile should be rendered to display the embedded object.
		/// </summary>
		/// <value>
		/// <c>true</c> if an enhanced metafile should be rendered; otherwise, <c>false</c>.
		/// </value>
		public bool RenderEnhancedMetafile
		{
			get { return _renderEnhancedMetafile; }
			set { _renderEnhancedMetafile = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether when rendering an enhanced metafile, it is rendered as metafile with included bitmap or is rendered as true vector metafile.
		/// </summary>
		/// <value>
		/// <c>true</c> if the metafile is rendered in true vector format,  otherwise, <c>false</c>.
		/// </value>
		public bool RenderEnhancedMetafileAsVectorFormat
		{
			get { return _renderEnhancedMetafileAsVectorFormat; }
			set { _renderEnhancedMetafileAsVectorFormat = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to render an windows metafile to display the embedded object. Since windows metafile doesn't support
		/// all operations neccessary for vector operations, it is always rendered as windows metafile with an embedded bitmap.
		/// </summary>
		/// <value>
		/// <c>true</c> if a windows metafile should be rendered; otherwise, <c>false</c>.
		/// </value>
		public bool RenderWindowsMetafile
		{
			get { return _renderWindowsMetafile || (!_renderBitmap && !_renderEnhancedMetafile); }
			set { _renderWindowsMetafile = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether a bitmap should be rendered to display the embedded object.
		/// </summary>
		/// <value>
		///   <c>true</c> if a bitmap should be rendered; otherwise, <c>false</c>.
		/// </value>
		public bool RenderBitmap
		{
			get { return _renderBitmap; }
			set { _renderBitmap = value; }
		}
	}
}