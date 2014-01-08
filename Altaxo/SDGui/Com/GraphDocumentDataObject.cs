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
	using UnmanagedApi.Kernel32;

	public class GraphDocumentDataObject : System.Runtime.InteropServices.ComTypes.IDataObject
	{
		private ManagedDataAdviseHolder _dataAdviseHolder;
		private AltaxoDocument _altaxoMiniProject;
		private string _altaxoGraphDocumentName;
		private Altaxo.Graph.PointD2D _graphDocumentSize;
		private System.Drawing.Image _graphDocumentClipboardImage;

		ComManager _comManager;



		public GraphDocumentDataObject(GraphDocument graphDocument, ProjectFileComObject fileComObject, ComManager comManager)
		{
			_comManager = comManager;

#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject constructor.");
#endif
			_dataAdviseHolder = new ManagedDataAdviseHolder();



			_altaxoGraphDocumentName = graphDocument.Name;
			_graphDocumentSize = graphDocument.Size;
			_graphDocumentClipboardImage = Altaxo.Graph.Gdi.GraphDocumentClipboardActions.GetImageForClipbard(graphDocument);
			var miniProjectBuilder = new Altaxo.Graph.Procedures.MiniProjectBuilder();
			_altaxoMiniProject = miniProjectBuilder.GetMiniProject(graphDocument);
		}

		#region Renderings

		private List<Rendering> GetRenderings()
		{
			var list = new List<Rendering>();

			list.Add(new Rendering(DataObjectHelper.CF_EMBEDSOURCE, TYMED.TYMED_ISTORAGE, null));
			list.Add(new Rendering(DataObjectHelper.CF_OBJECTDESCRIPTOR, TYMED.TYMED_HGLOBAL, RenderObjectDescriptor));
			list.Add(new Rendering(CF.CF_ENHMETAFILE, TYMED.TYMED_ENHMF, RenderEnhMetaFile));

			if (!string.IsNullOrEmpty(Current.ProjectService.CurrentProjectFileName))
			{
				list.Add(new Rendering(DataObjectHelper.CF_LINKSOURCE, TYMED.TYMED_ISTREAM, RenderMoniker));
				list.Add(new Rendering(DataObjectHelper.CF_LINKSRCDESCRIPTOR, TYMED.TYMED_HGLOBAL, RenderObjectDescriptor));
			}

			return list;
		}

		private IntPtr RenderEnhMetaFile(TYMED tymed)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.RenderEnhMetafile");
#endif


			if (_graphDocumentClipboardImage is System.Drawing.Imaging.Metafile)
				return ((System.Drawing.Imaging.Metafile)_graphDocumentClipboardImage).GetHenhmetafile();
			else
				return DataObjectHelper.RenderEnhMetaFile(_graphDocumentSize.X, _graphDocumentSize.Y,
				(grfx) =>
				{
					grfx.DrawImage(_graphDocumentClipboardImage, 0, 0);
				}
				);
		}

		private IntPtr RenderMoniker(TYMED tymed)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.RenderMoniker");
#endif
			return DataObjectHelper.RenderMonikerToNewStream(tymed, CreateNewDocumentMoniker());
		}

		public static IntPtr RenderObjectDescriptor(TYMED tymed)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.RenderObjectDescriptor");
#endif

			// Brockschmidt, Inside Ole 2nd ed. page 991
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_HGLOBAL);
			// Fill in the basic information.
			OBJECTDESCRIPTOR od = new OBJECTDESCRIPTOR();
			// According to the documentation this is used just to find an icon.
			od.clsid = typeof(GraphDocumentComObject).GUID;
			od.dwDrawAspect = DVASPECT.DVASPECT_CONTENT;
			od.sizelcx = 0; // zero in imitation of Word/Excel, but could be box.Extent.cx;
			od.sizelcy = 0; // zero in imitation of Word/Excel, but could be box.Extent.cy;
			od.pointlx = 0;
			od.pointly = 0;
			od.dwStatus = GraphDocumentComObject.MiscStatus((int)od.dwDrawAspect);

			// Descriptive strings to tack on after the struct.
			string name = GraphDocumentComObject.USER_TYPE;
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
			System.Diagnostics.Debug.Assert(hod != IntPtr.Zero);
			IntPtr buf = Kernel32Func.GlobalLock(hod);
			Marshal.Copy(strings, 0, buf, full_size);
			Marshal.StructureToPtr(od, buf, false);

			Kernel32Func.GlobalUnlock(hod);
			return hod;
		}

		#endregion

		#region Helper Functions

		private IMoniker CreateNewDocumentMoniker()
		{
			// create the moniker on the fly
			IMoniker documentMoniker = null;

			var fileMoniker = _comManager.FileComObject.FileMoniker;

			if (null != fileMoniker)
			{
				IMoniker itemMoniker;
				Ole32Func.CreateItemMoniker("!", ComManager.NormalStringToMonikerNameString(_altaxoGraphDocumentName), out itemMoniker);
				if (null != itemMoniker)
				{
					fileMoniker.ComposeWith(itemMoniker, false, out documentMoniker);
				}
			}
			return documentMoniker;
		}

		public static void InternalSaveMiniProject(IStorage pStgSave, AltaxoDocument projectToSave, string graphDocumentName)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.InternalSaveMiniProject BEGIN");
#endif

			try
			{
				Exception saveEx = null;
				Ole32Func.WriteClassStg(pStgSave, typeof(GraphDocumentComObject).GUID);

				using (var stream = new ComStreamWrapper(pStgSave.CreateStream("AltaxoProjectZip", (int)(STGM.DIRECT | STGM.READWRITE | STGM.CREATE | STGM.SHARE_EXCLUSIVE), 0, 0), true))
				{
					using (var zippedStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(stream))
					{
						var zippedStreamWrapper = new Altaxo.Main.ZipOutputStreamWrapper(zippedStream);
						var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
						projectToSave.SaveToZippedFile(zippedStreamWrapper, info);
						zippedStream.Close();
					}
					stream.Close();
				}

				// Store the name of the item
				using (var stream = new ComStreamWrapper(pStgSave.CreateStream("AltaxoGraphName", (int)(STGM.DIRECT | STGM.READWRITE | STGM.CREATE | STGM.SHARE_EXCLUSIVE), 0, 0), true))
				{
					byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(graphDocumentName);
					stream.Write(nameBytes, 0, nameBytes.Length);
				}

				if (null != saveEx)
					throw saveEx;
			}
			catch (Exception ex)
			{
#if COMLOGGING
				Debug.ReportError("InternalSaveMiniProject, Exception ", ex);
#endif
			}
			finally
			{
				Marshal.ReleaseComObject(pStgSave);
			}

#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.InternalSaveMiniProject END");
#endif
		}

		#endregion

		#region Implementation of  System.Runtime.InteropServices.ComTypes.IDataObject

		public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.DAdvise {0}, {1}", GraphDocumentComObjectRenderer.FormatEtcToString(pFormatetc), advf);
#endif
			try
			{
				if (pFormatetc.cfFormat != 0) // if a special format is required
				{
					int res = QueryGetData(ref pFormatetc); // ask the render helper for availability of that format
					if (res != ComReturnValue.S_OK) // if the required format is not available
					{
						connection = 0; //  return an invalid connection cookie
						return res; // and the error
					}
				}
				FORMATETC etc = pFormatetc;
				int conn = 0;
				_dataAdviseHolder.Advise((IDataObject)this, ref etc, advf, adviseSink, out conn);
				connection = conn;
				return ComReturnValue.NOERROR;
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("GraphDocumentDataObject.DAdvise exception: {0}", e);
#endif
				throw;
			}
		}

		public void DUnadvise(int connection)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.DUnadvise connection={0}", connection);
#endif
			try
			{
				_dataAdviseHolder.Unadvise(connection);
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("GraphDocumentDataObject.DUnadvise exception {0}", e);
#endif
				throw;
			}
		}

		public int EnumDAdvise(out IEnumSTATDATA enumAdvise)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.EnumAdvise");
#endif
			enumAdvise = _dataAdviseHolder.EnumAdvise();
			return ComReturnValue.S_OK;
		}

		public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.EnumFormatEtc");
#endif
			try
			{
				// We only support GET
				if (DATADIR.DATADIR_GET == direction)
					return new EnumFormatEtc(new List<FORMATETC>(GetRenderings().Select(x => x.format)));
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("GraphDocumentDataObject.EnumFormatEtc exception: {0}", e);
#endif
				throw;
			}

			throw new NotImplementedException("Can not use registry here because a return value is not supported");
		}

		public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.GetCanonicalFormatEtc {0}", GraphDocumentComObjectRenderer.FormatEtcToString(formatIn));
#endif

			formatOut = formatIn;

			return ComReturnValue.DV_E_FORMATETC;
		}

		public void GetData(ref FORMATETC format, out STGMEDIUM medium)
		{
			try
			{
				// Locate the data
				foreach (var rendering in GetRenderings())
				{
					if ((rendering.format.tymed & format.tymed) > 0
							&& rendering.format.dwAspect == format.dwAspect
							&& rendering.format.cfFormat == format.cfFormat
							&& rendering.renderer != null)
					{
						// Found it. Return a copy of the data.

						medium = new STGMEDIUM();
						medium.tymed = format.tymed;
						medium.unionmember = rendering.renderer(format.tymed);
						if (medium.tymed == TYMED.TYMED_ISTORAGE || medium.tymed == TYMED.TYMED_ISTREAM)
							medium.pUnkForRelease = Marshal.GetObjectForIUnknown(medium.unionmember);
						else
							medium.pUnkForRelease = null;
						return;
					}
				}
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("GetData occured an exception.", e);
#endif
				throw;
			}

#if COMLOGGING
			Debug.ReportInfo("-> DV_E_FORMATETC");
#endif
			medium = new STGMEDIUM();
			// Marshal.ThrowExceptionForHR(ComReturnValue.DV_E_FORMATETC);
		}

		public void GetDataHere(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.GetDataHere({0})", GraphDocumentComObjectRenderer.ClipboardFormatName(format.cfFormat));
#endif
			// Allows containers to duplicate this into their own storage.
			try
			{
				if (format.cfFormat == DataObjectHelper.CF_EMBEDSOURCE && (format.tymed & TYMED.TYMED_ISTORAGE) != 0)
				{
					medium.tymed = TYMED.TYMED_ISTORAGE;
					medium.pUnkForRelease = null;
					var stg = (IStorage)Marshal.GetObjectForIUnknown(medium.unionmember);
					// we don't save the document directly, since this would mean to save the whole (and probably huge) project
					// instead we first make a mini project with the neccessary data only and then save this instead
					InternalSaveMiniProject(stg, _altaxoMiniProject, _altaxoGraphDocumentName);
					return;
				}


				if (format.cfFormat == DataObjectHelper.CF_LINKSOURCE && (format.tymed & TYMED.TYMED_ISTREAM) != 0)
				{
					IMoniker documentMoniker = CreateNewDocumentMoniker();

					if (null != documentMoniker)
					{
						medium.tymed = TYMED.TYMED_ISTREAM;
						medium.pUnkForRelease = null;
						IStream strm = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
						DataObjectHelper.SaveMonikerToStream(documentMoniker, strm);
						return;
					}
				}
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("GraphDocumentDataObject.GetDataHere, exception: {0}", e);
#endif
				throw;
			}
			Marshal.ThrowExceptionForHR(ComReturnValue.DATA_E_FORMATETC);
		}

		public int QueryGetData(ref FORMATETC format)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.QueryGetData, tymed={0}, aspect={1}", format.tymed, format.dwAspect);
#endif

			// We only support CONTENT aspect
			if ((DVASPECT.DVASPECT_CONTENT & format.dwAspect) == 0)
			{
				return ComReturnValue.DV_E_DVASPECT;
			}

			int ret = ComReturnValue.DV_E_TYMED;

			// Try to locate the data
			// TODO: The ret, if not S_OK, is only relevant to the last item
			foreach (var rendering in GetRenderings())
			{
				if ((rendering.format.tymed & format.tymed) > 0)
				{
					if (rendering.format.cfFormat == format.cfFormat)
					{
						// Found it, return S_OK;
						return ComReturnValue.S_OK;
					}
					else
					{
						// Found the medium type, but wrong format
						ret = ComReturnValue.DV_E_FORMATETC;
					}
				}
				else
				{
					// Mismatch on medium type
					ret = ComReturnValue.DV_E_TYMED;
				}
			}

#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.QueryGetData returning {0:x}", ret);
#endif
			return ret;
		}

		public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
		{
#if COMLOGGING
			Debug.ReportError("GraphDocumentDataObject.SetData - NOT SUPPORTED!");
#endif
			throw new NotSupportedException();
		}

		#endregion

	}
}