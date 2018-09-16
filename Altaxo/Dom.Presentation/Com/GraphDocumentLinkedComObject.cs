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
using Altaxo.Graph.Gdi;

namespace Altaxo.Com
{
  using UnmanagedApi.Ole32;

  /// <summary>
  /// Supports the linking of Altaxo GraphDocuments in container applications.
  /// </summary>
  [
    Guid(GraphDocumentLinkedComObject.GUID_STRING),
    ProgId(GraphDocumentLinkedComObject.USER_TYPE),
    ClassInterface(ClassInterfaceType.None)
    ]
  public class GraphDocumentLinkedComObject :
    OleObjectBase,
    System.Runtime.InteropServices.ComTypes.IDataObject,
    IOleObject
  {
    public const string GUID_STRING = "070BA50F-5F5E-40F8-9A24-1565B6CA9B66";
    public const string USER_TYPE = "Altaxo.Graph(linked)";
    public const string USER_TYPE_LONG = "Altaxo Graph-Document";
    public const double PointsToHimetric = 2540 / 72.0;

    private ManagedDataAdviseHolder _dataAdviseHolder;

    private int _lastVerb;

    private GraphDocument _document;

    public GraphDocumentLinkedComObject(GraphDocument graphDocument, ProjectFileComObject fileComObject, ComManager comManager)
      : base(comManager)
    {
      ComDebug.ReportInfo("{0} constructor.", GetType().Name);

      _dataAdviseHolder = new ManagedDataAdviseHolder();
      _oleAdviseHolder = new ManagedOleAdviseHolderFM();

      Document = graphDocument;

      if (null != fileComObject)
      {
        fileComObject.FileMonikerChanged += EhFileMonikerChanged;
        EhFileMonikerChanged(fileComObject.FileMoniker);
      }
    }

    public void Dispose()
    {
      if (_isDocumentDirty)
      {
        ComDebug.ReportInfo("{0}.Dispose Step 0 : Document is dirty -> Advise DataChanged", GetType().Name);
        SendAdvise_DataChanged(); // update the image of the graph before we close
      }

      ComDebug.ReportInfo("{0}.Dispose Step 1 : SaveObject", GetType().Name);

      SendAdvise_SaveObject();

      ComDebug.ReportInfo("{0}.Dispose Step 2 : Calling SendAdvise.HideWindow", GetType().Name);

      SendAdvise_HideWindow();

      ComDebug.ReportInfo("{0}.Dispose Step 3 : Calling SendAdvise.Closed", GetType().Name);

      SendAdvise_Closed();

      ComDebug.ReportInfo("{0}.Dispose Step 4 : ROTUnregister(ref _documentMonikerRotCookie)", GetType().Name);

      if (0 != _documentMonikerRotCookie)
      {
        RunningObjectTableHelper.ROTUnregister(ref _documentMonikerRotCookie);
        _documentMoniker = null;
      }

      // Disconnect the container.
      ComDebug.ReportInfo("{0}.Dispose Step 5 : Disconnecting this object", GetType().Name);

      Ole32Func.CoDisconnectObject(this, 0);

      ComDebug.ReportInfo("{0}.Dispose completed.", GetType().Name);
    }

    #region Document management

    public GraphDocument Document
    {
      get
      {
        return _document;
      }
      private set
      {
        if (object.ReferenceEquals(_document, value))
          return;

        if (null != _document)
        {
          _document.Changed -= EhDocumentChanged;
        }

        var oldValue = _document;
        _document = value;

        if (null != _document)
        {
          _document.Changed += EhDocumentChanged;
        }
      }
    }

    private void EhDocumentChanged(object sender, EventArgs e)
    {
      ComDebug.ReportInfo("{0}.EhDocumentChanged", GetType().Name);
      _isDocumentDirty = true;

      // see Brockschmidt Inside Ole 2nd edition, page 909
      // we must send IDataAdviseHolder:SendOnDataChange
      SendAdvise_DataChanged();
    }

    /// <summary>
    /// Called by the ComManager when  document of this instance was renamed.
    /// </summary>
    public void EhDocumentRenamed(IMoniker fileMoniker)
    {
      // Trick to create a new document moniker, and send the advise
      EhFileMonikerChanged(fileMoniker);
    }

    #endregion Document management

    #region Moniker

    private void EhFileMonikerChanged(IMoniker fileMoniker)
    {
      if (null == _document)
        return;

      // see Brockschmidt, Inside Ole 2nd ed., p.998
      // TODO we must pimp up this function

      RunningObjectTableHelper.ROTUnregister(ref _documentMonikerRotCookie);
      _documentMoniker = null;

      if (null != fileMoniker)
      {
        Ole32Func.CreateItemMoniker("!", DataObjectHelper.NormalStringToMonikerNameString(_document.Name), out var itemMoniker);

        if (null != itemMoniker)
        {
          fileMoniker.ComposeWith(itemMoniker, false, out var compositeMoniker);
          if (null != compositeMoniker)
          {
            _documentMoniker = compositeMoniker;
            RunningObjectTableHelper.ROTRegisterAsRunning(_documentMoniker, this, ref _documentMonikerRotCookie, typeof(IOleObject));
          }
        }
      }

      SendAdvise_Renamed();
    }

    public IntPtr RenderLink(TYMED tymed)
    {
      return DataObjectHelper.RenderMonikerToNewStream(tymed, Moniker);
    }

    #endregion Moniker

    #region Properties

    public IMoniker Moniker
    {
      get
      {
        return _documentMoniker;
      }
    }

    #endregion Properties

    #region IDataObject members

    protected override IList<Rendering> Renderings
    {
      get
      {
        var renderings = new List<Rendering>();

        var renderingOptions = EmbeddedGraphDocumentRenderingHelper.GetRenderingOptions(_document);

        // Enhanced metafile is sometimes problematic because not all programs can interpret the scale and unit options
        if (renderingOptions.RenderEnhancedMetafile)
        {
          renderings.Add(new Rendering(CF.CF_ENHMETAFILE, TYMED.TYMED_ENHMF, RenderEnhancedMetafile));
        }

        // some applications - even PowerPoint 2010 - seems to require old fashioned Windows metafile
        if (renderingOptions.RenderWindowsMetafile)
        {
          renderings.Add(new Rendering(CF.CF_METAFILEPICT, TYMED.TYMED_MFPICT, RenderWindowsMetafilePict));
        }

        if (renderingOptions.RenderBitmap)
        {
          renderings.Add(new Rendering(CF.CF_BITMAP, TYMED.TYMED_GDI, RenderAsGdiBitmap));
          renderings.Add(new Rendering(CF.CF_DIB, TYMED.TYMED_HGLOBAL, RenderAsDIBBitmap));
        }

        // Allow linking, where we have a moniker.
        if (Moniker != null)
        {
          renderings.Add(new Rendering(DataObjectHelper.CF_LINKSOURCE, TYMED.TYMED_ISTREAM, RenderLink));
          renderings.Add(new Rendering(DataObjectHelper.CF_LINKSRCDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderLinkedObjectDescriptor));
        }

        return renderings;
      }
    }

    private IntPtr RenderEnhancedMetafile(TYMED tymed)
    {
      ComDebug.ReportInfo("{0}.RenderEnhancedMetafile", GetType().Name);

      if (!(tymed == TYMED.TYMED_ENHMF))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_TYMED_ENHMF");

      return EmbeddedGraphDocumentRenderingHelper.RenderEnhancedMetafile_TYMED_ENHMF(tymed, _document);
    }

    private IntPtr RenderWindowsMetafilePict(TYMED tymed)
    {
      ComDebug.ReportInfo("{0}.RenderWindowsMetafilePict", GetType().Name);

      if (!(tymed == TYMED.TYMED_MFPICT))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_MFPICT");

      return EmbeddedGraphDocumentRenderingHelper.RenderWindowsMetafilePict_TYMED_MFPICT(tymed, _document);
    }

    private IntPtr RenderAsGdiBitmap(TYMED tymed)
    {
      ComDebug.ReportInfo("{0}.RenderAsGdiBitmap", GetType().Name);

      if (!(tymed == TYMED.TYMED_GDI))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_GDI");

      return EmbeddedGraphDocumentRenderingHelper.RenderAsGdiBitmap_TYMED_GDI(tymed, _document);
    }

    private IntPtr RenderAsDIBBitmap(TYMED tymed)
    {
      ComDebug.ReportInfo("{0}.RenderAsDIBBitmap", GetType().Name);
      if (!(tymed == TYMED.TYMED_HGLOBAL))
        throw new ArgumentException(nameof(tymed) + " is not TYMED_HGLOBAL");

      return EmbeddedGraphDocumentRenderingHelper.RenderAsDIBBitmap_TYMED_HGLOBAL(tymed, _document);
    }

    protected override ManagedDataAdviseHolder DataAdviseHolder
    {
      get { return _dataAdviseHolder; }
    }

    protected override bool InternalGetDataHere(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
    {
      if (format.cfFormat == DataObjectHelper.CF_LINKSOURCE && (format.tymed & TYMED.TYMED_ISTREAM) != 0)
      {
        var moniker = Moniker;
        if (null != moniker)
        {
          medium.tymed = TYMED.TYMED_ISTREAM;
          medium.pUnkForRelease = null;
          var strm = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
          DataObjectHelper.SaveMonikerToStream(Moniker, strm);
          return true;
        }
      }
      return false;
    }

    #endregion IDataObject members

    #region IOleObject members

    public int Close(tagOLECLOSE dwSaveOption)
    {
      ComDebug.ReportInfo("{0}.IOleObject.Close {1}", GetType().Name, dwSaveOption);
      try
      {
        bool save = false, cancelled = false;

        switch (dwSaveOption)
        {
          case tagOLECLOSE.OLECLOSE_SAVEIFDIRTY:
            save = true;
            break;

          case tagOLECLOSE.OLECLOSE_PROMPTSAVE:
            _comManager.InvokeGuiThread(
               new Action(() =>
             {
               // If asked to prompt, do so only if dirty: if we get a YES, save as
               // usual and close.  On NO, just close.  On CANCEL, return
               // OLE_E_PROMPTSAVECANCELLED.
               var r = System.Windows.MessageBox.Show("Save?", "Box", System.Windows.MessageBoxButton.YesNoCancel);
               switch (r)
               {
                 case System.Windows.MessageBoxResult.Yes:
                   save = true;
                   break;

                 case System.Windows.MessageBoxResult.Cancel:
                   cancelled = true;
                   break;
               }
             }));
            break;

          case tagOLECLOSE.OLECLOSE_NOSAVE:
            break;

          default:
            ComDebug.ReportError("{0}.IOleObject.Close called with unknown parameter: {1}", GetType().Name, dwSaveOption);
            break;
        }

        if (cancelled)
        {
          ComDebug.ReportInfo("{0}.IOleObject.Close -> OLE_E_PROMPTSAVECANCELLED", GetType().Name);
          return ComReturnValue.OLE_E_PROMPTSAVECANCELLED;
        }

        if (save)
        {
          SendAdvise_SaveObject();
          SendAdvise_Saved();
        }

        // Regardless of whether the form has been shown we must
        // do all the normal shutdown actions.  (e.g. WinWord 2007)
        ComDebug.ReportInfo("{0}.IOleObject.Close -> BeginInvoke MainWindow.Close", GetType().Name);
        _comManager.ApplicationAdapter.BeginClosingApplication();
        return ComReturnValue.NOERROR;
      }
      catch (Exception e)
      {
        ComDebug.ReportError("{0}.IOleObject.Close threw an exception: {1}", GetType().Name, e);
        throw;
      }
      // }
    }

    public int DoVerb(int iVerb, IntPtr lpmsg, IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, COMRECT lprcPosRect)
    {
      ComDebug.ReportInfo("{0}.IOleObject.DoVerb {1}", GetType().Name, iVerb);
      try
      {
        // I saw OLEIVERB_HIDE come in as 253.  Perhaps a unsigned
        // byte conversion happening somewhere.
        if (iVerb >= 250 && iVerb <= 255)
        {
          int new_iverb = iVerb - 256;
          ComDebug.ReportWarning("{0}.IOleObject.DoVerb -> Fixing iVerb: {1} -> {2}", GetType().Name, iVerb, new_iverb);
          iVerb = new_iverb;
        }

        _lastVerb = iVerb;

        switch (iVerb)
        {
          case (int)OLEIVERB.OLEIVERB_HIDE:
            ComDebug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_HIDE", GetType().Name);
            _comManager.ApplicationAdapter.HideMainWindow();
            SendAdvise_HideWindow();
            break;

          case (int)OLEIVERB.OLEIVERB_PRIMARY:
          case (int)OLEIVERB.OLEIVERB_SHOW:
          case (int)OLEIVERB.OLEIVERB_OPEN:

            if ((int)OLEIVERB.OLEIVERB_PRIMARY == iVerb)
              ComDebug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_PRIMARY", GetType().Name);
            if ((int)OLEIVERB.OLEIVERB_SHOW == iVerb)
              ComDebug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_SHOW", GetType().Name);
            if ((int)OLEIVERB.OLEIVERB_OPEN == iVerb)
              ComDebug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_OPEN", GetType().Name);
            _comManager.ApplicationAdapter.ShowMainWindow();
            if (pActiveSite != null)
            {
              ComDebug.ReportInfo("{0}.IOleObject.DoVerb -> calling ClientSite.ShowObject()", GetType().Name);
              try
              {
                pActiveSite.ShowObject();
              }
              catch (Exception ex)
              {
                ComDebug.ReportInfo("{0}.IOleObject.DoVerb pActiveSite.ShowObject caused an exception: {1}", GetType().Name, ex);
              }

              SendAdvise_ShowWindow();
            }

            return ComReturnValue.NOERROR;

          default:
            ComDebug.ReportError("{0}.IOleObject.DoVerb Unexpected verb: {1}", GetType().Name, iVerb);
            return ComReturnValue.OLEOBJ_S_INVALIDVERB;
        }
        ComDebug.ReportInfo("{0}.IOleObject.DoVerb -> returning NOERROR", GetType().Name);
        return ComReturnValue.NOERROR;
      }
      catch (Exception e)
      {
        ComDebug.ReportError("{0}.IOleObject.DoVerb throw an exception. Details: {1}", GetType().Name, e);
        throw;
      }
    }

    public int SetExtent(int dwDrawAspect, tagSIZEL pSizel)
    {
      ComDebug.ReportInfo("{0}.IOleObject.SetExtent({1}x{2}) -> not supported.", GetType().Name, pSizel.cx, pSizel.cy);

      return ComReturnValue.E_FAIL;
    }

    public int GetExtent(int dwDrawAspect, tagSIZEL pSizel)
    {
      ComDebug.ReportInfo("{0}.IOleObject.GetExtent", GetType().Name);
      if ((dwDrawAspect & (int)DVASPECT.DVASPECT_CONTENT) == 0)
        return ComReturnValue.E_FAIL;

      var docSize_pt = _document.Size;

      pSizel = new tagSIZEL((int)(docSize_pt.X * PointsToHimetric), (int)(docSize_pt.Y * PointsToHimetric));

      return ComReturnValue.NOERROR;
    }

    public int GetMiscStatus(int dwAspect, out int misc)
    {
      misc = GraphDocumentDataObject.MiscStatus(dwAspect);

      ComDebug.ReportInfo("{0}.IOleObject.GetMiscStatus -> returning 0x{1:X}", GetType().Name, misc);

      return ComReturnValue.S_OK;
    }

    #endregion IOleObject members
  }
}
