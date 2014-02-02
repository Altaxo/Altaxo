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

using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

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
#if COMLOGGING
			Debug.ReportInfo("{0} constructor.", this.GetType().Name);
#endif

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
#if COMLOGGING
				Debug.ReportInfo("{0}.Dispose Step 0 : Document is dirty -> Advise DataChanged", this.GetType().Name);
#endif
				SendAdvise_DataChanged(); // update the image of the graph before we close
			}

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 1 : SaveObject", this.GetType().Name);
#endif

			SendAdvise_SaveObject();

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 2 : Calling SendAdvise.HideWindow", this.GetType().Name);
#endif

			SendAdvise_HideWindow();

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 3 : Calling SendAdvise.Closed", this.GetType().Name);
#endif

			SendAdvise_Closed();

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 4 : ROTUnregister(ref _documentMonikerRotCookie)", this.GetType().Name);
#endif

			if (0 != _documentMonikerRotCookie)
			{
				RunningObjectTableHelper.ROTUnregister(ref _documentMonikerRotCookie);
				_documentMoniker = null;
			}

			// Disconnect the container.
#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 5 : Disconnecting this object", this.GetType().Name);
#endif

			Ole32Func.CoDisconnectObject(this, 0);

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose completed.", this.GetType().Name);
#endif
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
#if COMLOGGING
			Debug.ReportInfo("{0}.EhDocumentChanged", this.GetType().Name);
#endif
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
				IMoniker itemMoniker;
				Ole32Func.CreateItemMoniker("!", DataObjectHelper.NormalStringToMonikerNameString(_document.Name), out itemMoniker);

				IMoniker compositeMoniker;
				if (null != itemMoniker)
				{
					fileMoniker.ComposeWith(itemMoniker, false, out compositeMoniker);
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
			return DataObjectHelper.RenderMonikerToNewStream(tymed, this.Moniker);
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
				List<Rendering> renderings = new List<Rendering>();

				// Packing a bitmap representation of the graph into a metafile in order to
				// have the size information (a bitmap does not contain size information)
				renderings.Add(new Rendering((short)CF.CF_ENHMETAFILE, TYMED.TYMED_ENHMF, RenderEnhMetaFile));

				// Allow linking, where we have a moniker.
				if (Moniker != null)
				{
					renderings.Add(new Rendering(DataObjectHelper.CF_LINKSOURCE, TYMED.TYMED_ISTREAM, this.RenderLink));
					renderings.Add(new Rendering(DataObjectHelper.CF_LINKSRCDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderLinkedObjectDescriptor));
				}

				return renderings;
			}
		}

		private IntPtr RenderEnhMetaFile(TYMED tymed)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.RenderEnhMetafile", this.GetType().Name);
#endif

			var docSize = _document.Size;
			using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(_document, System.Drawing.Brushes.Transparent, System.Drawing.Imaging.PixelFormat.Format32bppArgb, 300, 300))
			{
				return DataObjectHelper.RenderEnhMetafileIntPtr(docSize.X, docSize.Y,
				(grfx) =>
				{
					grfx.DrawImage(bmp, 0, 0);
				}
				);
			}
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
					IStream strm = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
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
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.Close {1}", this.GetType().Name, dwSaveOption);
#endif
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
#if COMLOGGING
						Debug.ReportError("{0}.IOleObject.Close called with unknown parameter: {1}", this.GetType().Name, dwSaveOption);
#endif
						break;
				}

				if (cancelled)
				{
#if COMLOGGING
					Debug.ReportInfo("{0}.IOleObject.Close -> OLE_E_PROMPTSAVECANCELLED", this.GetType().Name);
#endif
					return ComReturnValue.OLE_E_PROMPTSAVECANCELLED;
				}

				if (save)
				{
					SendAdvise_SaveObject();
					SendAdvise_Saved();
				}

				// Regardless of whether the form has been shown we must
				// do all the normal shutdown actions.  (e.g. WinWord 2007)
#if COMLOGGING
				Debug.ReportInfo("{0}.IOleObject.Close -> BeginInvoke MainWindow.Close", this.GetType().Name);
#endif
				_comManager.ApplicationAdapter.BeginClosingApplication();
				return ComReturnValue.NOERROR;
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IOleObject.Close threw an exception: {1}", this.GetType().Name, e);
#endif
				throw;
			}
			// }
		}

		public int DoVerb(int iVerb, IntPtr lpmsg, IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, COMRECT lprcPosRect)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.DoVerb {1}", this.GetType().Name, iVerb);
#endif
			try
			{
				// I saw OLEIVERB_HIDE come in as 253.  Perhaps a unsigned
				// byte conversion happening somewhere.
				if (iVerb >= 250 && iVerb <= 255)
				{
					int new_iverb = iVerb - 256;
#if COMLOGGING
					Debug.ReportWarning("{0}.IOleObject.DoVerb -> Fixing iVerb: {1} -> {2}", this.GetType().Name, iVerb, new_iverb);
#endif
					iVerb = new_iverb;
				}

				_lastVerb = iVerb;

				switch (iVerb)
				{
					case (int)OLEIVERB.OLEIVERB_HIDE:
#if COMLOGGING
						Debug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_HIDE", this.GetType().Name);
#endif
						_comManager.ApplicationAdapter.HideMainWindow();
						SendAdvise_HideWindow();
						break;

					case (int)OLEIVERB.OLEIVERB_PRIMARY:
					case (int)OLEIVERB.OLEIVERB_SHOW:
					case (int)OLEIVERB.OLEIVERB_OPEN:

#if COMLOGGING
						if ((int)OLEIVERB.OLEIVERB_PRIMARY == iVerb) Debug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_PRIMARY", this.GetType().Name);
						if ((int)OLEIVERB.OLEIVERB_SHOW == iVerb) Debug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_SHOW", this.GetType().Name);
						if ((int)OLEIVERB.OLEIVERB_OPEN == iVerb) Debug.ReportInfo("{0}.IOleObject.DoVerb OLEIVERB_OPEN", this.GetType().Name);
#endif
						_comManager.ApplicationAdapter.ShowMainWindow();
						if (pActiveSite != null)
						{
#if COMLOGGING
							Debug.ReportInfo("{0}.IOleObject.DoVerb -> calling ClientSite.ShowObject()", this.GetType().Name);
#endif
							try
							{
								pActiveSite.ShowObject();
							}
							catch (Exception ex)
							{
#if COMLOGGING
								Debug.ReportInfo("{0}.IOleObject.DoVerb pActiveSite.ShowObject caused an exception: {1}", this.GetType().Name, ex);
#endif
							}

							SendAdvise_ShowWindow();
						}

						return ComReturnValue.NOERROR;

					default:
#if COMLOGGING
						Debug.ReportError("{0}.IOleObject.DoVerb Unexpected verb: {1}", this.GetType().Name, iVerb);
#endif
						return ComReturnValue.OLEOBJ_S_INVALIDVERB;
				}
#if COMLOGGING
				Debug.ReportInfo("{0}.IOleObject.DoVerb -> returning NOERROR", this.GetType().Name);
#endif
				return ComReturnValue.NOERROR;
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IOleObject.DoVerb throw an exception. Details: {1}", this.GetType().Name, e);
#endif
				throw;
			}
		}

		public int SetExtent(int dwDrawAspect, tagSIZEL pSizel)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.SetExtent({1}x{2}) -> not supported.", this.GetType().Name, pSizel.cx, pSizel.cy);
#endif

			return ComReturnValue.E_FAIL;
		}

		public int GetExtent(int dwDrawAspect, tagSIZEL pSizel)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.GetExtent", this.GetType().Name);
#endif
			if ((dwDrawAspect & (int)DVASPECT.DVASPECT_CONTENT) == 0)
				return ComReturnValue.E_FAIL;

			var docSize_pt = _document.Size;

			pSizel = new tagSIZEL((int)(docSize_pt.X * PointsToHimetric), (int)(docSize_pt.Y * PointsToHimetric));

			return ComReturnValue.NOERROR;
		}

		public int GetMiscStatus(int dwAspect, out int misc)
		{
			misc = GraphDocumentDataObject.MiscStatus(dwAspect);

#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.GetMiscStatus -> returning 0x{1:X}", this.GetType().Name, misc);
#endif

			return ComReturnValue.S_OK;
		}

		#endregion IOleObject members
	}
}