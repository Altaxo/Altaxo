#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
  using Geometry;
  using Graph;
  using Graph.Gdi;
  using UnmanagedApi.Kernel32;
  using UnmanagedApi.Ole32;

  public class GraphDocumentDataObject : DataObjectBase, System.Runtime.InteropServices.ComTypes.IDataObject
  {
    private ManagedDataAdviseHolder _dataAdviseHolder;
    private AltaxoDocument _altaxoMiniProject;
    private string _graphDocumentName;
    private PointD2D _graphDocumentSize;
    private System.Drawing.Imaging.Metafile _graphDocumentMetafileImage;
    private System.Drawing.Bitmap _graphDocumentBitmapImage;
    private string _graphDocumentDropdownFileName;
    private ClipboardRenderingOptions _graphExportOptions;

    public GraphDocumentDataObject(GraphDocumentBase graphDocument, ProjectFileComObject fileComObject, ComManager comManager)
      : base(comManager)
    {
      ComDebug.ReportInfo("{0} constructor.", GetType().Name);
      _dataAdviseHolder = new ManagedDataAdviseHolder();

      _graphDocumentName = graphDocument.Name;
      _graphDocumentSize = graphDocument.Size;

      _graphExportOptions = graphDocument.GetPropertyValue(ClipboardRenderingOptions.PropertyKeyClipboardRenderingOptions, () => new ClipboardRenderingOptions()).Clone();
      var embeddedRenderingOptions = graphDocument.GetPropertyValue(EmbeddedObjectRenderingOptions.PropertyKeyEmbeddedObjectRenderingOptions, () => null);
      if (null != embeddedRenderingOptions)
        _graphExportOptions.CopyFrom(embeddedRenderingOptions); // merge embedded rendering options

      if ((_graphExportOptions.RenderEnhancedMetafile && _graphExportOptions.RenderEnhancedMetafileAsVectorFormat) ||
          (_graphExportOptions.RenderDropFile && _graphExportOptions.DropFileImageFormat == System.Drawing.Imaging.ImageFormat.Emf)
        )
      {
        if (graphDocument is Altaxo.Graph.Gdi.GraphDocument)
          _graphDocumentMetafileImage = GraphDocumentExportActions.RenderAsEnhancedMetafileVectorFormat((Altaxo.Graph.Gdi.GraphDocument)graphDocument, _graphExportOptions);
      }

      if (null == _graphDocumentMetafileImage ||
        _graphExportOptions.RenderBitmap ||
        _graphExportOptions.RenderWindowsMetafile ||
        (_graphExportOptions.RenderEnhancedMetafile && !_graphExportOptions.RenderEnhancedMetafileAsVectorFormat) ||
        _graphExportOptions.RenderDropFile)
      {
        if (graphDocument is Altaxo.Graph.Gdi.GraphDocument)
          _graphDocumentBitmapImage = GraphDocumentExportActions.RenderAsBitmap((Altaxo.Graph.Gdi.GraphDocument)graphDocument, _graphExportOptions.BackgroundBrush, System.Drawing.Imaging.PixelFormat.Format32bppArgb, _graphExportOptions.SourceDpiResolution, _graphExportOptions.SourceDpiResolution / _graphExportOptions.OutputScalingFactor);
        else if (graphDocument is Altaxo.Graph.Graph3D.GraphDocument)
          _graphDocumentBitmapImage = Altaxo.Graph.Graph3D.GraphDocumentExportActions.RenderAsBitmap((Altaxo.Graph.Graph3D.GraphDocument)graphDocument, _graphExportOptions.BackgroundBrush, System.Drawing.Imaging.PixelFormat.Format32bppArgb, _graphExportOptions.SourceDpiResolution, _graphExportOptions.SourceDpiResolution / _graphExportOptions.OutputScalingFactor);
        else
          throw new NotImplementedException();
      }

      if (_graphExportOptions.RenderEmbeddedObject)
      {
        var miniProjectBuilder = new Altaxo.Graph.Procedures.MiniProjectBuilder();
        _altaxoMiniProject = miniProjectBuilder.GetMiniProject(graphDocument, true);
      }
      else
      {
        _altaxoMiniProject = null;
      }
    }

    ~GraphDocumentDataObject()
    {
      ComDebug.ReportInfo("{0} destructor.", GetType().Name);

      if (null != _dataAdviseHolder)
      {
        _dataAdviseHolder.Dispose();
        _dataAdviseHolder = null;
      }
    }

    #region Base class overrides

    protected override IList<Rendering> Renderings
    {
      get
      {
        var list = new List<Rendering>();

        if (null != _altaxoMiniProject && _graphExportOptions.RenderEmbeddedObject)
        {
          list.Add(new Rendering(DataObjectHelper.CF_EMBEDSOURCE, TYMED.TYMED_ISTORAGE, null));
          list.Add(new Rendering(DataObjectHelper.CF_OBJECTDESCRIPTOR, TYMED.TYMED_HGLOBAL, RenderEmbeddedObjectDescriptor));
        }

        if (_graphExportOptions.RenderEnhancedMetafile)
        {
          list.Add(new Rendering(CF.CF_ENHMETAFILE, TYMED.TYMED_ENHMF, RenderEnhancedMetaFile));
        }

        if (_graphExportOptions.RenderWindowsMetafile)
        {
          list.Add(new Rendering(CF.CF_METAFILEPICT, TYMED.TYMED_MFPICT, RenderWindowsMetafilePict));
        }

        if (_graphExportOptions.RenderBitmap)
        {
          list.Add(new Rendering(CF.CF_BITMAP, TYMED.TYMED_GDI, RenderBitmap));
          list.Add(new Rendering(CF.CF_DIB, TYMED.TYMED_HGLOBAL, RenderBitmapDIB));
          list.Add(new Rendering(CF.CF_DIBV5, TYMED.TYMED_HGLOBAL, RenderBitmapDIBV5));
        }

        if (_graphExportOptions.RenderDropFile)
        {
          list.Add(new Rendering(CF.CF_HDROP, TYMED.TYMED_HGLOBAL, RenderBitmapAsDropFile));
        }

        if (_graphExportOptions.RenderLinkedObject && !string.IsNullOrEmpty(Current.IProjectService.CurrentProjectFileName))
        {
          list.Add(new Rendering(DataObjectHelper.CF_LINKSOURCE, TYMED.TYMED_ISTREAM, RenderMoniker));
          list.Add(new Rendering(DataObjectHelper.CF_LINKSRCDESCRIPTOR, TYMED.TYMED_HGLOBAL, RenderLinkedObjectDescriptor));
        }

        return list;
      }
    }

    public override void ConvertToNetDataObjectAndPutToClipboard()
    {
      var result = new System.Windows.Forms.DataObject();

      if (null != _graphDocumentMetafileImage)
      {
        result.SetImage(_graphDocumentMetafileImage);
      }
      else if (null != _graphDocumentBitmapImage)
      {
        result.SetImage(_graphDocumentBitmapImage);
      }

      if (!string.IsNullOrEmpty(_graphDocumentDropdownFileName))
      {
        var coll = new System.Collections.Specialized.StringCollection();
        EnsureDropFileCreated();
        coll.Add(_graphDocumentDropdownFileName);
        result.SetFileDropList(coll);
      }

      System.Windows.Forms.Clipboard.SetDataObject(result, true);
    }

    private void EnsureDropFileCreated()
    {
      var fileExtension = GraphExportOptions.GetDefaultFileNameExtension(_graphExportOptions.DropFileImageFormat);
      _graphDocumentDropdownFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "AltaxoClipboardImage" + fileExtension);
      if (System.IO.File.Exists(_graphDocumentDropdownFileName))
      {
        try
        {
          System.IO.File.Delete(_graphDocumentDropdownFileName);
        }
        catch (Exception)
        {
          _graphDocumentDropdownFileName = null;
          return;
        }
      }

      if (_graphExportOptions.DropFileImageFormat == System.Drawing.Imaging.ImageFormat.Emf)
      {
        if (!(null != _graphDocumentMetafileImage))
          throw new InvalidOperationException(nameof(_graphDocumentMetafileImage) + " should be != null");

        var clonedMF = (System.Drawing.Imaging.Metafile)_graphDocumentMetafileImage.Clone(); // have to clone metafile because after calling GetHenhmetafile() metafile would be destroyed
        DataObjectHelper.SaveMetafileToDisk(clonedMF.GetHenhmetafile(), _graphDocumentDropdownFileName);
      }
      else if (_graphExportOptions.DropFileImageFormat == System.Drawing.Imaging.ImageFormat.Wmf)
      {
        using (var rgbBitmap = GraphDocumentExportActions.ConvertBitmapToPixelFormat(_graphDocumentBitmapImage, System.Drawing.Imaging.PixelFormat.Format24bppRgb, _graphExportOptions.BackgroundColorForFormatsWithoutAlphaChannel))
        {
          var scaledDocSize = _graphDocumentSize * _graphExportOptions.OutputScalingFactor;
          using (var enhancedMetafile = GraphDocumentExportActions.RenderAsEnhancedMetafileBitmapFormat(rgbBitmap, scaledDocSize))
          {
            var hEmf = enhancedMetafile.GetHenhmetafile();
            var bytes = DataObjectHelper.ConvertEnhancedMetafileToWindowsMetafileBytes(hEmf);
            var placeableHeaderBytes = DataObjectHelper.GetWmfPlaceableHeaderBytes(scaledDocSize);

            using (var stream = new System.IO.FileStream(_graphDocumentDropdownFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read))
            {
              stream.Write(placeableHeaderBytes, 0, placeableHeaderBytes.Length);
              stream.Write(bytes, 0, bytes.Length);
            }
          }
        }
      }
      else // bitmap format
      {
        var bitmapToSave =
          _graphExportOptions.DropFileBitmapPixelFormat == _graphDocumentBitmapImage.PixelFormat ?
          _graphDocumentBitmapImage :
          GraphDocumentExportActions.ConvertBitmapToPixelFormat(_graphDocumentBitmapImage, _graphExportOptions.DropFileBitmapPixelFormat, _graphExportOptions.BackgroundColorForFormatsWithoutAlphaChannel);

        bitmapToSave.Save(_graphDocumentDropdownFileName, _graphExportOptions.DropFileImageFormat);
      }
    }

    protected override ManagedDataAdviseHolder DataAdviseHolder
    {
      get { return _dataAdviseHolder; }
    }

    protected override bool InternalGetDataHere(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
    {
      if (format.cfFormat == DataObjectHelper.CF_EMBEDSOURCE && (format.tymed & TYMED.TYMED_ISTORAGE) != 0)
      {
        medium.tymed = TYMED.TYMED_ISTORAGE;
        medium.pUnkForRelease = null;
        var stg = (IStorage)Marshal.GetObjectForIUnknown(medium.unionmember);
        // we don't save the document directly, since this would mean to save the whole (and probably huge) project
        // instead we first make a mini project with the neccessary data only and then save this instead
        InternalSaveMiniProject(stg, _altaxoMiniProject, _graphDocumentName);
        return true;
      }

      if (format.cfFormat == DataObjectHelper.CF_LINKSOURCE && (format.tymed & TYMED.TYMED_ISTREAM) != 0)
      {
        // we should make sure that ComManager is already started, so that the moniker can be used by the program
        if (!_comManager.IsActive)
          _comManager.StartLocalServer();

        IMoniker documentMoniker = CreateNewDocumentMoniker();

        if (null != documentMoniker)
        {
          medium.tymed = TYMED.TYMED_ISTREAM;
          medium.pUnkForRelease = null;
          var strm = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
          DataObjectHelper.SaveMonikerToStream(documentMoniker, strm);
          return true;
        }
      }

      return false;
    }

    #endregion Base class overrides

    #region Renderings

    private IntPtr RenderEnhancedMetaFile(TYMED tymed)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.RenderEnhancedMetafile");

      if (!(tymed == TYMED.TYMED_ENHMF))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_TYMED_ENHMF");

      if (null != _graphDocumentMetafileImage)
      {
        var mfCloned = (System.Drawing.Imaging.Metafile)_graphDocumentMetafileImage.Clone();
        return mfCloned.GetHenhmetafile();
      }
      else if (null != _graphDocumentBitmapImage)
      {
        var scaledDocSize = _graphDocumentSize * _graphExportOptions.OutputScalingFactor;
        return GraphDocumentExportActions.RenderAsEnhancedMetafileBitmapFormat(_graphDocumentBitmapImage, scaledDocSize).GetHenhmetafile();
      }
      else
      {
        throw new InvalidProgramException("Please report this exception to the author of the program and describe the steps to reproduce the exception");
      }
    }

    private IntPtr RenderWindowsMetafilePict(TYMED tymed)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.RenderWindowsMetafilePict");

      if (!(tymed == TYMED.TYMED_MFPICT))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_MFPICT");

      if (null != _graphDocumentBitmapImage)
      {
        using (var rgbBitmap = GraphDocumentExportActions.ConvertBitmapToPixelFormat(_graphDocumentBitmapImage, System.Drawing.Imaging.PixelFormat.Format24bppRgb, _graphExportOptions.BackgroundColorForFormatsWithoutAlphaChannel))
        {
          var scaledDocSize = _graphDocumentSize * _graphExportOptions.OutputScalingFactor;

          using (var enhancedMetafile = GraphDocumentExportActions.RenderAsEnhancedMetafileBitmapFormat(rgbBitmap, scaledDocSize))
          {
            var hEmf = enhancedMetafile.GetHenhmetafile();
            return DataObjectHelper.ConvertEnhancedMetafileToWindowsMetafilePict(hEmf, scaledDocSize.X, scaledDocSize.Y);
          }
        }
      }
      else
      {
        throw new InvalidProgramException("Please report this exception to the author of the program and describe the steps to reproduce the exception");
      }
    }

    private IntPtr RenderBitmap(TYMED tymed)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.RenderBitmap");

      if (!(tymed == TYMED.TYMED_GDI))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_GDI");

      if (null != _graphDocumentBitmapImage)
      {
        using (var convertedBitmap = GraphDocumentExportActions.ConvertBitmapToPixelFormat(_graphDocumentBitmapImage, System.Drawing.Imaging.PixelFormat.Format24bppRgb, _graphExportOptions.BackgroundColorForFormatsWithoutAlphaChannel))
        {
          return DataObjectHelper.RenderGdiBitmapToTYMED_GDI(convertedBitmap);
        }
      }
      else
      {
        throw new InvalidProgramException("Please report this exception to the author of the program and describe the steps to reproduce the exception");
      }
    }

    private IntPtr RenderBitmapDIB(TYMED tymed)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.RenderBitmapDIB");

      if (!(tymed == TYMED.TYMED_HGLOBAL))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_HGLOBAL");

      if (null != _graphDocumentBitmapImage)
      {
        using (var convertedBitmap = GraphDocumentExportActions.ConvertBitmapToPixelFormat(_graphDocumentBitmapImage, System.Drawing.Imaging.PixelFormat.Format24bppRgb, _graphExportOptions.BackgroundColorForFormatsWithoutAlphaChannel))
        {
          return DataObjectHelper.RenderDIBBitmapToHGLOBAL(convertedBitmap);
        }
      }
      else
      {
        throw new InvalidProgramException("Please report this exception to the author of the program and describe the steps to reproduce the exception");
      }
    }

    private IntPtr RenderBitmapDIBV5(TYMED tymed)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.RenderBitmapDIBV5");

      if (!(tymed == TYMED.TYMED_HGLOBAL))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_HGLOBAL");

      if (null != _graphDocumentBitmapImage)
      {
        return DataObjectHelper.RenderDIBV5BitmapToHGLOBAL(_graphDocumentBitmapImage);
      }
      else
      {
        throw new InvalidProgramException("Please report this exception to the author of the program and describe the steps to reproduce the exception");
      }
    }

    private IntPtr RenderBitmapAsDropFile(TYMED tymed)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.BitmapAsDropFile");

      if (!(tymed == TYMED.TYMED_HGLOBAL))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_HGLOBAL");

      EnsureDropFileCreated();

      if (!string.IsNullOrEmpty(_graphDocumentDropdownFileName))
      {
        return DataObjectHelper.RenderFiles(new string[] { _graphDocumentDropdownFileName });
      }
      return IntPtr.Zero;
    }

    private IntPtr RenderMoniker(TYMED tymed)
    {
      ComDebug.ReportInfo("{0}.RenderMoniker", GetType().Name);

      if (!(tymed == TYMED.TYMED_ISTREAM))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_ISTREAM");

      return DataObjectHelper.RenderMonikerToNewStream(tymed, CreateNewDocumentMoniker());
    }

    public static int MiscStatus(int aspect)
    {
      int misc = (int)OLEMISC.OLEMISC_CANTLINKINSIDE;
      if (aspect == (int)DVASPECT.DVASPECT_CONTENT)
      {
        // misc |= (int)OLEMISC.OLEMISC_RECOMPOSEONRESIZE;
        misc |= (int)OLEMISC.OLEMISC_RENDERINGISDEVICEINDEPENDENT;
      }
      return misc;
    }

    public IntPtr RenderEmbeddedObjectDescriptor(TYMED tymed)
    {
      return RenderEmbeddedObjectDescriptor(tymed, _graphDocumentSize);
    }

    public static IntPtr RenderEmbeddedObjectDescriptor(TYMED tymed, PointD2D graphDocumentSize)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.RenderEmbeddedObjectDescriptor");

      // Brockschmidt, Inside Ole 2nd ed. page 991
      if (!(tymed == TYMED.TYMED_HGLOBAL))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_HGLOBAL");
      // Fill in the basic information.
      var od = new OBJECTDESCRIPTOR
      {
        // According to the documentation this is used just to find an icon.
        clsid = typeof(GraphDocumentEmbeddedComObject).GUID,
        dwDrawAspect = DVASPECT.DVASPECT_CONTENT,
        sizelcx = 0,  // zero in imitation of Word/Excel, but could be (int)(graphDocumentSize.X * 2540 / 72.0)
        sizelcy = 0,  // zero in imitation of Word/Excel, but could be (int)(graphDocumentSize.Y * 2540 / 72.0);
        pointlx = 0,
        pointly = 0
      };
      od.dwStatus = MiscStatus((int)od.dwDrawAspect);

      // Descriptive strings to tack on after the struct.
      string name = GraphDocumentEmbeddedComObject.USER_TYPE;
      int name_size = (name.Length + 1) * sizeof(char);
      string source = "Altaxo";
      int source_size = (source.Length + 1) * sizeof(char);
      int od_size = Marshal.SizeOf(typeof(OBJECTDESCRIPTOR));
      od.dwFullUserTypeName = od_size;
      od.dwSrcOfCopy = od_size + name_size;
      int full_size = od_size + name_size + source_size;
      od.cbSize = full_size;

      // To avoid 'unsafe', we will arrange the strings in a byte array.
      byte[] strings = new byte[full_size];
      Encoding unicode = Encoding.Unicode;
      Array.Copy(unicode.GetBytes(name), 0, strings, od.dwFullUserTypeName, name.Length * sizeof(char));
      Array.Copy(unicode.GetBytes(source), 0, strings, od.dwSrcOfCopy, source.Length * sizeof(char));

      // Combine the strings and the struct into a single block of mem.
      IntPtr hod = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, full_size);
      if (!(hod != IntPtr.Zero))
        throw new InvalidOperationException("GlobalAlloc operation was not successful");
      IntPtr buf = Kernel32Func.GlobalLock(hod);
      Marshal.Copy(strings, 0, buf, full_size);
      Marshal.StructureToPtr(od, buf, false);

      Kernel32Func.GlobalUnlock(hod);
      return hod;
    }

    public static IntPtr RenderLinkedObjectDescriptor(TYMED tymed)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.RenderLinkedObjectDescriptor");

      // Brockschmidt, Inside Ole 2nd ed. page 991
      if (!(tymed == TYMED.TYMED_HGLOBAL))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_HGLOBAL");
      // Fill in the basic information.
      var od = new OBJECTDESCRIPTOR
      {
        // According to the documentation this is used just to find an icon.
        clsid = typeof(GraphDocumentLinkedComObject).GUID,
        dwDrawAspect = DVASPECT.DVASPECT_CONTENT,
        sizelcx = 0, // zero in imitation of Word/Excel, but could be box.Extent.cx;
        sizelcy = 0, // zero in imitation of Word/Excel, but could be box.Extent.cy;
        pointlx = 0,
        pointly = 0
      };
      od.dwStatus = MiscStatus((int)od.dwDrawAspect);

      // Descriptive strings to tack on after the struct.
      string name = GraphDocumentLinkedComObject.USER_TYPE;
      int name_size = (name.Length + 1) * sizeof(char);
      string source = "Altaxo";
      int source_size = (source.Length + 1) * sizeof(char);
      int od_size = Marshal.SizeOf(typeof(OBJECTDESCRIPTOR));
      od.dwFullUserTypeName = od_size;
      od.dwSrcOfCopy = od_size + name_size;
      int full_size = od_size + name_size + source_size;
      od.cbSize = full_size;

      // To avoid 'unsafe', we will arrange the strings in a byte array.
      byte[] strings = new byte[full_size];
      Encoding unicode = Encoding.Unicode;
      Array.Copy(unicode.GetBytes(name), 0, strings, od.dwFullUserTypeName, name.Length * sizeof(char));
      Array.Copy(unicode.GetBytes(source), 0, strings, od.dwSrcOfCopy, source.Length * sizeof(char));

      // Combine the strings and the struct into a single block of mem.
      IntPtr hod = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, full_size);
      if (!(hod != IntPtr.Zero))
        throw new InvalidOperationException("GlobalAlloc operation was not successful");
      IntPtr buf = Kernel32Func.GlobalLock(hod);
      Marshal.Copy(strings, 0, buf, full_size);
      Marshal.StructureToPtr(od, buf, false);

      Kernel32Func.GlobalUnlock(hod);
      return hod;
    }

    #endregion Renderings

    #region Helper Functions

    private IMoniker CreateNewDocumentMoniker()
    {
      // create the moniker on the fly
      IMoniker documentMoniker = null;

      var fileMoniker = _comManager.FileComObject.FileMoniker;

      if (null != fileMoniker)
      {
        Ole32Func.CreateItemMoniker("!", DataObjectHelper.NormalStringToMonikerNameString(_graphDocumentName), out var itemMoniker);
        if (null != itemMoniker)
        {
          fileMoniker.ComposeWith(itemMoniker, false, out documentMoniker);
        }
      }
      return documentMoniker;
    }

    public static void InternalSaveMiniProject(IStorage pStgSave, AltaxoDocument projectToSave, string graphDocumentName)
    {
      ComDebug.ReportInfo("GraphDocumentDataObject.InternalSaveMiniProject BEGIN");

      try
      {
        Exception saveEx = null;
        Ole32Func.WriteClassStg(pStgSave, typeof(GraphDocumentEmbeddedComObject).GUID);

        // Store the version of this assembly
        {
          var assembly = System.Reflection.Assembly.GetExecutingAssembly();
          Version version = assembly.GetName().Version;
          using (var stream = new ComStreamWrapper(pStgSave.CreateStream("AltaxoVersion", (int)(STGM.DIRECT | STGM.READWRITE | STGM.CREATE | STGM.SHARE_EXCLUSIVE), 0, 0), true))
          {
            string text = version.ToString();
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(text);
            stream.Write(nameBytes, 0, nameBytes.Length);
          }
        }

        // Store the name of the item
        using (var stream = new ComStreamWrapper(pStgSave.CreateStream("AltaxoGraphName", (int)(STGM.DIRECT | STGM.READWRITE | STGM.CREATE | STGM.SHARE_EXCLUSIVE), 0, 0), true))
        {
          byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(graphDocumentName);
          stream.Write(nameBytes, 0, nameBytes.Length);
        }

        // Store the project
        using (var stream = new ComStreamWrapper(pStgSave.CreateStream("AltaxoProjectZip", (int)(STGM.DIRECT | STGM.READWRITE | STGM.CREATE | STGM.SHARE_EXCLUSIVE), 0, 0), true))
        {
          using (var archive = new Main.Services.Files.ZipArchiveAsProjectArchive(stream, System.IO.Compression.ZipArchiveMode.Create, false))
          {
            var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
            projectToSave.SaveToZippedFile(archive, info);
          }
          stream.Close();
        }

        if (null != saveEx)
          throw saveEx;
      }
      catch (Exception ex)
      {
        ComDebug.ReportError("InternalSaveMiniProject, Exception ", ex);
      }
      finally
      {
        Marshal.ReleaseComObject(pStgSave);
      }

      ComDebug.ReportInfo("GraphDocumentDataObject.InternalSaveMiniProject END");
    }

    #endregion Helper Functions
  }
}
