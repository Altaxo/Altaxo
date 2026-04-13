#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Stores options used when rendering a graph document for the clipboard.
  /// </summary>
  public class ClipboardRenderingOptions : EmbeddedObjectRenderingOptions, ICloneable
  {
    private bool _renderDropFile;
    private ImageFormat _renderDropFileImageFormat;
    private PixelFormat _renderDropFileBitmapPixelFormat;

    private bool _renderEmbeddedObject;
    private bool _renderLinkedObject;

    #region Serialization

    /// <summary>
    /// 2014-09-24 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClipboardRenderingOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ClipboardRenderingOptions)o;

        info.AddBaseValueEmbedded(o, s.GetType().BaseType!);

        info.AddValue("RenderDropFile", s._renderDropFile);
        if (s._renderDropFile)
        {
          info.AddValue("DropFileImageFormat", s._renderDropFileImageFormat);
          info.AddEnum("DropFilePixelFormat", s._renderDropFileBitmapPixelFormat);
        }

        info.AddValue("RenderEmbeddedObject", s._renderEmbeddedObject);
        info.AddValue("RenderLinkedObject", s._renderLinkedObject);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ClipboardRenderingOptions?)o ?? new ClipboardRenderingOptions();

        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);

        s._renderDropFile = info.GetBoolean("RenderDropFile");
        if (s._renderDropFile)
        {
          s._renderDropFileImageFormat = (ImageFormat)info.GetValue("DropFileImageFormat", s);
          s._renderDropFileBitmapPixelFormat = (PixelFormat)info.GetEnum("DropFilePixelFormat", typeof(PixelFormat));
        }

        s._renderEmbeddedObject = info.GetBoolean("RenderEmbeddedObject");
        s._renderLinkedObject = info.GetBoolean("RenderLinkedObject");

        return s;
      }
    }

    #endregion Serialization

    #region Serialization deprecated

    /// <summary>
    /// Designates how to store the copied page in the clipboard.
    /// </summary>
    [Flags]
    private enum GraphCopyPageClipboardFormat
    {
      /// <summary>Store as native image.</summary>
      AsNative = 1,

      /// <summary>Store in a temporary file and set the file name in the clipboard as DropDownList.</summary>
      AsDropDownList = 2,

      /// <summary>
      /// As bitmap wrapped in an enhanced metafile (not applicable if native image is a metafile or enhanced metafile).
      /// </summary>
      AsNativeWrappedInEnhancedMetafile = 4,

      /// <summary>Copy the graph as Com object that can be embedded in another application</summary>
      AsEmbeddedObject = 8,

      /// <summary>
      /// Copy the graph as Com object that can be linked to in another application (is only available if the project has a valid file name).
      /// </summary>
      AsLinkedObject = 16,
    }

    /// <summary>
    /// Initial version (2014-01-31)
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.GraphClipboardExportOptions", 0)]
    private class XmlSerializationSurrogate20140131 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ClipboardRenderingOptions?)o ?? new ClipboardRenderingOptions();

        var oldBase = new GraphExportOptions();
        info.GetBaseValueEmbedded(oldBase, typeof(GraphExportOptions), parent);
        var clipboardFormat = (GraphCopyPageClipboardFormat)info.GetEnum("ClipboardFormat", typeof(GraphCopyPageClipboardFormat));

        s.SourceDpiResolution = oldBase.SourceDpiResolution;
        s.OutputScalingFactor = oldBase.SourceDpiResolution / oldBase.DestinationDpiResolution;
        s.BackgroundBrush = oldBase.BackgroundBrush;
        s._renderDropFileImageFormat = oldBase.ImageFormat;
        s._renderDropFileBitmapPixelFormat = oldBase.PixelFormat;

        s.RenderDropFile = clipboardFormat.HasFlag(GraphCopyPageClipboardFormat.AsDropDownList);
        s.RenderEmbeddedObject = clipboardFormat.HasFlag(GraphCopyPageClipboardFormat.AsEmbeddedObject);
        s.RenderLinkedObject = clipboardFormat.HasFlag(GraphCopyPageClipboardFormat.AsLinkedObject);
        s.RenderEnhancedMetafileAsVectorFormat = !clipboardFormat.HasFlag(GraphCopyPageClipboardFormat.AsNativeWrappedInEnhancedMetafile);

        return s;
      }
    }

    #endregion Serialization deprecated

    #region Construction

    /// <summary>
    /// Initializes a new instance of the <see cref="ClipboardRenderingOptions"/> class.
    /// </summary>
    public ClipboardRenderingOptions()
    {
      _renderDropFile = false;
      _renderDropFileImageFormat = ImageFormat.Png;
      _renderDropFileBitmapPixelFormat = PixelFormat.Format32bppArgb;
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="ClipboardRenderingOptions"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public ClipboardRenderingOptions(ClipboardRenderingOptions from)
    {
      CopyFrom(from ?? throw new ArgumentNullException(nameof(from)));
    }

    /// <summary>
    /// Copies the state from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    [MemberNotNull(nameof(_renderDropFileImageFormat))]
    protected void CopyFrom(ClipboardRenderingOptions from)
    {
      base.CopyFrom(from);
      _renderDropFile = from._renderDropFile;
      _renderDropFileImageFormat = from._renderDropFileImageFormat;
      _renderDropFileBitmapPixelFormat = from._renderDropFileBitmapPixelFormat;

      _renderEmbeddedObject = from._renderEmbeddedObject;
      _renderLinkedObject = from._renderLinkedObject;
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var result = base.CopyFrom(obj);
      var from = obj as ClipboardRenderingOptions;
      if (from is not null)
      {
        CopyFrom(from);
      }
      return result;
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new ClipboardRenderingOptions(this);
    }

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public new ClipboardRenderingOptions Clone()
    {
      return new ClipboardRenderingOptions(this);
    }

    #endregion Construction

    #region Property management

    /// <summary>
    /// Gets the property key used to store clipboard rendering options.
    /// </summary>
    public static readonly Altaxo.Main.Properties.PropertyKey<ClipboardRenderingOptions> PropertyKeyClipboardRenderingOptions = new Altaxo.Main.Properties.PropertyKey<ClipboardRenderingOptions>("DE1819F6-7E8C-4C43-9984-B5C405236289", "Graph\\ClipboardRenderingOptions", Altaxo.Main.Properties.PropertyLevel.All, typeof(Altaxo.Graph.Gdi.GraphDocument), () => new ClipboardRenderingOptions());
    //	public static readonly PropertyKey<GraphClipboardExportOptions> PropertyKeyCopyPageSettings = new PropertyKey<GraphClipboardExportOptions>("DE1819F6-7E8C-4C43-9984-B5C405236289", "Graph\\CopyPageOptions", PropertyLevel.All, typeof(GraphDocument), () => new GraphClipboardExportOptions());

    /// <summary>
    /// Gets or sets the persisted clipboard rendering options.
    /// </summary>
    public static ClipboardRenderingOptions CopyPageOptions
    {
      get
      {
        var doc = Current.PropertyService.GetValue(PropertyKeyClipboardRenderingOptions, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new ClipboardRenderingOptions());
        if (doc is null)
          throw new InvalidProgramException();
        return doc;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        Current.PropertyService.UserSettings.SetValue(PropertyKeyClipboardRenderingOptions, value);
      }
    }

    #endregion Property management

    #region Properies

    /// <summary>
    /// Gets or sets a value indicating whether to render an windows metafile to display the embedded object. Since windows metafile doesn't support
    /// all operations neccessary for vector operations, it is always rendered as windows metafile with an embedded bitmap.
    /// </summary>
    /// <value>
    /// <c>true</c> if a windows metafile should be rendered; otherwise, <c>false</c>.
    /// </value>
    public override bool RenderWindowsMetafile
    {
      get { return _renderWindowsMetafile || (!_renderBitmap && !_renderEnhancedMetafile && !_renderDropFile); }
      set { _renderWindowsMetafile = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a temporary drop file should be rendered.
    /// </summary>
    public bool RenderDropFile
    {
      get { return _renderDropFile; }
      set { _renderDropFile = value; }
    }

    /// <summary>
    /// Gets the image format used for the drop file.
    /// </summary>
    public ImageFormat DropFileImageFormat { get { return _renderDropFileImageFormat; } }

    /// <summary>
    /// Gets the bitmap pixel format used for the drop file.
    /// </summary>
    public PixelFormat DropFileBitmapPixelFormat { get { return _renderDropFileBitmapPixelFormat; } }

    /// <summary>
    /// Gets or sets a value indicating whether an embedded object representation should be rendered.
    /// </summary>
    public bool RenderEmbeddedObject
    {
      get { return _renderEmbeddedObject; }
      set { _renderEmbeddedObject = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a linked object representation should be rendered.
    /// </summary>
    public bool RenderLinkedObject
    {
      get { return _renderLinkedObject; }
      set { _renderLinkedObject = value; }
    }

    /// <summary>
    /// Tries to set the image and pixel format used for the drop file.
    /// </summary>
    /// <param name="imgfmt">The image format.</param>
    /// <param name="pixfmt">The pixel format.</param>
    /// <returns><c>true</c> if the combination is supported; otherwise, <c>false</c>.</returns>
    public bool TrySetImageAndPixelFormat(ImageFormat imgfmt, PixelFormat pixfmt)
    {
      if (!IsVectorFormat(imgfmt) && !CanCreateAndSaveBitmap(imgfmt, pixfmt))
        return false;

      _renderDropFileImageFormat = imgfmt;
      _renderDropFileBitmapPixelFormat = pixfmt;

      return true;
    }

    #endregion Properies

    #region Helper functions

    /// <summary>
    /// Determines whether the specified image format is a vector format.
    /// </summary>
    /// <param name="fmt">The image format.</param>
    /// <returns><c>true</c> if the format is vector based; otherwise, <c>false</c>.</returns>
    public static bool IsVectorFormat(ImageFormat fmt)
    {
      return ImageFormat.Emf == fmt || ImageFormat.Wmf == fmt;
    }

    /// <summary>
    /// Determines whether a bitmap can be created and saved with the specified format combination.
    /// </summary>
    /// <param name="imgfmt">The image format.</param>
    /// <param name="pixfmt">The pixel format.</param>
    /// <returns><c>true</c> if the bitmap can be created and saved; otherwise, <c>false</c>.</returns>
    public static bool CanCreateAndSaveBitmap(ImageFormat imgfmt, PixelFormat pixfmt)
    {
      try
      {
        using (var bmp = new Bitmap(8, 8, pixfmt))
        {
          using (var str = new System.IO.MemoryStream())
          {
            bmp.Save(str, imgfmt);
            str.Close();
          }
        }

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Converts the <see cref="CopyPageOptions"/> to <see cref="GraphExportOptions"/>.
    /// </summary>
    /// <returns>The <see cref="GraphExportOptions"/> retrieved from the <see cref="CopyPageOptions"/>.</returns>
    /// <summary>
    /// Converts the current clipboard rendering options to graph export options.
    /// </summary>
    /// <returns>The graph export options derived from <see cref="CopyPageOptions"/>.</returns>
    public static GraphExportOptions GetGraphExportOptionsFromCopyPageOptions()
    {
      var options = CopyPageOptions;
      var graphExportOptions = new GraphExportOptions
      {
        BackgroundBrush = options.BackgroundBrush,
        SourceDpiResolution = options.SourceDpiResolution,
        DestinationDpiResolution = options.SourceDpiResolution / options.OutputScalingFactor,
      };
      graphExportOptions.TrySetImageAndPixelFormat(options.DropFileImageFormat, options.DropFileBitmapPixelFormat);
      return graphExportOptions;
    }

    #endregion Helper functions
  }
}
