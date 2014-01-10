using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;  // For use of the GuidAttribute, ProgIdAttribute and ClassInterfaceAttribute.
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	/// <summary>
	/// Summary description for DocumentComObject.
	/// This is the class that will be exported to unmanaged COM client apps.
	/// </summary>
	[
		Guid(GraphDocumentComObject.GUID_STRING),  // We indicate a specific CLSID for "DocumentComObject" for convenience of searching the registry.
		ProgId(GraphDocumentComObject.USER_TYPE), //  "Altaxo.Graph.0"),  // This ProgId is used by default. Not 100% necessary.
		ClassInterface(ClassInterfaceType.None)  // Specify that we will not generate any additional interface with a name like _DocumentComObject.
		]
	public class GraphDocumentComObject :
		ReferenceCountedDataObjectBase, // DocumentComObject is derived from ReferenceCountedObjectBase so that we can track its creation and destruction.
		System.Runtime.InteropServices.ComTypes.IDataObject,  // DocumentComObject must implement the IDocumentComObject interface.
		IOleObject,
		IPersistStorage
	{
		public const string GUID_STRING = "0915F010-2A4C-43F5-B230-A89340CF862C";
		public const string USER_TYPE = "Altaxo.Graph.0";
		public const string USER_TYPE_LONG = "Altaxo Graph-Document";
		public const double PointsToHimetric = 2540 / 72.0;

		// Manages our communication with the container.
		private IOleClientSite _clientSite;

		private ManagedOleAdviseHolderUO _oleAdviseHolder;
		private ManagedDataAdviseHolder _dataAdviseHolder;

		private int _lastVerb;

		private IMoniker _documentMoniker;
		private int _documentMonikerRotCookie;

		private bool _isDocumentDirty;

		private GraphDocument _document;

		public GraphDocumentComObject(GraphDocument graphDocument, ProjectFileComObject fileComObject, ComManager comManager)
			: base(comManager)
		{
#if COMLOGGING
			Debug.ReportInfo("{0} constructor.", this.GetType().Name);
#endif

			Init(graphDocument, true, null);

			if (null != fileComObject)
			{
				fileComObject.FileMonikerChanged += EhFileMonikerChanged;
				EhFileMonikerChanged(fileComObject.FileMoniker);
			}
		}

		/// <summary>Copy an object for placing on the clipboard.</summary>
		public GraphDocumentComObject(GraphDocumentComObject from)
			: base(from._comManager)
		{
			Init(from._document, false, from.Moniker);
		}

		private void Init(GraphDocument doc, bool hasForm, IMoniker moniker)
		{
			// Note: we do not create a event link to Document.DocumentRenamed here
			// because this would keep the DocumentComObject alive as long as the document is alive
			// instead, we let the FileComObject watch if documents are renamed and then let it call the function EhDocumentRenamed here in this instance

#if COMLOGGING
			Debug.ReportInfo("{0} init.", this.GetType().Name);
#endif

			_dataAdviseHolder = new ManagedDataAdviseHolder();
			_oleAdviseHolder = new ManagedOleAdviseHolderUO();
			_documentMoniker = moniker;

			Document = doc;
		}

		~GraphDocumentComObject()
		{
			// ReferenceCountedObjectBase destructor will be invoked.
#if COMLOGGING
			Debug.ReportInfo("{0} destructor.", this.GetType().Name);
#endif
		}

		public void Dispose()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 1 : SaveObject", this.GetType().Name);
#endif

			SendAdvise(AdviseKind.SaveObject);

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 2 : Calling SendAdvise.HideWindow", this.GetType().Name);
#endif

			SendAdvise(AdviseKind.HideWindow);

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 3 : Calling SendAdvise.Closed", this.GetType().Name);
#endif

			SendAdvise(AdviseKind.Closed);

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 4 : ROTUnregister(ref _documentMonikerRotCookie)", this.GetType().Name);
#endif

			ROTUnregister(ref _documentMonikerRotCookie);
			_documentMoniker = null;

			// Disconnect the container.
#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose Step 5 : Disconnecting this object", this.GetType().Name);
#endif

			Ole32Func.CoDisconnectObject(this, 0);

#if COMLOGGING
			Debug.ReportInfo("{0}.Dispose completed.", this.GetType().Name);
#endif
		}

		#region Running Object Table management (ROT)

		internal static IRunningObjectTable GetROT()
		{
			IRunningObjectTable rot;
			Int32 hr = Ole32Func.GetRunningObjectTable(0, out rot);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.NOERROR);
			return rot;
		}

		internal static void ROTUnregister(ref int cookie)
		{
			// Revoke any existing file moniker. p988
			IRunningObjectTable rot = GetROT();
			if (0 != cookie)
			{
				rot.Revoke(cookie);
				cookie = 0;
			}
		}

		private static void ROTRegisterAsRunning(IMoniker new_moniker, object o, ref int rot_cookie, Type intf)
		{
			// Revoke any existing file moniker. p988
			ROTUnregister(ref rot_cookie);

			// Register the moniker in the running object table (ROT).
#if COMLOGGING
			Debug.ReportInfo("Registering {0} in ROT", DataObjectHelper.GetDisplayName(new_moniker));
#endif
			IRunningObjectTable rot = GetROT();
			// This flag solved a terrible problem where Word would stop
			// communicating after its first call to GetObject().
			rot_cookie = rot.Register(1 /*ROTFLAGS_REGISTRATIONKEEPSALIVE*/, o, new_moniker);
		}

		#endregion Running Object Table management (ROT)

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

				_comManager.NotifyDocumentOfDocumentsComObjectChanged(this, oldValue, _document);
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
			SendAdvise(AdviseKind.DataChanged);
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

			ROTUnregister(ref _documentMonikerRotCookie);
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
						ROTRegisterAsRunning(_documentMoniker, this, ref _documentMonikerRotCookie, typeof(IOleObject));
					}
				}
			}

			SendAdvise(AdviseKind.Renamed);
		}

		private void SaveMonikerToStream(IMoniker moniker, IStream strm)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.SaveMonikerToStream:{1}", this.GetType().Name, DataObjectHelper.GetDisplayName(moniker));
#endif
			int hr = Ole32Func.OleSaveToStream((IPersistStream)moniker, strm);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.S_OK);
			// I write this in imitation of Brockschmidt, but I do not see why
			// it is here.
			// BoxesDoc seems best when linking because when Word reloads a
			// document containing saved images then this is the class it
			// instantiates.  Otherwise it wrongly instantiates Box.
			// cf IOleObject.GetUserClassID
			//Win32.WriteClassStm(strm, BoxesDoc.ClsID);
			//System.Diagnostics.Debug.Assert(hr == (int)ComApi.ComReturnValue.S_OK);
		}

		//int attempt = 0;
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

		#region Functions

		public void SendAdvise(AdviseKind kind)
		{
			switch (kind)
			{
				case AdviseKind.Saved:
					_oleAdviseHolder.SendOnSave();
					break;

				case AdviseKind.Closed:
					_oleAdviseHolder.SendOnClose();
					break;

				case AdviseKind.Renamed:
					var moniker = Moniker;
					if (null != moniker)
						_oleAdviseHolder.SendOnRename(moniker);
					break;

				case AdviseKind.SaveObject:
					if (_isDocumentDirty && null != _clientSite)
					{
#if COMLOGGING
						Debug.ReportInfo("{0}.SendAdvise.SaveObject -> calling IOleClientSite.SaveObject()", this.GetType().Name);
#endif
						_clientSite.SaveObject();
					}
					else
					{
#if COMLOGGING
						Debug.ReportInfo("{0}.SendAdvise.SaveObject -> NOT DONE! isDirty={1}, isClientSiteNull={2} )", this.GetType().Name, _isDocumentDirty, null == _clientSite);
#endif
					}
					break;

				case AdviseKind.DataChanged:

					if (null != _dataAdviseHolder)
					{
#if COMLOGGING
						Debug.ReportInfo("{0}.SendAdvise.DataChanged -> Calling _dataAdviseHolder.SendOnDataChange()", this.GetType().Name);
#endif
						_dataAdviseHolder.SendOnDataChange((IDataObject)this, 0, 0);
					}
					// we must also note the change time to the running object table, see
					// Brockschmidt, Inside Ole 2nd ed., page 989
					if (_documentMonikerRotCookie != 0)
					{
						System.Runtime.InteropServices.ComTypes.FILETIME ft = new System.Runtime.InteropServices.ComTypes.FILETIME();
						Ole32Func.CoFileTimeNow(out ft);
						GetROT().NoteChangeTime(_documentMonikerRotCookie, ref ft);
					}
					break;

				case AdviseKind.ShowWindow:
#if COMLOGGING
					Debug.ReportInfo("{0}.SendAdvise.ShowWindow -> Calling IOleClientSite.OnShowWindow(true)", this.GetType().Name);
#endif
					if (null != _clientSite)
						_clientSite.OnShowWindow(true);
					break;

				case AdviseKind.HideWindow:
#if COMLOGGING
					Debug.ReportInfo("{0}.SendAdvise.HideWindow -> Calling IOleClientSite.OnShowWindow(false)", this.GetType().Name);
#endif
					try
					{
						if (null != _clientSite)
							_clientSite.OnShowWindow(false);
					}
					catch (Exception ex)
					{
#if COMLOGGING
						Debug.ReportError("{0}.SendAdvise.HideWindow -> Exception while calling _clientSite.OnShowWindow(false), Details: {0}", this.GetType().Name, ex.Message);
#endif
					}
					break;

				case AdviseKind.ShowObject:
#if COMLOGGING
					Debug.ReportInfo("{0}.SendAdvise.ShowObject -> Calling IOleClientSite.ShowObject()", this.GetType().Name);
#endif
					if (null != _clientSite)
						_clientSite.ShowObject();
					break;

				default:
					break;
			}
		}

		#endregion Functions

		#region IDataObject members

		protected override IList<Rendering> Renderings
		{
			get
			{
				List<Rendering> renderings = new List<Rendering>();

				// We add them in order of preference.  Embedding is best because user can resize and edit),
				// then enhanced metafile mainly because it contains size information

				// Allows us to be embedded with an OLE container.
				// No callback because this should go via GetDataHere.
				// EMBEDSOURCE and OBJECTDESCRIPTOR should be placed after private data,
				// but before the presentations (Brockschmidt, Inside Ole 2nd ed. page 911)
				renderings.Add(new Rendering(DataObjectHelper.CF_EMBEDSOURCE, TYMED.TYMED_ISTORAGE, null));
				renderings.Add(new Rendering(DataObjectHelper.CF_OBJECTDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderObjectDescriptor));

				// Nice because it is resolution independent.
				renderings.Add(new Rendering((short)CF.CF_ENHMETAFILE, TYMED.TYMED_ENHMF, RenderEnhMetaFile));

				// And allow linking, where we have a moniker.  This is last because
				// it should not happen by default.
				if (Moniker != null)
				{
					renderings.Add(new Rendering(DataObjectHelper.CF_LINKSOURCE, TYMED.TYMED_ISTREAM, this.RenderLink));
					renderings.Add(new Rendering(DataObjectHelper.CF_LINKSRCDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderObjectDescriptor));
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
			using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(_document, System.Drawing.Brushes.Transparent, System.Drawing.Imaging.PixelFormat.Format32bppArgb, GraphExportArea.GraphSize, 300, 300))
			{
				return DataObjectHelper.RenderEnhMetaFile(docSize.X, docSize.Y,
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
			if (format.cfFormat == DataObjectHelper.CF_EMBEDSOURCE && (format.tymed & TYMED.TYMED_ISTORAGE) != 0)
			{
				medium.tymed = TYMED.TYMED_ISTORAGE;
				medium.pUnkForRelease = null;
				var stg = (IStorage)Marshal.GetObjectForIUnknown(medium.unionmember);

				Save(stg, false);
				return true;
			}

			if (format.cfFormat == DataObjectHelper.CF_LINKSOURCE && (format.tymed & TYMED.TYMED_ISTREAM) != 0)
			{
				var moniker = Moniker;
				if (null != moniker)
				{
					medium.tymed = TYMED.TYMED_ISTREAM;
					medium.pUnkForRelease = null;
					IStream strm = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
					SaveMonikerToStream(Moniker, strm);
					return true;
				}
			}
			return false;
		}

		#endregion IDataObject members

		#region IOleObject members

		public int SetClientSite(IOleClientSite pClientSite)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.SetClientSite", this.GetType().Name);
#endif
			_clientSite = pClientSite;
			return ComReturnValue.NOERROR;
		}

		public IOleClientSite GetClientSite()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.GetClientSite", this.GetType().Name);
#endif

			return _clientSite;
		}

		public int SetHostNames(string containerApplicationName, string containerDocumentName)
		{
			// see Brockschmidt, Inside Ole 2nd ed. page 992
			// calling SetHostNames is the only sign that our object is embedded (and thus not linked)
			// this means that we have to switch the user interface from within this function

#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.SetHostNames ContainerAppName={1}, ContainerDocName={2}", this.GetType().Name, containerApplicationName, containerDocumentName);
#endif

			_comManager.SetHostNames(containerApplicationName, containerDocumentName, _document);
			return ComReturnValue.NOERROR;
		}

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
					SendAdvise(AdviseKind.SaveObject);
					SendAdvise(AdviseKind.Saved);
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

		public int SetMoniker(int dwWhichMoniker, object pmk)
		{
			// Brockschmidt Inside Ole 2nd ed. page 993
			// see there if you want to support linking to embedding
#if COMLOGGING
			Debug.ReportWarning("{0}.IOleObject.SetMoniker => not implemented!", this.GetType().Name);
#endif
			return ComReturnValue.E_NOTIMPL;
		}

		public int GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker)
		{
			// Brockschmidt Inside Ole 2nd ed. page 994
#if COMLOGGING
			Debug.ReportWarning("{0}.IOleObject.GetMoniker", this.GetType().Name);
#endif
			if (null != _documentMoniker)
			{
				moniker = _documentMoniker;
				return ComReturnValue.S_OK;
			}
			// see Brockschmidt if we want to support linking to embedding
			moniker = null;
			return ComReturnValue.E_FAIL;
		}

		public int InitFromData(System.Runtime.InteropServices.ComTypes.IDataObject pDataObject, int fCreation, int dwReserved)
		{
#if COMLOGGING
			Debug.ReportWarning("{0}.IOleObject.InitFromData => not implemented!", this.GetType().Name);
#endif
			return ComReturnValue.E_NOTIMPL;
		}

		public int GetClipboardData(int dwReserved, out System.Runtime.InteropServices.ComTypes.IDataObject data)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.GetClipboardData => not implemented!", this.GetType().Name);
#endif
			data = null;
			return ComReturnValue.E_NOTIMPL;
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
						SendAdvise(AdviseKind.HideWindow);
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

							SendAdvise(AdviseKind.ShowWindow);
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

		public int EnumVerbs(out IEnumOLEVERB e)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.EnumVerbs -> use registry", this.GetType().Name);
#endif
			e = null;
			return ComReturnValue.OLE_S_USEREG;
		}

		public int OleUpdate()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.OleUpdate", this.GetType().Name);
#endif
			return ComReturnValue.NOERROR;
		}

		public int IsUpToDate()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.IsUpToDate", this.GetType().Name);
#endif
			return ComReturnValue.NOERROR;
		}

		public int GetUserClassID(ref Guid pClsid)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.GetUserClassID", this.GetType().Name);
#endif
			pClsid = this.GetType().GUID;
			return ComReturnValue.NOERROR;
		}

		public int GetUserType(int dwFormOfType, out string userType)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.GetUserType -> use registry.", this.GetType().Name);
#endif
			userType = null;
			return ComReturnValue.OLE_S_USEREG;
		}

		public int SetExtent(int dwDrawAspect, tagSIZEL pSizel)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.SetExtent({1}x{2}) -> not supported.", this.GetType().Name, pSizel.cx, pSizel.cy);
#endif

			return ComReturnValue.E_FAIL;

			/*
			try
			{
				if ((dwDrawAspect & (int)DVASPECT.DVASPECT_CONTENT) == 0)
					return ComReturnValue.E_FAIL;

				Extent = new tagSIZEL(pSizel.cx, pSizel.cy);

				// Changes to size should be preserved.
				SendAdvise(AdviseKind.SaveObject);

				return ComReturnValue.S_OK;
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("SetExtent occured an exception.", e);
#endif
				throw;
			}
			*/
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

		public int Advise(System.Runtime.InteropServices.ComTypes.IAdviseSink pAdvSink, out int cookie)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.Advise", this.GetType().Name);
#endif
			try
			{
				_oleAdviseHolder.Advise(pAdvSink, out cookie);
				return ComReturnValue.NOERROR;
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IOleObject.Advise caused an exception: {1}", this.GetType().Name, e);
#endif
				throw;
			}
		}

		public int Unadvise(int dwConnection)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.Unadvise", this.GetType().Name);
#endif
			try
			{
				_oleAdviseHolder.Unadvise(dwConnection);
				return ComReturnValue.NOERROR;
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IOleObject.Unadvise threw an exception: {0}", this.GetType().Name, e);
#endif
				throw;
			}
		}

		public int EnumAdvise(out System.Runtime.InteropServices.ComTypes.IEnumSTATDATA e)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.EnumAdvise", this.GetType().Name);
#endif

			e = _oleAdviseHolder.EnumAdvise();
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

		public int SetColorScheme(tagLOGPALETTE pLogpal)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IOleObject.SetColorScheme (not implemented)", this.GetType().Name);
#endif
			return ComReturnValue.E_NOTIMPL;
		}

		#endregion IOleObject members

		#region IPersistStorage mebers

		public void GetClassID(out Guid pClassID)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IPersistStorage.GetClassID", this.GetType().Name);
#endif
			pClassID = this.GetType().GUID;
		}

		public int IsDirty()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IPersistStorage.IsDirty returning {1}", this.GetType().Name, _isDocumentDirty);
#endif

			if (_isDocumentDirty)
				return ComReturnValue.S_OK;
			else
				return ComReturnValue.S_FALSE;
		}

		public void InitNew(IStorage pstg)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IPersistStorage.InitNew", this.GetType().Name);
#endif

			Document = Current.ProjectService.CreateNewGraph().Doc;

			// We don't need an IStorage except at Load/Save.
			_isDocumentDirty = true; // but we set the document dirty flag thus it is saved
		}

		public int Load(IStorage pstg)
		{
			System.Diagnostics.Debug.Assert(null == _document);

			string documentName = null;
#if COMLOGGING
			Debug.ReportInfo("{0}.IPersistStorage.Load", this.GetType().Name);
#endif

			try
			{
				using (var stream = new ComStreamWrapper(pstg.OpenStream("AltaxoGraphName", IntPtr.Zero, (int)(STGM.READ | STGM.SHARE_EXCLUSIVE), 0), true))
				{
					var bytes = new byte[stream.Length];
					stream.Read(bytes, 0, bytes.Length);
					documentName = System.Text.Encoding.UTF8.GetString(bytes);
				}
#if COMLOGGING
				Debug.ReportInfo("{0}.IPersistStorage.Load -> Name of GraphDocument: {1}", this.GetType().Name, documentName);
#endif
			}
			catch (Exception ex)
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IPersistStorage.Load Failed to load stream GraphName, exception: {1}", this.GetType().Name, ex);
#endif
			}

			try
			{
				using (var streamWrapper = new ComStreamWrapper(pstg.OpenStream("AltaxoProjectZip", IntPtr.Zero, (int)(STGM.READ | STGM.SHARE_EXCLUSIVE), 0), true))
				{
					_comManager.InvokeGuiThread(() =>
					{
						Current.ProjectService.CloseProject(true);
						Current.ProjectService.LoadProject(streamWrapper);
					});
				}
#if COMLOGGING
				Debug.ReportInfo("{0}.IPersistStorage.Load Project loaded", this.GetType().Name);
#endif
			}
			catch (Exception ex)
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IPersistStorage.Load Failed to load stream AltaxoProjectZip, exception: {1}", this.GetType().Name, ex);
#endif
			}

			Marshal.ReleaseComObject(pstg);

			Altaxo.Graph.Gdi.GraphDocument newDocument = null;

			if (null != documentName && Current.Project.GraphDocumentCollection.Contains(documentName))
				newDocument = Current.Project.GraphDocumentCollection[documentName];
			else if (null != Current.Project.GraphDocumentCollection.FirstOrDefault())
				newDocument = Current.Project.GraphDocumentCollection.First();

			if (null != newDocument)
			{
				Document = newDocument;
				_comManager.InvokeGuiThread(() => Current.ProjectService.ShowDocumentView(_document));
			}

			if (null == Document)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IPersistStorage.Load Document is null, have to throw an exception now!!", this.GetType().Name);
#endif
				throw new InvalidOperationException();
			}

			_isDocumentDirty = false;
			return ComReturnValue.S_OK;
		}

		public void Save(IStorage pStgSave, bool fSameAsLoad)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IPersistStorage.Save", this.GetType().Name);
#endif

			try
			{
				Exception saveEx = null;

				Ole32Func.WriteClassStg(pStgSave, this.GetType().GUID);

				// Store the name of the item
				using (var stream = new ComStreamWrapper(pStgSave.CreateStream("AltaxoGraphName", (int)(STGM.DIRECT | STGM.READWRITE | STGM.CREATE | STGM.SHARE_EXCLUSIVE), 0, 0), true))
				{
					byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(_document.Name);
					stream.Write(nameBytes, 0, nameBytes.Length);
				}

				// Store the project
				using (var stream = new ComStreamWrapper(pStgSave.CreateStream("AltaxoProjectZip", (int)(STGM.DIRECT | STGM.READWRITE | STGM.CREATE | STGM.SHARE_EXCLUSIVE), 0, 0), true))
				{
					_comManager.InvokeGuiThread(() =>
					{
						saveEx = Current.ProjectService.SaveProject(stream);
					}
					);
				}

				_isDocumentDirty = false;

				if (null != saveEx)
					throw saveEx;
			}
			catch (Exception ex)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IPersistStorage:Save threw an exception: {1}", this.GetType().Name, ex);
#endif
			}
			finally
			{
				Marshal.ReleaseComObject(pStgSave);
			}
		}

		public void SaveCompleted(IStorage pStgNew)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IPersistStorage.SaveCompleted", this.GetType().Name);
#endif

			SendAdvise(AdviseKind.Saved);
		}

		public int HandsOffStorage()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IPersistStorage.HandsOffStorage", this.GetType().Name);
#endif
			return ComReturnValue.S_OK;
		}

		#endregion IPersistStorage mebers
	}
}